using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteThuCungBento.Models;

namespace WebsiteThuCungBento.ViewModels
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderId { get; set; }
        public string OrderDescription { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnpayResponseCode { get; set; }
    }
    public class VnPaymentRequestModel 
    {
        public string OrderId { get; set; }

        public string FullName { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }

        public int MADH {  get; set; }
        public DateTime CreateDate { get; set; }

        public string ReturnUrl { get; set; }

    }
    public class VnPaymentServiceResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderId { get; set; }
        public string OrderDescription { get; set; }
        public string PaymentId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnpayResponseCode { get; set; }
    }
    public class VnPaymentServiceRequestModel
    {
        public string OrderId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int MADV { get; set; }
        public string khunggio {  get; set; }
        public DateTime CreateDate { get; set; }
        public string ReturnUrl { get; set; }
    }

}
