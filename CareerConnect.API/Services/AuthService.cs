#nullable enable
using CareerConnect.API.Data;
using CareerConnect.API.DTOs;
using CareerConnect.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CareerConnect.API.Services;

public class AuthService
{
    private static readonly PasswordHasher<User> PasswordHasher = new();
    private readonly CareerConnectDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(CareerConnectDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Email and password are required.");

        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await _context.Users.AnyAsync(u => u.Email == normalizedEmail))
            throw new InvalidOperationException("A user with this email already exists.");

        var roleName = string.IsNullOrWhiteSpace(request.Role) ? "JobSeeker" : request.Role.Trim();
        if (roleName is not ("JobSeeker" or "Recruiter"))
            throw new InvalidOperationException("Only job seeker and recruiter accounts can be registered.");

        if (roleName == "Recruiter" && string.IsNullOrWhiteSpace(request.CompanyName))
            throw new InvalidOperationException("Company name is required for recruiter accounts.");

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

        if (role == null)
            throw new InvalidOperationException("No valid role was found.");

        var user = new User
        {
            Email = normalizedEmail,
            RoleId = role.Id,
            Role = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        user.PasswordHash = PasswordHasher.HashPassword(user, request.Password);

        _context.Users.Add(user);

        if (role.Name == "JobSeeker")
        {
            var seeker = new JobSeeker
            {
                Id = user.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                User = user
            };
            _context.JobSeekers.Add(seeker);
        }
        else
        {
            var companyName = request.CompanyName.Trim();
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Name == companyName);
            if (company == null)
            {
                company = new Company
                {
                    Name = companyName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Companies.Add(company);
            }

            _context.Recruiters.Add(new Recruiter
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Company = company,
                User = user
            });
        }

        await _context.SaveChangesAsync();

        var token = GenerateJwtToken(user);

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = role.Name,
            Token = token
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user == null || !await VerifyAndUpgradePasswordAsync(user, request.Password))
            return null;

        return new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role?.Name ?? "JobSeeker",
            Token = GenerateJwtToken(user)
        };
    }

    private async Task<bool> VerifyAndUpgradePasswordAsync(User user, string password)
    {
        if (string.IsNullOrEmpty(user.PasswordHash))
            return false;

        if (user.PasswordHash.Length == 44)
        {
            var legacyHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            var savedHash = new byte[32];
            if (!Convert.TryFromBase64String(user.PasswordHash, savedHash, out var bytesWritten) || bytesWritten != savedHash.Length)
                return false;
            if (!CryptographicOperations.FixedTimeEquals(legacyHash, savedHash))
                return false;

            user.PasswordHash = PasswordHasher.HashPassword(user, password);
            await _context.SaveChangesAsync();
            return true;
        }

        var result = PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            return false;
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = PasswordHasher.HashPassword(user, password);
            await _context.SaveChangesAsync();
        }

        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is not configured.");
        var issuer = _configuration["Jwt:Issuer"] ?? "CareerConnectAPI";
        var audience = _configuration["Jwt:Audience"] ?? "CareerConnectClient";

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role?.Name ?? "JobSeeker")
        };

        var expirationMinutes = int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var minutes)
            ? minutes
            : 120;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
