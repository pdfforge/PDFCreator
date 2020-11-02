using pdfforge.DataStorage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    /// <summary>
    ///     The SettingsUpgrader class performs updates to the PDFCreator settings.
    ///     This is done after loading the raw data and before loading them into the DataStorage classes.
    ///     There is one update method from each version to the next (0 to 1, 1 to 2 etc.) and they are called subsequently, if
    ///     required.
    /// </summary>
    public class SettingsUpgrader : DataUpgrader, ISettingsUpgrader
    {
        protected string[] VersionSettingPaths;

        protected readonly List<Action> UpgradeMethods = new List<Action>();

        public SettingsUpgrader(Data settingsData)
        {
            Data = settingsData;
            AddUpgradeMethods();
        }

        protected virtual void AddUpgradeMethods()
        {
        }

        public int SettingsVersion
        {
            get
            {
                var version = 0;

                foreach (string settingsVersion in VersionSettingPaths)
                {
                    var versionString = Data.GetValue(settingsVersion);
                    version = GetInt(versionString) ?? 0;

                    if (version != 0)
                        return version;
                }
                return version;
            }
        }

        public string SettingsVersionPath
        {
            get
            {
                var path = "";
                foreach (string settingsPath in VersionSettingPaths)
                {
                    var versionString = Data.GetValue(settingsPath);
                    var version = GetInt(versionString) ?? 0;
                    path = settingsPath;
                    if (version != 0)
                        return path;
                }
                return path;
            }
        }

        public int NumberOfUpgradeMethods()
        {
            return UpgradeMethods.Count;
        }

        public void Upgrade(int targetVersion)
        {
            for (var i = SettingsVersion; i < Math.Min(targetVersion, UpgradeMethods.Count); i++)
            {
                // Call upgrade methods subsequently, starting with the current version
                var upgradeMethod = UpgradeMethods[i];
                upgradeMethod();
            }
        }

        public bool RequiresUpgrade(int targetVersion)
        {
            return targetVersion > SettingsVersion;
        }

        protected void ForAllProfiles(Action<string, int> func, string profilePath)
        {
            var numProfiles = GetInt(Data.GetValue($@"{profilePath}\numClasses"));

            if (numProfiles != null)
            {
                for (var i = 0; i < numProfiles; i++)
                {
                    var path = $@"{profilePath}\{i}\";

                    func(path, i);
                }
            }
        }

        protected void MoveSettingInAllProfiles(string oldPath, string newPath)
        {
            var numProfiles = GetInt(Data.GetValue(@"ConversionProfiles\numClasses"));

            if (numProfiles != null)
            {
                for (var i = 0; i < numProfiles; i++)
                {
                    var path = string.Format(@"ConversionProfiles\{0}\", i);
                    MoveValue(path + oldPath, path + newPath);
                }
            }
        }

        protected IList<string> GetList(string pathOffset,string settingName)
        {
            var list = new List<string>();

            int.TryParse(Data.GetValue( $@"{pathOffset}{settingName}\numClasses"), out var listCount);
            for (int i = 0; i < listCount; i++)
            {
                list.Add(Data.GetValue($@"{pathOffset}{settingName}\{i}\{settingName}"));
            }
            return list;
        }

        protected void SetList(string pathOffset, IList<string> list, string settingName)
        {
            Data.SetValue($@"{pathOffset}{settingName}\numClasses", list.Count().ToString());
            for (int i = 0; i < list.Count(); i++)
            {
                Data.SetValue($@"{pathOffset}{settingName}\{i}\{settingName}", list.ElementAt(i));
            }
        }

        protected void MapSettingInAllProfiles(string path, Func<string, string> mapFunction)
        {
            var numProfiles = GetInt(Data.GetValue(@"ConversionProfiles\numClasses"));

            if (numProfiles != null)
            {
                for (var i = 0; i < numProfiles; i++)
                {
                    var p = string.Format(@"ConversionProfiles\{0}\" + path, i);
                    MapValue(p, mapFunction);
                }
            }
        }

        protected void ApplyNewSettingInAllProfiles(string path, string defaultValue)
        {
            var numProfiles = GetInt(Data.GetValue(@"ConversionProfiles\numClasses"));

            if (numProfiles != null)
            {
                for (var i = 0; i < numProfiles; i++)
                {
                    var p = string.Format(@"ConversionProfiles\{0}\" + path, i);
                    Data.SetValue(p, defaultValue);
                }
            }
        }

        protected void ExtractTimeServerAccounts(string sourceProfilePath, string targetAccountsPath)
        {
            var accounts = new Dictionary<TimeServerAccount, List<int>>();
            accounts[new TimeServerAccount { Url = "https://freetsa.org/tsr" }] = new List<int>();
            accounts[new TimeServerAccount { Url = "http://timestamp.digicert.com" }] = new List<int>();
            accounts[new TimeServerAccount { Url = "http://timestamp.globalsign.com/scripts/timstamp.dll" }] = new List<int>();

            ForAllProfiles((profilePath, profileIndex) =>
            {
                var path = profilePath + @"PdfSettings\Signature\";
                if (GetBool(Data.GetValue(path + "Enabled")) != true)
                    return;

                var account = new TimeServerAccount();
                account.IsSecured = GetBool(Data.GetValue(path + "TimeServerIsSecured")) == true;
                account.UserName = Data.GetValue(path + "TimeServerLoginName");
                account.Password = Data.Decrypt(Data.GetValue(path + "TimeServerPassword"));
                account.Url = Data.GetValue(path + "TimeServerUrl");

                var existingAccount = accounts.Keys.FirstOrDefault(a => a.Equals(account));
                if (existingAccount != null)
                {
                    accounts[existingAccount].Add(profileIndex);
                }
                else
                {
                    accounts[account] = new List<int>();
                    accounts[account].Add(profileIndex);
                }
            }, sourceProfilePath);

            for (var i = 0; i < accounts.Count; i++)
            {
                var account = accounts.Keys.ToArray()[i];
                account.AccountId = Guid.NewGuid().ToString();

                account.StoreValues(Data, $@"{targetAccountsPath}\Accounts\TimeServerAccounts\{i}\");

                foreach (var profileId in accounts[account])
                {
                    Data.SetValue($@"{sourceProfilePath}\{profileId}\PdfSettings\Signature\TimeServerAccountId", account.AccountId);
                }
            }
            Data.SetValue($@"{targetAccountsPath}\Accounts\TimeServerAccounts\numClasses", accounts.Keys.Count.ToString());
        }

        protected void AddReplacementTypeToTitleReplacements(string titleReplacementsPath)
        {
            string[] startReplacements =
            {
                "Microsoft Word - ",
                "Microsoft PowerPoint - ",
                "Microsoft Excel - "
            };

            string[] endReplacements =
            {
                ".doc",
                ".docx",
                ".xls",
                ".xlsx",
                ".ppt",
                ".pptx",
                ".png",
                ".jpg",
                ".jpeg",
                ".txt - Editor",
                " - Editor",
                ".txt",
                ".tif",
                ".tiff"
            };

            if (int.TryParse(Data.GetValue($@"{titleReplacementsPath}\numClasses"), out var replacementCount))
            {
                for (var i = 0; i < replacementCount; i++)
                {
                    string section = $"{titleReplacementsPath}\\{i}\\";

                    var type = ReplacementType.Replace;
                    var search = Data.GetValue(section + "Search");

                    if (startReplacements.Contains(search))
                    {
                        type = ReplacementType.Start;
                    }

                    if (endReplacements.Contains(search))
                    {
                        type = ReplacementType.End;
                    }

                    Data.SetValue(section + "ReplacementType", type.ToString());
                }
            }
        }

        protected void UpgradeToImprovedRegexTitleReplacements(string titleReplacementPath)
        {
            if (int.TryParse(Data.GetValue($@"{titleReplacementPath}\numClasses"), out var replacementCount))
            {
                for (var i = 0; i < replacementCount; i++)
                {
                    string section = $"{titleReplacementPath}\\{i}\\";

                    var replacement = new TitleReplacement();
                    replacement.ReadValues(Data, section);

                    if (replacement.ReplacementType == ReplacementType.RegEx)
                        continue;

                    if (String.IsNullOrEmpty(replacement.Replace))
                        continue;

                    replacement.ReplacementType = ReplacementType.RegEx;
                    replacement.Search = Regex.Escape(replacement.Search);
                    replacement.Replace = Regex.Escape(replacement.Replace);

                    replacement.StoreValues(Data, section);
                }
            }
        }

        protected void ExtractFtpAccounts(string sourceProfilePath, string targetAccountsPath)
        {
            var accounts = new Dictionary<FtpAccount, List<int>>();

            ForAllProfiles((profilePath, profileIndex) =>
            {
                var path = profilePath + @"Ftp\";
                if (GetBool(Data.GetValue(path + "Enabled")) != true)
                    return;

                var account = new FtpAccount();
                account.UserName = Data.GetValue(path + "UserName");
                account.Password = Data.Decrypt(Data.GetValue(path + "Password"));
                account.Server = Data.GetValue(path + "Server");

                var existingAccount = accounts.Keys.FirstOrDefault(a => a.Equals(account));
                if (existingAccount != null)
                {
                    accounts[existingAccount].Add(profileIndex);
                }
                else
                {
                    accounts[account] = new List<int>();
                    accounts[account].Add(profileIndex);
                }
            }, sourceProfilePath);

            for (var i = 0; i < accounts.Count; i++)
            {
                var account = accounts.Keys.ToArray()[i];
                account.AccountId = Guid.NewGuid().ToString();

                account.StoreValues(Data, $@"{targetAccountsPath}\Accounts\FtpAccounts\{i}\");

                foreach (var profileId in accounts[account])
                {
                    Data.SetValue($@"{sourceProfilePath}\{profileId}\Ftp\AccountId", account.AccountId);
                }
            }
            Data.SetValue($@"{targetAccountsPath}\Accounts\FtpAccounts\numClasses", accounts.Keys.Count.ToString());
        }

        protected void ExtractSmtpAccounts(string sourceProfilePath, string targetAccountsPath)
        {
            var accounts = new Dictionary<SmtpAccount, List<int>>();

            ForAllProfiles((pathToProfile, indexOfProfile) =>
            {
                var path = pathToProfile + @"EmailSmtpSettings\";
                if (GetBool(Data.GetValue(path + "Enabled")) != true)
                    return;

                var account = new SmtpAccount();
                account.Address = Data.GetValue(path + "Address");
                account.Password = Data.Decrypt(Data.GetValue(path + "Password"));
                account.Port = GetInt(Data.GetValue(path + "Port")) ?? 25;
                account.Server = Data.GetValue(path + "Server");
                account.Ssl = GetBool(Data.GetValue(path + "Ssl")) == true;
                account.UserName = Data.GetValue(path + "UserName");

                var existingAccount = accounts.Keys.FirstOrDefault(a => a.Equals(account));
                if (existingAccount != null)
                {
                    accounts[existingAccount].Add(indexOfProfile);
                }
                else
                {
                    accounts[account] = new List<int>();
                    accounts[account].Add(indexOfProfile);
                }
            }, sourceProfilePath);

            for (var i = 0; i < accounts.Count; i++)
            {
                var account = accounts.Keys.ToArray()[i];
                account.AccountId = Guid.NewGuid().ToString();

                account.StoreValues(Data, $@"{targetAccountsPath}\Accounts\SmtpAccounts\{i}\");

                foreach (var profileId in accounts[account])
                {
                    Data.SetValue($@"{sourceProfilePath}\{profileId}\EmailSmtpSettings\AccountId", account.AccountId);
                }
            }
            Data.SetValue($@"{targetAccountsPath}\Accounts\SmtpAccounts\numClasses", accounts.Keys.Count.ToString());
        }

        protected int? GetInt(string s)
        {
            int i;

            if (!int.TryParse(s, out i))
                return null;
            return i;
        }

        protected bool? GetBool(string s)
        {
            bool b;

            if (!bool.TryParse(s, out b))
                return null;
            return b;
        }
    }
}
