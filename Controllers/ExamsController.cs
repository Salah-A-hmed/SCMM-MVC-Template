using Microsoft.AspNetCore.Mvc;


namespace SCMM.Controllers
{
    public class ExamsController : Controller
    {
        private readonly AppDbContext _context;

        public ExamsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Exam/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentDateTime = DateTime.Now;

            // Get all available exams
            var availableExams = await _context.Exams
                .Where(e => e.IsActive)
                .Select(e => new ExamDashboardViewModel
                {
                    ExamId = e.ExamId,
                    Title = e.Title,
                    Description = e.Description,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    DurationInMinutes = e.DurationInMinutes,
                    HasStarted = currentDateTime >= e.StartTime,
                    HasEnded = currentDateTime > e.EndTime,
                    IsSubmitted = e.StudentExams.Any(se => se.StudentId == userId && se.IsSubmitted)
                })
                .ToListAsync();

            return View(availableExams);
        }

        // GET: Exam/Start/5
        public async Task<IActionResult> Start(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentDateTime = DateTime.Now;

            // Get exam
            var exam = await _context.Exams
                .Include(e => e.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(e => e.ExamId == id);

            if (exam == null)
            {
                return NotFound();
            }

            // Check if exam is active and within time constraints
            if (!exam.IsActive || currentDateTime < exam.StartTime || currentDateTime > exam.EndTime)
            {
                TempData["ErrorMessage"] = "This exam is not available at the moment.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Check if student has already submitted this exam
            var existingStudentExam = await _context.StudentExams
                .FirstOrDefaultAsync(se => se.ExamId == id && se.StudentId == userId);

            if (existingStudentExam != null && existingStudentExam.IsSubmitted)
            {
                return RedirectToAction(nameof(Result), new { id = existingStudentExam.StudentExamId });
            }

            // Create or update student exam entry
            if (existingStudentExam == null)
            {
                existingStudentExam = new StudentExam
                {
                    ExamId = id,
                    StudentId = userId,
                    StartedAt = currentDateTime,
                    IsSubmitted = false
                };
                _context.StudentExams.Add(existingStudentExam);
            }
            else if (!existingStudentExam.StartedAt.HasValue)
            {
                existingStudentExam.StartedAt = currentDateTime;
            }

             _context.SaveChanges();

            // Calculate remaining time
            TimeSpan remainingTime;
            if (existingStudentExam.StartedAt.HasValue)
            {
                var examEndTime = existingStudentExam.StartedAt.Value.AddMinutes(exam.DurationInMinutes);
                var systemEndTime = exam.EndTime;
                var effectiveEndTime = examEndTime < systemEndTime ? examEndTime : systemEndTime;
                remainingTime = effectiveEndTime - currentDateTime;
            }
            else
            {
                remainingTime = TimeSpan.FromMinutes(exam.DurationInMinutes);
            }

            if (remainingTime.TotalSeconds <= 0)
            {
                TempData["ErrorMessage"] = "Your exam time has expired.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Prepare active exam view model
            var viewModel = new ActiveExamViewModel
            {
                StudentExamId = existingStudentExam.StudentExamId,
                ExamId = exam.ExamId,
                Title = exam.Title,
                RemainingTimeInSeconds = (int)remainingTime.TotalSeconds,
                Questions = exam.Questions.OrderBy(q => q.OrderNumber).Select(q => new QuestionViewModel
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    QuestionType = q.QuestionType,
                    Points = q.Points,
                    Options = q.Options.Select(o => new OptionViewModel
                    {
                        OptionId = o.OptionId,
                        OptionText = o.OptionText
                    }).ToList()
                }).ToList()
            };

            // Load any existing answers
            var existingAnswers = await _context.StudentAnswers
                .Where(sa => sa.StudentExamId == existingStudentExam.StudentExamId)
                .ToListAsync();

            foreach (var question in viewModel.Questions)
            {
                var answer = existingAnswers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                if (answer != null)
                {
                    question.SelectedOptionId = answer.SelectedOptionId;
                    question.EssayAnswer = answer.EssayAnswer;
                }
            }

            return View("ActiveExam", viewModel);
        }

        // POST: Exam/Submit
        [HttpPost]
        public async Task<IActionResult> Submit(ActiveExamViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentDateTime = DateTime.Now;

            // Get student exam
            var studentExam = await _context.StudentExams
                .Include(se => se.Exam)
                .FirstOrDefaultAsync(se => se.StudentExamId == model.StudentExamId && se.StudentId == userId);

            if (studentExam == null)
            {
                return NotFound();
            }

            if (studentExam.IsSubmitted)
            {
                return RedirectToAction(nameof(Result), new { id = studentExam.StudentExamId });
            }

            // Process each question and save answers
            int totalScore = 0;

            // Get all questions for this exam
            var questions = await _context.Questions
                .Include(q => q.Options)
                .Where(q => q.ExamId == studentExam.ExamId)
                .ToListAsync();

            // Get existing answers or create new ones
            var existingAnswers = await _context.StudentAnswers
                .Where(sa => sa.StudentExamId == studentExam.StudentExamId)
                .ToListAsync();

            foreach (var questionModel in model.Questions)
            {
                var question = questions.FirstOrDefault(q => q.QuestionId == questionModel.QuestionId);
                if (question == null) continue;

                var studentAnswer = existingAnswers.FirstOrDefault(a => a.QuestionId == question.QuestionId);

                if (studentAnswer == null)
                {
                    studentAnswer = new StudentAnswer
                    {
                        StudentExamId = studentExam.StudentExamId,
                        QuestionId = question.QuestionId
                    };
                    _context.StudentAnswers.Add(studentAnswer);
                }

                // Process multiple choice questions
                if (question.QuestionType == QuestionType.MultipleChoice)
                {
                    studentAnswer.SelectedOptionId = questionModel.SelectedOptionId;

                    if (questionModel.SelectedOptionId.HasValue)
                    {
                        var selectedOption = question.Options.FirstOrDefault(o => o.OptionId == questionModel.SelectedOptionId);
                        studentAnswer.IsCorrect = selectedOption?.IsCorrect ?? false;
                    }
                    else
                    {
                        studentAnswer.IsCorrect = false;
                    }
                }
                // Process essay questions
                else if (question.QuestionType == QuestionType.Essay)
                {
                    studentAnswer.EssayAnswer = questionModel.EssayAnswer;
                    // Essay questions will need manual grading, marked as pending
                    studentAnswer.IsCorrect = null;
                }
            }

            // Mark as submitted
            studentExam.SubmittedAt = currentDateTime;
            studentExam.IsSubmitted = true;

            // Only set a score if all questions can be auto-graded (no essay questions)
            if (!questions.Any(q => q.QuestionType == QuestionType.Essay))
            {
                studentExam.Score = totalScore;
            }

            await _context.SaveChangesAsync();

            // Check if submission happened after deadline
            bool isLateSubmission = currentDateTime > studentExam.Exam.EndTime ||
                                   (studentExam.StartedAt.HasValue &&
                                    currentDateTime > studentExam.StartedAt.Value.AddMinutes(studentExam.Exam.DurationInMinutes));

            return RedirectToAction(nameof(Result), new { id = studentExam.StudentExamId, late = isLateSubmission });
        }

        public async Task<IActionResult> Result(int id, bool late = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get student exam
            var studentExam = await _context.StudentExams
                .Include(se => se.Exam)
                .FirstOrDefaultAsync(se => se.StudentExamId == id && se.StudentId == userId);

            if (studentExam == null)
            {
                return NotFound();
            }

            var resultViewModel = new ExamResultViewModel
            {
                StudentExamId = studentExam.StudentExamId,
                ExamId = studentExam.ExamId,
                ExamTitle = studentExam.Exam.Title,
                SubmittedAt = studentExam.SubmittedAt,
                Score = studentExam.Score,
                TotalPoints = studentExam.Exam.TotalPoints,
                HasEssayQuestions = await _context.Questions
                    .AnyAsync(q => q.ExamId == studentExam.ExamId && q.QuestionType == QuestionType.Essay),
                IsLateSubmission = late,
                IsFinalScoreAvailable = studentExam.Score.HasValue
            };

            return View(resultViewModel);
        }

        // GET: Exam/DetailedResult/5
        public async Task<IActionResult> DetailedResult(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Get student exam with all related data
            var studentExam = await _context.StudentExams
                .Include(se => se.Exam)
                .Include(se => se.StudentAnswers)
                    .ThenInclude(sa => sa.Question)
                        .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(se => se.StudentExamId == id && se.StudentId == userId);

            if (studentExam == null)
            {
                return NotFound();
            }

            // Create detailed result view model
            var viewModel = new DetailedExamResultViewModel
            {
                StudentExamId = studentExam.StudentExamId,
                ExamId = studentExam.ExamId,
                ExamTitle = studentExam.Exam.Title,
                SubmittedAt = studentExam.SubmittedAt,
                Score = studentExam.Score,
                TotalPoints = studentExam.Exam.TotalPoints,
                Questions = new List<DetailedQuestionViewModel>()
            };

            // Populate questions and answers
            foreach (var studentAnswer in studentExam.StudentAnswers)
            {
                var questionViewModel = new DetailedQuestionViewModel
                {
                    QuestionId = studentAnswer.QuestionId,
                    QuestionText = studentAnswer.Question.QuestionText,
                    QuestionType = studentAnswer.Question.QuestionType,
                    Points = studentAnswer.Question.Points,
                    IsCorrect = studentAnswer.IsCorrect,
                    EssayAnswer = studentAnswer.EssayAnswer,
                    SelectedOptionId = studentAnswer.SelectedOptionId,
                    Options = studentAnswer.Question.Options.Select(o => new DetailedOptionViewModel
                    {
                        OptionId = o.OptionId,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    }).ToList()
                };

                viewModel.Questions.Add(questionViewModel);
            }

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAnswer(SaveAnswerViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Verify student exam ownership
            var studentExam = await _context.StudentExams
                .FirstOrDefaultAsync(se => se.StudentExamId == model.StudentExamId && se.StudentId == userId && !se.IsSubmitted);

            if (studentExam == null)
            {
                return Json(new { success = false, message = "Invalid exam session" });
            }

            // Get or create student answer
            var studentAnswer = await _context.StudentAnswers
                .FirstOrDefaultAsync(sa => sa.StudentExamId == model.StudentExamId && sa.QuestionId == model.QuestionId);

            if (studentAnswer == null)
            {
                studentAnswer = new StudentAnswer
                {
                    StudentExamId = model.StudentExamId,
                    QuestionId = model.QuestionId
                };
                _context.StudentAnswers.Add(studentAnswer);
            }

            // Update answer based on question type
            var question = await _context.Questions.FindAsync(model.QuestionId);
            if (question.QuestionType == QuestionType.MultipleChoice)
            {
                studentAnswer.SelectedOptionId = model.OptionId;
                studentAnswer.EssayAnswer = null;
            }
            else
            {
                studentAnswer.EssayAnswer = model.EssayAnswer;
                studentAnswer.SelectedOptionId = null;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    

        public IActionResult Index()
        {
            return View();
        }
    }
}
