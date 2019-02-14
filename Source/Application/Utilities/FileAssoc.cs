using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IFileAssoc
    {
        /// <summary>
        ///     Tests if a file extension has the print verb associated.
        /// </summary>
        /// <param name="assoc">
        ///     The file association as string. It should start with a dot (.). If the initial dot is missing, it will be inserted
        ///     automatically.
        ///     There may not be any further dots in the string though.
        /// </param>
        /// <returns>true, if the association is registered</returns>
        bool HasPrint(string assoc);

        /// <summary>
        ///     Tests if a file extension has the printto verb associated.
        /// </summary>
        /// <param name="assoc">
        ///     The file association as string. It should start with a dot (.). If the initial dot is missing, it will be inserted
        ///     automatically.
        ///     There may not be any further dots in the string though.
        /// </param>
        /// <returns>true, if the association is registered</returns>
        bool HasPrintTo(string assoc);

        /// <summary>
        ///     Tests if a file extension has the open verb associated.
        /// </summary>
        /// <param name="assoc">
        ///     The file association as string. It should start with a dot (.). If the initial dot is missing, it will be inserted
        ///     automatically.
        ///     There may not be any further dots in the string though.
        /// </param>
        /// <returns>true, if the association is registered</returns>
        bool HasOpen(string assoc);

        /// <summary>
        /// Gets the ShellCommand to call a shell verb for a given file extension
        /// </summary>
        /// <param name="assoc">
        ///     The file association as string. It should start with a dot (.). If the initial dot is missing, it will be inserted
        ///     automatically.
        ///     There may not be any further dots in the string though.
        /// </param>
        /// <param name="verb">The shell verb to query, i.e. print or printto</param>
        /// <returns></returns>
        ShellCommand GetShellCommand(string assoc, string verb);

        void RegisterSpecialFileTypes(List<SpecialShellCommand> specialFileTypes);
    }

    public class FileAssoc : IFileAssoc
    {
        private List<SpecialShellCommand> _specialFileTypes;

        /// <inheritdoc />
        public bool HasPrint(string assoc)
        {
            return FileAssocHasVerb(assoc, "print");
        }

        /// <inheritdoc />
        public bool HasPrintTo(string assoc)
        {
            return FileAssocHasVerb(assoc, "printto");
        }

        /// <inheritdoc />
        public bool HasOpen(string assoc)
        {
            return FileAssocHasVerb(assoc, "open");
        }

        private bool FileAssocHasVerb(string assoc, string verb)
        {
            return GetShellCommand(assoc, verb) != null;
        }

        /// <inheritdoc />
        public ShellCommand GetShellCommand(string assoc, string verb)
        {
            assoc = MakeValidExtension(assoc);

            var fileType = GetFiletypeKey(assoc);
            var filetypeRegKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey($"{fileType}\\shell\\{verb}\\command");
            var command = filetypeRegKey?.GetValue("") as string;

            if (string.IsNullOrWhiteSpace(command))
            {
                if (fileType == null || _specialFileTypes == null || _specialFileTypes.Count == 0)
                    return null;

                var specialCommand = _specialFileTypes.FirstOrDefault(x => x.FileType == fileType && x.ShellVerb == verb);
                if (specialCommand == null)
                    return null;

                return new ShellCommand(specialCommand.CommandLine, specialCommand.Arguments, verb);
            }

            var commandArgs = CommandLineToArgs(command);
            var commandParams = commandArgs.Skip(1).ToArray();

            return new ShellCommand(commandArgs[0], commandParams, verb);
        }

        public void RegisterSpecialFileTypes(List<SpecialShellCommand> specialFileTypes)
        {
            _specialFileTypes = specialFileTypes;
        }

        private string[] GetVerbsByExtension(string assoc)
        {
            assoc = MakeValidExtension(assoc);

            var fileType = GetFiletypeKey(assoc);

            if (fileType == null)
                return new string[0];

            using (var extensionRegKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey($"{fileType}\\shell"))
            {
                if (extensionRegKey == null)
                    return new string[0];

                return extensionRegKey.GetSubKeyNames()
                    .Select(s => s.ToLower())
                    .ToArray();
            }
        }

        private string MakeValidExtension(string assoc)
        {
            if (string.IsNullOrEmpty(assoc))
            {
                throw new ArgumentNullException(nameof(assoc));
            }

            if (!assoc.StartsWith("."))
            {
                assoc = "." + assoc;
            }

            if (assoc.Length < 2)
            {
                throw new ArgumentException("The file extension must at least have a dot and one other character");
            }

            if (assoc.IndexOf(".", 1, StringComparison.Ordinal) > 0)
            {
                throw new ArgumentException(
                    "The file extension must start with a dot (.) and must not contain any dots after the first character");
            }

            return assoc;
        }

        private string GetFiletypeKey(string extension)
        {
            using (var extensionRegKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(extension))
            {
                var filetype = extensionRegKey?.GetValue("") as string;
                return filetype;
            }
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        private static string[] CommandLineToArgs(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }
    }

    public class ShellCommand
    {
        public ShellCommand(string command, string[] arguments, string verb)
        {
            Command = command;
            Arguments = arguments;
            Verb = verb;
        }

        public string Command { get; }
        public string[] Arguments { get; }
        public string Verb { get; }

        private string ReplaceArgs(string arg, string[] placeholders)
        {
            if (arg.StartsWith("%") && arg.Length == 2)
            {
                try
                {
                    var paramNumber = int.Parse(arg[1].ToString()) - 1;
                    if (paramNumber >= placeholders.Length)
                        return "";
                    return "\"" + placeholders[paramNumber] + "\"";
                }
                catch
                {
                    return arg;
                }
            }
            return arg;
        }

        public string GetReplacedCommandArgs(params string[] placeholders)
        {
            var argList = Arguments
                .Select(arg => ReplaceArgs(arg, placeholders))
                .Where(arg => !string.IsNullOrWhiteSpace(arg))
                .ToList();

            return string.Join(" ", argList);
        }
    }

    public class SpecialShellCommand
    {
        public SpecialShellCommand(string fileType, string shellVerb, string commandLine, string[] arguments)
        {
            FileType = fileType;
            CommandLine = commandLine;
            Arguments = arguments;
            ShellVerb = shellVerb;
        }

        public string FileType { get; }
        public string CommandLine { get; }
        public string[] Arguments { get; }
        public string ShellVerb { get; }
    }
}
