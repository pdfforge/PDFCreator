namespace pdfforge.PDFCreator.UI.CLI.CommandExecutors
{
    public class CheckResult
    {
        public CheckResult(bool isExecutable, string message)
        {
            IsExecutable = isExecutable;
            Message = message;
        }

        public bool IsExecutable { get; }
        public string Message { get; }

        public static CheckResult Success() => new CheckResult(true, "");

        public static CheckResult Error(string message) => new CheckResult(false, message);
    }
}
