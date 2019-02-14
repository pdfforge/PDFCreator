using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Core.Services.Translation;
using pdfforge.PDFCreator.UI.Interactions;
using pdfforge.PDFCreator.UI.Interactions.Enums;
using pdfforge.PDFCreator.UI.Presentation.DesignTime.Helper;
using pdfforge.PDFCreator.UI.Presentation.Helper;
using pdfforge.PDFCreator.UI.Presentation.UserControls.Misc;
using Translatable;

namespace pdfforge.PDFCreator.UI.Presentation.DesignTime
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class DesignTimeMessageViewModel : MessageViewModel
    {
        public DesignTimeMessageViewModel() : this(true) //Set true to view Errors in Designer
        { }

        public DesignTimeMessageViewModel(bool withErrors) : base(new DesignTimeTranslationUpdater(), new DesignTimeSoundPlayer(), new ErrorCodeInterpreter(new TranslationFactory()), null)
        {
            var messageInteraction = new MessageInteraction("The Message is Love", "The Title", MessageOptions.OK, MessageIcon.PDFForge);

            if (withErrors)
            {
                var actionResultDict = new ActionResultDict();
                var actionResult = new ActionResult(ErrorCode.Conversion_UnknownError) { ErrorCode.Conversion_UnknownError };
                actionResultDict.Add("ProfileName ", actionResult);
                actionResultDict.Add("Some other Profile", actionResult);
                messageInteraction = new MessageInteraction("You have defective or incomplete settings.", "The Title", MessageOptions.YesNoCancel, MessageIcon.Warning,
                    actionResultDict, "Are you sure you want to proceed?");
            }

            SetInteraction(messageInteraction);
        }
    }
}
