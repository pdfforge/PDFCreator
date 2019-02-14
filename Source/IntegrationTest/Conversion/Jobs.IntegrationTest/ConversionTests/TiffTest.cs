using BitMiracle.LibTiff.Classic;
using NUnit.Framework;
using PDFCreator.TestUtilities;
using pdfforge.PDFCreator.Conversion.Settings.Enums;
using System;
using System.IO;

namespace pdfforge.PDFCreator.IntegrationTest.Conversion.Jobs.ConversionTests
{
    [TestFixture]
    internal class TiffTest
    {
        [SetUp]
        public void SetUp()
        {
            var bootstrapper = new IntegrationTestBootstrapper();
            var container = bootstrapper.ConfigureContainer();
            _th = container.GetInstance<TestHelper>();
            _th.InitTempFolder("TiffTest");
        }

        [TearDown]
        public void CleanUp()
        {
            _th.CleanUp();
        }

        private TestHelper _th;

        private static byte[] GetBytesFromFile(string fullFilePath)
        {
            // this method is limited to 2^32 byte files (4.2 GB)
            var fs = File.OpenRead(fullFilePath);
            try
            {
                var bytes = new byte[32];
                fs.Read(bytes, 0, Convert.ToInt32(32));
                return bytes;
            }
            finally
            {
                fs.Close();
            }
        }

        public void MakeTiffTest()
        {
            _th.GenerateGsJob(PSfiles.ThreePDFCreatorTestpages, OutputFormat.Tif);
            _th.RunGsJob();

            Assert.IsTrue(_th.Job.Completed, "The job has not completed yet!");

            Assert.AreEqual(1, _th.Job.OutputFiles.Count, "More than one Tiff-File");

            var bytes = GetBytesFromFile(_th.Job.OutputFiles[0]);

            //check if tif
            if ((bytes[0] == 'I') && (bytes[1] == 'I')) //little-endian
            {
                Assert.AreEqual(42, bytes[2], "No Tiff-Form: 3rd byte is not 42 (little-endian)");
                Assert.AreEqual(0, bytes[3], "No Tiff-Form: 4th byte is not 0 (little-endian)");
            }
            else if ((bytes[0] == 'M') && (bytes[1] == 'M')) //big-endian
            {
                Assert.AreEqual(0, bytes[2], "No Tiff-Form: 3rd byte is not 0 (big-endian)");
                Assert.AreEqual(42, bytes[3], "No Tiff-Form: 4th byte is not 42 (big-endian)"); //42 is 4th Byte!
            }
            else
                Assert.IsTrue(false, "No Tiff-Form: 1st and 2nd Byte are not \"II\" or \"MM\"");

            using (var image = Tiff.Open(_th.Job.OutputFiles[0], "r"))
            {
                int pages = image.NumberOfDirectories();

                Assert.AreEqual(3, pages);

                for (var i = 0; i < pages; ++i)
                {
                    image.SetDirectory((short)i);

                    Assert.AreEqual(image.GetField(TiffTag.XRESOLUTION)[0].ToInt(),
                        image.GetField(TiffTag.YRESOLUTION)[0].ToInt(),
                        "vertical resolution != horizontal resolution on page " + (i + 1));
                    Assert.AreEqual(_th.Profile.TiffSettings.Dpi, image.GetField(TiffTag.XRESOLUTION)[0].ToInt(), 0.013,
                        "vertical resolution != choosen dpi on page " + (i + 1));

                    var expectedBitsPerPixel = 0;
                    var expectedCompression = Compression.NONE;

                    switch (_th.Profile.TiffSettings.Color)
                    {
                        case TiffColor.BlackWhiteG4Fax:
                            expectedBitsPerPixel = 1;
                            expectedCompression = Compression.CCITT_T6;
                            break;

                        case TiffColor.BlackWhiteG3Fax:
                            expectedBitsPerPixel = 1;
                            expectedCompression = Compression.CCITT_T4;
                            break;

                        case TiffColor.BlackWhiteLzw:
                            expectedBitsPerPixel = 1;
                            expectedCompression = Compression.LZW;
                            break;

                        case TiffColor.Gray8Bit:
                            expectedBitsPerPixel = 8;
                            expectedCompression = Compression.LZW;
                            break;

                        case TiffColor.Color12Bit:
                            expectedBitsPerPixel = 4;
                            expectedCompression = Compression.LZW;
                            break;

                        case TiffColor.Color24Bit:
                            expectedBitsPerPixel = 8;
                            expectedCompression = Compression.LZW;
                            break;
                    }

                    var actualBitsPerPixel = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                    var actualCompression = image.GetField(TiffTag.COMPRESSION)[0].Value;

                    Assert.AreEqual(expectedBitsPerPixel, actualBitsPerPixel, "Colormode: 8 BitsPerPixel required, but was " + actualBitsPerPixel + " on page " + (i + 1));
                    Assert.AreEqual(expectedCompression, actualCompression, "Compression does not match the configured value");
                }
            }
        }

        [Test]
        public void Test_BlackWhiteG3Fax()
        {
            _th.Profile.TiffSettings.Color = TiffColor.BlackWhiteG3Fax;
            _th.Profile.TiffSettings.Dpi = 304;
            MakeTiffTest();
        }

        [Test]
        public void Test_BlackWhiteG4Fax()
        {
            _th.Profile.TiffSettings.Color = TiffColor.BlackWhiteG4Fax;
            _th.Profile.TiffSettings.Dpi = 303;
            MakeTiffTest();
        }

        [Test]
        public void Test_BlackWhiteLzw()
        {
            _th.Profile.TiffSettings.Color = TiffColor.BlackWhiteLzw;
            _th.Profile.TiffSettings.Dpi = 305;
            MakeTiffTest();
        }

        [Test]
        public void Test_Color12Bit()
        {
            _th.Profile.TiffSettings.Color = TiffColor.Color12Bit;
            _th.Profile.TiffSettings.Dpi = 601;
            MakeTiffTest();
        }

        [Test]
        public void Test_Color24Bit()
        {
            _th.Profile.TiffSettings.Color = TiffColor.Color24Bit;
            _th.Profile.TiffSettings.Dpi = 257;
            MakeTiffTest();
        }

        [Test]
        public void Test_Grayscale()
        {
            _th.Profile.TiffSettings.Color = TiffColor.Gray8Bit;
            _th.Profile.TiffSettings.Dpi = 300;
            MakeTiffTest();
        }
    }
}
