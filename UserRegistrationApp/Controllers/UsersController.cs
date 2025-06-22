using Microsoft.AspNetCore.Mvc;
using UserRegistrationApp.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using UserRegistrationApp.Models;

namespace UserRegistrationApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserRegistrationDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(UserRegistrationDBContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Users
        public IActionResult Index()
        {
            ViewData["States"] = new SelectList(_context.States, "StateId", "StateName");
            return View();
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["States"] = new SelectList(_context.States, "StateId", "StateName");
            return View();
        }

        [HttpGet]
        public JsonResult GetCities(int stateId)
        {
            var cities = _context.Cities.Where(c => c.StateId == stateId).ToList();
            return Json(new SelectList(cities, "CityId", "CityName"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] User user, IFormFile? Photo, [FromForm] List<string> Hobbies)
        {
            user.Hobbies = Hobbies != null ? string.Join(",", Hobbies) : null;

            if (ModelState.IsValid)
            {
                if (Photo != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/photos");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Photo.FileName);
                    string filePath = Path.Combine(uploadsDir, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(fileStream);
                    }
                    user.PhotoPath = "/images/photos/" + uniqueFileName;
                }

                _context.Add(user);
                await _context.SaveChangesAsync();
                return Json(new { success = true, redirectUrl = Url.Action("Index", "Users") });
            }

            // If we got this far, something failed, redisplay form
            var errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

            return Json(new { success = false, errors = errors });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetUsers()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault()?.ToLower();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            
            // Custom search filter
            var nameSearch = Request.Form["nameSearch"].FirstOrDefault();
            var genderSearch = Request.Form["genderSearch"].FirstOrDefault();
            var stateSearch = Request.Form["stateSearch"].FirstOrDefault();

            IQueryable<User> userData = _context.Users.Include(u => u.State);

            // Total records
            int recordsTotal = await userData.CountAsync();

            // Filtering
            if (!string.IsNullOrEmpty(nameSearch))
            {
                userData = userData.Where(u => u.Name.Contains(nameSearch));
            }
            if (!string.IsNullOrEmpty(genderSearch))
            {
                userData = userData.Where(u => u.Gender == genderSearch);
            }
            if (!string.IsNullOrEmpty(stateSearch))
            {
                int stateId = Convert.ToInt32(stateSearch);
                userData = userData.Where(u => u.StateId == stateId);
            }

            // Sorting
            if (!(string.IsNullOrEmpty(sortColumnName) && string.IsNullOrEmpty(sortColumnDirection)))
            {
                switch (sortColumnName)
                {
                    case "userid":
                        userData = sortColumnDirection == "asc" ? userData.OrderBy(u => u.UserId) : userData.OrderByDescending(u => u.UserId);
                        break;
                    case "name":
                        userData = sortColumnDirection == "asc" ? userData.OrderBy(u => u.Name) : userData.OrderByDescending(u => u.Name);
                        break;
                    case "email":
                        userData = sortColumnDirection == "asc" ? userData.OrderBy(u => u.Email) : userData.OrderByDescending(u => u.Email);
                        break;
                    case "gender":
                        userData = sortColumnDirection == "asc" ? userData.OrderBy(u => u.Gender) : userData.OrderByDescending(u => u.Gender);
                        break;
                    case "statename":
                         userData = sortColumnDirection == "asc" ? userData.OrderBy(u => u.State.StateName) : userData.OrderByDescending(u => u.State.StateName);
                        break;
                    default:
                        userData = userData.OrderBy(u => u.UserId);
                        break;
                }
            }

            // Total records after filtering
            int recordsFiltered = await userData.CountAsync();

            // Pagination
            var data = await userData.Skip(Convert.ToInt32(start)).Take(Convert.ToInt32(length)).Select(u => new 
            {
                u.UserId,
                PhotoPath = $"<img src='{u.PhotoPath}' class='img-thumbnail' style='max-width: 50px;'/>",
                Name = $"<a href='/Users/Details/{u.UserId}'>{u.Name}</a>",
                u.Email,
                u.Gender,
                StateName = u.State.StateName,
            }).ToListAsync();

            var jsonData = new { draw = draw, recordsFiltered = recordsFiltered, recordsTotal = recordsTotal, data = data };
            return Ok(jsonData);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found." });
            }

            // Delete photo file if it exists
            if (!string.IsNullOrEmpty(user.PhotoPath))
            {
                var photoPath = Path.Combine(_webHostEnvironment.WebRootPath, user.PhotoPath.TrimStart('/'));
                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "User deleted successfully." });
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["States"] = new SelectList(_context.States, "StateId", "StateName", user.StateId);
            ViewData["Cities"] = new SelectList(_context.Cities.Where(c=>c.StateId == user.StateId), "CityId", "CityName", user.CityId);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] User user, IFormFile? Photo, [FromForm] List<string> Hobbies)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }
            
            user.Hobbies = Hobbies != null ? string.Join(",", Hobbies) : null;

            if (ModelState.IsValid)
            {
                try
                {
                    if (Photo != null)
                    {
                        // Delete old photo if it exists
                        if (!string.IsNullOrEmpty(user.PhotoPath))
                        {
                             var oldPhotoPath = Path.Combine(_webHostEnvironment.WebRootPath, user.PhotoPath.TrimStart('/'));
                             if (System.IO.File.Exists(oldPhotoPath))
                             {
                                 System.IO.File.Delete(oldPhotoPath);
                             }
                        }

                        // Save new photo
                        string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/photos");
                        if (!Directory.Exists(uploadsDir))
                        {
                            Directory.CreateDirectory(uploadsDir);
                        }
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(Photo.FileName);
                        string filePath = Path.Combine(uploadsDir, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Photo.CopyToAsync(fileStream);
                        }
                        user.PhotoPath = "/images/photos/" + uniqueFileName;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.UserId == user.UserId))
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
            ViewData["States"] = new SelectList(_context.States, "StateId", "StateName", user.StateId);
            ViewData["Cities"] = new SelectList(_context.Cities.Where(c => c.StateId == user.StateId), "CityId", "CityName", user.CityId);
            return View(user);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.State)
                .Include(u => u.City)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
    }
} 