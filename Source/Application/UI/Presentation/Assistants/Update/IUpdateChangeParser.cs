﻿using pdfforge.PDFCreator.Core.Services.Update;
using System.Collections.Generic;

namespace pdfforge.PDFCreator.UI.Presentation.Assistants.Update
{
    public interface IUpdateChangeParser
    {
        List<ReleaseInfo> Parse(string json);
    }
}
