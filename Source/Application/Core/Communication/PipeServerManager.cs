using NLog;
using pdfforge.Communication;
using System.Diagnostics;

namespace pdfforge.PDFCreator.Core.Communication
{
    public interface IPipeServerManager
    {
        bool StartServer();

        bool IsServerRunning();

        void PrepareShutdown();

        void Shutdown();

        bool TrySendPipeMessage(string message);
    }

    public class PipeServerManager : IPipeServerManager
    {
        private readonly string _pipeName = "PDFCreator-" + Process.GetCurrentProcess().SessionId;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IPipeMessageHandler _newPipeJobHandler;

        private readonly PipeServer _pipeServer;

        public PipeServerManager(IPipeMessageHandler newPipeJobHandler)
        {
            _newPipeJobHandler = newPipeJobHandler;
            _pipeServer = new PipeServer(_pipeName, _pipeName);
        }

        public bool StartServer()
        {
            _logger.Debug("Starting pipe server thread");

            if (!_pipeServer.Start())
                return false;

            _pipeServer.OnNewMessage += (sender, args) => _newPipeJobHandler.HandlePipeMessage(args.Message);

            return true;
        }

        public bool TrySendPipeMessage(string message)
        {
            var pipeClient = new PipeClient(_pipeName);
            return pipeClient.SendMessage(message);
        }

        public bool IsServerRunning()
        {
            return _pipeServer.IsServerRunning();
        }

        public void PrepareShutdown()
        {
            _logger.Debug("Preparing PipeServer for ShutDown");
            _pipeServer.PrepareShutdown();
        }

        public void Shutdown()
        {
            _logger.Debug("Stopping pipe server");

            _pipeServer.Stop();
        }
    }
}
