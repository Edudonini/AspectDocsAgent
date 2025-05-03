using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace AspectDocsAgent.Application
{
    internal class PdfHelper
    {
        public static string ExtractText(string pdfPath)
        {
            var sb = new StringBuilder();
            using var pdf = PdfDocument.Open(pdfPath);
            foreach (Page page in pdf.GetPages()) sb.AppendLine(page.Text);
            return sb.ToString();   
        }
    }
}
