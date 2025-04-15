﻿using pdfforge.Obsidian;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Presentation.Helper.Translation;
using pdfforge.PDFCreator.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Utilities.Messages;
using Translatable;
using pdfforge.PDFCreator.Core.SettingsManagement;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants
{
    public interface IUacAssistant
    {
        bool CheckForPrinterHelper();

        bool AddExplorerIntegration();

        bool RemoveExplorerIntegration();

        Task<bool> AddPrinter(string printerName, bool singlePort = true);

        Task<bool> AddPrinters(string[] printerNames, bool singlePort = true);

        bool RenamePrinter(string oldPrinterName, string newPrinterName);

        bool DeletePrinter(params string[] printerNames);

        bool StoreLicenseForAllUsers(string licenseServerCode, string licenseKey);
    }

    public class UacAssistant : IUacAssistant
    {
        private readonly IInteractionInvoker _invoker;
        private readonly IShellExecuteHelper _shellExecuteHelper;
        private readonly IPDFCreatorNameProvider _pdfCreatorNameProvider;
        private UacAssistantTranslation _translation;

        public UacAssistant(ITranslationUpdater translationUpdater, IInteractionInvoker invoker, IShellExecuteHelper shellExecuteHelper, IPDFCreatorNameProvider pdfCreatorNameProvider)
        {
            _invoker = invoker;
            _shellExecuteHelper = shellExecuteHelper;
            _pdfCreatorNameProvider = pdfCreatorNameProvider;

            translationUpdater.RegisterAndSetTranslation(tf => _translation = tf.UpdateOrCreateTranslation(_translation));
        }

        public bool CheckForPrinterHelper()
        {
            return GetApplicationPath("PrinterHelper.exe") != null;
        }

        public bool AddExplorerIntegration()
        {
            return CallSetupHelper("/FileExtensions=Add");
        }

        public bool RemoveExplorerIntegration()
        {
            return CallSetupHelper("/FileExtensions=Remove");
        }

        public async Task<bool> AddPrinter(string printerName, bool singlePort = true)
        {
            return await AddPrinters(new[] { printerName }, singlePort);
        }

        public async Task<bool> AddPrinters(string[] printerNames, bool singlePort = true)
        {
            const string command = "addPrinter";
            var escapedPrinterList = printerNames
                .Select(p => $"\"{p}\"");

            var args = $"{command} -name={string.Join(",", escapedPrinterList)}";

            if (singlePort)
                args += " /SinglePort";

            return await CallPrinterHelperAsync(args);
        }

        public bool RenamePrinter(string oldPrinterName, string newPrinterName)
        {
            return CallPrinterHelper($"renamePrinter -name=\"{oldPrinterName}\" -newName=\"{newPrinterName}\"");
        }

        public bool DeletePrinter(params string[] printerNames)
        {
            if (printerNames.Length == 0)
                return true;

            var escapedPrinters = printerNames.Select(p => $"\"{p}\"");
            var parameters = string.Join(",", escapedPrinters);
            return CallPrinterHelper($"deletePrinter -name={parameters}");
        }

        /// <summary>
        ///     Call the SetupHelper.exe to add or remove explorer context menu integration
        /// </summary>
        /// <param name="arguments">Command line arguments that will be passed to SetupHelper.exe</param>
        /// <returns>true, if the action was successful</returns>
        private bool CallSetupHelper(string arguments)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (assemblyPath == null)
                return false;

            var setupHelperPath = Path.Combine(assemblyPath, "SetupHelper.exe");

            if (!File.Exists(setupHelperPath))
            {
                var message = _translation.GetFormattedSetupFileMissing(Path.GetFileName(setupHelperPath));
                var caption = _translation.Error;
                ShowMessage(message, caption, MessageOptions.Ok, MessageIcon.Error);
                return false;
            }

            if (Environment.OSVersion.Version.Major <= 5)
            {
                var osHelper = new OsHelper();
                if (!osHelper.UserIsAdministrator())
                {
                    var message = _translation.OperationRequiresAdminPrivileges;
                    var caption = _translation.AdminPrivilegesRequired;

                    var response = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Info);
                    if (response == MessageResponse.No)
                        return false;
                }
            }

            return CallProgramAsAdmin(setupHelperPath, arguments);
        }

        private bool CallProgramAsAdmin(string path, string arguments)
        {
            var result = _shellExecuteHelper.RunAsAdmin(path, arguments);

            if (result == ShellExecuteResult.RunAsWasDenied)
            {
                var message = _translation.SufficientPermissions;
                var caption = _translation.Error;

                ShowMessage(message, caption, MessageOptions.Ok, MessageIcon.Error);

                return false;
            }

            return result == ShellExecuteResult.Success;
        }

        private async Task<bool> CallProgramAsAdminAsync(string path, string arguments)
        {
            var result = await _shellExecuteHelper.RunAsAdminAsync(path, arguments);

            if (result == ShellExecuteResult.RunAsWasDenied)
            {
                var message = _translation.SufficientPermissions;
                var caption = _translation.Error;

                ShowMessage(message, caption, MessageOptions.Ok, MessageIcon.Error);

                return false;
            }

            return result == ShellExecuteResult.Success;
        }

        /// <summary>
        ///     Call the PrinterHelper.exe to add, rename or delete printers
        /// </summary>
        /// <param name="arguments">Command line arguments that will be passed to PrinterHelper.exe</param>
        /// <returns>true, if the action was successful</returns>
        private bool CallPrinterHelper(string arguments)
        {
            var applicationPath = GetApplicationPath("PrinterHelper.exe");
            if (applicationPath == null)
                return false;

            return CallProgramAsAdmin(applicationPath, arguments);
        }

        private async Task<bool> CallPrinterHelperAsync(string arguments)
        {
            var applicationPath = GetApplicationPath("PrinterHelper.exe");
            if (applicationPath == null)
                return false;

            return await CallProgramAsAdminAsync(applicationPath, arguments);
        }

        private string GetApplicationPath(string applicationName)
        {
            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (assemblyPath == null)
                return null;

            var applicationPath = Path.Combine(assemblyPath, applicationName);

            if (!File.Exists(applicationPath))
            {
                var message = _translation.GetFormattedSetupFileMissing(Path.GetFileName(applicationPath));
                var caption = _translation.Error;
                ShowMessage(message, caption, MessageOptions.Ok, MessageIcon.Error);
                return null;
            }

            if (Environment.OSVersion.Version.Major <= 5)
            {
                var osHelper = new OsHelper();
                if (!osHelper.UserIsAdministrator())
                {
                    var message = _translation.OperationRequiresAdminPrivileges;
                    var caption = _translation.AdminPrivilegesRequired;

                    var response = ShowMessage(message, caption, MessageOptions.YesNo, MessageIcon.Info);
                    if (response == MessageResponse.No)
                        return null;
                }
            }

            return applicationPath;
        }

        private MessageResponse ShowMessage(string message, string title, MessageOptions buttons, MessageIcon icon)
        {
            var interaction = new MessageInteraction(message, title, buttons, icon);
            _invoker.Invoke(interaction);
            return interaction.Response;
        }

        public bool StoreLicenseForAllUsers(string licenseServerCode, string licenseKey)
        {
            var lsaBase64 = Convert.ToBase64String(Encoding.Default.GetBytes(licenseServerCode));
            return CallPDFCreatorCli($"StoreLicenseForAllUsers /LicenseServerCode=\"{lsaBase64}\" /LicenseKey=\"{licenseKey}\"");
        }

        private bool CallPDFCreatorCli(string arguments)
        {
            var applicationPath = _pdfCreatorNameProvider.GetPortApplicationPath();

            return CallProgramAsAdmin(applicationPath, arguments);
        }
    }

    public class UacAssistantTranslation : ITranslatable
    {
        public string AdminPrivilegesRequired { get; private set; } = "Admin privileges required";
        public string Error { get; private set; } = "Error";
        public string OperationRequiresAdminPrivileges { get; private set; } = "This operation requires admin privileges and it looks like you are not an admin. Do you want to continue anyway?\nNote: It is safe to continue even if you are unsure if you have appropriate rights, but the operation will not be completed.";
        public string SufficientPermissions { get; private set; } = "Operation failed. You probably do not have sufficient permissions.";
        private string SetupFileMissing { get; set; } = "An important PDFCreator file is missing ('{0}'). Please reinstall PDFCreator!";

        public string GetFormattedSetupFileMissing(string fileName)
        {
            return string.Format(SetupFileMissing, fileName);
        }
    }
}
