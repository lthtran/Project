﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.App_Start;
using WebsiteThuCungBento.Models;
using PagedList;

namespace WebsiteThuCungBento.Controllers
{
    [AdminPhanQuyen(MACHUCNANG = "QL_DONDATHANG")]
    public class DonHangController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        // GET: QLDonDatHang
        public Double iTONGTIEN { set; get; }
        public Double tTONGTIEN { set; get; }
        //public ActionResult Index()
        //{
        //    if (Session["Taikhoanadmin"] == null)
        //    {
        //        return RedirectToAction("dangnhap", "Admin");
        //    }
        //    else
        //    {
        //        return View();
        //    }
        //}
    public ActionResult Index(int? page)
    {
        if (Session["Taikhoanadmin"] == null)
            return RedirectToAction("dangnhap", "Admin");
        else { 
            int pageSize = 10; // Number of items per page
            int pageNumber = (page ?? 1);

            var donDatHangs = data.DONDATHANGs.OrderBy(d => d.NGAYDAT).ToList();
            return View(donDatHangs.ToPagedList(pageNumber, pageSize));
        }
    }


    public ActionResult DonDatHang()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var hang = from h in data.DONDATHANGs select h;
                return View(hang);
            }
        }

        public ActionResult DonDKDV()
        {
            if (Session["Taikhoanadmin"] == null)
            {
                return RedirectToAction("dangnhap", "Admin");
            }
            else
            {
                var hang = from h in data.DangKyDichVus select h;
                return View(hang);
            }
        }
        public ActionResult ChangeStatusSignService(int id, int status)
        {
            //Lấy đối tượng sản phẩm cần xóa theo mã
            DangKyDichVu ddk = data.DangKyDichVus.SingleOrDefault(n => n.SoDK == id);
            bool isSuccess = false;
            if (ddk != null)
            {
                ddk.TinhTrang = (int)status;
                data.SubmitChanges();
                isSuccess = true;
            }
            return Json(new { Success = isSuccess });
        }

        //private double TongTien()
        //{
        //    double iTongTien = 0;
        //    List<Giohang> dsGiohang = Session["GioHang"] as List<Giohang>;
        //    if (dsGiohang != null)
        //    {
        //        iTongTien = dsGiohang.Sum(n => n.dTHANHTIEN);
        //    }
        //    return iTongTien;
        //}

        public ActionResult ChiTietDonHang(int id)
        {
            if (Session["Taikhoanadmin"] == null)//Chưa đăng nhập => Login
            {
                return RedirectToAction("dangnhap", "Admin");
            }    
            else
            { 
            //Lấy ra thông tin Chi tiết đơn hàng từ mã đơn hàng truyền vào
            //Ở đây 1 đơn hàng có thể có nhiều chi tiết ĐH(mua nhiều SP), nên dùng where như trang Thú Cưng theo nhà sản xuất
            var CTDH = (from c in data.CTDONDATHANGs where c.MADH == id select c).ToList();
                return View(CTDH);
            }
        }

        [HttpGet]
        public ActionResult XoaDonHang(int id)
        {
            DONDATHANG hang = data.DONDATHANGs.SingleOrDefault(h => h.MADH == id);
            //ViewBag.MaHang = hang.MaHang;
            if (hang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(hang);
        }
        [HttpPost]
        public ActionResult XacNhanDonHang(int id)
        {
            // Tìm đơn hàng theo mã đơn hàng
            var donHang = data.DONDATHANGs.SingleOrDefault(h => h.MADH == id);
            if (donHang != null)
            {
                // Cập nhật trạng thái đơn hàng
                donHang.TINHTRANGDH = true; // Hoặc giá trị tương ứng với việc xác nhận
                data.SubmitChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            }

            // Chuyển hướng về danh sách đơn hàng hoặc thông báo
            return RedirectToAction("DonDatHang");
        }

        [HttpPost, ActionName("XoaDonHang")]

        //[HttpPost, ActionName("XacnhanXoa")]
        public ActionResult XacnhanXoaDon(int id)
        {
            DONDATHANG hang = data.DONDATHANGs.SingleOrDefault(h => h.MADH == id);

            if (hang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng cần xóa.";
                return RedirectToAction("DonDatHang", "DonHang");
            }

            // Xóa các chi tiết đơn hàng liên quan
            var chiTietDonHangs = data.CTDONDATHANGs.Where(ct => ct.MADH == id).ToList();
            data.CTDONDATHANGs.DeleteAllOnSubmit(chiTietDonHangs);

            // Xóa đơn hàng
            data.DONDATHANGs.DeleteOnSubmit(hang);
            data.SubmitChanges();

            TempData["SuccessMessage"] = "Đã xóa đơn hàng thành công.";
            return RedirectToAction("DonDatHang", "DonHang");
        }

        //public ActionResult XacnhanXoaDon(int id)
        //{
        //    DONDATHANG hang = data.DONDATHANGs.SingleOrDefault(h => h.MADH == id);
        //    data.DONDATHANGs.DeleteOnSubmit(hang);
        //    data.SubmitChanges();
        //    return RedirectToAction("DonDatHang", "DonHang");
        //}
    }
}