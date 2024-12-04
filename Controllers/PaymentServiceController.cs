using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using WebsiteThuCungBento.Models;
using WebsiteThuCungBento.ViewModels;
using System.Web.Script.Serialization;
using System.Web;
using System.Linq;
using System.Configuration;
using System.Net.Http;
using WebsiteThuCungBento.Utils;

namespace WebsiteThuCungBento.Controllers
{
    public class PaymentServiceController : Controller
    {
        DataClassesDataContext data = new DataClassesDataContext();
        [HttpPost]
        public ActionResult CreatePaymentUrl(string payment_method, string customer_name, string customer_phone, string address, decimal tongtien, string khunggio)
        {
            // Kiểm tra nếu không có giỏ hàng hoặc thông tin dịch vụ
            if (tongtien <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // Cấu hình thông tin người dùng
            KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
            if (kh == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Tạo thông tin thanh toán
            var paymentModel = new VnPaymentServiceRequestModel
            {
                Amount = tongtien,  // Số tiền thanh toán từ form
                CreateDate = DateTime.Now,
                Description = $"Thanh toán cho dịch vụ {customer_name} của khách hàng {kh.HOTENKH}",
                FullName = kh.HOTENKH,
                OrderId = Guid.NewGuid().ToString(), // Mã đơn hàng duy nhất
                ReturnUrl = Url.Action("PaymentCallback", "Payment"),
                khunggio = khunggio // Lấy khung giờ từ form
            };

            // Tạo URL thanh toán VNPAY
            string paymentUrl = GenerateVnPayPaymentUrl(paymentModel);

            // Chuyển hướng đến URL thanh toán
            return Redirect(paymentUrl);
        }

        private string GenerateVnPayPaymentUrl(VnPaymentServiceRequestModel model)
        {
            // Thực hiện việc tạo URL thanh toán VNPay
            var tick = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnpay = new VnPayLibrary();

            // Cấu hình lấy thông tin từ web.config
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_ReturnUrl = ConfigurationManager.AppSettings["vnp_ReturnUrl"];

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode); // Mã cửa hàng VNPay
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());  // Chuyển số tiền sang VND (multiply by 100)
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_CreateDate", model.CreateDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_TxnRef", model.OrderId);  // Mã tham chiếu giao dịch (Order ID)
            vnpay.AddRequestData("vnp_OrderInfo", model.Description);  // Mô tả dịch vụ (tên dịch vụ)
            vnpay.AddRequestData("vnp_OrderType", "service");  // Thanh toán dịch vụ
            vnpay.AddRequestData("vnp_Locale", "vn");  // Ngôn ngữ (Vietnamese)
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);  // URL trả về sau khi thanh toán
            vnpay.AddRequestData("vnp_IpAddr", Utils.Utils.GetIpAddress(HttpContext)); // Địa chỉ IP của người dùng

            // Tạo URL thanh toán và ký dữ liệu
            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }
        public ActionResult PaymentCallback(FormCollection collection)
        {
            string vnp_TxnRef = Request.QueryString["vnp_TxnRef"]; // Mã đơn hàng
            string vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"]; // Mã phản hồi VNPAY
            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; // Mã xác thực từ VNPAY

            // Kiểm tra mã xác thực (secure hash)
            var vnpayData = new Dictionary<string, string>();
            foreach (string key in Request.QueryString.Keys)
            {
                if (key != "vnp_SecureHash")
                {
                    vnpayData.Add(key, Request.QueryString[key]);
                }
            }

            VnPayLibrary vnpay = new VnPayLibrary();
            string queryString = BuildQueryString(vnpayData);
            string hashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            string secureHash = Utils.Utils.HmacSHA512(hashSecret, queryString);

            if (vnp_ResponseCode == "00") // 00 means success
            {
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
                // Lưu vào cơ sở dữ liệu
                data.DangKyDichVus.InsertOnSubmit(dkdv);
                data.SubmitChanges(); // Ghi vào cơ sở dữ liệu

                // Sau khi lưu thành công, có thể hiển thị thông báo
                ViewBag.PaymentStatus = "Success";
                ViewBag.OrderId = vnp_TxnRef;
            }
            else
            {
                // Thanh toán thất bại
                ViewBag.PaymentStatus = "Fail";
            }

            return View();
        }

        private string BuildQueryString(Dictionary<string, string> data)
        {
            var query = from d in data
                        select HttpUtility.UrlEncode(d.Key) + "=" + HttpUtility.UrlEncode(d.Value);
            return string.Join("&", query);
        }


        private string HmacSHA512(string key, string data)
{
    using (var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key)))
    {
        byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
    }
}

    }
}