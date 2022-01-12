using System;
using SystemInterface;
using SystemWrapper;

namespace pdfforge.PDFCreator.Utilities.Tokens
{
    public class EnvironmentToken : IToken
    {
        private readonly IEnvironment _environment;
        private readonly string _name;

        public EnvironmentToken() : this(new EnvironmentWrap())
        {
        }

        public EnvironmentToken(IEnvironment environment, string name = "Environment")
        {
            _environment = environment;
            _name = name;
        }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="name">Token Name</param>
        public EnvironmentToken(string name) : this(new EnvironmentWrap(), name)
        {
        }

        /// <summary>
        ///     Returns value of Token
        /// </summary>
        /// <returns>Value of Token as string</returns>
        public string GetValue()
        {
            return "";
        }

        /// <summary>
        ///     Returns Value of Token in given C#-format
        /// </summary>
        /// <param name="formatString">C#-format String</param>
        /// <returns>Formated Value as string</returns>
        public string GetValueWithFormat(string formatString)
        {
            var environmentOrder = new[] { EnvironmentVariableTarget.User, EnvironmentVariableTarget.Machine, EnvironmentVariableTarget.Process };

            try
            {
                foreach (var environment in environmentOrder)
                {
                    var value = _environment.GetEnvironmentVariable(formatString, environment);
                    if (!string.IsNullOrWhiteSpace(value))
                        return value;
                }

                return "";
            }
            catch (ArgumentNullException)
            {
                return "";
            }
        }

        /// <summary>
        ///     Returns Name of Token
        /// </summary>
        /// <returns>Name of Token as String</returns>
        public string GetName()
        {
            return _name;
        }
    }
}
