using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private MyContext _context;
    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        HttpContext.Session.Clear();
        return View();
    }

    // ***** REGISTER AND LOGIN ROUTES ******
    [HttpPost("register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid){

            if(_context.Users.Any(a => a.Email == newUser.Email))
            {
                // user already in db
                ModelState.AddModelError("Email", "Email is already in use");
                return View("Index");
            }
            // if user not in db, hash pw and add user to db
            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            _context.Add(newUser);
            _context.SaveChanges();
            // log new user in and add them to session
            HttpContext.Session.SetInt32("user", newUser.UserID);
            return RedirectToAction("Dashboard");
        } else {
            return View("Index");
        }
    }

    [HttpPost("login")]
    public IActionResult Login(LogUser loginUser)
    {
        if(ModelState.IsValid)
        {
            // find user in db; if not in db, throw error
            User userInDB = _context.Users.FirstOrDefault(a => a.Email == loginUser.LogEmail);
            if(userInDB == null)
            {
                // no email in db, throw error
                ModelState.AddModelError("LogEmail", "Invalid login attempt");
                return View("Index");
            }
            // if email in db, hash pw and compare to pw in db
            PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();
            var result = hasher.VerifyHashedPassword(loginUser, userInDB.Password, loginUser.LogPassword);

            if(result == 0)
            {
                // pw was not correct
                ModelState.AddModelError("LogEmail", "Invalid login attempt");
                return View("Index");
            } else {
                // pw was correct, log user in and add to session
                HttpContext.Session.SetInt32("user", userInDB.UserID);
                return RedirectToAction("Dashboard");
            }
        } else {
            return View("Index");
        }
    }

    // ***** DASHBOARD ROUTE *****
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        ViewBag.AllWeddings = _context.Weddings.Include(g => g.Attendees).ToList();
        ViewBag.CurrentUser = HttpContext.Session.GetInt32("user");
        
        // if no logged in user is in session, redirect to reg/login
        if(HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        }
        // need (int) in front of sess here bc will be comparing to UserID int
        User loggedinUser = _context.Users.FirstOrDefault(a => a.UserID == (int) HttpContext.Session.GetInt32("user"));
        return View();
    }

    // ***** RSVP ROUTES (i.e. add/unadd guest to wedding) *****
    [HttpGet("wedding/rsvp/{weddingID}")]
    public IActionResult RSVP(int WeddingID)
    {
        Guest newGuest = new Guest();
        newGuest.WeddingID = WeddingID;
        newGuest.UserID = (int)HttpContext.Session.GetInt32("user");

        _context.Add(newGuest);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");

    }

    [HttpGet("wedding/unrsvp/{weddingID}")]
    public IActionResult UnRSVP(int WeddingID, Guest currentGuest)
    {
        Guest guestToDelete = _context.Guests.SingleOrDefault(g => g.UserID == (int)HttpContext.Session.GetInt32("user") && g.WeddingID == WeddingID);
        _context.Guests.Remove(guestToDelete);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    // ***** CREATE WEDDING ROUTES *****
    [HttpGet("wedding")]
    public IActionResult Wedding(int WeddingID){
        if(HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("Index");
        } else {
            return View();
        }
    }

    [HttpPost("AddWedding")]
    public IActionResult AddWedding(Wedding newWedding)
    {
        var sess = HttpContext.Session.GetInt32("user");
        // System.Console.WriteLine(sess);
        // System.Console.WriteLine("error check");
        newWedding.CreatorID = (int) HttpContext.Session.GetInt32("user");
        if(ModelState.IsValid)
        {
            // adding validation for date - must be in future
            if(DateTime.Now.Year > newWedding.Date.Year)
            {
                ModelState.AddModelError("Date", "Must be a future date");
                return View("Wedding");
            } else {
                _context.Add(newWedding);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            }
        } else {
            return View("Wedding");
        }
    }

    // ***** SHOW ONE WEDDING ROUTE *****
    [HttpGet("wedding/{weddingID}")]
    public IActionResult OneWedding(int WeddingID)
    {
        ViewBag.OneWedding = _context.Weddings.FirstOrDefault(w => w.WeddingID == WeddingID);
        Wedding WeddingGuests = _context.Weddings.Include(a => a.Attendees).ThenInclude(att => att.User).FirstOrDefault(w => w.WeddingID == WeddingID);
        return View(WeddingGuests);
    }

    // ***** DELETE ROUTE *****
    [HttpGet("wedding/delete/{weddingID}")]
    public IActionResult DeleteWedding(int WeddingID)
    {
        Wedding weddingToDelete = _context.Weddings.SingleOrDefault(w => w.WeddingID == WeddingID);
        _context.Weddings.Remove(weddingToDelete);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
