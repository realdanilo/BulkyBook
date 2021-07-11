using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BulkyBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    //means someone has to log in
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderDetailsViewModel  OrderDetailsVM{ get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            OrderDetailsVM = new OrderDetailsViewModel()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(o=>o.OrderId==id,includeProperties:"Product")
            };

            return View(OrderDetailsVM);
        }
        #region API CALLS
        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            IEnumerable<OrderHeader> orderHeaderList;
            
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(User.IsInRole(SD.Role_Admin)|| User.IsInRole(SD.Role_Employee))
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                orderHeaderList = _unitOfWork.OrderHeader.GetAll( u => u.ApplicationUserId == claim.Value,includeProperties: "ApplicationUser");

            }
            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(order => order.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;

                case "completed":
                    orderHeaderList = orderHeaderList.Where(order => order.OrderStatus == SD.StatusShipped);
                    break;

                case "rejected":
                    orderHeaderList = orderHeaderList.Where(order => order.OrderStatus == SD.StatusCancelled ||
                    order.OrderStatus == SD.StatusRefunded ||
                    order.OrderStatus == SD.PaymentStatusDelayedRejected);
                    break;

                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(order => order.OrderStatus == SD.StatusApproved ||
                    order.OrderStatus == SD.StatusInProcess||
                    order.OrderStatus == SD.StatusPending
                    );

                    break;
                default:
                    break;

            }
            
            return Json(new { data = orderHeaderList });
        }

        #endregion
    }
}
