using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteBanSach.Models;

namespace WebSitebanSach.Controllers
{
    public class NguoiDungController : Controller
    {


		QuanLyBanSachModel db = new QuanLyBanSachModel();
		// GET: /NguoiDung/
		public ActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public ActionResult DangKy()
		{

			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DangKy(KhachHang kh)
		{
			if (ModelState.IsValid)
			{
				//Chèn dữ liệu vào bảng khách hàng
				db.KhachHang.Add(kh);
				//Lưu vào csdl 
				db.SaveChanges();
			}
			return View();
		}
		[HttpGet]
		public ActionResult DangNhap()
		{

			return View();
		}
		[HttpPost]
		public ActionResult DangNhap(FormCollection f)
		{
			string sTaiKhoan = f["txtTaiKhoan"].ToString();
			string sMatKhau = f.Get("txtMatKhau").ToString();
			KhachHang kh = db.KhachHang.SingleOrDefault(n => n.TaiKhoan == sTaiKhoan && n.MatKhau == sMatKhau);
			if (kh != null)
			{
				ViewBag.ThongBao = "Chúc mừng bạn đăng nhập thành công !";
				Session["TaiKhoan"] = kh;
				return View();
			}
			ViewBag.ThongBao = "Tên tài khoản hoặc mật khẩu không đúng!";
			return View();
		}

	}
}