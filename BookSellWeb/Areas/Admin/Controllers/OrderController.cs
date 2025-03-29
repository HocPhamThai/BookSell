using BookEcomWeb.DataAccess.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BookEcomWeb.Areas.Admin.Controllers
{
    [Area("admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var objOrderHeaders = _unitOfWork.OrderHeader.GetAll(inCludeProperties: "ApplicationUser").ToList();
            return Json(new { data = objOrderHeaders });
        }
        #endregion
    }
}
