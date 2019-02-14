using iTextSharp.text.pdf;
using NLog;
using pdfforge.PDFCreator.Conversion.Jobs;
using pdfforge.PDFCreator.Conversion.Settings;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.Globalization;
using System.Text;
using System.Xml;

namespace pdfforge.PDFCreator.Conversion.Processing.ITextProcessing
{
    internal class XmpMetadataUpdater
    {
        //ActionId = 27;
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Update XMP metadata for PDF/A files.
        ///     Function does nothing for other formats.
        /// </summary>
        /// <param name="stamper">Stamper with document</param>
        /// <param name="profile">Profile with output format settings</param>
        /// <exception cref="ProcessingException">In case of any error</exception>
        internal void UpdateXmpMetadata(PdfStamper stamper, ConversionProfile profile)
        {
            try
            {
                DoUpdateXmpMetadata(stamper, profile);
            }
            catch (Exception ex)
            {
                throw new ProcessingException(ex.GetType() + " while addding updating xmp metadata:" + Environment.NewLine + ex.Message, ErrorCode.Processing_GenericError, ex);
            }
        }

        private Tuple<int, string> GetPdfAConformance(OutputFormat outputFormat)
        {
            if (!outputFormat.ToString().ToUpper().StartsWith("PDFA"))
                return null;

            switch (outputFormat)
            {
                case OutputFormat.PdfA1B: return Tuple.Create(1, "B");
                case OutputFormat.PdfA2B: return Tuple.Create(2, "B");
                case OutputFormat.PdfA3B: return Tuple.Create(3, "B");
                default: throw new NotImplementedException($"Determining conformance for {outputFormat} was not implemented");
            }
        }

        private void DoUpdateXmpMetadata(PdfStamper stamper, ConversionProfile profile)
        {
            var conformance = GetPdfAConformance(profile.OutputFormat);

            if (conformance == null)
                return;

            Logger.Debug("Start updateing XMP Metadata for PDF/A");

            var ms = new PDFMetadataStrings("", "", "", "", "", "", "", "");
            PdfReader reader = stamper.Reader;

            var doc = new XmlDocument();
            string xmlMetadataString = Encoding.UTF8.GetString(reader.Metadata);
            doc.LoadXml(xmlMetadataString);

            var documentIDs = (PdfArray)(reader.Trailer.Get(PdfName.ID));

            string sDocumentId = GetHexString(GetRandomString(16));
            if (documentIDs != null)
            {
                var o = documentIDs.ArrayList[0];
                string s = o.ToString();
                if (s.Length > 0)
                {
                    sDocumentId = GetHexString(s);
                }
            }

            string xmlAuthor = "";
            if (reader.Info.ContainsKey("Author"))
            {
                ms.Author = reader.Info["Author"];
                xmlAuthor = "    <dc:creator>\n" +
                            "     <rdf:Seq>\n" +
                            "      <rdf:li>" + ms.Author + "</rdf:li>\n" +
                            "     </rdf:Seq>\n" +
                            "    </dc:creator>\n";
            }

            if (reader.Info.ContainsKey("CreationDate"))
                ms.CreationDate = reader.Info["CreationDate"];

            if (reader.Info.ContainsKey("Creator"))
                ms.Creator = reader.Info["Creator"];

            string xmlKeywords = "", xmlKeywords2 = "";
            if (reader.Info.ContainsKey("Keywords"))
            {
                ms.Keywords = reader.Info["Keywords"];
                xmlKeywords = "    <dc:subject>\n" +
                              "     <rdf:Bag>\n" +
                              "      <rdf:li>" + ms.Keywords + "</rdf:li>\n" +
                              "     </rdf:Bag>\n" +
                              "    </dc:subject>\n";
                xmlKeywords2 = "    <pdf:Keywords>" + ms.Keywords + "</pdf:Keywords>\n";
            }

            if (reader.Info.ContainsKey("ModDate"))
                ms.ModDate = reader.Info["ModDate"];

            if (reader.Info.ContainsKey("Producer"))
                ms.Producer = reader.Info["Producer"];

            string xmlSubject = "";
            if (reader.Info.ContainsKey("Subject"))
            {
                ms.Subject = reader.Info["Subject"];
                xmlSubject = "    <dc:description>\n" +
                             "     <rdf:Alt>\n" +
                             "      <rdf:li xml:lang='x-default'>" + ms.Subject + "</rdf:li>\n" +
                             "     </rdf:Alt>\n" +
                             "    </dc:description>\n";
            }

            string xmlTitle = "";
            if (reader.Info.ContainsKey("Title"))
            {
                ms.Title = reader.Info["Title"];
                xmlTitle = "    <dc:title>\n" +
                           "     <rdf:Alt>\n" +
                           "      <rdf:li xml:lang='x-default'>" + ms.Title + "</rdf:li>\n" +
                           "     </rdf:Alt>\n" +
                           "    </dc:title>\n";
            }

            string metadataStr = "<?xpacket begin='﻿' id='W5M0MpCehiHzreSzNTczkc9d'?>\n" +
                                 " <x:xmpmeta xmlns:x='adobe:ns:meta/' x:xmptk='Adobe XMP Core 4.2.1-c041 52.342996, 2008/05/07-20:48:00'>\n" +
                                 "  <rdf:RDF xmlns:rdf='http://www.w3.org/1999/02/22-rdf-syntax-ns#'>\n" +
                                 "   <rdf:Description rdf:about='' xmlns:pdfaid='http://www.aiim.org/pdfa/ns/id/' pdfaid:part='" + conformance.Item1 + "' pdfaid:conformance='" + conformance.Item2 + "'>\n" +
                                 "   </rdf:Description>\n" +
                                 "   <rdf:Description rdf:about=''\n" +
                                 "     xmlns:xmp='http://ns.adobe.com/xap/1.0/'>\n" +
                                 "    <xmp:CreateDate>" + GetXmpDate(ms.CreationDate) + "</xmp:CreateDate>\n" +
                                 "    <xmp:ModifyDate>" + GetXmpDate(ms.ModDate) + "</xmp:ModifyDate>\n" +
                                 "    <xmp:CreatorTool>" + ms.Creator + "</xmp:CreatorTool>\n" +
                                 "   </rdf:Description>\n" +
                                 "   <rdf:Description rdf:about=''\n" +
                                 "     xmlns:dc='http://purl.org/dc/elements/1.1/'>\n" +
                                 "    <dc:format>application/pdf</dc:format>\n" + xmlTitle + xmlSubject + xmlAuthor + xmlKeywords +
                                 "   </rdf:Description>\n" +
                                 "   <rdf:Description rdf:about=''\n" +
                                 "     xmlns:xmpMM='http://ns.adobe.com/xap/1.0/mm/'\n" +
                                 "     xmlns:stEvt='http://ns.adobe.com/xap/1.0/sType/ResourceEvent#'>\n" +
                                 "    <xmpMM:DocumentID>uuid:" + sDocumentId + "</xmpMM:DocumentID>\n" +
                                 "    <xmpMM:History><rdf:Seq><rdf:li rdf:parseType='Resource'></rdf:li></rdf:Seq></xmpMM:History>\n" +
                                 "   </rdf:Description>\n" +
                                 "   <rdf:Description rdf:about=''\n" +
                                 "     xmlns:pdf='http://ns.adobe.com/pdf/1.3/'>\n" +
                                 "    <pdf:Producer>" + ms.Producer + "</pdf:Producer>\n" +
                                 xmlKeywords2 +
                                 "   </rdf:Description>\n" +
                                 "  </rdf:RDF>\n" +
                                 " </x:xmpmeta>\n" +
                                 "<?xpacket end='w'?>";

            Encoding textEncoding = Encoding.GetEncoding("iso-8859-1");
            byte[] newMetadata = Encoding.Convert(Encoding.Default, Encoding.UTF8, textEncoding.GetBytes(metadataStr));

            stamper.Writer.XmpMetadata = newMetadata;
        }

