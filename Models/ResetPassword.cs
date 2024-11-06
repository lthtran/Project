using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteThuCungBento.Models
{
    public class ResetPassword
    {
        [Display(Name = "Mật khẩu Mới:")]
        [Required(ErrorMessage = "Mật khẩu không được để trống. ")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Độ dài mật khẩu ít nhất 6 - 20 ký tự. ")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Xác nhận mật khẩu:")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Xác nhận mật khẩu không đúng.")]
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống. ")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Độ dài mật khẩu ít nhất 6 - 20 ký tự. ")]
        public string ConfirmPassword { get; set; }

        [Required]
        public String Resetcode { get; set; }
    }
}