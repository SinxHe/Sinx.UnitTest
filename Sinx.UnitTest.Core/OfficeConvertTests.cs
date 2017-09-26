using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Aspose.Words;
using Xunit;

namespace Sinx.UnitTest.Core
{
    public class OfficeConvertTests
    {
        [Fact]
        public void Word2PdfTest()
        {
	        var stream = new FileStream(@"C:\Users\SinxHe\Desktop\LINQ.docx", FileMode.Open);
	        var pathDst = @"C:\Users\SinxHe\Desktop\LINQ.pdf";
	        new Document(stream).Save(pathDst, SaveFormat.Pdf);
	        File.WriteAllText(pathDst, Regex.Replace(File.ReadAllText(pathDst), "(Evaluation Only\\. Created with Aspose\\.(.+?)\\. Copyright \\d+-\\d+ Aspose Pty Ltd\\.)|(This document was truncated here because it was created in the Evaluation Mode\\.)", ""));
		}
    }
}
