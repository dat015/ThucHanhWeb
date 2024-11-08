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

        [Required(ErrorMessage = "H? v� t�n l� b?t bu?c")]
        [StringLength(100, ErrorMessage = "H? v� t�n kh�ng ???c v??t qu� 100 k� t?")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "T�i kho?n l� b?t bu?c")]
        [StringLength(50, ErrorMessage = "T�i kho?n kh�ng ???c v??t qu� 50 k� t?")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "T�i kho?n ch? ch?a ch? c�i v� s?")]
        public string TaiKhoan { get; set; }

        [Required(ErrorMessage = "M?t kh?u l� b?t bu?c")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "M?t kh?u ph?i c� �t nh?t 6 k� t?")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Email l� b?t bu?c")]
        [EmailAddress(ErrorMessage = "??a ch? email kh�ng h?p l?")]
        [StringLength(100, ErrorMessage = "Email kh�ng ???c v??t qu� 100 k� t?")]
        public string Email { get; set; }

        [Required(ErrorMessage = "??a ch? l� b?t bu?c")]
        [StringLength(200, ErrorMessage = "??a ch? kh�ng ???c v??t qu� 200 k� t?")]
        public string DiaChiKH { get; set; }

        [Required(ErrorMessage = "S? ?i?n tho?i l� b?t bu?c")]
        [Phone(ErrorMessage = "S? ?i?n tho?i kh�ng h?p l?")]
        [StringLength(15, ErrorMessage = "S? ?i?n tho?i kh�ng ???c v??t qu� 15 k� t?")]
        public string DienThoaiKH { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Ng�y sinh l� b?t bu?c")]
        public Nullable<System.DateTime> NgaySinh { get; set; }

        public virtual ICollection<DONDATHANG> DONDATHANGs { get; set; }
    }
}
