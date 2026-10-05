using CareerConnect.API.Models;
using Microsoft.EntityFrameworkCore;

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

            modelBuilder.Entity<Role>().HasIndex(r => r.Name).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Company>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Skill>().HasIndex(s => s.Name).IsUnique();
            modelBuilder.Entity<Job>().HasIndex(j => j.Title);
            modelBuilder.Entity<Job>().HasIndex(j => j.Location);

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

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Recruiter" },
                new Role { Id = 3, Name = "JobSeeker" }
            );
        }
    }
}
