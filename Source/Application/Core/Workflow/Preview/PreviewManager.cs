using NLog;
using pdfforge.PDFCreator.Conversion.Jobs.JobInfo;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Core.Workflow
{
    public interface IPreviewManager
    {
        IList<Task<PreviewPages>> LaunchPreviewTasks(JobInfo jobInfo);

        Task<IList<PreviewPage>> GetTotalPreviewPages(JobInfo jobInfo);
        
        void AbortAndCleanUpPreview(IList<SourceFileInfo> sourceFileInfos);

        void AbortAndCleanUpPreview(string sfiFilename);
    }

    public class PreviewManager : IPreviewManager
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private const int MaxConcurrentTasks = 2;
        private readonly SemaphoreSlim _jobSemaphore = new SemaphoreSlim(MaxConcurrentTasks);

        private readonly IPsToPdfConverter _psToPdfConverter;
        private readonly IPdfToPreviewConverter _pdfToPreviewConverter;
        private readonly IDirectory _directory;

        public PreviewManager(IPsToPdfConverter psToPdfConverter, IPdfToPreviewConverter pdfToPreviewConverter, IDirectory directory)
        {
            _psToPdfConverter = psToPdfConverter;
            _pdfToPreviewConverter = pdfToPreviewConverter;
            _directory = directory;
        }

        private readonly Dictionary<string, (Task<PreviewPages> PreviewTask, CancellationTokenSource Cts)> _fileTaskMapping = new();

        public IList<Task<PreviewPages>> LaunchPreviewTasks(JobInfo jobInfo)
        {
            var previewTaskList = new List<Task<PreviewPages>>();
            foreach (var sfi in jobInfo.SourceFiles)
            {
                var key = GetFileTaskMappingKey(sfi.Filename);
                if (_fileTaskMapping.TryGetValue(key, out var taskCtsTuple))
                {
                    previewTaskList.Add(taskCtsTuple.PreviewTask);
                }
                else
                {
                    var cts = new CancellationTokenSource();
                    _fileTaskMapping[key] = (Task.Run(() => GeneratePreview(sfi, cts.Token)), cts);
                    previewTaskList.Add(_fileTaskMapping[key].PreviewTask);
                }
            }

            return previewTaskList;
        }

        private async Task<PreviewPages> GeneratePreview(SourceFileInfo sfi, CancellationToken cts)
        {
            _logger.Debug("Start generate preview for " + sfi.Filename);
            await _jobSemaphore.WaitAsync(cts);
            if (cts.IsCancellationRequested)
                return new PreviewPages(null);

            try
            {
                _logger.Debug("Generate intermediate pdf for " + sfi.Filename);
                await _psToPdfConverter.ConvertSourceFileToPdf(sfi);
                _logger.Debug("Generate preview images for " + sfi.Filename);
                var preview = await _pdfToPreviewConverter.GeneratePreviewPages(sfi.Filename, cts);
                _logger.Debug("Finished with preview for {0} ({1} pages, {2})", sfi.Filename, preview.PreviewPageList.Count, preview.Directory);
                return preview;
            }
            finally
            {
                _jobSemaphore.Release();
            }
        }

        public async Task<IList<PreviewPage>> GetTotalPreviewPages(JobInfo jobInfo)
        {
            var previewTaskList = LaunchPreviewTasks(jobInfo);

            var result = await Task.WhenAll(previewTaskList);
            return AssembleTotalPreviewPages(result.ToList());
        }

        private IList<PreviewPage> AssembleTotalPreviewPages(IList<PreviewPages> previewPagesList)
        {
            var totalPreviewPages = new List<PreviewPage>();
            var pageNumber = 0;
            foreach (var previewPages in previewPagesList)
            {
                totalPreviewPages.AddRange(previewPages.PreviewPageList.Select(pp =>
                {
                    pp.PageNumber = ++pageNumber;
                    return pp;
                }));
            }
            return totalPreviewPages;
        }

        public void AbortAndCleanUpPreview(IList<SourceFileInfo> sourceFileInfos)
        {
            foreach (var sfi in sourceFileInfos)
                AbortAndCleanUpPreview(sfi.Filename);
        }

        public void AbortAndCleanUpPreview(string sfiFilename)
        {
            var key = GetFileTaskMappingKey(sfiFilename);
            if (_fileTaskMapping.TryGetValue(key, out var taskCtsTuple))
            {
                taskCtsTuple.Cts.Cancel();
                _logger.Debug("Cancel preview task for " + sfiFilename);
                var directory = taskCtsTuple.PreviewTask.GetAwaiter().GetResult().Directory;
                if (directory != null && _directory.Exists(directory))
                {
                    try
                    {
                        _logger.Debug("Deleting preview directory " + directory);
                        _directory.Delete(directory, true);
                    }
                    catch
                    {
                        _logger.Warn("Unable do delete preview directory: " + directory);
                    }
                }
            }
            _fileTaskMapping.Remove(key);
        }

        private string GetFileTaskMappingKey(string sfiFilename)
        {
            return PathSafe.ChangeExtension(sfiFilename, "key");
        }
    }
}
