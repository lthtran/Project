using WebsiteThuCungBento.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.Net.Mail;
using System.Net;
using System.IO;

using WebsiteThuCungBento.ViewModels;
using System.Configuration;
using System.Data.Linq;
using System.Web.Helpers;

namespace WebsiteThuCungBento.Controllers
{
    public class UserController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();

        public object CommonConstants { get; private set; }
        // GET: User
        public ActionResult index()
        {

            var allacc = (from a in data.SANPHAMs
                          join b in data.THUONGHIEUs on a.MATH equals b.MATH
                          join c in data.LOAIs on a.MALOAI equals c.MALOAI
                          join d in data.MAUSACs on a.MAMAUSAC equals d.MAMAUSAC

                          select new ProductViewModel
                          {
                              MASP = a.MASP,
                              TENSP = a.TENSP,
                              DONGIABAN = a.DONGIABAN,
                              HINHANH = a.HINHANH,
                              MATH = a.MATH,
                              MALOAI = a.MALOAI,
                              TENTH = b.TENTH,
                              TENLOAI = c.TENLOAI,
                              SOLUONG = (int)a.SOLUONG,
                              MOTA = a.MOTA,
                              TENMAUSAC = d.TENMAUSAC,
                              LOGO = b.LOGO
                          }).OrderBy(x => x.MASP).Take(count: 8).ToList(); /*Lấy ra 6 sản phẩm đầu tiên hiển thị ở sản phẩm nổi bật*/
           
            return View(allacc);
        }

        public ActionResult blogs()
        {
            return View();
        }


        #region Lấy sản phẩm
        public ActionResult sanpham(int? page)
        {
            if (page == null) page = 1;
            //Số mẫu tin tối đa cho 1 trang
            int pagesize = 12;
            //Nếu biến page là null thì pagenum=1, ngược pagenum = page.
            int pagenum = (page ?? 1);
            var allacc = (from a in data.SANPHAMs
                          join b in data.THUONGHIEUs on a.MATH equals b.MATH
                          join c in data.LOAIs on a.MALOAI equals c.MALOAI
                          join d in data.MAUSACs on a.MAMAUSAC equals d.MAMAUSAC

                          select new ProductViewModel
                          {
                              MASP = a.MASP,
                              TENSP = a.TENSP,
                              DONGIABAN = a.DONGIABAN,
                              HINHANH = a.HINHANH,
                              MATH = a.MATH,
                              MALOAI = a.MALOAI,
                              TENTH = b.TENTH,
                              TENLOAI = c.TENLOAI,
                              SOLUONG = (int)a.SOLUONG,
                              MOTA = a.MOTA,
                              TENMAUSAC = d.TENMAUSAC,

                          }).OrderBy(x => x.MASP);
            return View(allacc.ToPagedList(pagenum, pagesize));
        }
        #endregion

        public ActionResult hinhanh(int id)
        {
            var listhinhanh = from HINH in data.HINHs where HINH.MASP == id select HINH;
            return PartialView(listhinhanh);
        }

        public ActionResult listhinhanhnho(int id)
        {
            var listhinhanhnho = from HINH in data.HINHs where HINH.MASP == id select HINH;
            return PartialView(listhinhanhnho);
        }
        public ActionResult listhinhanhnhoduoi(int id)
        {
            var listhinhanhnho = from HINH in data.HINHs where HINH.MASP == id select HINH;
            return PartialView(listhinhanhnho);
        }


