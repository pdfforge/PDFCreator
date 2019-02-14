using System;

namespace pdfforge.PDFCreator.Utilities
{
    public class UpdateProgressChangedEventArgs : EventArgs
    {
        public bool Done { get; }
        public int Progress { get; }
        public long BytesReceived { get; }
        public long TotalBytesToReceive { get; }

        public UpdateProgressChangedEventArgs(bool done, int progress, long bytesReceived, long totalBytesToReceive)
        {
            Done = done;
            Progress = progress;
            BytesReceived = bytesReceived;
            TotalBytesToReceive = totalBytesToReceive;
        }
    }
}
