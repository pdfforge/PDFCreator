using NLog;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace pdfforge.PDFCreator.Conversion.Ghostscript
{
    /// <summary>
    ///     Provides access to Ghostscript, either through DLL access or by calling the Ghostscript exe
    /// </summary>
    public class GhostScript
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public GhostScript(GhostscriptVersion ghostscriptVersion)
        {
            GhostscriptVersion = ghostscriptVersion;
        }

        public GhostscriptVersion GhostscriptVersion { get; }

        public event EventHandler<OutputEventArgs> Output;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(30);

        private bool Run(IList<string> parameters, string tempOutputFolder)
        {
            var escapedParameters = EscapeParameters(parameters);

            LogParametersWithoutPasswords(escapedParameters);

            // Start the child process.
            var p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.FileName = GhostscriptVersion.ExePath;
            p.StartInfo.CreateNoWindow = true;

            var parameterFile = Path.Combine(tempOutputFolder, "parameters.txt");
            File.WriteAllText(parameterFile, string.Join("\r\n", escapedParameters));

            p.StartInfo.Arguments = $"@\"{parameterFile}\"";

            var gsThread = new Thread(() => RunAndReadStdOutAndError(p));
            gsThread.Start();

            if (!gsThread.Join(Timeout))
            {
                _logger.Error($"The ghostscript did not finish within {Timeout.TotalMinutes} minutes");
                p.Kill();
                return false;
            }

            _logger.Info($"ghostscript exited with code {p.ExitCode}");

            return p.ExitCode == 0;
        }

        private void LogParametersWithoutPasswords(string[] parameters)
        {
            var parametersWithoutPassword = parameters.Select(param => param.StartsWith(OutputDevice.PasswordParameter) ?
                $"{OutputDevice.PasswordParameter}=***" : param);

            _logger.Debug("Ghostscript Parameters:\r\n" + string.Join("\r\n", parametersWithoutPassword));
        }

        private string[] EscapeParameters(IList<string> parameters)
        {
            var escapedParams = new string[parameters.Count];

            for (var i = 0; i < parameters.Count; i++)
            {
                var param = parameters[i];

                if (param.Contains(" ") && !param.Contains("\""))
                {
                    param = param.Replace("\"", "\\\"");
                    param = "\"" + param + "\"";
                    param = " " + param + " "; // Add spaces around the quotes to prevent Ghostscript escaping issues
                }

                // A trailing backslash can escape a quote on the next line. Adding a space fixes that.
                if (param.EndsWith("\\"))
                    param += " ";

                escapedParams[i] = param;
            }

            return escapedParams;
        }

        private void RunAndReadStdOutAndError(Process p)
        {
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.

            p.OutputDataReceived += OutputDataReceivedHandler;
            p.ErrorDataReceived += ErrorDataReceivedHandler;

            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();

            p.OutputDataReceived -= OutputDataReceivedHandler;
            p.ErrorDataReceived -= ErrorDataReceivedHandler;
        }

        private void OutputDataReceivedHandler(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                RaiseOutputEvent(e.Data);
        }

        private void ErrorDataReceivedHandler(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data) && e.Data.Contains("Dereference"))
            {
                _logger.Error("We detected Ghostscript hanging on a specific error (error while dereferencing an object). Stopping the process...");
                if(sender is Process process)
                    process.Kill();
            }
        }

        private void RaiseOutputEvent(string message)
        {
            if (Output != null)
            {
                Output(this, new OutputEventArgs(message));
            }
        }

        /// <summary>
        ///     Runs Ghostscript with the parameters specified by the OutputDevice
        /// </summary>
        /// <param name="outputDevice">The output device to use for conversion</param>
        /// <param name="tempOutputFolder">Full path to the folder, where temporary files can be stored</param>
        public bool Run(OutputDevice outputDevice, string tempOutputFolder)
        {
            var parameters = outputDevice.GetGhostScriptParameters(GhostscriptVersion);
            var success = Run(parameters, tempOutputFolder);

            CollectTempOutputFiles(outputDevice);

            return success;
        }

        private void CollectTempOutputFiles(OutputDevice outputDevice)
        {
            switch (outputDevice.ConversionMode)
            {
                case ConversionMode.PdfConversion:
                case ConversionMode.IntermediateConversion:
                    CollectIntermediatePdfFile(outputDevice.Job);
                    break;

                case ConversionMode.ImmediateConversion:
                case ConversionMode.IntermediateToTargetConversion:
                    CollectTempOutputFiles(outputDevice.Job);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CollectTempOutputFiles(Job job)
        {
            var files = Directory.GetFiles(job.JobTempOutputFolder);

            foreach (var file in files)
                job.TempOutputFiles.Add(file);
        }

        private void CollectIntermediatePdfFile(Job job)
        {
            job.IntermediatePdfFile = Directory.GetFiles(job.IntermediateFolder).Single();
        }
    }

    public class OutputEventArgs : EventArgs
    {
        public OutputEventArgs(string output)
        {
            Output = output;
        }

        public string Output { get; private set; }
    }
}
