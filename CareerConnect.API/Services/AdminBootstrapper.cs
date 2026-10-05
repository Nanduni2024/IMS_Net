using CareerConnect.API.Data;
using CareerConnect.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CareerConnect.API.Services;

public class AdminBootstrapper
{
    private static readonly PasswordHasher<User> PasswordHasher = new();
    private readonly CareerConnectDbContext _context;
    private readonly IConfiguration _configuration;

    public AdminBootstrapper(CareerConnectDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task EnsureAdminAsync()
    {
        var email = _configuration["InitialAdmin:Email"];
        var password = _configuration["InitialAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password))
            return;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || password.Length < 12)
            throw new InvalidOperationException("InitialAdmin requires both an email and a password of at least 12 characters.");

        var adminRole = await _context.Roles.FirstOrDefaultAsync(role => role.Name == "Admin")
            ?? throw new InvalidOperationException("The Admin role is missing. Apply database migrations before bootstrapping an admin.");
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var existingUser = await _context.Users.FirstOrDefaultAsync(user => user.Email == normalizedEmail);
        if (existingUser != null)
        {
            if (existingUser.RoleId != adminRole.Id)
                throw new InvalidOperationException("The configured initial admin email already belongs to a non-admin account.");
            return;
        }

        var user = new User
        {
            Email = normalizedEmail,
            RoleId = adminRole.Id,
            Role = adminRole,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        user.PasswordHash = PasswordHasher.HashPassword(user, password);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}