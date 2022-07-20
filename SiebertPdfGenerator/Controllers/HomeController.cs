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
            var importedFile = FileImporter.Importer();
            Pdf(importedFile);
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


        [HttpPost, ValidateAntiForgeryToken]
        public void Pdf(List<SuitabilityModel> suitabilityModels)
        {
            PdfService.GetSubmissionPdfPackage(suitabilityModels);
        }
    }
}