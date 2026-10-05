using System.Collections.Generic;

namespace CareerConnect.API.Models
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // Admin, Recruiter, JobSeeker
        
        public ICollection<User> Users { get; set; }
    }
}