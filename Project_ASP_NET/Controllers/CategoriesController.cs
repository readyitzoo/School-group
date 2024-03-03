using Microsoft.AspNetCore.Mvc;
using Project_ASP_NET.Data;
using Project_ASP_NET.Models;

namespace Project_ASP_NET.Controllers
{
    public class CategoriesController : Controller
    {

        private readonly ApplicationDbContext _db;
        public CategoriesController(ApplicationDbContext context)
        {
            _db = context;
        }

        // List all categories
        public IActionResult Index()
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Groups");
            }

            List<Category> categories = _db.Categories.ToList();
            ViewBag.Categories = categories;

            return View();
        }

        // Show new category form
        public IActionResult New()
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Groups");
            }

            return View();
        }

        // Create a new category
        [HttpPost]
        public IActionResult New(Category category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(category);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                return View(category);
            }
        }

        // Delete category ( and delete groups from that category )
        [HttpPost]
        public IActionResult Delete(int id)
        {
            Category? category = _db.Categories.Find(id);

            if (category != null)
            {
                List<Group> groups = _db.Groups.Where(grp => grp.CategoryId == id).ToList();

                foreach (Group grp in groups)
                {
                    _db.Groups.Remove(grp);
                }

                _db.Categories.Remove(category);

                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Edit form
        public IActionResult Edit(int id)
        {
            if (User.Identity != null && !User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Register", new { area = "Identity" });
            }

            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Groups");
            }

            Category category = _db.Categories.Find(id);

   
            return View(category);
        }

        // Edit a category
        [HttpPost]
        public IActionResult Edit(int id, Category newCategory)
        {
            Category category = _db.Categories.Find(id);

            
            if (ModelState.IsValid)
            {

                category.Name = newCategory.Name;
                category.Description = newCategory.Description;
                _db.SaveChanges();

                return RedirectToAction("Index");


            }
            else
            {
                return View(newCategory);
            }
        }


    }
}
