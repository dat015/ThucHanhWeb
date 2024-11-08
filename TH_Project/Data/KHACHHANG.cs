using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;  // Include this for data annotations

namespace TH_Project.Data
{
    public partial class KHACHHANG
    {
        public KHACHHANG()
        {
            this.DONDATHANGs = new HashSet<DONDATHANG>();
        }

        [Key]  // Specifies that MaKH is the primary key
        public int MaKH { get; set; }

        [Required(ErrorMessage = "H? và tên là b?t bu?c")]
        [StringLength(100, ErrorMessage = "H? và tên không ???c v??t quá 100 ký t?")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Tài kho?n là b?t bu?c")]
        [StringLength(50, ErrorMessage = "Tài kho?n không ???c v??t quá 50 ký t?")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Tài kho?n ch? ch?a ch? cái và s?")]
        public string TaiKhoan { get; set; }

        [Required(ErrorMessage = "M?t kh?u là b?t bu?c")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "M?t kh?u ph?i có ít nh?t 6 ký t?")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Email là b?t bu?c")]
        [EmailAddress(ErrorMessage = "??a ch? email không h?p l?")]
        [StringLength(100, ErrorMessage = "Email không ???c v??t quá 100 ký t?")]
        public string Email { get; set; }

        [Required(ErrorMessage = "??a ch? là b?t bu?c")]
        [StringLength(200, ErrorMessage = "??a ch? không ???c v??t quá 200 ký t?")]
        public string DiaChiKH { get; set; }

        [Required(ErrorMessage = "S? ?i?n tho?i là b?t bu?c")]
        [Phone(ErrorMessage = "S? ?i?n tho?i không h?p l?")]
        [StringLength(15, ErrorMessage = "S? ?i?n tho?i không ???c v??t quá 15 ký t?")]
        public string DienThoaiKH { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Ngày sinh là b?t bu?c")]
        public Nullable<System.DateTime> NgaySinh { get; set; }

        public virtual ICollection<DONDATHANG> DONDATHANGs { get; set; }
    }
}
