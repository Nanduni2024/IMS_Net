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