using pdfforge.PDFCreator.Utilities.Process;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace pdfforge.PDFCreator.Core.Printing.Printing
{
    /// <summary>
    ///     The PrintCommandGroup provides a way to call multiple PrintCommands at the same time.
    /// </summary>
    public class PrintCommandGroup : IEnumerable<PrintCommand>
    {
        private readonly List<PrintCommand> _printCommands = new List<PrintCommand>();

        public ProcessWrapperFactory ProcessWrapperFactory { get; set; } = new ProcessWrapperFactory();

        /// <summary>
        ///     Cheks if all files are printable
        /// </summary>
        public bool IsPrintable
        {
            get { return UnprintableCommands.Count == 0; }
        }

        public List<PrintCommand> UnprintableCommands
        {
            get { return _printCommands.FindAll(p => !p.IsPrintable); }
        }

        /// <summary>
        ///     Checks if at least one file requires to change the default printer
        /// </summary>
        public bool RequiresDefaultPrinter
        {
            get { return _printCommands.Any(p => p.RequiresDefaultPrinter); }
        }

        /// <summary>
        ///     Get the PrintCommand with the given index
        /// </summary>
        /// <param name="i">index of the PrintCommand</param>
        /// <returns></returns>
        public PrintCommand this[int i]
        {
            get { return _printCommands[i]; }
            set { _printCommands[i] = value; }
        }

        /// <summary>
        ///     Get the Enumerator
        /// </summary>
        /// <returns>the Enumerator</returns>
        public IEnumerator<PrintCommand> GetEnumerator()
        {
            return _printCommands.GetEnumerator();
        }

        /// <summary>
        ///     Get the Enumerator
        /// </summary>
        /// <returns>the Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Add a print command to this group
        /// </summary>
        /// <param name="command">The PrintCommand to add</param>
        public void Add(PrintCommand command)
        {
            _printCommands.Add(command);
        }

        /// <summary>
        ///     Remove all unprintable files
        /// </summary>
        public void RemoveUnprintableCommands()
        {
            foreach (var command in UnprintableCommands)
            {
                _printCommands.Remove(command);
            }
        }

        public void RemoveAllCommands()
        {
            _printCommands.RemoveAll(x => true);
        }

        /// <summary>
        ///     Prints all items.
        /// </summary>
        /// <param name="processTimeout">The timeout the process will wait for the print job to succeed</param>
        /// <returns>true, if all items were converted successfully</returns>
        public bool PrintAll(TimeSpan processTimeout)
        {
            if (_printCommands.Any(p => p.CommandType == PrintType.Unprintable))
                throw new InvalidOperationException("The list of print commands contains unprintable files");

            foreach (var p in _printCommands.ToArray())
            {
                p.ProcessWrapperFactory = ProcessWrapperFactory;

                var success = p.Print(processTimeout);
                if (!success)
                    return false;
            }
            return true;
        }

        /// <summary>
        ///     Prints all items.
        /// </summary>
        /// <param name="processTimeout">The timeout in seconds the process will wait for the print job to succeed</param>
        /// <returns>true, if all items were converted successfully</returns>
        public bool PrintAll(int processTimeout)
        {
            return PrintAll(TimeSpan.FromSeconds(processTimeout));
        }

        /// <summary>
        ///     Finds all items matching the predicate
        /// </summary>
        /// <param name="match">Predicate to match</param>
        /// <returns>A list of matches</returns>
        public IList<PrintCommand> FindAll(Predicate<PrintCommand> match)
        {
            return _printCommands.FindAll(match);
        }
    }
}
