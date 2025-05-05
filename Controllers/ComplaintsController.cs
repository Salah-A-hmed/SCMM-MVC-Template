using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCMM.Data;
using SCMM.Models;
using System.Security.Claims;            
using System.Linq;

namespace SCMM.Controllers
{
    public class ComplaintsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ComplaintsController(ApplicationDbContext db)
        {
            _db = db;
        }

        #region Student Actions

       //  [Authorize (Roles = "Student")]
        public IActionResult Index()
        {
            return View();
        }

       //  [Authorize (Roles = "Student")]
        [HttpGet]
        public IActionResult FAQ()
        {
            if (_db.SupportTickets == null)
            {
                TempData["Error"] = "Support tickets data is unavailable.";
                return View(new List<string>());
            }

            var frequentQuestions = _db.SupportTickets
                .GroupBy(t => t.Title)
                .Where(g => g.Count() >= 3)
                .Select(g => g.Key)
                .ToList();

            if (!frequentQuestions.Any())
            {
                TempData["Message"] = "No frequently asked questions available yet.";
            }

            return View(frequentQuestions);
        }

       //  [Authorize (Roles = "Student")]
        [HttpGet]
        public IActionResult History()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int studentId))
            {
                TempData["Error"] = "User is not authenticated.";
                return RedirectToAction("Index");
            }

            var complaints = _db.SupportTickets
                .Where(c => c.StudentId == studentId)
                .OrderByDescending(c => c.DateTime)
                .ToList();

            if (!complaints.Any())
            {
                TempData["Message"] = "You haven't submitted any complaints yet.";
            }

            return View(complaints);
        }

       //  [Authorize (Roles = "Student")]
        [HttpGet]
        public IActionResult NewComplaint()
        {
            return View();
        }

       //  [Authorize (Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewComplaint(SupportTicket model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the validation errors.";
                return View(model);
            }

            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int studentId))
            {
                TempData["Error"] = "User is not authenticated.";
                return RedirectToAction("Index");
            }

            try
            {
                model.StudentId = studentId;
                model.DateTime = DateTime.Now;
                model.Status = Complaints_status.Pending;

                _db.SupportTickets.Add(model);
                _db.SaveChanges();
                TempData["Success"] = "Complaint submitted successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error saving complaint: {ex.Message}";
                return View(model);
            }
        }

       //  [Authorize (Roles = "Student")]
        [HttpGet]
        public IActionResult SeeAnswer(int id)
        {
            var complaint = _db.SupportTickets.FirstOrDefault(c => c.Id == id);

            if (complaint == null)
            {
                TempData["Error"] = "Complaint not found.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(complaint.Answer))
            {
                TempData["Warning"] = "No answer available yet for this complaint.";
            }

            return View(complaint);
        }

       //  [Authorize (Roles = "Student")]
        [HttpGet]
        public IActionResult EditComplaint(int id)
        {
            var complaint = _db.SupportTickets.FirstOrDefault(c => c.Id == id);

            if (complaint == null)
            {
                TempData["Error"] = "Complaint not found.";
                return RedirectToAction("Index");
            }

            if (complaint.Status == Complaints_status.Answered)
            {
                TempData["Warning"] = "Cannot edit an answered complaint.";
                return RedirectToAction("Index");
            }

            return View(complaint);
        }

       //  [Authorize (Roles = "Student")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditComplaint(SupportTicket model)
        {
            var existingComplaint = _db.SupportTickets.FirstOrDefault(c => c.Id == model.Id);

            if (existingComplaint == null)
            {
                TempData["Error"] = "Complaint not found.";
                return RedirectToAction("Index");
            }

            if (existingComplaint.Status == Complaints_status.Answered)
            {
                TempData["Warning"] = "Cannot edit an answered complaint.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                existingComplaint.Title = model.Title;
                existingComplaint.Content = model.Content;
                existingComplaint.Category = model.Category;
                _db.SaveChanges();
                TempData["Success"] = "Complaint updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Validation failed.";
            return View(model);
        }

        #endregion

        #region Employee Actions

        // [Authorize(Roles = "Employee")]
        [HttpGet]
        public IActionResult Emp_Dashboard()
        {
            var unansweredComplaints = _db.SupportTickets
                .Where(c => c.Answer == null)
                .ToList();

            if (!unansweredComplaints.Any())
            {
                TempData["Message"] = "No unanswered complaints. Great job!";
            }

            return View(unansweredComplaints);
        }

        // [Authorize(Roles = "Employee")]
        [HttpGet]
        public IActionResult SetAnswer(int id)
        {
            var complaint = _db.SupportTickets.FirstOrDefault(c => c.Id == id);

            if (complaint == null)
            {
                TempData["Error"] = "Complaint not found.";
                return RedirectToAction("Emp_Dashboard");
            }

            return View(complaint);
        }

        // [Authorize(Roles = "Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetAnswer(int id, string answer)
        {
            var complaint = _db.SupportTickets.FirstOrDefault(c => c.Id == id);

            if (complaint == null)
            {
                TempData["Error"] = "Complaint not found.";
                return RedirectToAction("Emp_Dashboard");
            }

            if (string.IsNullOrWhiteSpace(answer) || answer.Length < 10)
            {
                TempData["Error"] = "Answer must be at least 10 characters long.";
                return View(complaint);
            }

            complaint.Answer = answer;
            complaint.Status = Complaints_status.Answered;
            _db.SaveChanges();
            TempData["Success"] = "Answer submitted successfully!";
            return RedirectToAction("Emp_Dashboard");
        }

        #endregion
    }
}
