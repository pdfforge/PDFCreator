using System;
using System.Linq;
using System.Threading;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement.Helper;
using pdfforge.PDFCreator.Core.SettingsManagement.SettingsLoading;

namespace pdfforge.PDFCreator.Core.SettingsManagement
{
    public abstract class SettingsManager : ISettingsManager
    {
            private readonly ISettingsLoader _loader;
            private readonly IInstallationPathProvider _installationPathProvider;
            protected readonly SettingsProvider SettingsProvider;

            public SettingsManager(SettingsProvider settingsProvider, ISettingsLoader loader, IInstallationPathProvider installationPathProvider)
            {
                SettingsProvider = settingsProvider;
                _loader = loader;
                _installationPathProvider = installationPathProvider;
            }

            private void LoadAllSettingsSynchronized()
            {
                // Synchronize across processes with a Mutex to avaoid partial reads/writes
                RunSynchronized(DoLoadPdfCreatorSettings);
            }
        
            protected abstract void ProcessAfterLoading(PdfCreatorSettings settings);
            protected abstract void ProcessBeforeSaving(PdfCreatorSettings settings);
            protected abstract void ProcessAfterSaving(PdfCreatorSettings settings);

        private void DoLoadPdfCreatorSettings()
            {
                var settings = _loader.LoadPdfCreatorSettings();
            SettingsProvider.UpdateSettings(settings);
            ProcessAfterLoading(settings);
        }

            public ISettingsProvider GetSettingsProvider()
            {
                return SettingsProvider;
            }

            public void SaveCurrentSettings()
        {
            // Synchronize across processes with a Mutex to avoid partial reads/writes
            RunSynchronized(DoSaveSettings);
            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }

            private void DoSaveSettings()
            {
                //Remove shared profiles from copy and save this copy to preserve shred profile in settingsProvider
                var settings = SettingsProvider.Settings.Copy();
                ProcessBeforeSaving(settings);
            var sharedProfiles = settings.ConversionProfiles.Where(p => p.Properties.IsShared).ToArray();
                foreach (var profile in sharedProfiles)
                    settings.ConversionProfiles.Remove(profile);
                _loader.SaveSettingsInRegistry(settings);
                ProcessAfterSaving(settings);
        }

            public void ApplyAndSaveSettings(PdfCreatorSettings settings)
            {
                SettingsProvider.UpdateSettings(settings);
                SaveCurrentSettings();
            }

            public void LoadAllSettings()
            {
                LoadAllSettingsSynchronized();

                SettingsSaved?.Invoke(this, EventArgs.Empty);
            }

            public event EventHandler SettingsSaved;

            private void RunSynchronized(Action action)
            {
                var mutexName = "PDFCreator-Settings" + _installationPathProvider.ApplicationGuid;
                var mutex = new Mutex(false, mutexName);

                // we have two try blocks, because we can ignore the AbandonedMutexException
                // The action must be called and in case of an exception, ReleaseMutex must be called
                try
                {
                    try
                    {
                        mutex.WaitOne();
                    }
                    catch (AbandonedMutexException)
                    {
                    }

                    action();
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        
    }
}
