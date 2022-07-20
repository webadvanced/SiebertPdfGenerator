using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SiebertPdfGenerator.Models
{
    public class SuitabilityModel
    {
        [Name("AccountNumber")]
        public string AccountNum { get; set; }
        [Name("HouseholdNbr")]
        public long HouseholdNbr { get; set; }
        [Name("rr_cd")]
        public string RrCd { get; set; }
        [Name("Name")]
        public string Name { get; set; }
        [Name("AddressLine1")]
        public string Address1 { get; set; }
        [Name("AddressLine2")]
        public string Address2 { get; set; }
        [Name("AddressLine3")]
        public string Address3 { get; set; }
        [Name("AddressLine4")]
        public string Address4 { get; set; }
        [Name("AddressLine5")]
        public string Address5 { get; set; }
        [Name("AddressLine6")]
        public string Address6 { get; set; }
        [Name("City")]
        public string City { get; set; }
        [Name("State")]
        public string State { get; set; }
        [Name("Zip")]
        public string Zip { get; set; }
        [Name("Country")]
        public string Country { get; set; }
        [Name("PhoneNumber")]
        public string PhoneNumber { get; set; }
        [Name("EmailAddress")]
        public string Email { get; set; }
        [Name("Email2")]
        public string Email2 { get; set; }
        [Name("DateOfBirth")]
        public DateTime? Dob { get; set; }
        [Name("TaxBracket")]
        public string TaxBracket { get; set; }
        [Name("InvestObjectives")]
        public string InvestObjectives { get; set; }
        [Name("GeneralKnowldedge")]
        public string GeneralKnowldedge { get; set; }
        [Name("TimeHrzDescr")]
        public string TimeHrzDescr { get; set; }
        [Name("RiskTlrncDescr")]
        public string RiskTlrncDescr { get; set; }
        [Name("AnnualIncome")]
        public string AnnualIncome { get; set; }
        [Name("EstimatedNetWorth")]
        public string EstimatedNetWorth { get; set; }
        [Name("marital_status_cd")]
        public string marital_status_cd { get; set; }
        [Name("dependents_qty")]
        public string dependents_qty { get; set; }
        public List<Business.Document> Documents { get; set; }
    }
}