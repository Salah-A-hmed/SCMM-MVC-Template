using SCMM.Models;

namespace SCMM.ViewModels
{
    public class DetailedQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public QuestionType QuestionType { get; set; }
        public int Points { get; set; }
        public bool? IsCorrect { get; set; }
        public int? SelectedOptionId { get; set; }
        public string EssayAnswer { get; set; }
        public List<DetailedOptionViewModel> Options { get; set; }
    }
}
