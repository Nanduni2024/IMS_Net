import os

base_path = r'E:\IMS_Net\CareerConnect\CareerConnect.API'

files = {
    'Data/CareerConnectDbContext.cs': """using CareerConnect.API.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CareerConnect.API.Data
{
    public class CareerConnectDbContext : DbContext
    {
        public CareerConnectDbContext(DbContextOptions<CareerConnectDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<JobSeeker> JobSeekers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Recruiter> Recruiters { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<JobSkill> JobSkills { get; set; }
        public DbSet<JobSeekerSkill> JobSeekerSkills { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Interview> Interviews { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-Many: Job - Skill
            modelBuilder.Entity<JobSkill>()
                .HasKey(js => new { js.JobId, js.SkillId });
            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Job)
                .WithMany(j => j.JobSkills)
                .HasForeignKey(js => js.JobId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JobSkill>()
                .HasOne(js => js.Skill)
                .WithMany(s => s.JobSkills)
                .HasForeignKey(js => js.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many: JobSeeker - Skill
            modelBuilder.Entity<JobSeekerSkill>()
                .HasKey(jss => new { jss.JobSeekerId, jss.SkillId });
            modelBuilder.Entity<JobSeekerSkill>()
                .HasOne(jss => jss.JobSeeker)
                .WithMany(js => js.JobSeekerSkills)
                .HasForeignKey(jss => jss.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<JobSeekerSkill>()
                .HasOne(jss => jss.Skill)
                .WithMany(s => s.JobSeekerSkills)
                .HasForeignKey(jss => jss.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many: SavedJobs
            modelBuilder.Entity<SavedJob>()
                .HasKey(sj => new { sj.JobId, sj.JobSeekerId });
            modelBuilder.Entity<SavedJob>()
                .HasOne(sj => sj.Job)
                .WithMany(j => j.SavedJobs)
                .HasForeignKey(sj => sj.JobId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SavedJob>()
                .HasOne(sj => sj.JobSeeker)
                .WithMany(js => js.SavedJobs)
                .HasForeignKey(sj => sj.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1-to-Many Restricts to prevent cycles
            modelBuilder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Application>()
                .HasOne(a => a.JobSeeker)
                .WithMany(js => js.Applications)
                .HasForeignKey(a => a.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Recruiter)
                .WithMany(r => r.PostedJobs)
                .HasForeignKey(j => j.RecruiterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Recruiter" },
                new Role { Id = 3, Name = "JobSeeker" }
            );

            // Basic Seed Data for test accounts
            // Use a dummy hash for Phase 2, real hashing implemented in Phase 3
            var dummyHash = "dummy_hash_for_testing";

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "admin@test.com", PasswordHash = dummyHash, RoleId = 1, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Id = 2, Email = "recruiter1@test.com", PasswordHash = dummyHash, RoleId = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Id = 3, Email = "recruiter2@test.com", PasswordHash = dummyHash, RoleId = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Id = 4, Email = "recruiter3@test.com", PasswordHash = dummyHash, RoleId = 2, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Id = 5, Email = "jobseeker1@test.com", PasswordHash = dummyHash, RoleId = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Id = 6, Email = "jobseeker2@test.com", PasswordHash = dummyHash, RoleId = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Id = 7, Email = "jobseeker3@test.com", PasswordHash = dummyHash, RoleId = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Id = 8, Email = "jobseeker4@test.com", PasswordHash = dummyHash, RoleId = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new User { Id = 9, Email = "jobseeker5@test.com", PasswordHash = dummyHash, RoleId = 3, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            );
            
            // Note: In a real scenario we would also seed JobSeekers, Recruiters, Companies, Jobs, Skills here.
            // But seeding large graphs in OnModelCreating can be verbose. Doing a subset for Phase 2:
            modelBuilder.Entity<Company>().HasData(
                new Company { Id = 1, Name = "Tech Corp", Description = "A big tech company", Location = "Colombo" },
                new Company { Id = 2, Name = "Startup Inc", Description = "Fast growing startup", Location = "Kandy" },
                new Company { Id = 3, Name = "Global Solutions", Description = "Multinational firm", Location = "Galle" }
            );

            modelBuilder.Entity<Recruiter>().HasData(
                new Recruiter { Id = 2, FirstName = "Alice", LastName = "Smith", CompanyId = 1 },
                new Recruiter { Id = 3, FirstName = "Bob", LastName = "Jones", CompanyId = 2 },
                new Recruiter { Id = 4, FirstName = "Charlie", LastName = "Brown", CompanyId = 3 }
            );

            modelBuilder.Entity<JobSeeker>().HasData(
                new JobSeeker { Id = 5, FirstName = "David", LastName = "Doe" },
                new JobSeeker { Id = 6, FirstName = "Eva", LastName = "Green" },
                new JobSeeker { Id = 7, FirstName = "Frank", LastName = "White" },
                new JobSeeker { Id = 8, FirstName = "Grace", LastName = "Hall" },
                new JobSeeker { Id = 9, FirstName = "Henry", LastName = "Ford" }
            );

            modelBuilder.Entity<Skill>().HasData(
                new Skill { Id = 1, Name = "C#" },
                new Skill { Id = 2, Name = "React" },
                new Skill { Id = 3, Name = "SQL Server" },
                new Skill { Id = 4, Name = "TypeScript" },
                new Skill { Id = 5, Name = "ASP.NET Core" }
            );
            
            modelBuilder.Entity<Job>().HasData(
                new Job { Id = 1, Title = "Software Engineer", Description = "C# Developer needed", CompanyId = 1, RecruiterId = 2, JobType = JobType.FullTime, WorkMode = WorkMode.Hybrid, Salary = "100k", Location = "Colombo", Deadline = DateTime.UtcNow.AddDays(30), Vacancies = 2, Status = JobStatus.Active },
                new Job { Id = 2, Title = "Frontend Developer", Description = "React expert", CompanyId = 2, RecruiterId = 3, JobType = JobType.FullTime, WorkMode = WorkMode.Remote, Salary = "90k", Location = "Remote", Deadline = DateTime.UtcNow.AddDays(15), Vacancies = 1, Status = JobStatus.Active },
                new Job { Id = 3, Title = "Intern", Description = "Learn .NET", CompanyId = 3, RecruiterId = 4, JobType = JobType.Internship, WorkMode = WorkMode.OnSite, Salary = "30k", Location = "Galle", Deadline = DateTime.UtcNow.AddDays(7), Vacancies = 5, Status = JobStatus.Active }
            );
        }
    }
}"""
}

for rel_path, content in files.items():
    full_path = os.path.join(base_path, rel_path)
    with open(full_path, 'w', encoding='utf-8') as f:
        f.write(content)

print("DbContext scaffolded.")
