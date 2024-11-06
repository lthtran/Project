using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteThuCungBento.Models
{
    public class DoiMKadmin
    {
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Độ dài mật khẩu ít nhất 6 - 20 ký tự. ")]
        [Required(ErrorMessage = "Mật khẩu không được để trống. ")]
        public string CheckPass { get; set; }


        [Required(ErrorMessage = "Mật khẩu không được để trống. ")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Độ dài mật khẩu ít nhất 6 - 20 ký tự. ")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không đúng.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Độ dài mật khẩu ít nhất 6 - 20 ký tự. ")]
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống. ")]
        public string ConfirmPassword { get; set; }

    }
}