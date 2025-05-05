namespace SCMM.ViewModels
{
    public class DetailedExamResultViewModel
    {
        public int StudentExamId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? Score { get; set; }
        public int TotalPoints { get; set; }
        public List<DetailedQuestionViewModel> Questions { get; set; }
    }
}
