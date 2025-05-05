namespace SCMM.ViewModels
{
    public class ExamResultViewModel
    {
        public int StudentExamId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? Score { get; set; }
        public int TotalPoints { get; set; }
        public bool HasEssayQuestions { get; set; }
        public bool IsLateSubmission { get; set; }
        public bool IsFinalScoreAvailable { get; set; }
    }
}
