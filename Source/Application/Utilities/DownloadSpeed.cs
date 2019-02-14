using System;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.Utilities
{
    public class DownloadSpeed
    {
        private readonly object _lockObject = new object();
        private readonly List<SpeedItem> _speedItems = new List<SpeedItem>();
        private int _capacity = 10000;
        private long _lastByteCount;
        private DateTime _lastProgressEvent;
        private long _totalBytes;

        public DownloadSpeed(DateTime startTime)
        {
            _lastProgressEvent = startTime;
        }

        public DownloadSpeed()
            : this(DateTime.Now)
        {
        }

        public int Capacity
        {
            get { return _capacity; }
            set
            {
                _capacity = value;
                CleanUpSpeedItems();
            }
        }

        public double BytesPerSecond { get; private set; }

        public TimeSpan EstimatedDuration
        {
            get
            {
                var seconds = _totalBytes / BytesPerSecond;

                try
                {
                    return TimeSpan.FromSeconds(seconds);
                }
                catch (Exception)
                {
                    return TimeSpan.FromHours(1);
                }
            }
        }

        public TimeSpan EstimatedRemainingDuration
        {
            get
            {
                var seconds = (_totalBytes - _lastByteCount) / BytesPerSecond;

                try
                {
                    return TimeSpan.FromSeconds(seconds);
                }
                catch (Exception)
                {
                    return TimeSpan.FromHours(1);
                }
            }
        }

        public void DownloadProgressChanged(object sender, UpdateProgressChangedEventArgs e)
        {
            ReportProgress(DateTime.Now, e.TotalBytesToReceive, e.BytesReceived);
        }

        public void webClient_DownloadFileCompleted(object sender, UpdateProgressChangedEventArgs e)
        {
            Reset();
        }

        private void CalculateSpeed()
        {
            var totalDuration = new TimeSpan(0);
            long totalBytes = 0;

            SpeedItem[] items;

            lock (_lockObject)
            {
                items = _speedItems.ToArray();
            }

            foreach (var speedItem in items)
            {
                totalDuration += speedItem.Duration;
                totalBytes += speedItem.Bytes;
            }

            BytesPerSecond = totalBytes / totalDuration.TotalSeconds;

            if (double.IsNaN(BytesPerSecond))
                BytesPerSecond = 0;
        }

        /// <summary>
        ///     Report a download progress
        /// </summary>
        /// <param name="time">The current time when this took place</param>
        /// <param name="totalBytes">The total number of bytes that are transferred</param>
        /// <param name="bytesReceived">The total number of bytes received so far</param>
        public void ReportProgress(DateTime time, long totalBytes, long bytesReceived)
        {
            _totalBytes = totalBytes;

            var bytes = bytesReceived - _lastByteCount;
            var duration = time - _lastProgressEvent;

            var speed = new SpeedItem(bytes, duration);
            AddItem(speed);

            _lastByteCount = bytesReceived;
            _lastProgressEvent = time;

            CalculateSpeed();
        }

        private void AddItem(SpeedItem speedItem)
        {
            lock (_lockObject)
            {
                _speedItems.Insert(0, speedItem);

                CleanUpSpeedItems();
            }
        }

        /// <summary>
        ///     remove items that are over the capacity
        /// </summary>
        private void CleanUpSpeedItems()
        {
            lock (_lockObject)
            {
                if (_speedItems.Count > _capacity)
                {
                    var overCapactiy = _speedItems.Count - _capacity;
                    _speedItems.RemoveRange(_capacity, overCapactiy);
                }
            }
        }

        public void Reset()
        {
            _speedItems.Clear();
            CalculateSpeed();
        }
    }

    public class SpeedItem
    {
        public SpeedItem(long bytes, TimeSpan duration)
        {
            Bytes = bytes;
            Duration = duration;
        }

        public long Bytes { get; }
        public TimeSpan Duration { get; }
    }
}
