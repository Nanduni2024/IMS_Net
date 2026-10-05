using System;
using System.Collections.Generic;

namespace CareerConnect.API.Models
{
    public class Application
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public Job Job { get; set; }

        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }

        public ApplicationStatus Status { get; set; }
        public string CvUrl { get; set; }
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;

        public ICollection<Interview> Interviews { get; set; }
    }
}