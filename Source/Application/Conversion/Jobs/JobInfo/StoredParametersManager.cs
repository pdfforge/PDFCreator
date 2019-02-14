using Microsoft.Win32;
using NLog;
using pdfforge.DataStorage.Storage;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Core.SettingsManagement;
using System;

namespace pdfforge.PDFCreator.Conversion.Jobs.JobInfo
{
    public interface IStoredParametersManager
    {
        bool HasPredefinedParameters();

        Parameters GetAndResetParameters();

        void SaveParameterSettings(string outputFile, string profileParameter, string originalFilePath);
    }

    public class StoredParametersManager : IStoredParametersManager
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IInstallationPathProvider _installationPathProvider;
        private readonly IDataStorageFactory _dataStorageFactory;

        public StoredParametersManager(IInstallationPathProvider installationPathProvider, IDataStorageFactory dataStorageFactory)
        {
            _installationPathProvider = installationPathProvider;
            _dataStorageFactory = dataStorageFactory;
        }

        private ParameterSettings LoadParameterSettings()
        {
            var regStorage = BuildStorage();
            var parametersettings = new ParameterSettings();

            parametersettings.LoadData(regStorage);

            return parametersettings;
        }

        private IStorage BuildStorage()
        {
            return _dataStorageFactory.BuildRegistryStorage(RegistryHive.CurrentUser, _installationPathProvider.ApplicationRegistryPath);
        }

        private bool ParametersAreSet(Parameters parameters)
        {
            return !string.IsNullOrWhiteSpace(parameters.Outputfile)
                   || !string.IsNullOrWhiteSpace(parameters.Profile)
                   || !string.IsNullOrWhiteSpace(parameters.OriginalFilePath);
        }

        public bool HasPredefinedParameters()
        {
            var parameters = LoadParameterSettings();

            return ParametersAreSet(parameters.Parameters);
        }

        public Parameters GetAndResetParameters()
        {
            var parameterSettings = LoadParameterSettings();

            if (!ParametersAreSet(parameterSettings.Parameters))
                throw new InvalidOperationException();

            var parameters = parameterSettings.Parameters;

            parameterSettings.Parameters = new Parameters();
            parameterSettings.SaveData(BuildStorage());
            _logger.Debug($"Loaded Parameters from registry:\n" +
                          $"OutputFile: {parameters.Outputfile}\n" +
                          $"Profile: {parameters.Profile}\n" +
                          $"OriginalFilePath: {parameters.OriginalFilePath}"
            );
            return parameters;
        }

        public void SaveParameterSettings(string outputFile, string profileParameter, string originalFilePath)
        {
            var parameters = new Parameters();
            parameters.Outputfile = outputFile;
            parameters.Profile = profileParameter;
            parameters.OriginalFilePath = originalFilePath;

            if (!ParametersAreSet(parameters))
                return;

            var parameterSettings = new ParameterSettings { Parameters = parameters };
            var regStorage = BuildStorage();
            parameterSettings.SaveData(regStorage);
            _logger.Debug($"Saved Parameters into registry:\n" +
                          $"OutputFile: {outputFile}\n" +
                          $"Profile: {profileParameter}\n" +
                          $"OriginalFilePath: {originalFilePath}");
        }
    }
}
