using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser>_userManager;

        [BindProperty]
        public ShoppingCartViewModel shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            //get user id of loggedin user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM = new ShoppingCartViewModel()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.AplicationUserId == claim.Value, includeProperties:"Product")
            };
            shoppingCartVM.OrderHeader.OrderTotal = 0;
            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties:"Company");

            //decrypt(get all info) items in list
            foreach(var list in shoppingCartVM.ListCart)
            {
                //not mapped, so we need to calculate it
                list.Price = SD.GetPriceBasedOnQty(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);


                shoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                //bc of viewmodel, convert to raw html 
                list.Product.Description = SD.ConvertToRawHtml(list.Product.Description);
                //display only first 100 chars
                if(list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }
            return View(shoppingCartVM);
        }

        //post for index, but using annotations
        //do this bc action names are the same and gives error
        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            //get user id of loggedin user
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            //get user obj
            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
            if(user == null)
            {
                //adding error
                ModelState.AddModelError(string.Empty, "Error finding the correct user");

            }

            //Email Verification**
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code, returnUrl = string.Empty },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
              
            ModelState.AddModelError(string.Empty, "Verification email sent, check your email");


            return RedirectToAction("Index");
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Product");

            cart.Count += 1;
            cart.Price = SD.GetPriceBasedOnQty(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
            _unitOfWork.Save();
            
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Product");

            //if is 0, remove from shoppingCart and session #
            if(cart.Count == 1)
            {
                //get count first before we delete
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.AplicationUserId == cart.AplicationUserId).ToList().Count();
                //remove
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();
                //update session
                HttpContext.Session.SetObject(SD.ssShoppingCart, 0);
            }
            else
            {

                cart.Count -= 1;
            
                cart.Price = SD.GetPriceBasedOnQty(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                _unitOfWork.Save();
            
            }


            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Product");

            
                //get count first before we delete
                var count = _unitOfWork.ShoppingCart.GetAll(u => u.AplicationUserId == cart.AplicationUserId).ToList().Count();
                //remove
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();
                //update session
                HttpContext.Session.SetObject(SD.ssShoppingCart, 0);
           


            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM = new ShoppingCartViewModel()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.AplicationUserId == claim.Value, includeProperties: "Product")

            };
            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties:"Company");

            //calc prices
            foreach (var list in shoppingCartVM.ListCart)
            {
                //not mapped, so we need to calculate it
                list.Price = SD.GetPriceBasedOnQty(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price100);


                shoppingCartVM.OrderHeader.OrderTotal += (list.Price * list.Count);
                
            }
            //generate order header info from user 
            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.State = shoppingCartVM.OrderHeader.ApplicationUser.State;
            shoppingCartVM.OrderHeader.StreetAddress = shoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            shoppingCartVM.OrderHeader.PhoneNumber = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            shoppingCartVM.OrderHeader.PostalCode = shoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;

            return View(shoppingCartVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value, includeProperties: "Company");
            shoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(c => c.AplicationUserId == claim.Value, includeProperties:"Product");

            shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            //we will need order header id
            _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            foreach(var item in shoppingCartVM.ListCart)
            {
                item.Price = SD.GetPriceBasedOnQty(item.Count, item.Product.Price, item.Product.Price50, item.Product.Price100);
                OrderDetails orderDetail = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderId = shoppingCartVM.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                shoppingCartVM.OrderHeader.OrderTotal += orderDetail.Count * orderDetail.Price;
                _unitOfWork.OrderDetails.Add(orderDetail);
            }
            //remove items from shopping cart
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCartVM.ListCart);
            HttpContext.Session.SetObject(SD.ssShoppingCart, 0);

            //check tokens
            if(stripeToken ==null)
            {
                //only null if company pays, give them x days to pay => no stripe => to token
                //authorized user placing order
                //create order
                shoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            else
            {
                //process payment
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(shoppingCartVM.OrderHeader.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Order Id "+shoppingCartVM.OrderHeader.Id,
                    Source = stripeToken,

                };
                var service = new ChargeService();
                //creates transaction to credit card
                Charge charge = service.Create(options);
                if(charge.BalanceTransactionId == null)
                {
                    //issue with payment
                    shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedRejected;
                }
                else
                {
                    shoppingCartVM.OrderHeader.TransactionId = charge.BalanceTransactionId;
                }
                if(charge.Status.ToLower()=="succeeded")
                {
                    //update 
                    shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                    shoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;

                }
            }
            _unitOfWork.Save();

            return RedirectToAction("OrderConfirmation","Cart", new { id=shoppingCartVM.OrderHeader.Id});
        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}
