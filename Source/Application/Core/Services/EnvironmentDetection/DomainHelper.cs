namespace pdfforge.PDFCreator.Core.Services.EnvironmentDetection;
public interface IDomainDetector
{
    bool ComputerIsPartOfDomain();
}

public class DomainDetector() : IDomainDetector
{
    public bool ComputerIsPartOfDomain()
    {
        return !string.IsNullOrEmpty(System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName);
    }
}