        /// <summary>
        ///     Convert a string to a hex string.
        /// </summary>
        /// <param name="s">
        ///     A string to convert.
        /// </param>
        /// <returns>
        ///     The hex string.
        /// </returns>
        private string GetHexString(string s)
        {
            var sb = new StringBuilder();
            foreach (var t in s)
            {
                var ts = Convert.ToInt32(t).ToString("x");
                if (ts.Length == 1) ts = "0" + ts;
                sb.Append(ts);
            }
            s = sb.ToString().ToUpper();
            return s;
        }

        /// <summary>
        ///     Get a string with random chars.
        /// </summary>
        /// <param name="length">Length of he random string.</param>
        /// <returns>A string that contains random chars.</returns>
        private string GetRandomString(int length)
        {
            var rnd = new Random();
            var sb = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                sb.Append(Convert.ToChar((byte)rnd.Next(254)).ToString(CultureInfo.InvariantCulture));
            }
            return sb.ToString();
        }

        /// <summary>
        ///     Converts a metadata date to a xmp date.
        /// </summary>
        /// <param name="metadataDate">
        ///     A string with a metadata date.
        /// </param>
        /// <returns>
        ///     The xmp date.
        /// </returns>
        private string GetXmpDate(string metadataDate)
        {
            if (metadataDate.Length != 23)
                return metadataDate;

            var s = metadataDate.Substring(2, 4) + "-" + metadataDate.Substring(6, 2) + "-" + metadataDate.Substring(8, 2) + "T" +
                    metadataDate.Substring(10, 2) + ":" + metadataDate.Substring(12, 2) + ":" + metadataDate.Substring(14, 2) + "+" +
                    metadataDate.Substring(17, 2) + ":" + metadataDate.Substring(20, 2);

            return s;
        }

        private struct PDFMetadataStrings
        {
            public string Author, CreationDate, Creator, Keywords, ModDate, Producer, Subject, Title;

            public PDFMetadataStrings(string author, string creationDate, string creator, string keywords, string modDate, string producer, string subject, string title)
            {
                Author = author;
                CreationDate = creationDate;
                Creator = creator;
                Keywords = keywords;
                ModDate = modDate;
                Producer = producer;
                Subject = subject;
                Title = title;
            }
        }
    }
}
