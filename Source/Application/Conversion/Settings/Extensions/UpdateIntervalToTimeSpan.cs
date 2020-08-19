using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using pdfforge.PDFCreator.Conversion.Settings.Enums;

namespace pdfforge.PDFCreator.Conversion.Settings.Extensions
{
    public static class UpdateIntervalToTimeSpan
    {
        public static TimeSpan ToTimeSpan(this UpdateInterval interval)
        {
            switch (interval)
            {
                case UpdateInterval.Never:
                    return TimeSpan.MaxValue;
                case UpdateInterval.Daily:
                    return TimeSpan.FromDays(1);
                case UpdateInterval.Weekly:
                    return TimeSpan.FromDays(7);
                case UpdateInterval.Monthly:
                    var today = DateTime.Now;
                    return TimeSpan.FromDays(DateTime.DaysInMonth(today.Year, today.Month));

                default:
                    throw new ArgumentOutOfRangeException(nameof(interval), interval, null);
            }
        }
    }
}
