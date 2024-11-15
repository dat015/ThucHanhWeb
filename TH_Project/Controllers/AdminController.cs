using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TH_Project.Data;

namespace TH_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly QLBANSACHEntities _db;
        public AdminController(QLBANSACHEntities db)
        {
            _db = db;
        }
        public AdminController() : this(new QLBANSACHEntities())
        {
        }
        // GET: Admin
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Sach(int? page)
        {
            int pageSize = 9; // Số lượng sách trên mỗi trang
            int pageNumber = (page ?? 1); // Trang hiện tại, mặc định là trang 1

            var books = await _db.SACHes.ToListAsync(); // Lấy tất cả sách từ cơ sở dữ liệu
            var pagedBooks = books.ToPagedList(pageNumber, pageSize); // Phân trang danh sách sách

            return View(pagedBooks); // Trả về view với danh sách đã phân trang
        }






        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            // Lấy dữ liệu từ FormCollection
            var tendn = collection["username"];
            var matkhau = collection["password"];

            // Kiểm tra dữ liệu nhập vào
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Phải nhập tên đăng nhập";
            }
            if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu";
            }

            // Kiểm tra nếu không có lỗi
            if (string.IsNullOrEmpty(ViewData["Loi1"]?.ToString()) && string.IsNullOrEmpty(ViewData["Loi2"]?.ToString()))
            {
                // Kiểm tra tên đăng nhập và mật khẩu
                Admin ad = _db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    // Lưu thông tin vào session và chuyển hướng
                    Session["Taikhoanadmin"] = ad;
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ViewData["Thongbao"] = "Tên đăng nhập hoặc mật khẩu không đúng";
                }
            }

            // Trả về view nếu có lỗi
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> Create()
        {

            ViewBag.MaCD = new SelectList(_db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(_db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]

        public async Task<ActionResult> Create(SACH sach, HttpPostedFileBase fileUpload)
        {
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                var fileName = Path.GetFileName(fileUpload.FileName); // Lưu đường dẫn của file
                var path = Path.Combine(Server.MapPath("~/Content/images"), fileName); // Kiểm tra hình ảnh tồn tại chưa?

                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                }
                else
                {
                    // Lưu hình ảnh vào đường dẫn
                    fileUpload.SaveAs(path);
                    sach.Anhbia = "/Content/images/" + fileName; // Lưu đường dẫn hình ảnh vào đối tượng sach
                }
            }
            else
            {
                ViewBag.Thongbao = "Vui lòng chọn hình ảnh để tải lên."; // Thông báo nếu không có tệp nào được tải lên
            }

            // Gửi dữ liệu cho View
            ViewBag.MaCD = new SelectList(_db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChude");
            ViewBag.MaNXB = new SelectList(_db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            // Lưu thông tin sách vào cơ sở dữ liệu
            _db.SACHes.Add(sach);
            await _db.SaveChangesAsync();

            // Trả về View
            return RedirectToAction("Sach");
        }

        public async Task UploadImageAsync(SACH product, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                // Tạo tên file duy nhất để tránh trùng lặp
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                var filePath = Path.Combine(Server.MapPath("~/Content/images/"), uniqueFileName);

                // Kiểm tra xem thư mục đã tồn tại chưa, nếu chưa thì tạo mới
                var directoryPath = Server.MapPath("~/Content/images/");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath); // Tạo thư mục nếu chưa có
                }

                // Lưu tệp vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.InputStream.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn hình ảnh vào đối tượng product
                product.Anhbia = "/Content/images/" + uniqueFileName;
            }
            else
            {
                throw new InvalidOperationException("Không có tệp nào được tải lên.");
            }
        }



        public async Task<ActionResult> Details(int id)
        {
            SACH sach = await _db.SACHes.FindAsync(id);
            ViewBag.Masach = sach.Masach;
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sach);

        }

        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        { // lấy ra đối tượng sách cần xóa theo mã
            SACH sach = await _db.SACHes.SingleOrDefaultAsync(n => n.Masach == id);
            ViewBag.Masach = sach.Masach;
            if (sach == null) { Response.StatusCode = 404; return null; }
            return View(sach);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            var sach = await _db.SACHes.FindAsync(id);

            if (sach == null)
            {
                return HttpNotFound();
            }

            _db.SACHes.Remove(sach);
            await _db.SaveChangesAsync();

            return RedirectToAction("Sach");
        }

        // Chỉnh sửa sản phẩm
        [HttpGet]
        public ActionResult Suasach(int id)
        {
            // Lấy đối tượng sách
            SACH sach = _db.SACHes.SingleOrDefault(n => n.Masach == id);

            if (sach == null)
            {
                Response.StatusCode = 404;
                return HttpNotFound("Không tìm thấy sách."); // Trả về thông báo lỗi
            }

            // Tạo SelectList cho dropdown
            ViewBag.MaCD = new SelectList(_db.CHUDEs.OrderBy(n => n.TenChuDe), "MaCD", "TenChude", sach.MaCD);
            ViewBag.MaNXB = new SelectList(_db.NHAXUATBANs.OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);

            return View(sach); // Truyền model về view
        }


        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Suasach(SACH sach, HttpPostedFileBase fileUpload)
        {
            // Kiểm tra xem model có hợp lệ không
            if (sach == null)
            {
                ViewBag.Thongbao = "Thông tin sách không hợp lệ.";
                return View(sach);
            }

            if (sach.Masach == 0)
            {
                ViewBag.Thongbao = "ID sách không hợp lệ.";
                return View(sach);
            }

            // Xử lý upload file
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                var fileName = Path.GetFileName(fileUpload.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images"), fileName);

                // Kiểm tra nếu file đã tồn tại
                if (System.IO.File.Exists(path))
                {
                    ViewBag.Thongbao = "Hình ảnh đã tồn tại.";
                }
                else
                {
                    // Lưu file vào server
                    fileUpload.SaveAs(path);
                    sach.Anhbia = "/Content/images/" + fileName;
                }
            }
            else if (string.IsNullOrEmpty(sach.Anhbia))
            {
                ViewBag.Thongbao = "Vui lòng chọn hình ảnh.";
                return View(sach);
            }

            try
            {
                // Tìm sách trong database
                var model = await _db.SACHes.FirstOrDefaultAsync(n => n.Masach == sach.Masach);

                if (model != null)
                {
                    // Cập nhật thông tin sách
                    model.Tensach = sach.Tensach;
                    model.Mota = sach.Mota;
                    model.MaCD = sach.MaCD;
                    model.MaNXB = sach.MaNXB;
                    model.Giaban = sach.Giaban;
                    model.Soluongton = sach.Soluongton;
                    model.Anhbia = sach.Anhbia;
                    model.Ngaycapnhat = DateTime.Now;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _db.SaveChangesAsync();

                    ViewBag.Thongbao = "Cập nhật sách thành công!";
                    return RedirectToAction("Sach");
                }
                else
                {
                    ViewBag.Thongbao = "Không tìm thấy sách cần sửa.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Thongbao = "Đã xảy ra lỗi: " + ex.Message;
            }

            // Tạo lại SelectList cho dropdown
            ViewBag.MaCD = new SelectList(_db.CHUDEs.OrderBy(n => n.TenChuDe), "MaCD", "TenChude", sach.MaCD);
            ViewBag.MaNXB = new SelectList(_db.NHAXUATBANs.OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);

            // Trả về view với dữ liệu model
            return View(sach);
        }


        public ActionResult ThongKeSach()
        {
            var chartData = _db.SACHes
                               .GroupBy(s => s.MaCD)
                               .Select(g => new
                               {
                                   TenChuDe = g.FirstOrDefault().CHUDE.TenChuDe,
                                   SoLuongSach = g.Count() 
                               })
                               .ToList();

            return View(chartData);
        }





    }
}