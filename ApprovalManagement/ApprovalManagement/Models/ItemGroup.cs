using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApprovalManagement.Models
{
    public class ItemGroup
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}