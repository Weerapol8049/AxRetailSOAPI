using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AxRetailSOAPI.Models
{
    public class Series
    {
        [JsonProperty(PropertyName = "Model")]
        public string Model { get; set; }

        [JsonProperty(PropertyName = "ProdSeries")]
        public string ProdSeries { get; set; }

        [JsonProperty(PropertyName = "Pool")]
        public string Pool { get; set; }
    }
}