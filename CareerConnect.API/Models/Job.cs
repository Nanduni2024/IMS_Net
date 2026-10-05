using System;
using System.Collections.Generic;

namespace CareerConnect.API.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        
        public int CompanyId { get; set; }
        public Company Company { get; set; }
        
        public int RecruiterId { get; set; }
        public Recruiter Recruiter { get; set; }

        public JobType JobType { get; set; }
        public string Location { get; set; }
        public WorkMode WorkMode { get; set; }
        public string Salary { get; set; }
        public string Qualifications { get; set; }
        public string ExperienceLevel { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public int Vacancies { get; set; }
        public JobStatus Status { get; set; }

        public ICollection<JobSkill> JobSkills { get; set; }
        public ICollection<Application> Applications { get; set; }
        public ICollection<SavedJob> SavedJobs { get; set; }
    }
}