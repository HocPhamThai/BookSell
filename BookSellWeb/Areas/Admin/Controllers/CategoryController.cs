﻿using BookEcomWeb.DataAccess.IRepository;
using BookEcomWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookEcomWeb.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var categoryList = unitOfWork.Category.GetAll().ToList();
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
                unitOfWork.Category.Add(obj);
                unitOfWork.Save();
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

            Category? categoryFromDb = unitOfWork.Category.Get(u => u.Id == id);
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
                unitOfWork.Category.Update(obj);
                unitOfWork.Save();
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

            Category? categoryFromDb = unitOfWork.Category.Get(u => u.Id == id);
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
            Category? obj = unitOfWork.Category.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            unitOfWork.Category.Remove(obj);
            unitOfWork.Save();
            TempData["success"] = "Category Deleted Successfully!!!";
            return RedirectToAction("Index");
        }
    }
}