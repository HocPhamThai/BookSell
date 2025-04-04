using BookEcomWeb.DataAccess.IRepository;
using BookEcomWeb.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookEcomWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimIdentity = User.Identity as ClaimsIdentity;
            Claim? claim = claimIdentity?.Claims.FirstOrDefault();
            
            if (claim != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    var count = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value).Count();
                    HttpContext.Session.SetInt32(SD.SessionCart, count);    
                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
