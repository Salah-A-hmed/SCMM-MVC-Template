namespace SCMM.ViewModels
{
    public class ActiveExamViewModel
    {
        public int StudentExamId { get; set; }
        public int ExamId { get; set; }
        public string Title { get; set; }
        public int RemainingTimeInSeconds { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
    }
}
