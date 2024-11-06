using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using WebsiteThuCungBento.Helpers;
namespace WebsiteThuCungBento.Helpers
{
    public class Common
    {
        public static string HtmlRate(int rate)
        {
            var str = "";
            if (rate == 0 || rate == 1)
            {
                str = @"
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>";
            }
            else if (rate == 2)
            {
                str = @"
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>";
            }
            else if (rate == 3)
            {
                str = @"
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>";
            }
            else if (rate == 4)
            {
                str = @"
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: lightgray;'></i>";
            }
            else if (rate == 5)
            {
                str = @"
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>
        <i class='fa fa-star' style='color: #FFD700;'></i>";
            }
            return str;
        }


    }
}
