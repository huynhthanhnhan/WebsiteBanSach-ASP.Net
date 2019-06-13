using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
			if (Session["TaiKhoan"] != null)
			{
				var cart = Session[CartSession];
				var list = new List<GioHang>();
				if (cart != null)
				{
					list = (List<GioHang>)cart;
				}
				ViewBag.KhachHang = (KhachHang)Session["TaiKhoan"];
				return View(list);
			}
			else
				return Redirect("/nguoidung/dangnhap");
		}

		public long Insert(DonHang order)
		{
			db.DonHang.Add(order);
			db.SaveChanges();
			return order.MaDonHang;
		}

		public bool Insert(ChiTietDonHang detail)
		{
			try
			{
				db.ChiTietDonHang.Add(detail);
				db.SaveChanges();
				return true;
			}
			catch
			{
				return false;

			}
		}

		[HttpPost]
		public ActionResult Payment(int MaKH)
		{
			var donHang = new DonHang();
			donHang.NgayDat = DateTime.Now;
			donHang.NgayGiao = DateTime.Now;
			donHang.MaKH = MaKH;
			donHang.TinhTrangGiaoHang = 1;
			

			try
			{
				var id = Insert(donHang);
				var cart = (List<GioHang>)Session[CartSession];
				
				decimal total = 0;
				foreach (var item in cart)
				{
					var chiTietDonHang = new ChiTietDonHang();
					chiTietDonHang.MaSach = item.Sach.MaSach;
					chiTietDonHang.MaDonHang =(int) id;
					chiTietDonHang.DonGia = item.Sach.GiaBan;
					chiTietDonHang.SoLuong = item.SoLuong;
					Insert(chiTietDonHang);

					total += (item.Sach.GiaBan.GetValueOrDefault(0) * item.SoLuong);
				}
				Session[CartSession] = null;
				string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/templateClient/donhang.html"));
				KhachHang kh = db.KhachHang.SingleOrDefault(x => x.MaKH == MaKH);
				content = content.Replace("{{CustomerName}}", kh.HoTen);
				content = content.Replace("{{Phone}}", kh.DienThoai);
				content = content.Replace("{{Email}}", kh.Email);
				content = content.Replace("{{Address}}", kh.DiaChi);
				content = content.Replace("{{Total}}", total.ToString("N0"));
				var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

				SendMail(kh.Email, "Đơn hàng mới từ Cửa hàng bán sách online", content);
				SendMail(toEmail, "Đơn hàng mới từ Cửa hàng bán sách online", content);
				
			}
			catch (Exception ex)
			{
				//ghi log
				Console.WriteLine(ex.ToString());
				ViewBag.Loi = ex.ToString();
				return View("Fail");
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
		public void SendMail(string toEmailAddress, string subject, string content)
		{
			var fromEmailAddress = ConfigurationManager.AppSettings["FromEmailAddress"].ToString();
			var fromEmailDisplayName = ConfigurationManager.AppSettings["FromEmailDisplayName"].ToString();
			var fromEmailPassword = ConfigurationManager.AppSettings["FromEmailPassword"].ToString();
			var smtpHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
			var smtpPort = ConfigurationManager.AppSettings["SMTPPort"].ToString();

			bool enabledSsl = bool.Parse(ConfigurationManager.AppSettings["EnabledSSL"].ToString());

			string body = content;
			MailMessage message = new MailMessage(new MailAddress(fromEmailAddress, fromEmailDisplayName), new MailAddress(toEmailAddress));
			message.Subject = subject;
			message.IsBodyHtml = true;
			message.Body = body;

			var client = new SmtpClient();
			client.Credentials = new NetworkCredential(fromEmailAddress, fromEmailPassword);
			client.Host = smtpHost;
			client.EnableSsl = enabledSsl;
			client.Port = !string.IsNullOrEmpty(smtpPort) ? Convert.ToInt32(smtpPort) : 0;
			client.Send(message);
		}

	}
}