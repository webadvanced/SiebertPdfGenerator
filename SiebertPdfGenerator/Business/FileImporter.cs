using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using SiebertPdfGenerator.Models;

namespace SiebertPdfGenerator.Business
{
    static public class FileImporter
    {
        static public List<SuitabilityModel> Importer()
        {
            var fileName = ConfigurationManager.AppSettings["csvLocation"];

            List<SuitabilityModel> records;

            using (var reader = new StreamReader(fileName))
            {
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.ToLower(),

                };

                using (var csv = new CsvReader(reader, config))
                {
                    records = csv.GetRecords<SuitabilityModel>().ToList();
                }
            }
            return records;
        }

        static public string GetPageHtml(SuitabilityModel suitabilityModel, int pageNum)
        {
            string html;

            switch (pageNum)
            {
                case 1:
                    html = ReadFile("page1.cshtml");
                    html = string.Format(html,  suitabilityModel.Address1, suitabilityModel.Address2, suitabilityModel.Address3,
                            suitabilityModel.Address4, suitabilityModel.Address5, suitabilityModel.Address6, DateTime.Now.ToShortDateString(), suitabilityModel.AccountNum);
                    break;
                case 2:
                    html = ReadFile("page2.cshtml");

                    // Subheader
                    if (!string.IsNullOrEmpty(suitabilityModel.AccountNum))
                    {
                        string tempHtml = ReadFile("page2-subheader.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.AccountNum);
                    }
                    else
                        html += ReadFile("page2-subheader.cshtml");

                    // Dob
                    if (suitabilityModel.Dob != null && suitabilityModel.Dob != DateTime.MinValue)
                    {
                        string tempHtml = ReadFile("page2p2-dob-filled.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.Dob.Value.ToShortDateString());
                    }
                    else
                        html += ReadFile("page2p2-dob-empty.cshtml");

                    // Annual Income
                    if (!string.IsNullOrEmpty(suitabilityModel.AnnualIncome))
                    {
                        string tempHtml = ReadFile("page2p3-income-filled.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.AnnualIncome);
                    }
                    else
                        html += ReadFile("page2p3-income-empty.cshtml");

                    // Estimated Net Worth
                    if (!string.IsNullOrEmpty(suitabilityModel.EstimatedNetWorth))
                    {
                        string tempHtml = ReadFile("page2p4-networth-filled.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.EstimatedNetWorth);
                    }
                    else
                        html += ReadFile("page2p4-networth-empty.cshtml");

                    // Fed Tax Bracket
                    if (!string.IsNullOrEmpty(suitabilityModel.TaxBracket))
                    {
                        string tempHtml = ReadFile("page2p5-fedtax-filled.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.TaxBracket);
                    }
                    else
                        html += ReadFile("page2p5-fedtax-empty.cshtml");

                    // Marital Status
                    if (!string.IsNullOrEmpty(suitabilityModel.marital_status_cd))
                    {
                        string tempHtml = ReadFile("page2p6-marital-filled.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.marital_status_cd);
                    }
                    else
                        html += ReadFile("page2p6-marital-empty.cshtml");

                    // Dependents
                    if (!string.IsNullOrEmpty(suitabilityModel.dependents_qty))
                    {
                        string tempHtml = ReadFile("page2p7-dependents-filled.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.dependents_qty);
                    }
                    else
                        html += ReadFile("page2p7-dependents-empty.cshtml");

                    // General Investment Knowledge
                    if (!string.IsNullOrEmpty(suitabilityModel.GeneralKnowldedge))
                    {
                        string tempHtml = ReadFile("page2p8-general-filled.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.GeneralKnowldedge);
                    }
                    else
                        html += ReadFile("page2p8-general-empty.cshtml");

                    // Investment Time Horizon
                    if (!string.IsNullOrEmpty(suitabilityModel.TimeHrzDescr))
                    {
                        string tempHtml = ReadFile("page2p10-time-filled.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.TimeHrzDescr);
                    }
                    else
                        html += ReadFile("page2p10-time-empty.cshtml");

                    // Risk Tolerance
                    if (!string.IsNullOrEmpty(suitabilityModel.RiskTlrncDescr))
                    {
                        string tempHtml = ReadFile("page2p11-risk-filled.cshtml");
                        html += string.Format(tempHtml, suitabilityModel.RiskTlrncDescr);
                    }
                    else
                        html += ReadFile("page2p11-risk-empty.cshtml");

                    // Investment Objectives
                    if (!string.IsNullOrEmpty(suitabilityModel.InvestObjectives))
                    {
                        string tempHtml = ReadFile("page2p12-investment-filled.cshtml");
                        // Split the column into each response (response is in order)
                        var objectives = suitabilityModel.InvestObjectives.Split(',');
                        // Convert to list
                        var objectivesList = objectives.ToList();
                        // Count list
                        var count = objectivesList.Count();
                        // 
                        for(int i = 0; i < count; i++)
                        {
                            objectivesList[i] = objectivesList[i] + ": " + (i + 1);
                        }
                        // Get difference in max amount of response (4) and count
                        var difference = 4 - count;
                        // Add empty strings until list has at least 4 items so the format doesn't break on next line
                        while(difference > 0)
                        {
                            objectivesList.Add(String.Empty);
                            difference--;
                        }

                        // Format the html with the objectives in order
                        html += string.Format(tempHtml, objectivesList[0], objectivesList[1], objectivesList[2], objectivesList[3]);
                    }
                    else
                        html += ReadFile("page2p12-investment-empty.cshtml");

                    html += ReadFile("page2-footer.cshtml");

                    break;
                default:
                    html = ReadFile("page3.cshtml");
                    break;
            }            
            
            return html;
        }

        private static string ReadFile(string fileName)
        {
            string html;
            using (var reader = new StreamReader(ConfigurationManager.AppSettings["projectDirectory"] + ConfigurationManager.AppSettings["htmlLocation"] + fileName))
            {
                html = reader.ReadToEnd();
            }
            return html;
        }
    }
}