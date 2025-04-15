using System;
using System.Threading;
using System.Threading.Tasks;

namespace pdfforge.PDFCreator.UI.Presentation.Helper.Interfaces;

public interface ISetupDownloadHelper
{
    Task<bool> DownloadSetup(string updateInfoUrl, string sectionName, CancellationToken cancellationToken, Action<int> progress);
    Task<bool> StartDownloadedSetup();

}
