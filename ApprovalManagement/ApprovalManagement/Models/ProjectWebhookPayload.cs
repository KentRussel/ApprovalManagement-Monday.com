using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApprovalManagement.Models
{
    public class MyViewModel
    {
        public DataItems DataItems { get; set; }
        public PayloadToken PayloadToken { get; set; }
    }

    public class PayloadToken
    {
        public string userId { get; set; }
        public string boardId { get; set; }
        public string groupId { get; set; }
        public string pulseId { get; set; }
        public string pulseName { get; set; }
        public string columnId { get; set; }
        public string columnType { get; set; }
        public string columnTitle { get; set; }
    }
    public class DataItems 
    {
        public string name { get; set; }
        public List<ColumnValue> column_values { get; set; }
    }

    public class ColumnValue
    {
        public string id { get; set; }
        public string text { get; set; }
        public string title { get; set; }
        public object value { get; set; }
    }

    public class PersonValue
    {
        public string id { get; set; }
        public string kind { get; set; }
    }

    public class UserValue
    {
        public string name { get; set; }
        public string email { get; set; }
        public string url { get; set; }
    }

}
