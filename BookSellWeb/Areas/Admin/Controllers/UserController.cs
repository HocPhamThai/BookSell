using BookEcomWeb.DataAccess.Data;
using BookEcomWeb.DataAccess.IRepository;
using BookEcomWeb.Models;
using BookEcomWeb.Models.ViewModels;
using BookEcomWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        //private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            //_roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string? userId)
        {
            string roleId = _dbContext.UserRoles.FirstOrDefault(u => u.UserId == userId).RoleId;

            RoleManagementVM roleManagementVM = new()
            {
                ApplicationUser = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Id == userId),
                RoleList = _dbContext.Roles.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                CompanyList = _dbContext.Companies.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            };
            if (roleManagementVM.ApplicationUser == null)
            {
                return RedirectToAction(nameof(Index));
            }
            roleManagementVM.ApplicationUser.Role = _dbContext.Roles.FirstOrDefault(u => u.Id == roleId).Name;
            roleManagementVM.RoleId = roleId;

            return View(roleManagementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            var applicationUser = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Id == roleManagementVM.ApplicationUser.Id);
            if (applicationUser == null)
                return RedirectToAction(nameof(Index));

            var userRole = _dbContext.UserRoles.FirstOrDefault(u => u.UserId == applicationUser.Id);
            var currentRoleId = userRole?.RoleId;

            var oldRoleName = _dbContext.Roles.FirstOrDefault(u => u.Id == currentRoleId)?.Name;
            var newRoleName = _dbContext.Roles.FirstOrDefault(u => u.Id == roleManagementVM.RoleId)?.Name;

            var isRoleChanged = currentRoleId != roleManagementVM.RoleId;
            var isCompanyChanged = applicationUser.CompanyID != roleManagementVM.ApplicationUser.CompanyID;

            if (isRoleChanged || isCompanyChanged)
            {
                if (newRoleName == SD.Role_Company)
                {
                    applicationUser.CompanyID = roleManagementVM.ApplicationUser.CompanyID;
                }
                else
                {
                    applicationUser.CompanyID = null;
                }

                if (isRoleChanged)
                {
                    _userManager.RemoveFromRoleAsync(applicationUser, oldRoleName).GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(applicationUser, newRoleName).GetAwaiter().GetResult();
                }
             
                _dbContext.SaveChanges();

            }

            return RedirectToAction(nameof(Index));
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
