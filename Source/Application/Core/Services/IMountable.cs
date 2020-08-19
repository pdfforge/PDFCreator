using System.Threading.Tasks;

namespace pdfforge.PDFCreator.Core.Services
{
    public interface IMountable
    {
        void MountView();

        void UnmountView();
    }

    public interface IMountableAsync
    {
        Task MountViewAsync();

        Task UnmountViewAsync();
    }
}
