using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebSitebanSach.Models;
using WebSiteBanSach.Models;
namespace WebSiteBanSach.Controllers
{
	public class GioHangController : Controller
	{
		QuanLyBanSachModel db = new QuanLyBanSachModel();
		private const string CartSession = "CartSession";
		// GET: Cart
		public ActionResult Index()
		{
			var cart = Session[CartSession];
			var list = new List<GioHang>();
			if (cart != null)
			{
				list = (List<GioHang>)cart;
			}
			return View(list);
		}

		//Tính tổng số lượng
		private int TongSoLuong()
		{
			int TongSoLuong = 0;
			List<GioHang> lstGioHang = Session[CartSession] as List<GioHang>;
			if (lstGioHang != null)
			{
				TongSoLuong = lstGioHang.Sum(n => n.SoLuong);
			}
			return TongSoLuong;
		}
		

		//tạo partial giỏ hàng
		public ActionResult GioHangPartial()
		{
			if (TongSoLuong() == 0)
			{
				return PartialView();
			}
			ViewBag.TongSoLuong = TongSoLuong();
			
			return PartialView();
		}

		public JsonResult DeleteAll()
		{
			Session[CartSession] = null;
			return Json(new
			{
				status = true
			});
		}

		public JsonResult Delete(long id)
		{
			var sessionCart = (List<GioHang>)Session[CartSession];
			sessionCart.RemoveAll(x => x.Sach.MaSach == id);
			Session[CartSession] = sessionCart;
			return Json(new
			{
				status = true
			});
		}
		public JsonResult Update(string cartModel)
		{
			var jsonCart = new JavaScriptSerializer().Deserialize<List<GioHang>>(cartModel);
			var sessionCart = (List<GioHang>)Session[CartSession];

			foreach (var item in sessionCart)
			{
				var jsonItem = jsonCart.SingleOrDefault(x => x.Sach.MaSach == item.Sach.MaSach);
				if (jsonItem != null)
				{
					item.SoLuong = jsonItem.SoLuong;
				}
			}
			Session[CartSession] = sessionCart;
			return Json(new
			{
				status = true
			});
		}
		public ActionResult AddItem(long productId, int quantity)
		{
			var product = db.Sach.Find(productId);

			var cart = Session[CartSession];
			if (cart != null)
			{
				var list = (List<GioHang>)cart;
				if (list.Exists(x => x.Sach.MaSach == productId))
				{

					foreach (var item in list)
					{
						if (item.Sach.MaSach == productId)
						{
							item.SoLuong += quantity;
						}
					}
				}
				else
				{
					//tạo mới đối tượng cart item
					var item = new GioHang();
					item.Sach = product;
					item.SoLuong = quantity;
					list.Add(item);
				}
				//Gán vào session
				Session[CartSession] = list;
			}
			else
			{
				//tạo mới đối tượng cart item
				var item = new GioHang();
				item.Sach = product;
				item.SoLuong = quantity;
				var list = new List<GioHang>();
				list.Add(item);
				//Gán vào session
				Session[CartSession] = list;
			}
			return RedirectToAction("Index");
		}
		[HttpGet]
		public ActionResult Payment()
		{
			var cart = Session[CartSession];
			var list = new List<GioHang>();
			if (cart != null)
			{
				list = (List<GioHang>)cart;
			}
			return View(list);
		}

		public long Insert(Order order)
		{
			//db.Orders.Add(order);
			db.SaveChanges();
			return order.ID;
		}

		[HttpPost]
		public ActionResult Payment(string shipName, string mobile, string address, string email)
		{
			var order = new Order();
			order.CreatedDate = DateTime.Now;
			order.ShipAddress = address;
			order.ShipMobile = mobile;
			order.ShipName = shipName;
			order.ShipEmail = email;

			try
			{
				var id = Insert(order);
				var cart = (List<GioHang>)Session[CartSession];
				//var detailDao = new Model.Dao.OrderDetailDao();
				decimal total = 0;
				foreach (var item in cart)
				{
					//var orderDetail = new OrderDetail();
					//orderDetail.ProductID = item.Product.ID;
					//orderDetail.OrderID = id;
					//orderDetail.Price = item.Product.Price;
					//orderDetail.Quantity = item.Quantity;
					//detailDao.Insert(orderDetail);

					//total += (item.Product.Price.GetValueOrDefault(0) * item.Quantity);
				}
				string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/templateClient/donhang.html"));

				content = content.Replace("{{CustomerName}}", shipName);
				content = content.Replace("{{Phone}}", mobile);
				content = content.Replace("{{Email}}", email);
				content = content.Replace("{{Address}}", address);
				content = content.Replace("{{Total}}", total.ToString("N0"));
				var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

				//new MailHelper().SendMail(email, "Đơn hàng mới từ OnlineShop", content);
				//new MailHelper().SendMail(toEmail, "Đơn hàng mới từ OnlineShop", content);
			}
			catch (Exception ex)
			{
				//ghi log
				return Redirect("/giohang/fail");
			}
			return Redirect("/giohang/success");
		}

		public ActionResult Success()
		{
			return View();
		}
		public ActionResult Fail()
		{
			return View();
		}

	}
}