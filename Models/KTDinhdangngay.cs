using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebsiteThuCungBento.Models
{
    public class KTDinhdangngay : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime dateTime;
            var isValid = DateTime.TryParseExact(Convert.ToString(value), "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTime);
            return (isValid);
        }
    }
}