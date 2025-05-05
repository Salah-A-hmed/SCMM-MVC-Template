namespace SCMM.ViewModels
{
    public class ExamDashboardViewModel
    {
        public int ExamId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationInMinutes { get; set; }
        public bool HasStarted { get; set; }
        public bool HasEnded { get; set; }
        public bool IsSubmitted { get; set; }
    }
}
