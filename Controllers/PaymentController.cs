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

public class PaymentController : Controller
{
    DataClassesDataContext data = new DataClassesDataContext();

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
    private int TongSoLuong(List<GiohangModels> dsGiohang)
    {
        int iTongSoLuong = 0;

        if (dsGiohang != null)
        {
            iTongSoLuong = dsGiohang.Sum(n => n.iSOLUONG);
        }

        return iTongSoLuong;
    }
    private double TongTien(List<GiohangModels> dsGiohang)
    {
        double iTongTien = 0;

        if (dsGiohang != null)
        {
            iTongTien = dsGiohang.Sum(n => n.dTHANHTIEN);  // Assuming dTHANHTIEN is the total price for each product
        }

        return iTongTien;
    }
    public ActionResult CreatePaymentUrl(VnPaymentRequestModel model)
    {
        // Get the logged-in user and their cart
        KHACHHANG kh = (KHACHHANG)Session["Taikhoan"];
        List<GiohangModels> gh = Laygiohang();

        if (gh == null || gh.Count == 0)
        {
            return RedirectToAction("Index", "User");
        }
        var tempOrder = new DONDATHANG
        {
            MAKH = kh.MAKH,
            TONGTIEN = (decimal)TongTien(gh),
            // Do not submit changes here, just save required info for payment
        };
        TempData["PendingOrder"] = tempOrder;
        var paymentModel = new VnPaymentRequestModel
        {
            Amount = TongTien(gh),
            CreateDate = DateTime.Now,
            Description = $"{kh.HOTENKH} {kh.DIENTHOAI} {kh.DIACHI}",
            FullName = kh.HOTENKH,
            OrderId = Guid.NewGuid().ToString(), // Use unique ID for temporary reference
            ReturnUrl = Url.Action("PaymentCallback", "Payment")
        };
        string paymentUrl = GenerateVnPayPaymentUrl(paymentModel); // Function to generate the VNPAY payment URL

        // Redirect the user to the VNPAY payment gateway
        return Redirect(paymentUrl);
    }
    private string GenerateVnPayPaymentUrl(VnPaymentRequestModel model)
    {
        // Prepare the data for VNPAY
        var tick = DateTime.Now.ToString();
        var vnpay = new VnPayLibrary();
        // Read VNPAY configuration
        string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
        string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
        string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
        string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_ReturnUrl"];

        vnpay.AddRequestData("vnp_Version", "2.1.0");
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", "FBI7TWMJ");
        //vnpay.AddRequestData("vnp_HashSecret", "E775C2MCL4IAOA37VI2OCAA5U6N38GIJ");
        vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());  // Multiply by 100 to convert to VND
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_CreateDate", model.CreateDate.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_TxnRef", tick);  // Transaction reference ID
        vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng " + model.MADH);  // Order description
        vnpay.AddRequestData("vnp_OrderType", "other");  // Payment type
        vnpay.AddRequestData("vnp_Locale", "vn");  // Language (Vietnamese)
        vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);  // Return URL
        vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(HttpContext));
        // Build query string without the secure hash
        var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, "VF3WVOX8XUYDSSW9VRSOHXB1EDTBBCWC");
        return paymentUrl;
    }

    private string BuildQueryString(Dictionary<string, string> data)
    {
        var query = from d in data
                    select HttpUtility.UrlEncode(d.Key) + "=" + HttpUtility.UrlEncode(d.Value);
        return string.Join("&", query);
    }

    public ActionResult PaymentCallback(FormCollection collection)
    {
        string vnp_TxnRef = Request.QueryString["vnp_TxnRef"]; // Order ID
        string vnp_ResponseCode = Request.QueryString["vnp_ResponseCode"]; // VNPAY response code
        string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; // VNPAY secure hash

        // Prepare the data for hashing
        var vnpayData = new Dictionary<string, string>();
        foreach (string key in Request.QueryString.Keys)
        {
            if (key != "vnp_SecureHash")
            {
                vnpayData.Add(key, Request.QueryString[key]);
            }
        }
        VnPayLibrary vnpay = new VnPayLibrary();
        // Build query string and verify secure hash
        string queryString = BuildQueryString(vnpayData);
        string hashSecret = "VF3WVOX8XUYDSSW9VRSOHXB1EDTBBCWC"; // The same hash secret you used earlier
        string secureHash = Utils.HmacSHA512(hashSecret, queryString);
        
        if (vnp_ResponseCode == "00") // 00 means success
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
            ddh.DATHANHTOAN = true; // Payment status is false until confirmed
            ddh.TONGTIEN = (decimal)TongTien(gh);
            ddh.HINHTHUCTT = "Chuyển khoản VNPAY";
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

            ClearCart();
            return RedirectToAction("Xacnhandonhang", "Giohang");

        }
         return RedirectToAction("ThongBao", "Giohang");

    }


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
}

//https://localhost:44335/giohang/PaymentCallBack?
//      //vnp_Amount=900000000&vnp_BankCode=NCB&
//      //vnp_BankTranNo=VNP14603739&vnp_CardType=ATM&
//        vnp_OrderInfo=Thanh+to%C3%A1n+cho+%C4%91%C6%A1n+h%C3%A0ng+0&
//      //vnp_PayDate=20241005150558&
//      //vnp_ResponseCode=00&
//      //vnp_TmnCode=FBI7TWMJ&vnp_TransactionNo=14603739&vnp_TransactionStatus=00&vnp_TxnRef=05%2F10%2F2024+3%3A05%3A06+PM&
//      //vnp_SecureHash=448ebf818afd746de8a1f1ffc0163694d7b60b28119c9112918a51992224933a11b314c769bc244f4a1cb02cfc60a8a5320cc2b12622da17e088212d4fdd057a