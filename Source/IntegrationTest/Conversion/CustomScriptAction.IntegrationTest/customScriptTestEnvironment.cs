using NUnit.Framework;
using pdfforge.CustomScriptAction;
using pdfforge.PDFCreator.Utilities;
using System.IO;

namespace CustomScriptActionIntegrationTest
{
    public enum TestScript
    {
        SetFilenameInPreConversionCreateBackUpInPostConversion
    }

    internal class CustomScriptTestEnvironment
    {
        public IAssemblyHelper AssemblyHelper { get; }
        public string AssemblyDir { get; }
        public string ExcpectedScriptFolder { get; }
        public string ScriptFilename { get; internal set; }
        public string ScriptFile { get; internal set; }

        private CustomScriptTestEnvironment()
        {
            AssemblyHelper = new AssemblyHelper(GetType().Assembly);

            AssemblyDir = AssemblyHelper.GetAssemblyDirectory();
            Assert.IsNotNull(AssemblyDir);

            ExcpectedScriptFolder = Path.Combine(AssemblyDir, CsScriptLoader.CsScriptsFolderName);
        }

        public static CustomScriptTestEnvironment Init(TestScript testScript)
        {
            var environment = new CustomScriptTestEnvironment();
            environment.CleanUp();
            environment.ScriptFilename = GetTestScriptFilename(testScript);
            environment.CopyTestScriptFormSourceToCurrentAssembly();
            return environment;
        }

        private static string GetTestScriptFilename(TestScript testScript)
        {
            switch (testScript)
            {
                case TestScript.SetFilenameInPreConversionCreateBackUpInPostConversion:
                    return "SetFilenameInPreConversionCreateBackUpInPostConversionScript.cs";
            }
            Assert.Fail("No TestscriptFilename for selected TestScriptEnum.");
            return "";
        }

        private string CopyTestScriptFormSourceToCurrentAssembly()
        {
            var testScriptSourceFile = GetTestScriptSourceFile(ScriptFilename);
            ScriptFile = Path.Combine(ExcpectedScriptFolder, ScriptFilename);

            Directory.CreateDirectory(ExcpectedScriptFolder);
            File.Copy(testScriptSourceFile, ScriptFile, true);

            Assert.IsTrue(File.Exists(ScriptFile), $"Test ScriptFile \"{ScriptFile}\" does not exist. Ensure that it gets copied into the AssemblyDir.");

            return ScriptFile;
        }

        private string GetTestScriptSourceFile(string testScriptFilename)
        {
            var partialScriptPath = Path.Combine(CsScriptLoader.CsScriptsFolderName, testScriptFilename);
            var candidates = new[]
            {
                Path.GetFullPath(Path.Combine(AssemblyDir, @"..\..", partialScriptPath)),
                Path.GetFullPath(Path.Combine(AssemblyDir, @"..\..\..", partialScriptPath)),
                Path.GetFullPath(Path.Combine(AssemblyDir, @"..\..\..\..", partialScriptPath)),
                Path.GetFullPath(Path.Combine(AssemblyDir, @"..\..\..\..\..", partialScriptPath)),
                Path.GetFullPath(Path.Combine(AssemblyDir, @"..\..\..\..\..\..", partialScriptPath)),
            };

            foreach (var file in candidates)
            {
                if (File.Exists(file))
                    return file;
            }

            Assert.Fail($"Ensure that you have a \"{CsScriptLoader.CsScriptsFolderName}\" with the \"{testScriptFilename}\" TestScript.");
            return "";
        }

        public void CleanUp()
        {
            try
            {
                if (Directory.Exists(ExcpectedScriptFolder))
                    Directory.Delete(ExcpectedScriptFolder, true);
            }
            catch (IOException)
            {
            }
        }
    }
}
