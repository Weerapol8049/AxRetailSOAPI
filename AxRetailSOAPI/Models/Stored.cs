using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AxRetailSOAPI.Models
{
    public class Stored
    {
        [JsonProperty(PropertyName = "StoreId")]
        public string StoreId { get; set; }

        [JsonProperty(PropertyName = "StoreName")]
        public string StoreName { get; set; }
    }
}