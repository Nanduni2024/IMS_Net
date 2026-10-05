using System;

namespace CareerConnect.API.Models
{
    public class Interview
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public Application Application { get; set; }

        public DateTime ScheduledDate { get; set; }
        public TimeSpan Time { get; set; }
        public InterviewType InterviewType { get; set; }
        public string MeetingLink { get; set; }
        public string Notes { get; set; }
        public InterviewStatus Status { get; set; }
    }
}