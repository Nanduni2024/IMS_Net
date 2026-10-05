using System;

namespace CareerConnect.API.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ReportType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string DataUrl { get; set; }
    }
}