        #region Lấy chi tiết sản phẩm
        public ActionResult Chitiet(int id)
        {
            var detail = from a in data.SANPHAMs
                         join b in data.THUONGHIEUs on a.MATH equals b.MATH
                         join c in data.LOAIs on a.MALOAI equals c.MALOAI
                         join d in data.MAUSACs on a.MAMAUSAC equals d.MAMAUSAC
                         join h in data.HINHs on a.MASP equals h.MASP
                         where a.MASP == id
                         select new ProductViewModel
                         {
                             MASP = a.MASP,
                             TENSP = a.TENSP,
                             DONGIABAN = a.DONGIABAN,
                             HINHANH = a.HINHANH,
                             MATH = a.MATH,
                             MALOAI = a.MALOAI,
                             TENTH = b.TENTH,
                             TENLOAI = c.TENLOAI,
                             SOLUONG = (int)a.SOLUONG,
                             MOTA = a.MOTA,
                             TENMAUSAC = d.TENMAUSAC,
                             HINH1 = h.HINH1,
                             //THANHTOANON = a.THANHTOANON
                         };
            return View(detail.SingleOrDefault());
        }
        #endregion

        public ActionResult gioithieu()
        {
            return View();
        }

        public ActionResult loai()
        {
            var listloai = from LOAI in data.LOAIs select LOAI;
            return PartialView(listloai);
        }

        #region Lấy kích thước sản phẩm
        public ActionResult kichthuoc()
        {
            var listkichthuoc = from KICHTHUOC in data.KICHTHUOCs select KICHTHUOC;
            return PartialView(listkichthuoc);
        }
        #endregion

        #region Lấy thương hiệu sản phẩm
        public ActionResult thuonghieu()
        {
            var listthuonghieu = from THUONGHIEU in data.THUONGHIEUs select THUONGHIEU;
            return PartialView(listthuonghieu);
        }
        #endregion

        #region Lấy hình thương hiệu
        public ActionResult hinhthuonghieu()
        {
            var listthuonghieu = from THUONGHIEU in data.THUONGHIEUs select THUONGHIEU;
            return PartialView(listthuonghieu);
        }
        #endregion

        #region Lấy sản phẩm theo loại sản phẩm
        public ActionResult SPTheoloai(int id)
        {
            var ttsp = data.LOAIs.SingleOrDefault(n => n.MALOAI == id);
            ViewBag.tenloai = ttsp.TENLOAI;
            var sanpham = from SANPHAM in data.SANPHAMs where SANPHAM.MALOAI == id select SANPHAM;
            return View(sanpham);
        }
        #endregion


        #region Lấy sản phẩm theo thương hiệu
        public ActionResult SPTheothuonghieu(int id)
        {
            var ttsp = data.THUONGHIEUs.SingleOrDefault(n => n.MATH == id);
            ViewBag.tenth = ttsp.TENTH;
            var sanpham = from SANPHAM in data.SANPHAMs where SANPHAM.MATH == id select SANPHAM;
            return View(sanpham);
        }
        #endregion


        //Đăng ký và Đăng Nhập
        [HttpGet]
        public ActionResult dangky()
        {
            return View();
        }

        #region Kiểm tra số ký tự lớn hơn 0
        public bool kiemtratendn(string tendn)
        {
            return data.KHACHHANGs.Count(x => x.TENDNKH == tendn) > 0;
        }
        #endregion


        public bool kiemtraemail(string email)
        {
            return data.KHACHHANGs.Count(x => x.EMAIL == email) > 0;
        }


        #region Đăng ký tài khoản người dùng
        [HttpPost]
        public ActionResult dangky(DangKyModel model)
        {
            if (ModelState.IsValid)
            {
                if (kiemtratendn(model.tendn))
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
                }
                else if (kiemtraemail(model.email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại");
                }
                else
                {
                    var mahoa_matkhau = MahoaMD5.GetMD5(model.matkhau);
                    var kh = new KHACHHANG();
                    kh.HOTENKH = model.hoten;
                    kh.TENDNKH = model.tendn;
                    kh.MATKHAUKH = mahoa_matkhau;
                    kh.EMAIL = model.email;
                    kh.DIACHI = model.diachi;
                    kh.DIENTHOAI = model.dienthoai;
                    kh.HINHANH = model.hinhanh;
                    kh.NGAYSINH = model.ngaysinh;
                    data.KHACHHANGs.InsertOnSubmit(kh);
                    data.SubmitChanges();
                    return RedirectToAction("dangnhap");
                }
            }
            return View(model);
        }
        #endregion


