namespace pdfforge.PDFCreator.Utilities
{
    public enum Edition
    {
        Free,
        Professional,
        TerminalServer,
        Custom,
        Server
    }

    public class EditionHelper
    {
        private Edition _currentEdition;
        public bool IsFreeEdition { get; private set; }
        public bool IsProfessional { get; private set; }
        public bool IsServer { get; private set; }
        public bool IsTerminalServer { get; private set; }
        public bool IsCustom { get; private set; }
        public bool AlwaysUsePdfArchitect { get; private set; }

        public Edition CurrentEdition
        {
            get => _currentEdition;
            set
            {
                _currentEdition = value;
                IsFreeEdition = _currentEdition == Edition.Free;
                IsProfessional = _currentEdition == Edition.Professional;
                IsServer = _currentEdition == Edition.Server;
                IsCustom = _currentEdition == Edition.Custom;
                IsTerminalServer = _currentEdition == Edition.TerminalServer;
            }
        }

        public EditionHelper(Edition edition, bool alwaysUsePdfArchitect = true)
        {
            CurrentEdition = edition;
            AlwaysUsePdfArchitect = alwaysUsePdfArchitect;
        }
    }
}
