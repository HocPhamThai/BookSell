using BookEcomWeb.DataAccess.IRepository;
using BookEcomWeb.Models;
using BookEcomWeb.Models.ViewModels;
using BookEcomWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookEcomWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM? ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // when user click on Cart Icon, it will redirect to Index Page
        public IActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.Claims.First().Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, inCludeProperties: "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count ?? 0) * cart.Price;
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            /*
             B1: Get Application User ID to getAll Shopping Cart List, Include properties: "Product"
             B2: Create ShoppingVM with OrederHeader
             B3: Set Data for ShoppingCartVM from ApplicationUser to return to the Summary View 
             B4: ForEach ShoppingList to Get Cart Price and OrderTotal
             */
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.Claims.First().Value;

            ShoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, inCludeProperties: "Product"),
                OrderHeader = new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count ?? 0) * cart.Price;
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")] 
        public IActionResult SummaryPOST()
        {
            /*
             B1: Get Application User ID to getAll Shopping Cart List, Include properties: "Product"
             B2: Create ShoppingVM with OrederHeader
             B3: Set Data for ShoppingCartVM from ApplicationUser to return to the Summary View 
             B4: ForEach ShoppingList to Get Cart Price and OrderTotal
             */
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.Claims.First().Value;

            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId, inCludeProperties: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;
            // Do Not Creating navigation Properites here just call it
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(x => x.Id == userId);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Count ?? 0) * cart.Price;
            }

            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                // it is a regular customer account and we need to capture payment
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                // it is a company account
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            // Create Order Detail here
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = (int) cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            
            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                // it is a regular customer account and we need to capture payment
                // We will be using stripe to process the payment
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int? id)
        {
            return View(id);
        }

        // Functionalities for Plus, Minus, Remove, GetPriceBasedOnQuantity
        public IActionResult Plus(int? cartId)
        {
            /*
                Step 01: Get the cart from db by unit of work
                Step 02: Update Count
                Step 03: Save the cart
             */
            var cartFromDb = _unitOfWork.ShoppingCart.Get(c => c.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int? cartId)
        {
            /*
                Step 01: Get the cart from db by unit of work
                Step 02: if Count > 1 Update Count-- else Remove Count (count <= 1)
                Step 03: Save the cart
             */
            var cartFromDb = _unitOfWork.ShoppingCart.Get(c => c.Id == cartId);
            if (cartFromDb.Count <= 1)
            {
                // Remove Cart 
                _unitOfWork.ShoppingCart.Remove(cartFromDb);
            }
            else
            {
                // Update Cart
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int? cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(c => c.Id == cartId);
            _unitOfWork.ShoppingCart.Remove(cartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else if (cart.Count <= 100)
            {
                return cart.Product.Price50;
            }
            else
            {
                return cart.Product.Price100;
            }
        }
    }
}