        [HttpGet]
        public ActionResult dangnhap()
        {
            return View();
        }


        #region Đăng nhập tài khoản người dùng
        [HttpPost]
        //public ActionResult dangnhap(DangNhapModel model)
        //{
        //    var mahoa_matkhaudangnhap = MahoaMD5.GetMD5(model.matkhau);
        //    if (ModelState.IsValid)
        //    {
        //        KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.TENDNKH == model.tendn && n.MATKHAUKH == mahoa_matkhaudangnhap);
        //        if (kh != null)
        //        {
        //            ViewBag.Thongbao = "Đăng nhập thành công";
        //            Session["Taikhoan"] = kh;
        //            return RedirectToAction("index", "User");
        //        }
        //        else
        //            ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
        //    }
        //    else
        //    {

        //    }
        //    return View(model);
        //}
        public ActionResult dangnhap(DangNhapModel model)
        {
            var mahoa_matkhaudangnhap = MahoaMD5.GetMD5(model.matkhau);

            if (ModelState.IsValid)
            {
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.TENDNKH == model.tendn && n.MATKHAUKH == mahoa_matkhaudangnhap);

                if (kh != null)
                {
                    ViewBag.Thongbao = "Đăng nhập thành công";
                    Session["Taikhoan"] = kh;

                    // Kiểm tra URL nguồn gốc
                    if (Session["ReturnUrl"] != null)
                    {
                        string returnUrl = Session["ReturnUrl"].ToString();
                        Session["ReturnUrl"] = null; // Xóa URL sau khi đã chuyển hướng
                        return Redirect(returnUrl);
                    }

                    // Nếu không có URL, chuyển về Index
                    return RedirectToAction("index", "User");
                }
                else
                {
                    ViewBag.Thongbao = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }

            return View(model);
        }

        #endregion


