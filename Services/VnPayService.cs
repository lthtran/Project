using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using WebsiteThuCungBento.Utils;
using WebsiteThuCungBento.ViewModels;

namespace WebsiteThuCungBento.Services
{
    public class VnPayService : IVnPayService
    {
        public string CreatePaymentUrl(HttpContextBase httpContext, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.ToString();
            var vnpay = new VnPayLibrary();
            // Read VNPAY configuration
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_ReturnUrl"];

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());  // Multiply by 100 to convert to VND
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_CreateDate", model.CreateDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_TxnRef", tick);  // Transaction reference ID
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng " + model.MADH);  // Order description
            vnpay.AddRequestData("vnp_OrderType", "other");  // Payment type
            vnpay.AddRequestData("vnp_Locale", "vn");  // Language (Vietnamese)
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);  // Return URL
            vnpay.AddRequestData("vnp_IpAddr", Utils.Utils.GetIpAddress(httpContext));

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;

        }
        public VnPaymentResponseModel PaymentExecute(Dictionary<string, string> parameters)
        {
            var vnpay = new  VnPayLibrary();
            var responseModel = new VnPaymentResponseModel();
            foreach (KeyValuePair<string, string> entry in parameters)
            {
                var key = entry.Key;
                var value = entry.Value;

                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
            }

            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"];
            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = parameters.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                };
            }
            return new VnPaymentResponseModel
            { 
                Success = true, 
                PaymentMethod="VnPay",
                OrderId = vnp_orderId.ToString(),
                OrderDescription = vnp_OrderInfo.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash.ToString(),
                VnpayResponseCode = vnp_ResponseCode.ToString(),
            };

        }
    }
}
