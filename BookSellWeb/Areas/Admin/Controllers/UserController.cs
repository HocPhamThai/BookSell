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
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(string? userId)
        {
            var user = _userManager.FindByIdAsync(userId).GetAwaiter().GetResult();

            if (user == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            RoleManagementVM roleManagementVM = new()
            {
                ApplicationUser = (ApplicationUser)user,
                RoleList = _roleManager.Roles.ToList().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            };

            var roleNames = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();

            string? roleId = null;
            if (roleNames.Any())
            {
                var role = _roleManager.FindByNameAsync(roleNames[0]).GetAwaiter().GetResult();
                roleManagementVM.RoleId = role?.Id;
            }
            roleManagementVM.ApplicationUser.Role = _roleManager.FindByIdAsync(roleId).GetAwaiter().GetResult()?.Name;

            return View(roleManagementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
        {
            var applicationUser = (ApplicationUser)_userManager.FindByIdAsync(roleManagementVM.ApplicationUser.Id).GetAwaiter().GetResult();
            if (applicationUser == null)
                return RedirectToAction(nameof(Index));

            var roles = _userManager.GetRolesAsync(applicationUser).GetAwaiter().GetResult();
            var oldRoleName = roles.FirstOrDefault();

            var newRole = _roleManager.Roles.FirstOrDefault(r => r.Id == roleManagementVM.RoleId);
            var newRoleName = newRole?.Name;

            var isRoleChanged = oldRoleName != newRoleName;
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

                if (isRoleChanged && oldRoleName != null && newRoleName != null)
                {
                    _userManager.RemoveFromRoleAsync(applicationUser, oldRoleName).GetAwaiter().GetResult();
                    _userManager.AddToRoleAsync(applicationUser, newRoleName).GetAwaiter().GetResult();
                }

                _unitOfWork.Save(); // Save CompanyID change
            }

            return RedirectToAction(nameof(Index));
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var objUserList = _unitOfWork.ApplicationUser.GetAll(inCludeProperties:"Company");

            var roleList = _roleManager.Roles.ToList();
            var userRoleList = new List<(string UserId, string RoleName)>();

            foreach (var user in _userManager.Users.ToList())
            {
                var roleName = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();
                userRoleList.Add((user.Id, roleName ?? string.Empty));
            }

            foreach (var user in objUserList)
            {
                var userRole = userRoleList.FirstOrDefault(u => u.UserId == user.Id).RoleName;
                user.Role = userRole;

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
            var objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id, inCludeProperties:"Company");
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

            _unitOfWork.Save();
            return Json(new { success = true, message = message});
        }
        #endregion
    }
}
