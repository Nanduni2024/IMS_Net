#nullable enable
using CareerConnect.API.Models;
using System.ComponentModel.DataAnnotations;

namespace CareerConnect.API.DTOs;

public class JobCreateRequest
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required, MaxLength(10000)]
    public string Description { get; set; } = string.Empty;
    public int CompanyId { get; set; }

    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public string JobType { get; set; } = "FullTime";
    public string WorkMode { get; set; } = "Remote";

    [Required, MaxLength(200)]
    public string Salary { get; set; } = string.Empty;

    [Required, MaxLength(2000)]
    public string Qualifications { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string ExperienceLevel { get; set; } = string.Empty;
    public DateTime Deadline { get; set; } = DateTime.UtcNow.AddDays(30);

    [Range(1, 10000)]
    public int Vacancies { get; set; } = 1;
    public string Status { get; set; } = "Active";
    public List<string> Skills { get; set; } = new();
}

public class JobSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string WorkMode { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public string ExperienceLevel { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public DateTime PostedDate { get; set; }
    public int Vacancies { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
}

public class JobDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string WorkMode { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public string Qualifications { get; set; } = string.Empty;
    public string ExperienceLevel { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
    public DateTime PostedDate { get; set; }
    public int Vacancies { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
}

public class ApplicationResponse
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime AppliedDate { get; set; }
}

public class UpdateApplicationStatusRequest
{
    public string Status { get; set; } = "Applied";
}

public class ProfileDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string AboutMe { get; set; } = string.Empty;
    public string GitHubUrl { get; set; } = string.Empty;
    public string LinkedInUrl { get; set; } = string.Empty;
    public string PortfolioUrl { get; set; } = string.Empty;
    public string CvUrl { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
}

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalCompanies { get; set; }
    public int TotalJobs { get; set; }
    public int TotalApplications { get; set; }
    public int PendingJobApprovals { get; set; }
}
