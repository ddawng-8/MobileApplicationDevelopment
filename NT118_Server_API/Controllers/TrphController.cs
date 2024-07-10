using Microsoft.AspNetCore.Mvc;
using NT118_Server_API.Models;

namespace NT118_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrphController : Controller
    {
        private readonly ILogger<TrphController> _logger;
        public TrphController(ILogger<TrphController> logger)
        {
            _logger = logger;
        }
        private bool Authentication(NhanVien trph, string? maph = null)
        {
            DataBase db = new DataBase();
            if (db.Login(trph.MANV, trph.MK) != true) return false;
            if (db.CheckIfTRPHExists(trph.MANV) == null) return false;
            if (maph != null)
            {
                NhanVien? nhanVien = db.GetPHBanFromNhanVien(trph.MANV);
                if (nhanVien != null && nhanVien.PHBAN == maph) return true;
                else return false;
            }
            return true;
        }
        [HttpPost]
        [Route("ThemCongViec")]
        public async Task<IActionResult> ThemCongViec(KeyValuePair<NhanVien,LichLamViec> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key, data.Value.PhBan))
            {
                db.ThemCongViec(data.Value);
                return Ok();
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("XoaCongViec")]
        public async Task<IActionResult> XoaCongViec(KeyValuePair<NhanVien,LichLamViec> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key, data.Value.PhBan))
            {
                db.XoaCongViec(data.Value);
                return Ok();
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("SuaCongViec")]
        public async Task<IActionResult> SuaCongViec(KeyValuePair<NhanVien,LichLamViec> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key, data.Value.PhBan))
            {
                db.SuaCongViec(data.Value);
                return Ok();
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("ThemNhanVienVaoCongViec")]
        public async Task<IActionResult> ThemNhanVienVaoCongViec(KeyValuePair<NhanVien, ThamGiaLamViec> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key))
            {
                db.ThemNhanVienVaoCongViec(data.Value.MALV, data.Value.MANV);
                return Ok();
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("XoaNhanVienKhoiCongViec")]
        public async Task<IActionResult> XoaNhanVienKhoiCongViec(KeyValuePair<NhanVien,ThamGiaLamViec> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key))
            {
                db.XoaNhanVienKhoiCongViec(data.Value.MALV, data.Value.MANV);
                return Ok();
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("LaySoLuongNhanVienTrongCongViec")]
        public async Task<IActionResult> LaySoLuongNhanVienTrongCongViec(KeyValuePair<NhanVien, LichLamViec> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key, data.Value.PhBan))
            {
                return Ok(db.LaySoLuongNhanVienTrongCongViec(data.Value));
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("LayDanhSachNhanVienTrongCongViecCuaPhongBan")]
        public async Task<IActionResult> LayDanhSachNhanVienTrongCongViecCuaPhongBan(KeyValuePair<NhanVien, LichLamViec> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key, data.Value.PhBan))
            {
                return Ok(db.LayDanhSachNhanVienTrongCongViecCuaPhongBan(data.Value));
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("LayDanhSachCongViecCuaPhongBan")]
        public async Task<IActionResult> LayDanhSachCongViecCuaPhongBan(KeyValuePair<NhanVien, LichLamViec> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key, data.Value.PhBan))
            {
                return Ok(db.LayDanhSachCongViecCuaPhongBan(data.Value));
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("LayDanhSachCongViecCuaPhongBan_Limit10")]
        public async Task<IActionResult> LayDanhSachCongViecCuaPhongBan_Limit10(KeyValuePair<NhanVien, LichLamViec> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key, data.Value.PhBan))
            {
                return Ok(db.LayDanhSachCongViecCuaPhongBan(data.Value.PhBan,(int) data.Value.MaLV));
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("GetNhanViens")]
        public async Task<IActionResult> GetNhanViens(KeyValuePair<NhanVien, NhanVien> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key, data.Key.PHBAN))
            {
                return Ok(db.GetNhanViens(new Admin(), data.Value, data.Key));
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("InsertNotification")]
        public async Task<IActionResult> InsertNotification(KeyValuePair<NhanVien, NotificationManagerData> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key, data.Value.Phban))
            {
                db.InsertNotification(data.Value, data.Key.MANV);
                return Ok();
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("GetNotificationsForEmployee")]
        public async Task<IActionResult> GetNotificationsForEmployee(NhanVien trph)
        {
            DataBase db = new DataBase();
            if (Authentication(trph))
            {
                return Ok(db.GetNotificationsForEmployee(trph.MANV));
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("DeleteNotification")]
        public async Task<IActionResult> DeleteNotification(KeyValuePair<NhanVien, NotificationManagerData> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key))
            {            
                int id = (data.Value.ID == null ? 0 : (int)data.Value.ID);
                db.DeleteNotification(id);
                return Ok();
            }
            return BadRequest("Authentication failed");
        }    
        [HttpPost]
        [Route("GetNhanVien")]
        public async Task<IActionResult> GetNhanVien(KeyValuePair<NhanVien, NhanVien> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key))
            {                        
                return Ok(db.GetNhanVienByManager(data.Value.MANV));
            }
            return BadRequest("Authentication failed");
        }    
    }
}
