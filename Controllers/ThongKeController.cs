using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebsiteThuCungBento.Models;

namespace WebsiteThuCungBento.Controllers
{
    public class ThongKeController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        // GET: ThongKe
        public ActionResult Index()
        {
            if (Session["Taikhoanadmin"] == null)
                return RedirectToAction("dangnhap", "Admin");
            else 
                ViewBag.TongDoanhThu = ThongKeTongDoanhThu();
                ViewBag.TongDonHang = ThongKeDonHang();
                ViewBag.TongSanPham = ThongKeSanPham();
                ViewBag.TongKhachHang = ThongKeKhachHang();
                ViewBag.TongAdmin = ThongKeNhanVien();

            // Fetch product category stats and pass to ViewBag

            var thongKeLoaiSanPham = from sp in data.SANPHAMs
                                     group sp by sp.LOAI into g
                                     select new
                                     {
                                         Loai = g.Key.TENLOAI,
                                         SoLuong = g.Count(),
                                         TongGiaTri = g.Sum(x => x.SOLUONG * x.DONGIABAN),
                                         GiaReNhat = g.Min(x => x.DONGIABAN),
                                         GiaDatNhat = g.Max(x => x.DONGIABAN)
                                     };

            var stats = thongKeLoaiSanPham.ToList();

            // Assuming you have a fixed number of categories:
            ViewBag.Loai1_SoLuong = stats.Count > 0 ? stats[0].SoLuong : 0;
            ViewBag.Loai2_SoLuong = stats.Count > 1 ? stats[1].SoLuong : 0;
            ViewBag.Loai3_SoLuong = stats.Count > 2 ? stats[2].SoLuong : 0;
            ViewBag.Loai4_SoLuong = stats.Count > 3 ? stats[3].SoLuong : 0;
            ViewBag.Loai5_SoLuong = stats.Count > 4 ? stats[4].SoLuong : 0;
            ViewBag.Loai6_SoLuong = stats.Count > 5 ? stats[5].SoLuong : 0;
            ViewBag.Loai7_SoLuong = stats.Count > 6 ? stats[6].SoLuong : 0;

            // Similarly for other stats like TongTien, GiaReNhat, GiaDatNhat
            ViewBag.Loai1_TongTien = stats.Count > 0 ? stats[0].TongGiaTri : 0;
            ViewBag.Loai2_TongTien = stats.Count > 1 ? stats[1].TongGiaTri : 0;
            ViewBag.Loai3_TongTien = stats.Count > 2 ? stats[2].TongGiaTri : 0;
            ViewBag.Loai4_TongTien = stats.Count > 3 ? stats[3].TongGiaTri : 0;
            ViewBag.Loai5_TongTien = stats.Count > 4 ? stats[4].TongGiaTri : 0;
            ViewBag.Loai6_TongTien = stats.Count > 5 ? stats[5].TongGiaTri : 0;
            ViewBag.Loai7_TongTien = stats.Count > 6 ? stats[6].TongGiaTri : 0;

            ViewBag.Loai1_GiaReNhat = stats.Count > 0 ? stats[0].GiaReNhat : 0;
            ViewBag.Loai2_GiaReNhat = stats.Count > 1 ? stats[1].GiaReNhat : 0;
            ViewBag.Loai3_GiaReNhat = stats.Count > 2 ? stats[2].GiaReNhat : 0;
            ViewBag.Loai4_GiaReNhat = stats.Count > 3 ? stats[3].GiaReNhat : 0;
            ViewBag.Loai5_GiaReNhat = stats.Count > 4 ? stats[4].GiaReNhat : 0;
            ViewBag.Loai6_GiaReNhat = stats.Count > 5 ? stats[5].GiaReNhat : 0;
            ViewBag.Loai7_GiaReNhat = stats.Count > 6 ? stats[6].GiaReNhat : 0;

            ViewBag.Loai1_GiaDatNhat = stats.Count > 0 ? stats[0].GiaDatNhat : 0;
            ViewBag.Loai2_GiaDatNhat = stats.Count > 1 ? stats[1].GiaDatNhat : 0;
            ViewBag.Loai3_GiaDatNhat = stats.Count > 2 ? stats[2].GiaDatNhat : 0;
            ViewBag.Loai4_GiaDatNhat = stats.Count > 3 ? stats[3].GiaDatNhat : 0;
            ViewBag.Loai5_GiaDatNhat = stats.Count > 4 ? stats[4].GiaDatNhat : 0;
            ViewBag.Loai6_GiaDatNhat = stats.Count > 5 ? stats[5].GiaDatNhat : 0;
            ViewBag.Loai7_GiaDatNhat = stats.Count > 6 ? stats[6].GiaDatNhat : 0;
            return View();
        }
        
        public double ThongKeDonHang()
        {
            double slDonHang = data.DONDATHANGs.Count();
            return slDonHang;
        }

        public double ThongKeSanPham()
        {
            double slDonHang = data.LOAIs.Count();
            return slDonHang;
        }

        public double ThongKeKhachHang()
        {
            double slDonHang = data.KHACHHANGs.Count();
            return slDonHang;
        }

        public double ThongKeNhanVien()
        {
            double slDonHang = data.ADMINs.Count();
            return slDonHang;
        }

