using CareerConnect.API.Data;
using CareerConnect.API.DTOs;
using CareerConnect.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly CareerConnectDbContext _context;

    public AdminController(CareerConnectDbContext context)
    {
        _context = context;
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = new AdminStatsDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalCompanies = await _context.Companies.CountAsync(),
            TotalJobs = await _context.Jobs.CountAsync(),
            TotalApplications = await _context.Applications.CountAsync(),
            PendingJobApprovals = await _context.Jobs.CountAsync(j => j.Status == JobStatus.PendingApproval)
        };

        return Ok(stats);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.Users
            .Include(u => u.Role)
            .Select(u => new
            {
                u.Id,
                u.Email,
                Role = u.Role.Name,
                u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("companies")]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await _context.Companies
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.Location,
                c.Description,
                c.CreatedAt
            })
            .ToListAsync();

        return Ok(companies);
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> GetJobs()
    {
        var jobs = await _context.Jobs
            .Include(j => j.Company)
            .Select(j => new
            {
                j.Id,
                j.Title,
                Company = j.Company.Name,
                j.Location,
                j.Status,
                j.PostedDate
            })
            .ToListAsync();

        return Ok(jobs);
    }
}
