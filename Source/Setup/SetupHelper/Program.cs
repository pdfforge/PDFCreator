using Microsoft.Win32;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NLog.LayoutRenderers;
using pdfforge.SetupHelper.dotnet;

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
                            MaybeInvokeWow6432(RegisterContextMenuInterface);
                            break;

                        case "Remove":
                            MaybeInvokeWow6432(UnRegisterContextMenuInterface);
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
                            RegisterComInterface();
                            break;

                        case "Unregister":
                            UnregisterComInterface();
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
        private static void RegisterContextMenuInterface(bool wow6432)
        {
            CallRegAsmForShell(wow6432, "./ShellExtension/PDFCreatorShell.dll", "/codebase /tlb");
        }
        private static void UnRegisterContextMenuInterface(bool wow6432)
        {
            CallRegAsmForShell(wow6432, "./ShellExtension/PDFCreatorShell.dll", "/unregister");
        }

        private static void Usage()
        {
            Console.WriteLine("SetupHelper " + Assembly.GetExecutingAssembly().GetName().Version + "             © pdfforge");
            Console.WriteLine();
            Console.WriteLine("usage:");
            Console.WriteLine("SetupHelper.exe [/FileExtensions=Add|Remove] [/ComInterface=Register|Unregister]");
        }

        private static void RegisterComInterface()
        {
            if (HasSameDotnetCoreMayorVersion(GetInstalledDotNetCoreFrameworks(), new DotNetVersion(8, 0, 0, 0)))
            {
                CallRegSvr32("PDFCreator.COM.comhost.dll", "/s");
            }
            else
            {
                Console.WriteLine("DotNet Core 8 runtime not found");
            }
        }

        private static void UnregisterComInterface()
        {
            if (HasSameDotnetCoreMayorVersion(GetInstalledDotNetCoreFrameworks(), new DotNetVersion(8, 0, 0, 0)))
            {
                CallRegSvr32( "PDFCreator.COM.comhost.dll", "/u /s");
            }
            else
            {
                Console.WriteLine("DotNet Core 8 runtime not found");
            }
        }

        private static void CallRegSvr32(string fileName, string parameters)
        {
            var appDir = GetApplicationDirectory();
            var comDll = Path.Combine(appDir, fileName);
            
            var arguments = $"\"{comDll}\" {parameters}";
            var shellExecuteHelper = new ShellExecuteHelper();

            Console.WriteLine("Running RegSvr32 " + arguments);
            var result = shellExecuteHelper.RunAsAdmin("RegSvr32", arguments);
            Console.WriteLine(result.ToString());

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
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        private static IEnumerable<Framework> GetInstalledDotNetCoreFrameworks()
        {
            var dotNetCoreFramework = new List<Framework>();

            var regKey = Registry.LocalMachine.OpenSubKey(@"Software\WOW6432Node\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App");
            if (regKey != null)
            {
                foreach (var coreVersion in regKey.GetValueNames())
                {
                    dotNetCoreFramework.Add(new Framework(new DotNetVersion(coreVersion), 0, "Core_" + coreVersion,
                        Framework.Type.Core, Framework.Package.Full, Framework.Architecture.X64));
                }
            }

            return dotNetCoreFramework;
        }

        private static bool HasSameDotnetCoreMayorVersion(IEnumerable<Framework> frameworks, DotNetVersion version)
        {
            foreach (var framework in frameworks)
            {
                if (framework.Version.Major == version.Major)
                    return true;
            }
            return false;
        }
    }
}
