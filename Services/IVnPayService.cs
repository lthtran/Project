using System.Collections.Generic;
using System.Web;
using WebsiteThuCungBento.ViewModels;

namespace WebsiteThuCungBento.Services
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(HttpContextBase context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(Dictionary<string, string> parameters);
    }
}
