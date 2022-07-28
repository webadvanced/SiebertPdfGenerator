using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SiebertPdfGenerator.Business;
using SiebertPdfGenerator.Models;
using SiebertPdfGenerator.Services;

namespace SiebertPdfGenerator.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        public ActionResult Pdf()
        {
            var importedFile = FileImporter.Importer();
            PdfService.GetSubmissionPdfPackage(importedFile);

            return View();
        }

        public ActionResult RemoveBlank(int pdfCount)
        {
            PdfService.RemoveBlank(pdfCount + 1);

            return View();
        }
    }
}