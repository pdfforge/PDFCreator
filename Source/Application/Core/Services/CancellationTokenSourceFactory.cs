using System;
using System.Threading;

namespace pdfforge.PDFCreator.Core.Services
{
    public interface ICancellationTokenSourceFactory
    {
        CancellationTokenSource CreateSource(int millisecondsDelay);

        CancellationTokenSource CreateSource(TimeSpan delay);

        CancellationTokenSource CreateSource();
    }

    public class CancellationTokenSourceFactory : ICancellationTokenSourceFactory
    {
        public CancellationTokenSource CreateSource(int millisecondsDelay)
        {
            return new CancellationTokenSource(millisecondsDelay);
        }

        public CancellationTokenSource CreateSource(TimeSpan delay)
        {
            return new CancellationTokenSource(delay);
        }

        public CancellationTokenSource CreateSource()
        {
            return new CancellationTokenSource();
        }
    }
}
