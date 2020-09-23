using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AxRetailSOAPI.Models
{
    public class Pool
    {
        [JsonProperty(PropertyName = "PoolId")]
        public string PoolId { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }
    }
}