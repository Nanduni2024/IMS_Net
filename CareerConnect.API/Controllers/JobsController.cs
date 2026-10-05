#nullable enable
using CareerConnect.API.Data;
using CareerConnect.API.DTOs;
using CareerConnect.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CareerConnect.API.Controllers;

[ApiController]
[Route("api/jobs")]
public class JobsController : ControllerBase
{
    private readonly CareerConnectDbContext _context;

    public JobsController(CareerConnectDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetJobs(
        [FromQuery] string? keyword,
        [FromQuery] string? location,
        [FromQuery] string? jobType,
        [FromQuery] string? workMode,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var query = _context.Jobs
            .AsNoTracking()
            .Include(j => j.Company)
            .Include(j => j.JobSkills)
            .ThenInclude(js => js.Skill)
            .Where(j => j.Status == JobStatus.Active && j.Deadline >= DateTime.UtcNow)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(j =>
                j.Title.Contains(keyword) ||
                j.Description.Contains(keyword) ||
                j.Location.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(j => j.Location.Contains(location));
        }

        if (!string.IsNullOrWhiteSpace(jobType) && Enum.TryParse<JobType>(jobType, true, out var parsedJobType))
        {
            query = query.Where(j => j.JobType == parsedJobType);
        }

        if (!string.IsNullOrWhiteSpace(workMode) && Enum.TryParse<WorkMode>(workMode, true, out var parsedWorkMode))
        {
            query = query.Where(j => j.WorkMode == parsedWorkMode);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(j => j.PostedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JobSummaryDto
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                CompanyName = j.Company.Name,
                Location = j.Location,
                JobType = j.JobType.ToString(),
                WorkMode = j.WorkMode.ToString(),
                Salary = j.Salary,
                ExperienceLevel = j.ExperienceLevel,
                Deadline = j.Deadline,
                PostedDate = j.PostedDate,
                Vacancies = j.Vacancies,
                Status = j.Status.ToString(),
                Skills = j.JobSkills.Select(js => js.Skill.Name).ToList()
            })
            .ToListAsync();

        return Ok(new { total, page, pageSize, items });
    }

    [Authorize(Roles = "Recruiter")]
    [HttpGet("mine")]
    public async Task<IActionResult> MyJobs()
    {
        var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(recruiterId, out var id))
            return Unauthorized();

        var jobs = await _context.Jobs
            .AsNoTracking()
            .Include(j => j.Company)
            .Where(j => j.RecruiterId == id)
            .OrderByDescending(j => j.PostedDate)
            .Select(j => new
            {
                j.Id,
                j.Title,
                CompanyName = j.Company.Name,
                j.Location,
                Status = j.Status.ToString(),
                j.PostedDate,
                j.Deadline
            })
            .ToListAsync();

        return Ok(jobs);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetJob(int id)
    {
        var job = await _context.Jobs
            .AsNoTracking()
            .Include(j => j.Company)
            .Include(j => j.JobSkills)
            .ThenInclude(js => js.Skill)
            .FirstOrDefaultAsync(j => j.Id == id && j.Status == JobStatus.Active && j.Deadline >= DateTime.UtcNow);

        if (job == null)
            return NotFound();

        var dto = new JobDetailDto
        {
            Id = job.Id,
            Title = job.Title,
            Description = job.Description,
            CompanyId = job.CompanyId,
            CompanyName = job.Company.Name,
            Location = job.Location,
            JobType = job.JobType.ToString(),
            WorkMode = job.WorkMode.ToString(),
            Salary = job.Salary,
            Qualifications = job.Qualifications,
            ExperienceLevel = job.ExperienceLevel,
            Deadline = job.Deadline,
            PostedDate = job.PostedDate,
            Vacancies = job.Vacancies,
            Status = job.Status.ToString(),
            Skills = job.JobSkills.Select(js => js.Skill.Name).ToList()
        };

        return Ok(dto);
    }

    [Authorize(Roles = "Recruiter")]
    [HttpPost]
    public async Task<IActionResult> CreateJob([FromBody] JobCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(currentUserEmail))
            return Unauthorized();

        var currentUser = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == currentUserEmail);

        if (currentUser == null)
            return Unauthorized();

        var recruiter = await _context.Recruiters
            .FirstOrDefaultAsync(r => r.Id == currentUser.Id);

