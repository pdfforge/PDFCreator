using System;
using SystemInterface;
using SystemWrapper;

namespace pdfforge.PDFCreator.Utilities.Tokens
{
    public class SingleEnvironmentToken : IToken
    {
        private readonly IEnvironment _environment;

        public SingleEnvironmentToken(EnvironmentVariable environmentVariableName)
            : this(environmentVariableName, new EnvironmentWrap())
        {
        }

        public SingleEnvironmentToken(EnvironmentVariable environmentVariableName, IEnvironment environment)
        {
            EnvironmentVariable = environmentVariableName;
            _environment = environment;
        }

        public EnvironmentVariable EnvironmentVariable { get; }

        public string GetValue()
        {
            switch (EnvironmentVariable)
            {
                case EnvironmentVariable.Username:
                    return _environment.UserName;

                case EnvironmentVariable.ComputerName:
                    return _environment.MachineName;

                case EnvironmentVariable.Desktop:
                    return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                case EnvironmentVariable.MyDocuments:
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                case EnvironmentVariable.MyPictures:
                    return Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }

            throw new ArgumentOutOfRangeException();
        }

        public string GetValueWithFormat(string formatString)
        {
            return GetValue();
        }

        public string GetName()
        {
            return EnvironmentVariable.ToString();
        }
    }

    public enum EnvironmentVariable
    {
        Username,
        ComputerName,
        Desktop,
        MyDocuments,
        MyPictures
    }
}
