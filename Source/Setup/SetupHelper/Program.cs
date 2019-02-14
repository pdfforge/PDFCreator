using Microsoft.Win32;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace pdfforge.SetupHelper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var showUsage = true;

            var clp = new CommandLineParser(args);

            if (clp.HasArgumentWithValue("FileExtensions"))
            {
                showUsage = false;
                try
                {
                    switch (clp.GetArgument("FileExtensions"))
                    {
                        case "Add":
                            MaybeInvokeWow6432(AddExplorerIntegration);
                            break;

                        case "Remove":
                            MaybeInvokeWow6432(RemoveExplorerIntegration);
                            break;

                        default:
                            showUsage = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Environment.ExitCode = 1;
                }
            }

            if (clp.HasArgumentWithValue("ComInterface"))
            {
                showUsage = false;
                try
                {
                    switch (clp.GetArgument("ComInterface"))
                    {
                        case "Register":
                            MaybeInvokeWow6432(RegisterComInterface);
                            break;

                        case "Unregister":
                            MaybeInvokeWow6432(UnregisterComInterface);
                            break;

                        default:
                            showUsage = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Environment.ExitCode = 1;
                }
            }

            if (showUsage)
                Usage();

            if (Debugger.IsAttached)
                Console.Read();
        }

        private static void Usage()
        {
            Console.WriteLine("SetupHelper " + Assembly.GetExecutingAssembly().GetName().Version + "             © pdfforge");
            Console.WriteLine();
            Console.WriteLine("usage:");
            Console.WriteLine("SetupHelper.exe [/FileExtensions=Add|Remove] [/ComInterface=Register|Unregister]");
        }

        private static bool Is64Bit()
        {
            var is64BitOs = Environment.Is64BitOperatingSystem;
            var is64BitProcess = Environment.Is64BitOperatingSystem;
            Console.WriteLine($"x64 platform: {is64BitOs}, x64 process: {is64BitProcess}");

            return is64BitOs;
        }

        private static void MaybeInvokeWow6432(Action<bool> wowAwareAction)
        {
            wowAwareAction(false);

            if (Is64Bit())
                wowAwareAction(true);
        }

        private static void AddExplorerIntegration(bool wow6432)
        {
            CallRegAsmForShell(wow6432, "PDFCreatorShell.dll", "/codebase");
        }

        private static void RemoveExplorerIntegration(bool wow6432)
        {
            CallRegAsmForShell(wow6432, "PDFCreatorShell.dll", "/unregister");
        }

        private static void RegisterComInterface(bool wow6432)
        {
            CallRegAsmForShell(wow6432, "PDFCreator.COM.dll", "/codebase /tlb");
        }

        private static void UnregisterComInterface(bool wow6432)
        {
            CallRegAsmForShell(wow6432, "PDFCreator.COM.dll", "/unregister");
        }

        private static void CallRegAsmForShell(bool wow6432, string fileName, string parameters)
        {
            var regAsmPath = GetRegAsmPath(wow6432);

            var appDir = GetApplicationDirectory();
            var shellDll = Path.Combine(appDir, fileName);

            var shellExecuteHelper = new ShellExecuteHelper();

            var paramString = $"\"{shellDll}\" {parameters}";
            Console.WriteLine(regAsmPath + " " + paramString);

            var result = shellExecuteHelper.RunAsAdmin(regAsmPath, paramString);
            Console.WriteLine(result.ToString());
        }

        private static string GetRegAsmPath(bool wow6432)
        {
            var regPath = wow6432
                ? @"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\.NETFramework"
                : @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework";

            Console.WriteLine(regPath);

            var dotNetPath = Registry.GetValue(regPath, "InstallRoot", null)?.ToString();

            if (string.IsNullOrEmpty(dotNetPath))
                throw new InvalidOperationException("Cannot find .Net Framework in HKLM\\" + regPath);

            return Path.Combine(dotNetPath, "v4.0.30319\\RegAsm.exe");
        }

        private static string GetApplicationDirectory()
        {
            return new AssemblyHelper(typeof(Program).Assembly).GetAssemblyDirectory();
        }
    }
}
