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
        public ActionResult Index(int count = 8)
        {
            // Tính số lượng bán cho từng sản phẩm
            var bestSellingProductIds = (from cthd in data.CTDONDATHANGs
                                         group cthd by cthd.MASP into g
                                         select new
                                         {
                                             MASP = g.Key,
                                             TotalSold = g.Sum(x => x.SOLUONG)
                                         })
                                        .OrderByDescending(x => x.TotalSold)
                                        .Take(count)
                                        .Select(x => x.MASP)
                                        .ToList();

            // Lấy thông tin chi tiết sản phẩm dựa trên danh sách sản phẩm nổi bật
            var allacc = (from a in data.SANPHAMs
                          join b in data.THUONGHIEUs on a.MATH equals b.MATH
                          join c in data.LOAIs on a.MALOAI equals c.MALOAI
                          join d in data.MAUSACs on a.MAMAUSAC equals d.MAMAUSAC
                          where bestSellingProductIds.Contains(a.MASP)
                          select new ProductViewModels
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
                              LOGO = b.LOGO ?? "default-logo.png"
                          }).ToList();

            // Truy vấn dữ liệu cho ListDV
            var listDV = new ListDVViewModel
            {
                listDV = data.DichVus.ToList(),
                listCTDV = data.ChiTietDichVus.ToList(),
                ADMINs = data.ADMINs.ToList(),
            };

            // Tạo ViewModel kết hợp
            var viewModel = new CombinedViewModel
            {
                Products = allacc,
                ListDV = listDV
            };

            return View(viewModel);
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

                          select new ProductViewModels
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
            // First, check if the product exists
            var product = data.SANPHAMs.FirstOrDefault(a => a.MASP == id);
            if (product == null)
            {
                // Redirect to error page or product list if no product is found
                return RedirectToAction("index", "User");
            }

            // Perform left joins to handle cases where related records might be missing
            var detail = from a in data.SANPHAMs.Where(x => x.MASP == id)
                         join b in data.THUONGHIEUs on a.MATH equals b.MATH into brandGroup
                         from brand in brandGroup.DefaultIfEmpty()
                         join c in data.LOAIs on a.MALOAI equals c.MALOAI into categoryGroup
                         from category in categoryGroup.DefaultIfEmpty()
                         join d in data.MAUSACs on a.MAMAUSAC equals d.MAMAUSAC into colorGroup
                         from color in colorGroup.DefaultIfEmpty()
                         join h in data.HINHs on a.MASP equals h.MASP into imageGroup
                         from image in imageGroup.DefaultIfEmpty()
                         select new ProductViewModels
                         {
                             MASP = a.MASP,
                             TENSP = a.TENSP,
                             DONGIABAN = a.DONGIABAN,
                             HINHANH = a.HINHANH,
                             MATH = a.MATH,
                             MALOAI = a.MALOAI,
                             TENTH = brand != null ? brand.TENTH : "Không xác định",
                             TENLOAI = category != null ? category.TENLOAI : "Không xác định",
                             SOLUONG = (int)(a.SOLUONG ?? 0),
                             MOTA = a.MOTA,
                             TENMAUSAC = color != null ? color.TENMAUSAC : "Không xác định",
                             HINH1 = image != null ? image.HINH1 : null
                         };

            var result = detail.FirstOrDefault();

            if (result == null)
            {
                // Log the error or handle as needed
                return RedirectToAction("index", "User");
            }

            return View(result);
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
            return data.KHACHHANGs.Any(x => x.TENDNKH == tendn);
        }

        #endregion


        public bool kiemtraemail(string email)
        {
            return data.KHACHHANGs.Any(x => x.EMAIL == email);
        }


        #region Đăng ký tài khoản người dùng
        [HttpPost]
        //public ActionResult dangky(DangKyModels model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (kiemtratendn(model.tendn))
        //        {
        //            ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
        //        }
        //        else if (kiemtraemail(model.email))
        //        {
        //            ModelState.AddModelError("", "Email đã tồn tại");
        //        }
        //        else
        //        {
        //            var mahoa_matkhau = MahoaMD5Models.GetMD5(model.matkhau);
        //            var kh = new KHACHHANG();
        //            kh.HOTENKH = model.hoten;
        //            kh.TENDNKH = model.tendn;
        //            kh.MATKHAUKH = mahoa_matkhau;
        //            kh.EMAIL = model.email;
        //            kh.DIACHI = model.diachi;
        //            kh.DIENTHOAI = model.dienthoai;
        //            kh.HINHANH = model.hinhanh;
        //            kh.NGAYSINH = model.ngaysinh;
        //            data.KHACHHANGs.InsertOnSubmit(kh);
        //            data.SubmitChanges();
        //            return RedirectToAction("dangnhap");
        //        }
        //    }
        //    return View(model);
        //}
        public ActionResult dangky(DangKyModels model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tính hợp lệ của tên đăng nhập
                if (kiemtratendn(model.tendn))
                {
                    ModelState.AddModelError("", "Tên đăng nhập đã tồn tại.");
                }
                else if (kiemtraemail(model.email))
                {
                    ModelState.AddModelError("", "Email đã tồn tại.");
                }
                else
                {
                    // Mã hóa mật khẩu
                    var mahoa_matkhau = MahoaMD5Models.GetMD5(model.matkhau);

                    // Tạo khách hàng mới
                    var kh = new KHACHHANG
                    {
                        HOTENKH = model.hoten,
                        TENDNKH = model.tendn,
                        MATKHAUKH = mahoa_matkhau,
                        EMAIL = model.email,
                        DIACHI = model.diachi,
                        DIENTHOAI = model.dienthoai,
                        HINHANH = model.hinhanh,
                        NGAYSINH = model.ngaysinh
                    };

                    // Thêm khách hàng vào cơ sở dữ liệu
                    data.KHACHHANGs.InsertOnSubmit(kh);
                    data.SubmitChanges();

                    // Chuyển hướng đến trang đăng nhập sau khi đăng ký thành công
                    TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng đăng nhập.";
                    return RedirectToAction("dangnhap");
                }
            }

            // Nếu có lỗi, giữ nguyên model để hiển thị lại form
            return View(model);
        }

        #endregion


        [HttpGet]
        public ActionResult dangnhap()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
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
        public ActionResult dangnhap(DangNhapModels model)
        {
            var mahoa_matkhaudangnhap = MahoaMD5Models.GetMD5(model.matkhau);

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
        public ActionResult QuenMK(QuenMKModels quenMK)
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
                ResetPasswordModels model = new ResetPasswordModels();
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
        public ActionResult ResetPassword(ResetPasswordModels model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                KHACHHANG kh = data.KHACHHANGs.SingleOrDefault(n => n.KHOIPHUCMATKHAU == model.Resetcode);
                if (kh != null)
                {
                    //kh.MATKHAUKH = model.NewPassword;
                    kh.MATKHAUKH = MahoaMD5Models.GetMD5(model.NewPassword);
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
        public ActionResult Lienhe(LienheModels lienhe)
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
                ADMINs = data.ADMINs.ToList()
            };
            return View(model);
        }
        public ActionResult spathucung()
        {
            var model = new ListDVViewModel()
            {
                listDV = data.DichVus.Where(n => n.LoaiDV == 1).ToList(),
                listCTDV = data.ChiTietDichVus.ToList(),
                ADMINs = data.ADMINs.ToList()
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
                List<DangKyDVModels> lstDangKy = Session["DonDK"] as List<DangKyDVModels>;
                return View(lstDangKy);
            }
        }
        [HttpPost]
        public ActionResult TrangDangKy(FormCollection collection)
        {
            DateTime? ngayDangKy = TempData["NgayDangKy"] as DateTime?;
            DangKyDichVu dkdv = new DangKyDichVu();
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            List<DangKyDVModels> lstDangKy = Session["DonDK"] as List<DangKyDVModels>;

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
                dkdv.KhungGio = i.iKHUNGGIO;
                dkdv.MAADMIN = i?.iMAADMIN;
                dkdv.TENNV = i.iTENNV;
            }
           
            dkdv.TongTien = Convert.ToInt32(collection["tongtien"]);
            dkdv.TinhTrang = 0;
            if (ngayDangKy.HasValue)
            {
                // Sử dụng ngayDangKy ở đây
                dkdv.NgayDangKy = ngayDangKy.Value;
            }
            else
            {
                TempData["ErrorMessage"] = "Ngày đăng ký không hợp lệ!";
                return RedirectToAction("TrangDangKy");
            }
            data.DangKyDichVus.InsertOnSubmit(dkdv);
            data.SubmitChanges();

            Session["Giohang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }
        public ActionResult DangKyDichVu_khachsan(FormCollection f)
        {
            Session["DonDK"] = null;
            int iMaCT = int.Parse(f["serviceid"].ToString());
            var item = data.ChiTietDichVus.SingleOrDefault(n => n.MaCT == iMaCT);
            List<DangKyDVModels> lstDangKy = Session["DonDK"] as List<DangKyDVModels>;
            if (lstDangKy == null)
            {
                lstDangKy = new List<DangKyDVModels>();
                Session["DonDK"] = lstDangKy;
                Session.Timeout = 60;
            }
            DangKyDVModels dondk = lstDangKy.Find(n => n.iMACT == iMaCT);
            if (dondk == null)
            {
                dondk = new DangKyDVModels(iMaCT);
                lstDangKy.Add(dondk);
            }
            return Redirect("TrangDangKy");
            
        }
        public ActionResult DangKyDichVu(FormCollection f)
        {
            Session["DonDK"] = null;
            if(string.IsNullOrEmpty(f["serviceid"]))
            {
                // Nếu đã có người đăng ký ca này, hiển thị thông báo và không tiếp tục
                TempData["ErrorMessage"] = "Vui Lòng Chọn Cân Nặng Của Bé!";
                return RedirectToAction("spathucung");
            }
            if (string.IsNullOrEmpty(f["NgayDangKy"]))
            {
                TempData["ErrorMessage"] = "Vui Lòng Chọn Ngày Đăng Ký!";
                return RedirectToAction("spathucung");
            }
            if (string.IsNullOrEmpty(f["MAADMIN"]))
            {
                // Nếu đã có người đăng ký ca này, hiển thị thông báo và không tiếp tục
                TempData["ErrorMessage"] = "Vui Lòng Chọn Người phụ trách dịch vụ này!";
                return RedirectToAction("spathucung");
            }
            if (string.IsNullOrEmpty(f["KhungGio"]))
            {
                // Nếu đã có người đăng ký ca này, hiển thị thông báo và không tiếp tục
                TempData["ErrorMessage"] = "Vui Lòng Chọn Khung Giờ Dịch Vụ!";
                return RedirectToAction("spathucung");
            }

            int iMaCT = int.Parse(f["serviceid"].ToString());
            int MAADMIN = int.Parse(f["MAADMIN"].ToString());
            var item = data.ChiTietDichVus.SingleOrDefault(n => n.MaCT == iMaCT);
            string TENNV = data.ADMINs.FirstOrDefault(n => n.MAADMIN == MAADMIN)?.HOTEN;
            DateTime ndk = DateTime.Parse(f["NgayDangKy"]); // Lấy Ngày Đăng Ký từ form

            if (TENNV == null)
            {
                TENNV = "Chưa có tên";  // Hoặc bạn có thể để mặc định khác
            }
            string KhungGio = f["KhungGio"];
            var existingBooking = data.DangKyDichVus
             .Where(d => d.MAADMIN == MAADMIN
                    && d.KhungGio == KhungGio
                    && d.NgayDangKy == ndk) // Chỉ kiểm tra ngày hôm nay
             .FirstOrDefault();

            if (existingBooking != null)
            {

                // Nếu đã có người đăng ký ca này, hiển thị thông báo và không tiếp tục
                TempData["ErrorMessage"] = "Nhân viên này đã có lịch đăng ký trong ca này hôm nay. Vui Lòng chọn nhân viên khác";
                return RedirectToAction("spathucung"); // Hoặc chuyển hướng đến trang khác
            }
            List<DangKyDVModels> lstDangKy = Session["DonDK"] as List<DangKyDVModels>;
            if (lstDangKy == null)
            {
                lstDangKy = new List<DangKyDVModels>();
                Session["DonDK"] = lstDangKy;
                Session.Timeout = 60;
                foreach(var i in lstDangKy)
                {
                    i.iTENNV = TENNV;
                    i.iKHUNGGIO = KhungGio;
                    i.iMAADMIN = MAADMIN;
                    i.iTENNV = TENNV;
                }
            }
            DangKyDVModels dondk = lstDangKy.Find(n => n.iMACT == iMaCT);
            if (dondk == null)
            {
                dondk = new DangKyDVModels(iMaCT);
                dondk.iKHUNGGIO = KhungGio;
                dondk.iMAADMIN = MAADMIN;
                dondk.iTENNV = TENNV;
                lstDangKy.Add(dondk);
            }
            DangKyDichVu dkdv = new DangKyDichVu();

            if (DateTime.TryParse(f["NgayDangKy"], out DateTime ngayDangKy))
            {
                KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
                if (kh == null)
                {
                    return Redirect("dangnhap"); // Hoặc chuyển hướng đến trang khác
                }
                dkdv.MaCT = dondk.iMACT;
                dkdv.MaKH = kh.MAKH;
                TempData["NgayDangKy"] = ngayDangKy; // Lưu vào TempData
                return RedirectToAction("TrangDangKy");
            }
            else
            {
                TempData["ErrorMessage"] = "Ngày đăng ký không hợp lệ!";
                return RedirectToAction("TrangDangKy");
            }
        }
        //public ActionResult ListServiceRegis()
        //{
        //    KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
        //    if (Session["Taikhoan"] == null || Session["Taikhoan"].ToString() == "")
        //    {
        //        return RedirectToAction("dangnhap", "User");
        //    }
        //    else
        //    {
        //        return View(data.DangKyDichVus.Where(i => i.KHACHHANG.TENDNKH == kh.TENDNKH).ToList().OrderByDescending(n => n.NgayDangKy));
        //    }
        //}

        public ActionResult ListServiceRegis()
        {
            // Lấy thông tin khách hàng từ Session
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];

            // Kiểm tra nếu khách hàng chưa đăng nhập
            if (kh == null || string.IsNullOrEmpty(kh.TENDNKH))
            {
                return RedirectToAction("dangnhap", "User");
            }

            // Lấy danh sách đăng ký dịch vụ của khách hàng và sắp xếp theo ngày đăng ký giảm dần
            var danhSachDangKy = data.DangKyDichVus
                                       .Where(i => i.KHACHHANG.TENDNKH == kh.TENDNKH)
                                       .OrderByDescending(n => n.NgayDangKy)
                                       .ToList();

            // Lấy tên nhân viên cho từng đăng ký dịch vụ
            foreach (var item in danhSachDangKy)
            {
                // Tìm tên nhân viên từ bảng ADMINs dựa trên MAADMIN
                var admin = data.ADMINs.FirstOrDefault(n => n.MAADMIN == item.MAADMIN);

                if (admin != null)
                {
                    item.TENNV = admin.HOTEN;  // Gán tên nhân viên vào thuộc tính TENNV của DangKyDichVu
                }
                else
                {
                    item.TENNV = "Không xác định";  // Nếu không có thông tin nhân viên, gán giá trị mặc định
                }
            }

            // Truyền danh sách đăng ký dịch vụ vào View
            return View(danhSachDangKy);
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

        [HttpGet]

        public ActionResult AutomationSearch(string searchString)
        {
            var products = data.SANPHAMs
                             .Where(p => p.TENSP.Contains(searchString))
                             .Select(p => new
                             {
                                 name = p.TENSP,
                                 imageUrl = p.HINHANH,
                                 url = Url.Action("Chitiet", "User", new { id = p.MASP })
                             }).ToList();

            return Json(products, JsonRequestBehavior.AllowGet);
        }

        // GET: User/Danhgiadichvu
        public ActionResult Danhgiadichvu(int id)
        {
            var model = new DanhGiaDVModels
            {
                SoDK = id
            };
            return View(model);
        }
        [HttpGet]
        public ActionResult XemDanhGia(int id)
        {
            using (var db = new DataClassesDataContext())
            {
                var danhGia = db.DanhGiaDichVus.FirstOrDefault(d => d.SoDK == id);
                if (danhGia != null)
                {
                    var model = new DanhGiaDVModels
                    {
                        MaDanhGia = danhGia.MaDanhGia,
                        SoDK = danhGia.SoDK,
                        DanhGia = danhGia.DanhGia,
                        BinhLuan = danhGia.BinhLuan,
                        NgayDanhGia = danhGia.NgayDanhGia,
                        DaDanhGia = true
                    };
                    return Json(new { Success = true, Data = model }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Success = false, Message = "Không tìm thấy đánh giá!" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Danhgiadichvu(DanhGiaDVModels model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage).ToList();
                return Json(new { Success = false, Message = "Dữ liệu không hợp lệ", Errors = errors });
            }
            if (ModelState.IsValid)
            {

                using (var db = new DataClassesDataContext())
                {
                    // Kiểm tra xem người dùng đã đánh giá chưa
                    var existingRating = db.DanhGiaDichVus.FirstOrDefault(d => d.SoDK == model.SoDK);
                    if (existingRating == null)
                    {
                        try
                        {
                            var danhGia = new DanhGiaDichVu
                            {
                                SoDK = model.SoDK,
                                DanhGia = model.DanhGia,
                                BinhLuan = model.BinhLuan,
                                NgayDanhGia = DateTime.Now
                            };
                            db.DanhGiaDichVus.InsertOnSubmit(danhGia);

                            // Cập nhật trạng thái của dịch vụ đăng ký
                            var dangKyDichVu = db.DangKyDichVus.FirstOrDefault(d => d.SoDK == model.SoDK);
                            if (dangKyDichVu != null)
                            {
                                dangKyDichVu.TinhTrang = 3; // Đã Đánh Giá
                            }
                            db.SubmitChanges();
                            return Json(new { Success = true, Message = "Đánh giá thành công!" });
                        }
                        catch (Exception ex)
                        {
                            return Json(new { Success = false, Message = "Có lỗi xảy ra: " + ex.Message });
                        }
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "Bạn đã đánh giá dịch vụ này rồi!" });
                    }
                }
            }
            return Json(new { Success = false, Message = "Dữ liệu không hợp lệ!" });
        }

    }
}