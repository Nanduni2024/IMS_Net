namespace CareerConnect.API.Models
{
    public enum JobType { Internship, FullTime, PartTime, Contract }
    public enum WorkMode { Remote, OnSite, Hybrid }
    public enum JobStatus { Active, Closed, Draft, PendingApproval, Rejected }
    public enum ApplicationStatus { Applied, UnderReview, Shortlisted, Interview, Selected, Rejected }
    public enum InterviewType { Online, InPerson, Phone }
    public enum InterviewStatus { Scheduled, Completed, Cancelled, Rescheduled }
}