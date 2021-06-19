using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using System;
using System.Linq;

namespace pdfforge.PDFCreator.Conversion.Actions.Actions
{
    public abstract class RetypePasswordActionBase<TSetting> : ActionBase<TSetting> where TSetting : class, IProfileSetting
    {
        protected RetypePasswordActionBase(Func<ConversionProfile, TSetting> settingsGetter)
            : base(settingsGetter)
        { }

        protected abstract ActionResult DoActionProcessing(Job job);

        //todo: Test this
        protected override ActionResult DoProcessJob(Job job)
        {
            var settings = new CurrentCheckSettings(job.AvailableProfiles, job.PrinterMappings, job.Accounts);
            var actionResult = Check(job.Profile, settings, CheckLevel.RunningJob);
            if (!actionResult)
                return actionResult;

            actionResult = DoActionProcessing(job);

            while (actionResult.Contains(ErrorCode.PasswordAction_Login_Error))
            {
                LoginQueryResult? abortReason = null;

                job.OnErrorDuringLogin(password => SetPassword(job, password),
                                        reason => abortReason = reason, PasswordText);

                if (abortReason != null)
                {
                    if (abortReason == LoginQueryResult.AbortedByUser)
                    {
                        // if the user decides to abort, we don't want to see this as error. If there is no way to requery (i.e. during autosave), we want to keep this error!
                        actionResult.Remove(actionResult.First(code => code == ErrorCode.PasswordAction_Login_Error));
                    }

                    break;
                }

                actionResult = DoActionProcessing(job);
            }

            return actionResult;
        }

        protected abstract void SetPassword(Job job, string password);

        protected abstract string PasswordText { get; }
    }
}
