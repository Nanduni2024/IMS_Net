using System;

namespace CareerConnect.API.Models
{
    public class SavedJob
    {
        public int JobId { get; set; }
        public Job Job { get; set; }

        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }

        public DateTime SavedDate { get; set; } = DateTime.UtcNow;
    }
}