using System;

namespace CareerConnect.API.Models
{
    public class Experience
    {
        public int Id { get; set; }
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }

        public string JobTitle { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }
}