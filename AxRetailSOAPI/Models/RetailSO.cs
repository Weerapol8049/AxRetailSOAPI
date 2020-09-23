using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AxRetailSOAPI.Models
{
    public class RetailSO
    {
        [JsonProperty(PropertyName = "No")]
        public int No { get; set; }

        [JsonProperty(PropertyName = "RecId")]
        public string RecId { get; set; }

        [JsonProperty(PropertyName = "SalesId")]
        public string SalesId { get; set; }

        [JsonProperty(PropertyName = "PurchId")]
        public string PurchId { get; set; }

        [JsonProperty(PropertyName = "StoreId")]
        public string StoreId { get; set; }

        [JsonProperty(PropertyName = "StoreName")]
        public string StoreName { get; set; }

        [JsonProperty(PropertyName = "CustName")]
        public string CustName { get; set; }

        [JsonProperty(PropertyName = "Pool")]
        public string Pool { get; set; }

        [JsonProperty(PropertyName = "Qty")]
        public double Qty { get; set; }

        [JsonProperty(PropertyName = "Amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "DueDate")]
        public DateTime DueDate { get; set; }

        [JsonProperty(PropertyName = "ConfirmDate")]
        public DateTime ConfirmDate { get; set; }

        [JsonProperty(PropertyName = "LineCount")]
        public int LineCount { get; set; }
        
        [JsonProperty(PropertyName = "ImageCount")]
        public int ImageCount { get; set; }

        [JsonProperty(PropertyName = "CreateBy")]
        public string CreateBy { get; set; }


    }
}