        if (currentUser.Role?.Name == "Recruiter" && recruiter == null)
            return Forbid();

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description)
            || string.IsNullOrWhiteSpace(request.Location) || string.IsNullOrWhiteSpace(request.Salary)
            || string.IsNullOrWhiteSpace(request.Qualifications) || string.IsNullOrWhiteSpace(request.ExperienceLevel))
            return BadRequest(new { message = "Complete all required job listing fields." });

        if (request.Deadline <= DateTime.UtcNow || request.Vacancies < 1)
            return BadRequest(new { message = "Choose a future deadline and at least one vacancy." });

        if (!Enum.TryParse<JobType>(request.JobType, true, out var jobType))
            return BadRequest(new { message = "Invalid job type." });

        if (!Enum.TryParse<WorkMode>(request.WorkMode, true, out var workMode))
            return BadRequest(new { message = "Invalid work mode." });

        var status = JobStatus.Active;
        if (recruiter == null)
            return Forbid();

        var job = new Job
        {
            Title = request.Title,
            Description = request.Description,
            CompanyId = recruiter.CompanyId,
            RecruiterId = recruiter.Id,
            JobType = jobType,
            Location = request.Location ?? "Remote",
            WorkMode = workMode,
            Salary = request.Salary,
            Qualifications = request.Qualifications,
            ExperienceLevel = request.ExperienceLevel,
            Deadline = request.Deadline,
            PostedDate = DateTime.UtcNow,
            Vacancies = request.Vacancies,
            Status = status
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        var skillNames = (request.Skills ?? new List<string>())
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Select(skill => skill.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (skillNames.Count > 0)
        {
            var existingSkills = await _context.Skills
                .Where(skill => skillNames.Contains(skill.Name))
                .ToListAsync();

            foreach (var skillName in skillNames)
            {
                var skill = existingSkills.FirstOrDefault(existing =>
                    string.Equals(existing.Name, skillName, StringComparison.OrdinalIgnoreCase));
                if (skill == null)
                {
                    skill = new Skill { Name = skillName };
                    _context.Skills.Add(skill);
                    existingSkills.Add(skill);
                }

                _context.JobSkills.Add(new JobSkill { JobId = job.Id, Skill = skill });
            }

            await _context.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetJob), new { id = job.Id }, new { job.Id, job.Title });
    }

    [Authorize(Roles = "Recruiter,Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateJob(int id, [FromBody] JobCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description)
            || string.IsNullOrWhiteSpace(request.Location) || string.IsNullOrWhiteSpace(request.Salary)
            || string.IsNullOrWhiteSpace(request.Qualifications) || string.IsNullOrWhiteSpace(request.ExperienceLevel)
            || request.Deadline <= DateTime.UtcNow || request.Vacancies < 1)
            return BadRequest(new { message = "Complete all required job fields and choose a future deadline." });

        var query = _context.Jobs.AsQueryable();
        if (!User.IsInRole("Admin"))
        {
            var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(recruiterId, out var ownerId))
                return Unauthorized();
            query = query.Where(j => j.RecruiterId == ownerId);
        }

        var job = await query.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
            return NotFound();

        if (!Enum.TryParse<JobType>(request.JobType, true, out var jobType))
            return BadRequest(new { message = "Invalid job type." });

        if (!Enum.TryParse<WorkMode>(request.WorkMode, true, out var workMode))
            return BadRequest(new { message = "Invalid work mode." });

        if (!Enum.TryParse<JobStatus>(request.Status, true, out var status))
            status = JobStatus.Active;

        job.Title = request.Title;
        job.Description = request.Description;
        if (User.IsInRole("Admin") && request.CompanyId > 0
            && await _context.Companies.AnyAsync(company => company.Id == request.CompanyId))
            job.CompanyId = request.CompanyId;
        job.JobType = jobType;
        job.Location = request.Location ?? job.Location;
        job.WorkMode = workMode;
        job.Salary = request.Salary;
        job.Qualifications = request.Qualifications;
        job.ExperienceLevel = request.ExperienceLevel;
        job.Deadline = request.Deadline;
        job.Vacancies = request.Vacancies;
        job.Status = status;

        await _context.SaveChangesAsync();
        return Ok(job);
    }

    [Authorize(Roles = "Recruiter,Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var query = _context.Jobs.AsQueryable();
        if (!User.IsInRole("Admin"))
        {
            var recruiterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(recruiterId, out var ownerId))
                return Unauthorized();
            query = query.Where(j => j.RecruiterId == ownerId);
        }

        var job = await query.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
            return NotFound();

        _context.Jobs.Remove(job);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpPost("{id:int}/apply")]
    public async Task<IActionResult> ApplyToJob(int id)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(currentUserEmail))
            return Unauthorized();

        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
        if (currentUser == null)
            return Unauthorized();

        var seeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.Id == currentUser.Id);
        if (seeker == null)
            return BadRequest(new { message = "Job seeker profile is required." });

        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);
        if (job == null)
            return NotFound();
        if (job.Status != JobStatus.Active || job.Deadline < DateTime.UtcNow)
            return Conflict(new { message = "This job is no longer accepting applications." });

        var alreadyApplied = await _context.Applications.AnyAsync(a => a.JobId == id && a.JobSeekerId == seeker.Id);
        if (alreadyApplied)
            return Conflict(new { message = "You have already applied for this job." });

        var application = new Application
        {
            JobId = id,
            JobSeekerId = seeker.Id,
            Status = ApplicationStatus.Applied,
            AppliedDate = DateTime.UtcNow,
            CvUrl = seeker.CvUrl
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Application submitted successfully.", applicationId = application.Id });
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpPost("{id:int}/save")]
    public async Task<IActionResult> SaveJob(int id)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(currentUserEmail))
            return Unauthorized();

        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
        if (currentUser == null)
            return Unauthorized();

        var seeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.Id == currentUser.Id);
        if (seeker == null)
            return BadRequest(new { message = "Job seeker profile is required." });

        var jobExists = await _context.Jobs.AnyAsync(j => j.Id == id);
        if (!jobExists)
            return NotFound();

        var exists = await _context.SavedJobs.AnyAsync(s => s.JobId == id && s.JobSeekerId == seeker.Id);
        if (exists)
            return Conflict(new { message = "This job is already saved." });

        _context.SavedJobs.Add(new SavedJob { JobId = id, JobSeekerId = seeker.Id, SavedDate = DateTime.UtcNow });
        await _context.SaveChangesAsync();

        return Ok(new { message = "Job saved successfully." });
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpDelete("{id:int}/save")]
    public async Task<IActionResult> RemoveSavedJob(int id)
    {
        var currentUserEmail = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(currentUserEmail))
            return Unauthorized();

        var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUserEmail);
        if (currentUser == null)
            return Unauthorized();

        var seeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.Id == currentUser.Id);
        if (seeker == null)
            return BadRequest(new { message = "Job seeker profile is required." });

        var saved = await _context.SavedJobs.FirstOrDefaultAsync(s => s.JobId == id && s.JobSeekerId == seeker.Id);
        if (saved == null)
            return NotFound();

        _context.SavedJobs.Remove(saved);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
