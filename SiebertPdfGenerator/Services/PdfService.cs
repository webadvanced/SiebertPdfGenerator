using SiebertPdfGenerator.Business;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using SiebertPdfGenerator.Models;
using SiebertPdfGenerator.Common;
using NReco.PdfGenerator;

namespace SiebertPdfGenerator.Services
{
    public static class PdfService
    {
        public static void GetSubmissionPdfPackage(List<SuitabilityModel> suitabilityModels)
        {
            var documents = new List<Document>();
            //var counts = 0;
            foreach (var suitabilityModel in suitabilityModels)
            {
                var document = ToSubmissionPdfPackage(suitabilityModel);
                if(document != null)
                    documents.Add(document);
                //counts++;
                //if (counts == 32)
                //    break;
            }
            var count = int.Parse(ConfigurationManager.AppSettings["pdfSplit"]);

            //ToDo: split documents and combine pdfs 
            var splitDocuments = Helpers.ChunkBy(documents, count);
            
            for(int i = 0; i < splitDocuments.Count; i++)
            {
                var page = i + 1;
                CombinePdfs(page.ToString(), splitDocuments[i], true);
            }
        }

        private static Document ToSubmissionPdfPackage(SuitabilityModel suitabilityModel)
        {
            IEnumerable<Document> documents = HtmlToPdf(suitabilityModel);

            if (documents != null)
                return CombinePdfs(suitabilityModel.AccountNum, documents, false);
            return null;
        }

        private static IEnumerable<Document> HtmlToPdf(SuitabilityModel suitabilityModel)
        {
            var rootUrl = ConfigurationManager.AppSettings["root"];
            var fileName = suitabilityModel.AccountNum + "page";
            var documents = new List<Document>();

            for (int page = 1; page < 4; page++)
            {
                var document = new Document()
                {
                    Completed = true,
                    RelativeFilePath = SaveHtmlAsPdf(suitabilityModel, page, fileName + page + ConfigurationManager.AppSettings["FileSuffix"])
                };

                documents.Add(document);
            }
            
            return documents;
        }

        private static string SaveHtmlAsPdf(SuitabilityModel suitabilityModel, int page, string fileName)
        {
            var (filePath, relativeFilePath) = GetApplicationFilePath(fileName, false);

            if (File.Exists(filePath))
            {
                return relativeFilePath;
            }

            var htmlToPdf = new HtmlToPdfConverter();
            //htmlToPdf.CustomWkHtmlArgs = "--dpi 110";
            if (page == 1)
                htmlToPdf.Margins = new PageMargins { Top = 55.372f, Bottom = 19.812f, Left = 19.812f, Right = 19.812f };

            var stream = new MemoryStream();
            htmlToPdf.GeneratePdf(FileImporter.GetPageHtml(suitabilityModel, page), null, stream);
            var relativePath = SaveApplicationPdfStream(stream, fileName);
            return relativePath;
        }        

        private static string SaveApplicationPdfStream(Stream stream, string fileName)
        {
            var (filePath, relativeFilePath) = GetApplicationFilePath(fileName, false);

            using (var fileStream = File.Create(filePath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }

            return relativeFilePath;
        }

        private static (string, string) GetApplicationFilePath(string fileName, bool finalMerge)
        {
            var directoryPath = finalMerge ?
                ConfigurationManager.AppSettings["projectDirectory"] + ConfigurationManager.AppSettings["completedFolder"] :
                ConfigurationManager.AppSettings["projectDirectory"] + ConfigurationManager.AppSettings["FileFolder"];

            var filePath = Path.Combine(directoryPath, fileName);
            var relativePath = Path.Combine(@"PDF\", fileName);
            return (filePath, relativePath);
        }

        public static Document CombinePdfs(
            string AccountNum,
            IEnumerable<Document> documents,
            bool finalMerge)
        {
            var pdfs = new List<PdfDocument>();

            var filePaths = new List<string>();

            filePaths.AddRange(documents
                .Where(d => d.Completed)
                .Select(d => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, d.RelativeFilePath)));


            foreach (var filePath in filePaths)
            {
                try
                {
                    var pdf = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);

                    if (pdf != null)
                    {
                        pdfs.Add(pdf);
                    }
                }
                catch (FileNotFoundException)
                {

                }
            }

            var outputDocument = new PdfDocument();

            foreach (var pdf in pdfs)
            {
                var count = pdf.PageCount;
                for (var i = 0; i < count; i++)
                {
                    var page = pdf.Pages[i];
                    outputDocument.AddPage(page);
                }
            }
            var fileName = AccountNum + ConfigurationManager.AppSettings["FileSuffix"];

            if (outputDocument.PageCount > 0)
            {
                try
                {
                    var (filePath, relativeFilePath) = GetApplicationFilePath(fileName, finalMerge);
                    outputDocument.Save(filePath);
                    return new Document
                    {
                        Completed = true,
                        RelativeFilePath = relativeFilePath
                    };
                }
                catch (InvalidOperationException)
                {

                }
            }
            return null;
        }
    }
}