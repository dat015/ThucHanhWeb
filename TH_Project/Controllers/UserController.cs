using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TH_Project.Data;
using TH_Project.ViewModel;
using System.Threading.Tasks;
using System.Data.Entity;

namespace TH_Project.Controllers
{
    public class UserController : Controller
    {
        private readonly QLBANSACHEntities _db;
        public UserController(QLBANSACHEntities db)
        {
            _db = db;
        }
        public UserController() : this(new QLBANSACHEntities())
        {
        }
        // GET: User
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewData["Chude"] = await _db.CHUDEs.ToListAsync();
            ViewData["NXB"] = await _db.NHAXUATBANs.ToListAsync();
            return View(new KHACHHANG());
        }

        [HttpPost]
        public async Task<ActionResult> DangNhap(string TaiKhoan, string MatKhau)
        {
            var user = await _db.KHACHHANGs
                .FirstOrDefaultAsync(u => u.TaiKhoan == TaiKhoan && u.MatKhau == MatKhau);

            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng.");
                return View("Index");
            }


            Session["TaiKhoan"] = user;
            Session["UserName"] = user.HoTen;

            ViewData["Chude"] = await _db.CHUDEs.ToListAsync();
            ViewData["NXB"] = await _db.NHAXUATBANs.ToListAsync();

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<ActionResult> DangKy()
        {
            ViewData["Chude"] = await _db.CHUDEs.ToListAsync();
            ViewData["NXB"] = await _db.NHAXUATBANs.ToListAsync();
            return View(new KHACHHANG());
        }

        [HttpPost]
        public async Task<ActionResult> DangKy(KHACHHANG model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = _db.KHACHHANGs.FirstOrDefault(u => u.TaiKhoan == model.TaiKhoan);
            if (existingUser != null)
            {
                ModelState.AddModelError("TaiKhoan", "Tài khoản đã tồn tại.");
                return View(model);
            }

            var existingEmail = _db.KHACHHANGs.FirstOrDefault(u => u.Email == model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng.");
                return View(model);
            }

            var user = new KHACHHANG()
            {
                HoTen = model.HoTen,
                TaiKhoan = model.TaiKhoan,
                MatKhau = model.MatKhau,
                Email = model.Email,
                DiaChiKH = model.DiaChiKH,
                DienThoaiKH = model.DienThoaiKH,
                NgaySinh = model.NgaySinh
            };

            try
            {
                _db.KHACHHANGs.Add(user);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã có lỗi xảy ra, vui lòng thử lại.");
                return View(model);
            }
        }



    }
}