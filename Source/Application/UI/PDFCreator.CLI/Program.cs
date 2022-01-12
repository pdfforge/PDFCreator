using CommandLineParser;
using pdfforge.PDFCreator.UI.CLI.CommandExecutors;
using pdfforge.PDFCreator.UI.CLI.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.CLI
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // As a fallback, we support the old pdfcmon call without command name
            if (args.Any() && args[0].StartsWith("/InfoDataFile", StringComparison.InvariantCultureIgnoreCase))
            {
                var argList = new List<string>(args);
                argList.Insert(0, "NewPrintJob");
                args = argList.ToArray();
            }

            var parser = ConfigureParser();

            if (!args.Any())
            {
                Console.WriteLine(parser.CommandReference());
                return;
            }

            ICommand command;

            try
            {
                command = parser.Parse(args);
            }
            catch (ParseException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.HelpText);
                Environment.ExitCode = 1;
                return;
            }

            var commandResult = await ExecuteCommand(command, parser);

            Environment.ExitCode = commandResult.ExitCode;

            if (!commandResult.IsSuccess)
            {
                Console.WriteLine("The command was not successful: " + commandResult.Message);
            }
        }

        private static async Task<CommandResult> ExecuteCommand(ICommand command, CommandLineParser.CommandLineParser parser)
        {
            var executor = GetCommandExecutor(command, parser);
            executor.InitializeDependencies();

            var checkResult = executor.IsExecutable();
            if (!checkResult.IsExecutable)
                return CommandResult.Error(1, checkResult.Message);

            return await executor.Execute().ConfigureAwait(false);
        }

        private static ICommandExecutor GetCommandExecutor(ICommand command, CommandLineParser.CommandLineParser parser)
        {
            switch (command)
            {
                case NewPrintJobCommand c:
                    return new NewPrintJobExecutor(c);

                case StoreLicenseForAllUsersCommand c:
                    return new StoreLicenseForAllUsersExecutor(c);

                case InitializeDefaultSettingsCommand c:
                    return new InitializeDefaultSettingsExecutor(c);

                case InitializeSettingsCommand _:
                    return new InitializeSettingsExecutor();

                case RestorePrintersCommand _:
                    return new RestorePrintersExecutor();

                case PrintFileCommand c:
                    return new PrintFileCommandExecutor(c);

                case PrintFilesCommand c:
                    return new PrintFilesCommandExecutor(c);

                case ProcessFileCommand c:
                    return new ProcessFileCommandExecutor(c);

                case MergeAndProcessFilesCommand c:
                    return new MergeAndProcessFilesCommandExecutor(c);

                case HelpCommand c:
                    return new HelpCommandExecutor(c, parser);

                default: throw new NotImplementedException($"Executing the command {command.GetType().Name} is not implemented!");
            }
        }

        private static CommandLineParser.CommandLineParser ConfigureParser()
        {
            var parser = new CommandLineParser.CommandLineParser();

            parser.LineWidth = Console.BufferWidth;

            parser.AddCommand<NewPrintJobCommand>("NewPrintJob", cb => cb
                    .WithDescription("Inform PDFCreator about a new print job")
                    .WithParameter<string>("InfoDataFile", pb => pb
                                .IsRequired()
                                .WriteTo(c => c.InfFilePath)
                            )
                );

            parser.AddCommand<StoreLicenseForAllUsersCommand>("StoreLicenseForAllUsers", cb => cb
                .WithDescription("Store a license activation for all users in HKLM")
                .WithParameter<string>("LicenseServerCode", pb => pb
                    .IsRequired()
                    .WriteTo(c => c.LicenseServerCode))
                .WithParameter<string>("LicenseKey", pb => pb
                    .IsRequired()
                    .WriteTo(c => c.LicenseKey)
                )
            );

            parser.AddCommand<InitializeDefaultSettingsCommand>("InitializeDefaultSettings", cb => cb
                .WithDescription("Load the PDFCreator default settings that are used when initializing the settings for a user from an INI file")
                .WithParameter<string>("SettingsFile", pb => pb
                    .IsRequired()
                    .WriteTo(c => c.SettingsFile)
                )
            );

            parser.AddCommand<InitializeSettingsCommand>("InitializeSettings", cb => cb
                .WithDescription("Initialize the current user's registry with the default settings")
            );

            parser.AddCommand<RestorePrintersCommand>("RestorePrinters", cb => cb
                .WithDescription("Restore printers that are used in the settings, but do not exist on the machine")
            );

            parser.AddCommand<PrintFileCommand>("PrintFile", cb => cb
                .WithDescription("Print one file with the given printer and settings")
                .WithParameter<bool>("AllowSwitchDefaultPrinter", pb => pb
                    .IsOptional()
                    .WithDescription("When set, PDFCreator will be temporarily set as default printer if required")
                    .WriteTo(c => c.AllowSwitchDefaultPrinter)
                )
                .WithParameter<string>("Printer", pb => pb
                    .IsOptionalWithDefault("")
                    .WithDescription("The PDFCreator printer to use. If no printer is specified, the default PDFCreator printer will be used.")
                    .WriteTo(c => c.Printer)
                )
                .WithParameter<string>("Profile", pb => pb
                    .IsOptionalWithDefault("")
                    .WithDescription("Pre-select a profile in PDFCreator. Please note: This sets the profile for the next print job. This will break when sending multiple prints in parallel.")
                    .WriteTo(c => c.Profile)
                )
                .WithParameter<string>("File", pb => pb
                    .IsRequired()
                    .WithDescription("The file to print")
                    .WriteTo(c => c.File)
                )
                .WithParameter<string>("OutputFile", pb => pb
                    .IsOptionalWithDefault("")
                    .WithDescription("Pre-select the output file path in PDFCreator. Please note: This sets the output file for the next print job. This will break when sending multiple prints in parallel.")
                    .WriteTo(c => c.OutputFile)
                )
            );

            parser.AddCommand<PrintFilesCommand>("PrintFiles", cb => cb
                .WithDescription("Print multiple files with the given printer and settings")
                .WithParameter<bool>("AllowSwitchDefaultPrinter", pb => pb
                    .IsOptional()
                    .WithDescription("When set, PDFCreator will be temporarily set as default printer if required")
                    .WriteTo(c => c.AllowSwitchDefaultPrinter)
                )
                .WithParameter<string>("Printer", pb => pb
                    .IsOptional()
                    .WithDescription("The PDFCreator printer to use. If no printer is specified, the default PDFCreator printer will be used.")
                    .WriteTo(c => c.Printer)
                )
                .WithUnnamedParameterList<string>(pb => pb
                    .IsOptional()
                    .WithDescription("One or multiple files that will be printed.")
                    .WriteToList(c => c.Files)
                )
            );

            parser.AddCommand<ProcessFileCommand>("ProcessFile", cb => cb
                .WithDescription("Process a PDF or PS file with PDFCreator")
                .WithParameter<string>("Profile", pb => pb
                    .IsOptionalWithDefault("")
                    .WithDescription("Pre-select a profile in PDFCreator. You can use the profile name or the GUID here.")
                    .WriteTo(c => c.Profile)
                )
                .WithParameter<string>("OutputFile", pb => pb
                    .IsOptionalWithDefault("")
                    .WithDescription("Pre-select the output file path in PDFCreator.")
                    .WriteTo(c => c.OutputFile)
                )
                .WithParameter<string>("File", pb => pb
                    .IsRequired()
                    .WithDescription("The PDF or PS file to process with PDFCreator.")
                    .WriteTo(c => c.File)
                )
            );

            parser.AddCommand<MergeAndProcessFilesCommand>("MergeFiles", cb => cb
                .WithDescription("Merge and process multiple PDF or PS files with PDFCreator")
                .WithParameter<string>("Profile", pb => pb
                    .IsOptionalWithDefault("")
                    .WithDescription("Pre-select a profile in PDFCreator. You can use the profile name or the GUID here.")
                    .WriteTo(c => c.Profile)
                )
                .WithParameter<string>("OutputFile", pb => pb
                    .IsOptionalWithDefault("")
                    .WithDescription("Pre-select the output file path in PDFCreator.")
                    .WriteTo(c => c.OutputFile)
                )
                .WithUnnamedParameterList<string>(pb => pb
                    .IsOptional()
                    .WithDescription("One or multiple files that will be merged in the given order and processed with PDFCreator.")
                    .WriteToList(c => c.Files)
                )
            );

            parser.AddCommand<HelpCommand>("help", cb => cb
                .WithDescription("Shows an overview of all commands or details about a specific command with 'help [command-name]'")
                .WithUnnamedParameterList<string>(pb => pb.WriteToList(c => c.Parameters))
            );

            return parser;
        }
    }
}
