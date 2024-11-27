using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebsiteThuCungBento.Models
{
    public class DangKyModels
    {
        [Display(Name = "Tên đăng nhập:")]
        [Required(ErrorMessage = " Tên đang nhập không được để trống. ")]
        public string tendn { set; get; }
        [Display(Name = "Mật khẩu:")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Độ dài mật khẩu ít nhất 6 - 20 ký tự. ")]
        [Required(ErrorMessage = "Mật khẩu không được để trống. ")]
        public string matkhau { set; get; }

        [Display(Name = "Xác nhận mật khẩu:")]
        [Compare("matkhau", ErrorMessage = " Xác nhận mật khẩu không đúng. ")]
        [Required(ErrorMessage = " mật khẩu không được để trống. ")]
        public string xacnhanmatkhau { set; get; }

        [Display(Name = "Họ và tên: ")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Họ tên không hợp lệ")]
        [Required(ErrorMessage = " Họ tên không được để trống. ")]
        public string hoten { set; get; }

        [Display(Name = "Số điện thoại:")]
        [StringLength(11, MinimumLength = 9, ErrorMessage = "Số điện thoại không hợp lệ")]
        [Required(ErrorMessage = " Số điện thoại không được để trống. ")]
        public string dienthoai { set; get; }

        [Display(Name = "Email:")]
        [Required(ErrorMessage = " Email không được để trống. ")]
        public string email { set; get; }

        [Display(Name = "Hình ảnh:")]
        public string hinhanh { set; get; }

        [Display(Name = "Ngày sinh:")]
        //[KTDinhdangngay(ErrorMessage = " Ngày sinh không hợp lệ!. ")]
        public DateTime? ngaysinh { set; get; }


        [Display(Name = "Địa chỉ:")]
        public string diachi { set; get; }
    }
}