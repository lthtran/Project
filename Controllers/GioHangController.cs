using WebsiteThuCungBento.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebsiteThuCungBento.Services;
using WebsiteThuCungBento.ViewModels;
using WebsiteThuCungBento.Utils;
using System.Text;
using System.Security.Cryptography;

namespace WebsiteThuCungBento.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        DataClassesDataContext data = new DataClassesDataContext();
        //public List<Giohang> Laygiohang()
        //{
        //    List<Giohang> dsGiohang = Session["Giohang"] as List<Giohang>;
        //    if (dsGiohang == null)
        //    {
        //        dsGiohang = new List<Giohang>();
        //        Session["Giohang"] = dsGiohang;
        //    }
        //    return dsGiohang;
        //}
        //public GioHangController(IVnPayService vnPayService)
        //{
        //    _vnPayService = vnPayService;
        //}

        public List<GiohangModels> Laygiohang()
        {
            string userId = ""; // Default in case user is not logged in

            // Retrieve user from session (this assumes you're saving the user in session)
            if (Session["Taikhoan"] != null)
            {
                KHACHHANG user = (KHACHHANG)Session["Taikhoan"];
                userId = user.MAKH.ToString(); // Use User ID or username
            }

            // Retrieve cart cookie for the specific user
            HttpCookie cartCookie = Request.Cookies["Giohang_" + userId];
            List<GiohangModels> dsGiohang = new List<GiohangModels>();

            if (cartCookie != null && !string.IsNullOrEmpty(cartCookie.Value))
            {
                string json = Server.UrlDecode(cartCookie.Value);
                var serializer = new JavaScriptSerializer();
                dsGiohang = serializer.Deserialize<List<GiohangModels>>(json);
            }

            if (dsGiohang == null || dsGiohang.Count == 0)
            {
                dsGiohang = new List<GiohangModels>();
            }

            return dsGiohang;
        }




        //Them hang vao gio
        //public ActionResult ThemGiohang(int iMASP, string strURL)
        //    {
        //        List<Giohang> dsGiohang = Laygiohang();
        //        Giohang sanpham = dsGiohang.Find(n => n.iMASP == iMASP);
        //        if (sanpham == null)
        //        {
        //            sanpham = new Giohang(iMASP);
        //            dsGiohang.Add(sanpham);
        //            return Redirect(strURL);
        //        }
        //        else
        //        {
        //            sanpham.iSOLUONG++;
        //            return Redirect(strURL);
        //        }
        //    }

        public ActionResult ThemGiohang(int iMASP, string strURL)
        {
            List<GiohangModels> dsGiohang = Laygiohang();
            GiohangModels sanpham = dsGiohang.Find(n => n.iMASP == iMASP);
            if(Session["Taikhoan"] != null){
                if (sanpham == null)
                {
                    sanpham = new GiohangModels(iMASP);
                    dsGiohang.Add(sanpham);
                }
                else
                {
                sanpham.iSOLUONG++;
            }
            // Serialize and store cart as a cookie
            SaveCartCookie(dsGiohang);
            }
            else
            {
                return RedirectToAction("Dangnhap","User" );
            }
            return Redirect(strURL);
        }

        private void SaveCartCookie(List<GiohangModels> dsGiohang)
        {
            string userId = ""; // Default in case user is not logged in

            // Retrieve user from session
            if (Session["Taikhoan"] != null)
            {
                KHACHHANG user = (KHACHHANG)Session["Taikhoan"];
                userId = user.MAKH.ToString(); // Use User ID or username
            }

            // Serialize cart to JSON
            var serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(dsGiohang);
            string encodedJson = Server.UrlEncode(json);

            // Create or update cookie for the user
            HttpCookie cartCookie = new HttpCookie("Giohang_" + userId, encodedJson);
            cartCookie.Expires = DateTime.Now.AddDays(7); // Set the cookie expiration
            Response.Cookies.Add(cartCookie);
        }



        //Xay dung trang Gio hang
        public ActionResult GioHang()
        {
            List<GiohangModels> dsGiohang = Laygiohang();
            if (dsGiohang.Count == 0)
            {
                return RedirectToAction("Index", "User");
            }
            ViewBag.Tongsoluong = TongSoLuong(dsGiohang);
            ViewBag.Tongtien = TongTien(dsGiohang);
            return View(dsGiohang);
        }
        //Tong so luong
        //private int TongSoLuong()
        //{
        //    int iTongSoLuong = 0;
        //    List<Giohang> dsGiohang = Session["GioHang"] as List<Giohang>;
        //    if (dsGiohang != null)
        //    {
        //        iTongSoLuong = dsGiohang.Sum(n => n.iSOLUONG);
        //    }
        //    return iTongSoLuong;
        //}
        private int TongSoLuong(List<GiohangModels> dsGiohang)
        {
            int iTongSoLuong = 0;

            if (dsGiohang != null)
            {
                iTongSoLuong = dsGiohang.Sum(n => n.iSOLUONG);
            }

            return iTongSoLuong;
        }


        //Tinh tong tien
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

        private double TongTien(List<GiohangModels> dsGiohang)
        {
            double iTongTien = 0;

            if (dsGiohang != null)
            {
                iTongTien = dsGiohang.Sum(n => n.dTHANHTIEN);  // Assuming dTHANHTIEN is the total price for each product
            }

            return iTongTien;
        }


        //Tao Partial view de hien thi thong tin gio hang
        public ActionResult GiohangPartial()
        {
            List<GiohangModels> dsGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong(dsGiohang);
            ViewBag.Tongtien = TongTien(dsGiohang);
            return PartialView();
        }

        //Cap nhat Giỏ hàng
        public ActionResult CapnhatGiohang(int iMaSP, FormCollection f)
        {
            List<GiohangModels> dsGiohang = Laygiohang();
            GiohangModels sanpham = dsGiohang.SingleOrDefault(n => n.iMASP == iMaSP);
            if (sanpham != null)
            {
                sanpham.iSOLUONG = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }
        //Xoa Giohang
        //public ActionResult XoaGiohang(int iMaSP)
        //{
        //    List<Giohang> dsGiohang = Laygiohang();
        //    Giohang sanpham = dsGiohang.SingleOrDefault(n => n.iMASP == iMaSP);
        //    if (sanpham != null)
        //    {
        //        dsGiohang.RemoveAll(n => n.iMASP == iMaSP);
        //        return RedirectToAction("GioHang");

        //    }
        //    if (dsGiohang.Count == 0)
        //    {
        //        return RedirectToAction("index", "User");
        //    }
        //    return RedirectToAction("GioHang");
        //}

        public ActionResult XoaGiohang(int iMaSP)
        {
            List<GiohangModels> dsGiohang = Laygiohang();
            GiohangModels sanpham = dsGiohang.SingleOrDefault(n => n.iMASP == iMaSP);

            if (sanpham != null)
            {
                dsGiohang.Remove(sanpham);
                SaveCartCookie(dsGiohang);
            }

            return RedirectToAction("GioHang");
        }


        //Xoa tat ca thong tin trong Gio hang
        //public ActionResult XoaTatcaGiohang()
        //{
        //    List<Giohang> dsGiohang = Laygiohang();
        //    dsGiohang.Clear();
        //    return RedirectToAction("index", "User");
        //}

        public ActionResult XoaTatcaGiohang()
        {
            // Clear all cookies related to the shopping cart
            foreach (string cookieName in Request.Cookies.AllKeys)
            {
                // You can adjust the condition to match your naming convention for cart cookies
                if (cookieName.StartsWith("Giohang"))
                {
                    HttpCookie cartCookie = new HttpCookie(cookieName)
                    {
                        Expires = DateTime.Now.AddDays(-1) // Expire the cookie
                    };
                    Response.Cookies.Add(cartCookie);
                }
            }

            // Optionally clear the session if you're using session to track the cart
            Session["Giohang"] = null;

            return RedirectToAction("Index", "User");
        }


        [HttpGet]
        public ActionResult DatHang()
        {
            //Kiem tra dang nhap
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                Session["ReturnUrl"] = Url.Action("DatHang");
                return RedirectToAction("Dangnhap", "User");
            }
            List<GiohangModels> lstGiohang = Laygiohang();

            //if (Session["Giohang"] == null)
            //{
            //    return RedirectToAction("Index", "User");
            //}
            if (lstGiohang == null || lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "User");
            }
            //Lay gio hang tu Session
            ViewBag.Tongsoluong = TongSoLuong(lstGiohang);
            ViewBag.TongTien = TongTien(lstGiohang);

            return View(lstGiohang);
        }

        #region Thêm đơn đặt hàng mới
        //[HttpPost]
        //public ActionResult DatHang(FormCollection collection)
        //{
        //    //Them Don hang
        //    DONDATHANG ddh = new DONDATHANG();
        //    KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
        //    List<Giohang> gh = Laygiohang();
        //    ddh.MAKH = kh.MAKH;
        //    ddh.NGAYDAT = DateTime.Now;
        //    if (collection["Ngaygiao"].Equals(""))
        //    {
        //        DateTime aDateTime = DateTime.Now;
        //        DateTime newTime = aDateTime.AddDays(7);
        //        ddh.NGAYGIAO = newTime;
        //    }
        //    else
        //    {
        //        var ngaygiao = String.Format("{0:dd/MM/yyyy}", collection["Ngaygiao"]);
        //        ddh.NGAYGIAO = DateTime.Parse(ngaygiao);
        //    }

        //    ddh.TINHTRANGDH = false;

        //    int HTTH = int.Parse(collection["sl_ThanhToan"]);
        //    if (HTTH == 0)
        //        ddh.DATHANHTOAN = false;
        //    else
        //        ddh.DATHANHTOAN = true;

        //    ddh.TONGTIEN = (decimal)TongTien();
        //    data.DONDATHANGs.InsertOnSubmit(ddh);
        //    data.SubmitChanges();
        //    foreach (var item in gh)
        //    {
        //        SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == item.iMASP);
        //        if (sanpham.SOLUONG >= item.iSOLUONG)
        //        {
        //            CTDONDATHANG ctdh = new CTDONDATHANG();
        //            ctdh.MADH = ddh.MADH;
        //            ctdh.MASP = item.iMASP;
        //            ctdh.SOLUONG = item.iSOLUONG;
        //            ctdh.DONGIA = (int)item.dDONGIA;
        //            ctdh.THANHTIEN = (decimal)item.dTHANHTIEN;
        //            data.CTDONDATHANGs.InsertOnSubmit(ctdh);
        //            sanpham.SOLUONG = sanpham.SOLUONG - item.iSOLUONG;
        //            data.SubmitChanges();
        //        }
        //        else
        //        {
        //            return RedirectToAction("ThongBao", "Giohang");
        //        }

        //    }
        //    ClearCart();
        //    return RedirectToAction("Xacnhandonhang", "Giohang");

        //}

      

        [HttpPost]
        public ActionResult DatHang(FormCollection collection)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<GiohangModels> gh = Laygiohang();

            if (gh == null || gh.Count == 0)
            {
                return RedirectToAction("Index", "User");
            }

            // Set basic order details
            ddh.MAKH = kh.MAKH;
            ddh.NGAYDAT = DateTime.Now;
            ddh.NGAYGIAO = string.IsNullOrEmpty(collection["Ngaygiao"]) ? DateTime.Now.AddDays(7) : DateTime.Parse(collection["Ngaygiao"]);
            ddh.TINHTRANGDH = false; // Order is not confirmed yet
            ddh.DATHANHTOAN = false; // Payment status is false until confirmed
            ddh.HINHTHUCTT = "Ship COD thanh toán khi nhận hàng";
            ddh.TONGTIEN = (decimal)TongTien(gh);

            // Insert the order into database but not commit (we'll commit after payment confirmation)
            data.DONDATHANGs.InsertOnSubmit(ddh);
            data.SubmitChanges();
            // Non-VNPAY payment (cash on delivery)
            foreach (var item in gh)
            {
                SANPHAM sanpham = data.SANPHAMs.Single(n => n.MASP == item.iMASP);
                if (sanpham.SOLUONG >= item.iSOLUONG)
                {
                    CTDONDATHANG ctdh = new CTDONDATHANG
                    {
                        MADH = ddh.MADH,
                        MASP = item.iMASP,
                        SOLUONG = item.iSOLUONG,
                        DONGIA = (int)item.dDONGIA,
                        THANHTIEN = (decimal)item.dTHANHTIEN
                    };
                    data.CTDONDATHANGs.InsertOnSubmit(ctdh);

                    // Deduct the product quantity
                    sanpham.SOLUONG -= item.iSOLUONG;
                    data.SubmitChanges();
                }
                else
                {
                    return RedirectToAction("ThongBao", "Giohang");
                }
            }

            // Clear cart after order placement
            ClearCart();

            return RedirectToAction("Xacnhandonhang", "Giohang");
        }


        #endregion
        private void ClearCart()
        {
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            int userId = kh.MAKH;
            string cookieName = "Giohang_" + userId;
            HttpCookie cartCookie = Request.Cookies["Giohang_" + userId];
            // Clear the cookie by setting its expiry date to a past date
            if (cartCookie != null)
            {
                // The cookie exists and you can access its values
                string cartData = cartCookie.Value; // Get the value of the cookie
                                                    // Process the cookie data as needed
                HttpCookie expiredCookie = new HttpCookie(cookieName)
                {
                    Expires = DateTime.Now.AddDays(-1) // Set the expiration date to a past date
                };
                Response.Cookies.Add(expiredCookie);
            }
            // Optionally, also clear the session
        }



        public ActionResult ThongBao()
        {
            return View();
        }
            
        public ActionResult Xacnhandonhang()
        {
            var dh = from d in data.DONDATHANGs select d.NGAYGIAO;
            return View(dh);
        }

        #region Lấy hình thương hiệu
        public ActionResult hinhthuonghieu()
        {
            var listthuonghieu = from THUONGHIEU in data.THUONGHIEUs select THUONGHIEU;
            return PartialView(listthuonghieu);
        }
        #endregion

    

    }
}