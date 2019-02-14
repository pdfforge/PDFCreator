namespace pdfforge.PDFCreator.Core.ServiceLocator
{
    /// <summary>
    ///  IWhitelistedServiceLocator can only create instanced of the type <see cref="IWhitelisted"/>. This restriction ensures that we can still verify our DI container.
    /// </summary>
    public interface IWhitelistedServiceLocator
    {
        T GetInstance<T>() where T : class, IWhitelisted;
    }
}
