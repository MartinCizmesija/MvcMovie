using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly MvcMovieContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentsController(MvcMovieContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Payments
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View();
            }

            return View(await _context.Payment.Where(p => p.UserMail.Equals(user.Email)).ToListAsync());
        }

        // GET: Payments/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var payment = await _context.Payment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payment == null)
            {
                return NotFound();
            }

            if (!payment.UserMail.Equals(user.Email)) return RedirectToAction("Index", "Home");

            return View(payment);
        }

        // GET: Payments/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {

            var user = await _userManager.GetUserAsync(User);
            var movies = _context.Movie
                .Where(m => m.UserMail.Equals(user.Email))
                .Select(g => new {g.Title})
                .ToList();

            ViewBag.Movies = new SelectList(movies, nameof(Movie.Title), nameof(Movie.Title));

            return View();
        }

        // POST: Payments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserMail,Title,Price,PaymentDate")] Payment payment)
        {

            var user = await _userManager.GetUserAsync(User);
            payment.UserMail = user.Email;
            payment.MovieId = _context.Movie.FirstOrDefault(m => m.Title.Equals(payment.Title)).Id;

            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payments/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }


            var user = await _userManager.GetUserAsync(User);

            if (!payment.UserMail.Equals(user.Email)) return RedirectToAction("Index", "Home");

            var movies = _context.Movie
                .Where(m => m.UserMail.Equals(user.Email))
                .Select(g => new { g.Title })
                .ToList();

            ViewBag.Movies = new SelectList(movies, nameof(Movie.Title), nameof(Movie.Title));


            return View(payment);
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserMail,Title,Price,PaymentDate")] Payment payment)
        {
            var user = await _userManager.GetUserAsync(User);

            if (id != payment.Id)
            {
                return NotFound();
            }

            payment.UserMail = user.Email;
            payment.MovieId = _context.Movie.FirstOrDefault(m => m.Title.Equals(payment.Title)).Id;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        // GET: Payments/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payment
                .FirstOrDefaultAsync(m => m.Id == id);

            var user = await _userManager.GetUserAsync(User);

            if (payment == null)
            {
                return NotFound();
            }

            if (!payment.UserMail.Equals(user.Email)) return RedirectToAction("Index", "Home");

            return View(payment);
        }

        // POST: Payments/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _context.Payment.FindAsync(id);
            _context.Payment.Remove(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult MoviePayments(int id)
       {
            var payment = _context.Payment.Where(p => p.MovieId == id);

            return View("Index", payment);
        }

        private bool PaymentExists(int id)
        {
            return _context.Payment.Any(e => e.Id == id);
        }
    }
}
