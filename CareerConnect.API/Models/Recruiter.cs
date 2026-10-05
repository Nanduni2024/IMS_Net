using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerConnect.API.Models
{
    public class Recruiter
    {
        [Key, ForeignKey("User")]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public User User { get; set; }
        public ICollection<Job> PostedJobs { get; set; }
    }
}