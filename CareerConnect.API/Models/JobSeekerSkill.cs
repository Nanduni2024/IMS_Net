namespace CareerConnect.API.Models
{
    public class JobSeekerSkill
    {
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
        
        public int SkillId { get; set; }
        public Skill Skill { get; set; }
    }
}