        #region Kiểm tra thông tin cá nhân của tài khoản đăng nhập
        public ActionResult thongtincanhan()
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("index", "User");

            }
            return View();
        }
        #endregion


        #region Sửa thông tin cá nhân khách hàng
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Taikhoan"] == null)
                return RedirectToAction("index", "User");
            else
            {
                var thongtin = from tt in data.KHACHHANGs where tt.MAKH == id select tt;
                return View(thongtin.Single());
            }
        }
        #endregion


        #region Cập nhật thông tin cá nhân người dùng
        [HttpPost, ActionName("Edit")]
        [ValidateInput(false)]
        //public ActionResult Capnhat(int id, HttpPostedFileBase fileUpload)
        //{
        //    if (Session["Taikhoan"] == null)
        //        return RedirectToAction("index", "User");
        //    else
        //    {
        //        KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);
        //        UpdateModel(kh);

        //        if (fileUpload == null)
        //        {
        //            ViewBag.Thongbao = "Vui lòng chọn ảnh đại diện";
        //            return View();
        //        }
        //        else
        //        {
        //            var fileName = Path.GetFileName(fileUpload.FileName);
        //            var path = Path.Combine(Server.MapPath("~/img/"), fileName);
        //            fileUpload.SaveAs(path);
        //            kh.HINHANH = fileName;
        //            Session["Taikhoan"] = kh;

        //        }

        //    }
        //    data.SubmitChanges();
        //    return RedirectToAction("thongtincanhan", "User");
        //}
        public ActionResult Capnhat(int id, HttpPostedFileBase fileUpload)
        {
            if (Session["Taikhoan"] == null)
                return RedirectToAction("index", "User");

            KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.MAKH == id);

            if (kh == null) // Kiểm tra null
            {
                return HttpNotFound(); // Trả về 404 nếu không tìm thấy khách hàng
            }

            // Cập nhật các thông tin khác
            UpdateModel(kh);

            // Nếu có tệp tải lên, xử lý nó
            if (fileUpload != null)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/img/"), fileName);
                fileUpload.SaveAs(path);
                kh.HINHANH = fileName; // Cập nhật hình ảnh mới
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            data.SubmitChanges();

            // Cập nhật lại session nếu cần
            Session["Taikhoan"] = kh;

            return RedirectToAction("thongtincanhan", "User");
        }

        #endregion

        #region Đăng xuất tài khoản
        public ActionResult dangxuat()
        {
            Session.Clear();
            return RedirectToAction("index", "User");
        }
        #endregion


        #region Tìm kiếm sản phẩm
        public ActionResult Ketquatimkiem(string searchString)
        {

            var links = from l in data.SANPHAMs
                        select l;

            if (!String.IsNullOrEmpty(searchString)) /*Nếu không phải trống thì lấy ra sản phẩm có tên với từ khóa tìm kiếm*/
            {
                links = links.Where(s => s.TENSP.Contains(searchString));
            }
            ViewBag.searchString = searchString;
            //Trả về tất cả sản phẩm
            return View(links);
        }
        #endregion


        #region Lấy kích thước sản phẩm theo mã sản phẩm
        public ActionResult KTtheoMaSP(int id)
        {
            var KichThuoc = from KICHTHUOC in data.KICHTHUOCs where KICHTHUOC.MASP == id select KICHTHUOC;
            return PartialView(KichThuoc);
        }
        #endregion


        #region Gửi liên kết xác minh thay đổi mật khẩu về mail khách hàng yêu cầu
        [NonAction]
        public void SendVerificationLinkEmail(string emailId, string activationCode, string emailFor = "ResetPassword")
        {
            // Generate the URL dynamically using Url.Action
            var verifyUrl = Url.Action(emailFor, "User", new { id = activationCode }, Request.Url.Scheme);

            // Setup email credentials and other information
            var fromEmail = new MailAddress("tranb2014710@student.ctu.edu.vn", "SpaPetShop");
            var toEmail = new MailAddress(emailId);

            // Get the email password securely from configuration
            var fromEmailPassword = ConfigurationManager.AppSettings["EmailPassword"]; // Ensure to add this to web.config or appsettings.json

            string subject = "";
            string body = "";

            // Create email body for ResetPassword
            if (emailFor == "ResetPassword")
            {
                subject = "Đặt lại mật khẩu";
                body = "<b>Xin chào bạn</b>,<br/><br/> Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn. " +
                       "Vui lòng nhấp vào liên kết dưới đây để thiết lập mật khẩu mới cho tài khoản của bạn: " +
                       "<br/><br/><a href=\"" + verifyUrl + "\">Link đặt lại mật khẩu</a>";
            }

            // Setup SMTP client
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            // Send the email
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
            {
                smtp.Send(message);
            }
        }
        //[NonAction]
        //public void SendVerificationLinkEmail(string emailId, string activationCode, string emailFor = "ResetPassword")
        //{
        //    // Generate the URL dynamically using Url.Action
        //    var verifyUrl = Url.Action(emailFor, "User", new { id = activationCode }, Request.Url.Scheme);
        //    var fromEmail = new MailAddress("huyentrantb66666@gmail.com", "FromPetShop");
        //    var toEmail = new MailAddress(emailId);
        //    var fromEmailPassword = "24082002hT"; // Thay thế bằng mật khẩu của Gmail hoặc App Password
        //    string subject = "Đặt lại mật khẩu";
        //    string body = "<b>Xin chào bạn</b>,<br/><br/> Chúng tôi đã nhận được yêu cầu đặt lại mật khẩu của bạn. Vui lòng nhấp vào liên kết dưới đây để thiết lập mật khẩu mới cho tài khoản của bạn " + "<br/><br/><a href=" + verifyUrl + ">Link đặt lại mật khẩu</a>";

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com",
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
        //    };
        //    using (var message = new MailMessage(fromEmail, toEmail)
        //    {
        //        Subject = subject,
        //        Body = body,
        //        IsBodyHtml = true
        //    }) 
        //    smtp.Send(message);
        //}
        #endregion


        [HttpGet]
        public ActionResult QuenMK()
        {
            return View();
        }


        #region Chức năng quên mật khẩu
        [HttpPost]
        public ActionResult QuenMK(QuenMKModel quenMK)
        {
            if (ModelState.IsValid)
            {
                //Xác nhận email
                //tạo liên kết đặt lại mật khẩu
                //gửi email               

                var account = data.KHACHHANGs.SingleOrDefault(n => n.EMAIL == quenMK.Email);
                if (account != null)
                {
                    //gửi mail để thay đổi mật khẩu

                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.EMAIL, resetCode, "ResetPassword");
                    account.KHOIPHUCMATKHAU = resetCode;
                    data.SubmitChanges();
                    TempData["Message"] = "Yêu cầu khôi phục mật khẩu đã được gửi tới email của bạn.";
                    TempData["Status"] = "success";
                    return RedirectToAction("QuenMKxacnhan", "User");

                }
                else
                {

                    ViewBag.message = "Email không đúng";
                }

            }
            else
            {

            }
            return View(quenMK);
        }
        #endregion


        public ActionResult ResetPassword(string id)
        {
            //xác nhận liên kết đặt lại mật khẩu
            //tìm tài khoản được chỉ định với liên kết này
            //chuyển hướng để đặt lại trang mật khẩu
            KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.KHOIPHUCMATKHAU == id);
            if (kh != null)
            {
                ResetPassword model = new ResetPassword();
                model.Resetcode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }


        #region Thay đổi mật khẩu thành công
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.KHOIPHUCMATKHAU == model.Resetcode);
                if (kh != null)
                {
                    //kh.MATKHAUKH = model.NewPassword;
                    kh.MATKHAUKH = MahoaMD5.GetMD5(model.NewPassword);
                    kh.KHOIPHUCMATKHAU = "";
                    UpdateModel(kh);
                    data.SubmitChanges();
                    message = "Cập nhật mật khẩu mới thành công!";
                    return RedirectToAction("dangnhap", "User");
                }
            }
            else
            {
                message = "Liên kết đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";

            }
            ViewBag.Message = message;
            return View(model);
        }
        #endregion


        public ActionResult QuenMKxacnhan()
        {
            return View();
        }


        [NonAction]
        public void sendcontact(string Name, string Email, string Subject, string Content)
        {
            KHACHHANG kh = new KHACHHANG();
            var fromEmail = new MailAddress("tranb2014710@student.ctu.edu.vn");
            var toEmail = new MailAddress(kh.EMAIL);
            var fromEmailPassword = "fgcd ytoi qeij dhrk"; // replace with actual password
            string subject = Subject;
            string body = "<br/> Họ tên: " + Name + "<br/><br/> Email: " + " " + Email + "<br/><br/> Nội dung: " + Content;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            }) smtp.Send(message);
        }


        [HttpGet]
        public ActionResult Lienhe()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Lienhe(LienheModel lienhe)
        {
            if (ModelState.IsValid)
            {
                sendcontact(lienhe.Name, lienhe.Email, lienhe.Subject, lienhe.Message);
                return RedirectToAction("thongbaolienhe", "User");
            }
            else
            {

            }
            return View(lienhe);
        }
        public ActionResult thongbaolienhe()
        {
            return View();
        }

        public ActionResult khachsanthucung()
        {
            var model = new ListDVViewModel()
            {
                listDV = data.DichVus.Where(n => n.LoaiDV == 0).ToList(),
                listCTDV = data.ChiTietDichVus.ToList(),
            };
            return View(model);
        }
        public ActionResult spathucung()
        {
            var model = new ListDVViewModel()
            {
                listDV = data.DichVus.Where(n => n.LoaiDV == 1).ToList(),
                listCTDV = data.ChiTietDichVus.ToList(),
            };
            return View(model);
        }
        public ActionResult TrangDangKy()
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("dangnhap", "User");
            }
            else
            {
                List<DangKy> lstDangKy = Session["DonDK"] as List<DangKy>;
                return View(lstDangKy);
            }
        }
        [HttpPost]
        public ActionResult TrangDangKy(FormCollection collection)
        {
            DangKyDichVu dkdv = new DangKyDichVu();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<DangKy> lstDangKy = Session["DonDK"] as List<DangKy>;
            if (kh != null)
            {
                dkdv.MaKH = kh.MAKH;
                dkdv.Hoten = kh.HOTENKH;
                dkdv.SDT = kh.DIENTHOAI;
                dkdv.Diachi = collection["address"];
            }
            else
            {
                dkdv.Hoten = collection["customer_name"];
                dkdv.SDT = collection["customer_phone"];
                dkdv.Diachi = collection["address"];
            }
            foreach (var i in lstDangKy)
            {
                dkdv.MaCT = i.iMACT;
            }
            dkdv.NgayDangKy = DateTime.Now;
            dkdv.TongTien = Convert.ToInt32(collection["tongtien"]);
            dkdv.TinhTrang = 0;
            data.DangKyDichVus.InsertOnSubmit(dkdv);
            data.SubmitChanges();

            Session["Giohang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }

        public ActionResult DangKyDichVu(FormCollection f)
        {
            Session["DonDK"] = null;

            int iMaCT = int.Parse(f["serviceid"].ToString());

            List<DangKy> lstDangKy = Session["DonDK"] as List<DangKy>;
            if (lstDangKy == null)
            {
                lstDangKy = new List<DangKy>();
                Session["DonDK"] = lstDangKy;
                Session.Timeout = 60;
            }
            DangKy dondk = lstDangKy.Find(n => n.iMACT == iMaCT);
            if (dondk == null)
            {
                dondk = new DangKy(iMaCT);
                lstDangKy.Add(dondk);
            }
            return Redirect("TrangDangKy");
        }
        public ActionResult ListServiceRegis()
        {
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("dangnhap", "User");
            }
            else
            {
                return View(data.DangKyDichVus.Where(i => i.KHACHHANG.TENDNKH == kh.TENDNKH).ToList().OrderByDescending(n => n.NgayDangKy));
            }
        }

        //public ActionResult DONDATHANG(FormCollection f)
        //{
        //    Session["DonDK"] = null;

        //    int iMaCT = int.Parse(f["serviceid"].ToString());

        //    List<DangKy> lstDangKy = Session["DonDK"] as List<DangKy>;
        //    if (lstDangKy == null)
        //    {
        //        lstDangKy = new List<DangKy>();
        //        Session["DonDK"] = lstDangKy;
        //        Session.Timeout = 60;
        //    }
        //    DangKy dondk = lstDangKy.Find(n => n.iMACT == iMaCT);
        //    if (dondk == null)
        //    {
        //        dondk = new DangKy(iMaCT);
        //        lstDangKy.Add(dondk);
        //    }
        //    return Redirect("TrangDangKy");
        //}
        public ActionResult OrderHistory()
        {
            // Kiểm tra nếu khách hàng chưa đăng nhập
            if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
            {
                return RedirectToAction("dangnhap", "User");
            }

            // Lấy thông tin khách hàng từ session
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];

            // Lấy danh sách đơn hàng theo MaKH
            var orders = data.DONDATHANGs
                            .Where(o => o.MAKH == kh.MAKH)
                            .OrderByDescending(o => o.NGAYDAT)
                            .ToList();

            return View(orders); // Truyền danh sách đơn hàng sang view
        }
        [HttpPost]
        public ActionResult ConfirmReceived(int id)
        {
            var order = data.DONDATHANGs.SingleOrDefault(h => h.MADH == id);
            if (order != null)
            {
                order.TINHTRANGDH = true; // Cập nhật trạng thái đã nhận
                order.DATHANHTOAN = true;
                data.SubmitChanges();     // Lưu thay đổi vào cơ sở dữ liệu
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        public ActionResult OrderDetail(int id)
        {
            var orderDetails = data.CTDONDATHANGs.Where(od => od.MADH == id).ToList();
            if (orderDetails == null || !orderDetails.Any())
            {
                return HttpNotFound();
            }
            return PartialView("_OrderDetailPartial", orderDetails);
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
    }
}