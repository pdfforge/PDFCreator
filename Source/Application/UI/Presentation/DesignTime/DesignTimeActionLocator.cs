using pdfforge.PDFCreator.Conversion.ActionsInterface;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Jobs.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.UI.Presentation.UserControls;
using System;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    public class DesignTimeAction : IAction
    {
        public ActionResult ProcessJob(Job job)
        {
            return new ActionResult();
        }

        public void ApplyPreSpecifiedTokens(Job job)
        { }

        public bool IsRestricted(ConversionProfile profile)
        {
            return false;
        }

        public void ApplyRestrictions(Job job)
        { }

        public ActionResult Check(ConversionProfile profile, CurrentCheckSettings settings, CheckLevel checkLevel)
        {
            return new ActionResult();
        }

        public bool IsEnabled(ConversionProfile profile)
        {
            return true;
        }

        public void SetIsEnabled(ConversionProfile profile, bool value)
        { }

        public IProfileSetting GetProfileSetting(ConversionProfile profile)
        {
            throw new NotImplementedException();
        }

        public void ReplaceWith(ConversionProfile profile, IProfileSetting value)
        {
            throw new NotImplementedException();
        }

        public IProfileSetting GetProfileSettingCopy(ConversionProfile profile)
        {
            throw new NotImplementedException();
        }

        public void SetProfileSetting(ConversionProfile profile, IProfileSetting value)
        {
            throw new NotImplementedException();
        }

        public Type SettingsType { get; }
    }

    public class DesignTimeActionLocator : IActionLocator
    {
        public IAction GetAction<T>() where T : class, IAction
        {
            return null;
        }
    }
}
