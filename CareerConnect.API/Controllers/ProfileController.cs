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
[Authorize(Roles = "JobSeeker")]
public class ProfileController : ControllerBase
{
    private readonly CareerConnectDbContext _context;

    public ProfileController(CareerConnectDbContext context)
    {
        _context = context;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
            return Unauthorized();

        var jobSeeker = await _context.JobSeekers
            .Include(js => js.JobSeekerSkills)
            .ThenInclude(jss => jss.Skill)
            .FirstOrDefaultAsync(js => js.Id == user.Id);

        if (jobSeeker == null)
            return NotFound();

        var dto = new ProfileDto
        {
            Id = jobSeeker.Id,
            FirstName = jobSeeker.FirstName ?? string.Empty,
            LastName = jobSeeker.LastName ?? string.Empty,
            Email = user.Email,
            Phone = jobSeeker.Phone ?? string.Empty,
            Location = jobSeeker.Location ?? string.Empty,
            AboutMe = jobSeeker.AboutMe ?? string.Empty,
            GitHubUrl = jobSeeker.GitHubUrl ?? string.Empty,
            LinkedInUrl = jobSeeker.LinkedInUrl ?? string.Empty,
            PortfolioUrl = jobSeeker.PortfolioUrl ?? string.Empty,
            CvUrl = jobSeeker.CvUrl ?? string.Empty,
            Skills = jobSeeker.JobSeekerSkills?.Select(jss => jss.Skill.Name).ToList() ?? new List<string>()
        };

        return Ok(dto);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] ProfileDto request)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
            return Unauthorized();

        var jobSeeker = await _context.JobSeekers.FirstOrDefaultAsync(js => js.Id == user.Id);
        if (jobSeeker == null)
            return NotFound();

        jobSeeker.FirstName = request.FirstName;
        jobSeeker.LastName = request.LastName;
        jobSeeker.Phone = request.Phone;
        jobSeeker.Location = request.Location;
        jobSeeker.AboutMe = request.AboutMe;
        jobSeeker.GitHubUrl = request.GitHubUrl;
        jobSeeker.LinkedInUrl = request.LinkedInUrl;
        jobSeeker.PortfolioUrl = request.PortfolioUrl;
        jobSeeker.CvUrl = request.CvUrl;

        var requestedSkills = (request.Skills ?? new List<string>())
            .Where(skill => !string.IsNullOrWhiteSpace(skill))
            .Select(skill => skill.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var oldSkills = await _context.JobSeekerSkills
            .Where(link => link.JobSeekerId == jobSeeker.Id)
            .ToListAsync();
        _context.JobSeekerSkills.RemoveRange(oldSkills);

        var skillEntities = await _context.Skills
            .Where(skill => requestedSkills.Contains(skill.Name))
            .ToListAsync();
        foreach (var skillName in requestedSkills)
        {
            var skill = skillEntities.FirstOrDefault(existing =>
                string.Equals(existing.Name, skillName, StringComparison.OrdinalIgnoreCase));
            if (skill == null)
            {
                skill = new Skill { Name = skillName };
                _context.Skills.Add(skill);
                skillEntities.Add(skill);
            }

            _context.JobSeekerSkills.Add(new JobSeekerSkill
            {
                JobSeekerId = jobSeeker.Id,
                Skill = skill
            });
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Profile updated successfully." });
    }
}
