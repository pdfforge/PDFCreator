namespace pdfforge.SetupHelper.dotnet
{
    public class Framework
    {
        public enum Type { All, Framework, Core }
        public enum Architecture { AllArchitectures, X86, X64 }
        public enum Package { Full, Client }

        public DotNetVersion Version { get; }
        private int ServicePack { get; }
        public string Name { get; }
        private Type FrameworkType { get; }
        private Package FrameworkPackage { get; }
        private Architecture FrameworkArchitecture { get; }

        public Framework(DotNetVersion version, int servicePack, string name, Type type, Package package, Architecture architecture)
        {
            Version = version;
            ServicePack = servicePack;
            Name = name;
            FrameworkType = type;
            FrameworkPackage = package;
            FrameworkArchitecture = architecture;
        }
    }
}