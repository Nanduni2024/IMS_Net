using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerConnect.API.Models
{
    public class JobSeeker
    {
        [Key, ForeignKey("User")]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Location { get; set; }
        public string ProfilePhoto { get; set; }
        public string AboutMe { get; set; }
        public string GitHubUrl { get; set; }
        public string LinkedInUrl { get; set; }
        public string PortfolioUrl { get; set; }
        public string CvUrl { get; set; }

        public User User { get; set; }
        
        public ICollection<Education> Educations { get; set; }
        public ICollection<Experience> Experiences { get; set; }
        public ICollection<Project> Projects { get; set; }
        public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; }
        public ICollection<Application> Applications { get; set; }
        public ICollection<SavedJob> SavedJobs { get; set; }
    }
}