        //public decimal ThongKeTongDoanhThu()
        //{
        //    decimal TongDoanhThu = decimal.Parse(data.CTDONDATHANGs.Sum(n => n.SOLUONG * n.DONGIA).ToString());
        //    return TongDoanhThu;
        //}
        public decimal ThongKeTongDoanhThu()
        {
            decimal TongDoanhThu = decimal.Parse(data.CTDONDATHANGs.Sum(n => (n.DONGIA - n.SANPHAM.DONGIAMUA) * n.SOLUONG).ToString());
            return TongDoanhThu;
        }


        public decimal ThongKeDoanhThuThang(int Thang, int Nam)
        {
            var listDH = data.DONDATHANGs.Where(n => n.NGAYDAT.Month == Thang && n.NGAYDAT.Year == Nam);
            decimal TongTien = 0;
            foreach(var item in listDH)
            {
                TongTien += decimal.Parse(item.CTDONDATHANGs.Sum(n => n.SOLUONG * n.DONGIA).ToString());
            }     
            return TongTien;
        }
        public ActionResult ThongKeDonHangTheoThang()
        {
            var donHangTheoThang = from dh in data.DONDATHANGs
                                   group dh by new { dh.NGAYDAT.Year, dh.NGAYDAT.Month } into g
                                   select new
                                   {
                                       Nam = g.Key.Year,
                                       Thang = g.Key.Month,
                                       SoDonHang = g.Count()
                                   };

            return Json(donHangTheoThang, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ThongKeDoanhThuTheoThang()
        {
            var doanhThuTheoThang = from dh in data.DONDATHANGs
                                    group dh by new { dh.NGAYDAT.Year, dh.NGAYDAT.Month } into g
                                    select new
                                    {
                                        Nam = g.Key.Year,
                                        Thang = g.Key.Month,
                                        TongDoanhThu = g.Sum(x => x.CTDONDATHANGs.Sum(ct => ct.SOLUONG * ct.DONGIA))
                                    };

            return Json(doanhThuTheoThang, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ThongKeKhachHangQuayLai(int top = 5)
        {
            var khachHangQuayLai = (from kh in data.KHACHHANGs
                                    join dh in data.DONDATHANGs on kh.MAKH equals dh.MAKH
                                    group kh by kh.TENDNKH into g
                                    where g.Count() > 1
                                    orderby g.Count() descending
                                    select new
                                    {
                                        TenKhachHang = g.Key,
                                        SoLanMuaHang = g.Count()
                                    }).Take(top); // Giới hạn số lượng khách hàng quay lại

            return Json(khachHangQuayLai, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ThongKeDoanhThuTheoNgay(int month, int year)
        {
            var doanhThuTheoNgay = from dh in data.DONDATHANGs
                                   where dh.NGAYDAT.Month == month && dh.NGAYDAT.Year == year
                                   group dh by dh.NGAYDAT.Day into g
                                   select new
                                   {
                                       Ngay = g.Key,
                                       TongDoanhThu = g.Sum(x => x.CTDONDATHANGs.Sum(ct => ct.SOLUONG * ct.DONGIA))
                                   };

            return Json(doanhThuTheoNgay.ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ThongKeSanPhamBanChayNhat(int top=10)
        {
            var sanPhamBanChay = (from ct in data.CTDONDATHANGs
                                  group ct by ct.SANPHAM into g
                                  orderby g.Sum(x => x.SOLUONG) descending
                                  select new
                                  {
                                      TenSanPham = g.Key.TENSP,
                                      SoLuongBan = g.Sum(x => x.SOLUONG)
                                  }).Take(top);

            return Json(sanPhamBanChay, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ThongKeLoaiSanPham()
        {
            var thongKeLoaiSanPham = from sp in data.SANPHAMs
                                     group sp by sp.LOAI into g
                                     select new
                                     {
                                         Loai = g.Key.TENLOAI,
                                         SoLuong = g.Count(),
                                         TongGiaTri = g.Sum(x => x.SOLUONG * x.DONGIABAN),
                                         GiaReNhat = g.Min(x => x.DONGIABAN),
                                         GiaDatNhat = g.Max(x => x.DONGIABAN)
                                     };

            return Json(thongKeLoaiSanPham, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ThongKeSoLuongTheoLoaiSanPham()
        {
            var thongKeSoLuongLoai = from sp in data.SANPHAMs
                                     group sp by sp.LOAI into g
                                     select new
                                     {
                                         LoaiSP = g.Key.TENLOAI,
                                         SoLuong = g.Count()
                                     };

            return Json(thongKeSoLuongLoai, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ThongKeSoLuongTungSanPham()
        {
            var thongKeSoLuong = from sp in data.SANPHAMs
                                 join loai in data.LOAIs on sp.MALOAI equals loai.MALOAI
                                 where sp.SOLUONG > 0 // Lọc chỉ các sản phẩm còn hàng
                                 group sp by new { loai.MALOAI, loai.TENLOAI } into g
                                 orderby g.Sum(x => x.SOLUONG) descending // Sắp xếp theo số lượng giảm dần
                                 select new
                                 {
                                     TenLoai = g.Key.TENLOAI, // Lấy tên loại từ bảng LOAI
                                     SoLuong = g.Sum(x => x.SOLUONG) // Tính tổng số lượng sản phẩm theo loại
                                 };

            return Json(thongKeSoLuong, JsonRequestBehavior.AllowGet);
        }


    }
}