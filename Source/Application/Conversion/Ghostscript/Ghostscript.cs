using NLog;
using pdfforge.PDFCreator.Conversion.Ghostscript.OutputDevices;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
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

        public TimeSpan Timeout { get; set; } = TimeSpan.FromHours(24);

        private bool Run(IList<string> parameters, string tempOutputFolder)
        {
            var parametersWithoutPassword = parameters.Select(param => param.StartsWith(PrintingDevice.PasswordParameter) ?
                PrintingDevice.PasswordParameter + "***" : param);
            _logger.Debug("Ghostscript Parameters:\r\n" + string.Join("\r\n", parametersWithoutPassword));

            // Start the child process.
            var p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = GhostscriptVersion.ExePath;
            p.StartInfo.CreateNoWindow = true;

            var isFirst = true;
            var sb = new StringBuilder();

            foreach (var s in parameters)
            {
                var tmp = s;
                if (isFirst)
                {
                    isFirst = false;
                    continue;
                }

                if (tmp.Contains(" ") && !tmp.Contains("\""))
                {
                    tmp = tmp.Replace("\"", "\\\"");
                    tmp = "\"" + tmp + "\"";
                }

                sb.AppendLine(tmp);
            }

            var parameterFile = Path.Combine(tempOutputFolder, "parameters.txt");
            File.WriteAllText(parameterFile, sb.ToString());

            p.StartInfo.Arguments = string.Format("@\"{0}\"", parameterFile);

            var gsThread = new Thread(() => RunAndReadStdOut(p));
            gsThread.Start();

            if (!gsThread.Join(Timeout))
            {
                _logger.Error($"The ghostscript did not finish within {Timeout.TotalMinutes} minutes");
                p.Kill();
                return false;
            }

            return p.ExitCode == 0;
        }

        private void RunAndReadStdOut(Process p)
        {
            p.Start();
            // Do not wait for the child process to exit before
            // reading to the end of its redirected stream.
            // p.WaitForExit();
            // Read the output stream first and then wait.

            var sbout = new StringBuilder();

            while (!p.StandardOutput.EndOfStream)
            {
                var c = (char)p.StandardOutput.Read();
                sbout.Append(c);

                if ((c == ']') || (c == '\n'))
                {
                    RaiseOutputEvent(sbout.ToString());

                    sbout.Length = 0;
                }
            }

            if (sbout.Length > 0)
                RaiseOutputEvent(sbout.ToString());

            p.WaitForExit();
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
        /// <param name="output">The output device to use for conversion</param>
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
                    CollectIntermediateFiles(outputDevice.Job);
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

        private void CollectIntermediateFiles(Job job)
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
