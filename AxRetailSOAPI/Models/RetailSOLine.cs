using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AxRetailSOAPI.Models
{
    public class RetailSOLine
    {
        [JsonProperty(PropertyName = "No")]
        public int No { get; set; }

        [JsonProperty(PropertyName = "RecIdHeader")]
        public int RecIdHeader { get; set; }

        [JsonProperty(PropertyName = "RecId")]
        public string RecId { get; set; }

        [JsonProperty(PropertyName = "Series")]
        public string Series { get; set; }

        [JsonProperty(PropertyName = "Model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "Sink")]
        public string Sink { get; set; }

        [JsonProperty(PropertyName = "Top")]
        public string Top { get; set; }

        [JsonProperty(PropertyName = "Qty")]
        public double Qty { get; set; }

        [JsonProperty(PropertyName = "Amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "Date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "CreateBy")]
        public string CreateBy { get; set; }
    }
}