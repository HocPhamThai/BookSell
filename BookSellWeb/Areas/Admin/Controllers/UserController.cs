using BookEcomWeb.DataAccess.Data;
using BookEcomWeb.DataAccess.IRepository;
using BookEcomWeb.Models;
using BookEcomWeb.Models.ViewModels;
using BookEcomWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookEcomWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var objUserList = _dbContext.ApplicationUsers.Include(u => u.Company).ToList();

            var roleList = _dbContext.Roles.ToList();
            var UserRoleList = _dbContext.UserRoles.ToList();

            foreach (var user in objUserList)
            {
                var userRole = UserRoleList.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                var role = roleList.FirstOrDefault(u => u.Id == userRole).Name;
                user.Role = role;
                
                if (user.Company == null)
                {
                    user.Company = new Company() { Name = "" };
                }
            }

            return Json(new { data = objUserList });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string? id)
        {
            var objFromDb = _dbContext.ApplicationUsers.Where(u => u.Id == id).FirstOrDefault();
            var message = "";
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                // user is currently locked and we need to unlock here
                message = "User UnLocked successfully";
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                message = "User Locked successfully";
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _dbContext.SaveChanges();
            return Json(new { success = true, message = message});
        }
        #endregion
    }
}
