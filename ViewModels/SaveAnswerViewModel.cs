namespace SCMM.ViewModels
{
    public class SaveAnswerViewModel
    {
        public int StudentExamId { get; set; }
        public int QuestionId { get; set; }
        public int? OptionId { get; set; }
        public string EssayAnswer { get; set; }
    }
}
