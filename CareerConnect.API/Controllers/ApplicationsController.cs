using CareerConnect.API.Data;
using CareerConnect.API.DTOs;
using CareerConnect.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CareerConnect.API.Controllers;

[ApiController]
[Route("api")]
public class ApplicationsController : ControllerBase
{
    private readonly CareerConnectDbContext _context;

    public ApplicationsController(CareerConnectDbContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "JobSeeker")]
    [HttpGet("applications/my")]
    public async Task<IActionResult> MyApplications()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return Unauthorized();

        var applications = await _context.Applications
            .AsNoTracking()
            .Include(a => a.Job)
            .ThenInclude(j => j.Company)
            .Where(a => a.JobSeekerId == user.Id)
            .OrderByDescending(a => a.AppliedDate)
            .Select(a => new ApplicationResponse
            {
                Id = a.Id,
                JobId = a.JobId,
                JobTitle = a.Job.Title,
                CompanyName = a.Job.Company.Name,
                Status = a.Status.ToString(),
                AppliedDate = a.AppliedDate
            })
            .ToListAsync();

        return Ok(applications);
    }

    [Authorize(Roles = "Recruiter,Admin")]
    [HttpGet("jobs/{jobId:int}/applications")]
    public async Task<IActionResult> GetJobApplications(int jobId)
    {
        var job = await _context.Jobs.AsNoTracking().FirstOrDefaultAsync(j => j.Id == jobId);
        if (job == null)
            return NotFound();
        if (!User.IsInRole("Admin") && !IsCurrentRecruiter(job.RecruiterId))
            return Forbid();

        var applications = await _context.Applications
            .AsNoTracking()
            .Include(a => a.JobSeeker)
            .Include(a => a.Job)
            .ThenInclude(j => j.Company)
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.AppliedDate)
            .Select(a => new
            {
                a.Id,
                a.JobId,
                a.JobSeekerId,
                CandidateName = a.JobSeeker.FirstName + " " + a.JobSeeker.LastName,
                Status = a.Status.ToString(),
                a.AppliedDate,
                a.CvUrl
            })
            .ToListAsync();

        return Ok(applications);
    }

    [Authorize(Roles = "Recruiter,Admin")]
    [HttpPut("applications/{applicationId:int}/status")]
    public async Task<IActionResult> UpdateStatus(int applicationId, [FromBody] UpdateApplicationStatusRequest request)
    {
        if (!Enum.TryParse<ApplicationStatus>(request.Status, true, out var status))
            return BadRequest(new { message = "Invalid application status." });

        var application = await _context.Applications
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == applicationId);
        if (application == null)
            return NotFound();
        if (!User.IsInRole("Admin") && !IsCurrentRecruiter(application.Job.RecruiterId))
            return Forbid();

        application.Status = status;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Application status updated." });
    }

    private bool IsCurrentRecruiter(int recruiterId)
    {
        return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            && userId == recruiterId;
    }
}
