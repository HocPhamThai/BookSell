using BookSellWeb.Data;
using BookSellWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookSellWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext db;

        public CategoryController(ApplicationDbContext db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            var categoryList = db.Categories.ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The name must be different from the order!");
            }
            if (ModelState.IsValid)
            {
                db.Categories.Add(obj);
                db.SaveChanges();
                TempData["success"] = "Category Created Successfully!!!";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }

            Category? categoryFromDb = db.Categories.Find(id);
            //Category categoryFromDb1 = db.Categories.FirstOrDefault(x => x.Id == id);
            //Category categoryFromDb2 = db.Categories.Where(x => x.Id == id).FirstOrDefault();

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Update(obj);
                db.SaveChanges();
                TempData["success"] = "Category Edited Successfully!!!";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return View();
            }

            Category? categoryFromDb = db.Categories.Find(id);
            //Category categoryFromDb1 = db.Categories.FirstOrDefault(x => x.Id == id);
            //Category categoryFromDb2 = db.Categories.Where(x => x.Id == id).FirstOrDefault();

            if (categoryFromDb == null)
            {
                return NotFound();
            }
            return View(categoryFromDb);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? id)
        {
            Category? obj = db.Categories.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            db.Categories.Remove(obj);
            db.SaveChanges();
            TempData["success"] = "Category Deleted Successfully!!!";
            return RedirectToAction("Index");
        }
    }
}
