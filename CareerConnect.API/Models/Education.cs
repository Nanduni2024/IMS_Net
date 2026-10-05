using System;

namespace CareerConnect.API.Models
{
    public class Education
    {
        public int Id { get; set; }
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }

        public string Institution { get; set; }
        public string Degree { get; set; }
        public string FieldOfStudy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Grade { get; set; }
    }
}