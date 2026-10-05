using System;
using System.Collections.Generic;

namespace CareerConnect.API.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Recruiter> Recruiters { get; set; }
        public ICollection<Job> Jobs { get; set; }
    }
}