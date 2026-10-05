namespace CareerConnect.API.Models
{
    public class Project
    {
        public int Id { get; set; }
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ProjectUrl { get; set; }
        public string RepoUrl { get; set; }
    }
}