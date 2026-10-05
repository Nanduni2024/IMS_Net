using System.Collections.Generic;

namespace CareerConnect.API.Models
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<JobSkill> JobSkills { get; set; }
        public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; }
    }
}