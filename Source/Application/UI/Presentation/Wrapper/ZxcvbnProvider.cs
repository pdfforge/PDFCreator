using NLog;
using pdfforge.PDFCreator.Utilities;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Zxcvbn_cs;

namespace pdfforge.PDFCreator.UI.Presentation.Wrapper
{
    public class ZxcvbnProvider
    {
        private Zxcvbn _zxcvbn;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public async Task<Zxcvbn> GetInstanceAsync()
        {
            await _semaphoreSlim.RunSynchronized(() =>
            {
                if (_zxcvbn != null)
                    return;
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceNames = assembly.GetManifestResourceNames();

                    var resName = nameof(Properties.Resources.passwordsmatcher);
                    var resourceName = resourceNames.FirstOrDefault(x => x.Contains(resName));

                    if (resourceName == null)
                        return;

                    var stream = assembly.GetManifestResourceStream(resourceName);

                    _zxcvbn = new Zxcvbn(stream);
                }
                catch (Exception e)
                {
                    _logger.Warn($"Error while trying to load password dictionary for entropy check:{e.Message}");
                }
            });

            return _zxcvbn;
        }
    }
}
