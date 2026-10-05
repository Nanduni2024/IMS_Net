const fs = require('fs');
const path = require('path');

const basePath = 'E:/IMS_Net/CareerConnect/CareerConnect.API';
const modelsPath = path.join(basePath, 'Models');
const dataPath = path.join(basePath, 'Data');

if (!fs.existsSync(modelsPath)) fs.mkdirSync(modelsPath);
if (!fs.existsSync(dataPath)) fs.mkdirSync(dataPath);

const files = {
    'Models/Enums.cs': 
namespace CareerConnect.API.Models
{
    public enum JobType { Internship, FullTime, PartTime, Contract }
    public enum WorkMode { Remote, OnSite, Hybrid }
    public enum JobStatus { Active, Closed, Draft, PendingApproval, Rejected }
    public enum ApplicationStatus { Applied, UnderReview, Shortlisted, Interview, Selected, Rejected }
    public enum InterviewType { Online, InPerson, Phone }
    public enum InterviewStatus { Scheduled, Completed, Cancelled, Rescheduled }
}
,
    'Models/Role.cs': 
using System.Collections.Generic;

namespace CareerConnect.API.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // Admin, Recruiter, JobSeeker
        
        public ICollection<User> Users { get; set; }
    }
}
,
    'Models/User.cs': 
using System;
using System.Collections.Generic;

namespace CareerConnect.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public JobSeeker JobSeeker { get; set; }
        public Recruiter Recruiter { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
,
    'Models/JobSeeker.cs': 
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
,
    'Models/Company.cs': 
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
,
    'Models/Recruiter.cs': 
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerConnect.API.Models
{
    public class Recruiter
    {
        [Key, ForeignKey("User")]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public User User { get; set; }
        public ICollection<Job> PostedJobs { get; set; }
    }
}
,
    'Models/Job.cs': 
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
,
    'Models/Skill.cs': 
using System.Collections.Generic;

namespace CareerConnect.API.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<JobSkill> JobSkills { get; set; }
        public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; }
    }
}
,
    'Models/JobSkill.cs': 
namespace CareerConnect.API.Models
{
    public class JobSkill
    {
        public int JobId { get; set; }
        public Job Job { get; set; }
        
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
,
    'Models/JobSeekerSkill.cs': 
namespace CareerConnect.API.Models
{
    public class JobSeekerSkill
    {
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
        
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}
,
    'Models/Education.cs': 
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
,
    'Models/Experience.cs': 
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
,
    'Models/Project.cs': 
namespace CareerConnect.API.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ProjectUrl { get; set; }
        public string RepoUrl { get; set; }
    }
}
,
    'Models/Application.cs': 
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
,
    'Models/SavedJob.cs': 
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
,
    'Models/Notification.cs': 
using System;

namespace CareerConnect.API.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
,
    'Models/Interview.cs': 
using System;

namespace CareerConnect.API.Models
{
    public class Interview
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public DateTime ScheduledDate { get; set; }
        public TimeSpan Time { get; set; }
        public InterviewType InterviewType { get; set; }
        public string MeetingLink { get; set; }
        public string Notes { get; set; }
        public InterviewStatus Status { get; set; }
    }
}
,
    'Models/Report.cs': 
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

};

for (const [relPath, content] of Object.entries(files)) {
    fs.writeFileSync(path.join(basePath, relPath), content.trim());
}
console.log('Entities scaffolded.');
