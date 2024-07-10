using Microsoft.AspNetCore.Mvc;
using NT118_Server_API.Models;

namespace NT118_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        [HttpPost("insert")]
        public IActionResult InsertAdmin([FromBody] Admin admin)
        {
            DataBase data = new DataBase();
            string? status = data.InsertAdmin(admin);
            if (string.IsNullOrEmpty(status))
            {
                return Ok();
            }
            else return Ok(status);
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] Admin admin)
        {
            DataBase data = new DataBase();
            bool? status = data.AdminLogin(admin);
            if (status == null)
            {
                return StatusCode(204);
            } else
            {
                return Ok(status);
            }
        }
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordData data)
        {
            if (string.IsNullOrEmpty(data.Username))
                return BadRequest("Username is required.");
            if (data.OldPassword != null) data.OldPassword = data.OldPassword.Trim();
            data.NewPassword = data.NewPassword.Trim();
            DataBase dataBase = new DataBase();
            Admin admin = new Admin();
            admin.MK = data.OldPassword;
            admin.Id = int.Parse(data.Username);
            bool? checking = dataBase.AdminLogin(admin);
            if (checking == null || checking == true)
            {
                return Ok(dataBase.AdminChangePassword(data));
            }
            return Ok("Wrong password or username.");
        }
        [HttpPost]
        [Route("GetNhanViens")]
        public async Task<IActionResult> GetNhanViens(KeyValuePair<Admin,NhanVien> data)
        {
            DataBase dataBase = new DataBase();
            
            return Ok(dataBase.GetNhanViens(data.Key, data.Value));
        }
        [HttpPost]
        [Route("GetNhanViensEmptyPHBAN")]
        public async Task<IActionResult> GetNhanViensEmptyPHBAN(KeyValuePair<Admin, NhanVien> data)
        {
            DataBase dataBase = new DataBase();

            return Ok(dataBase.GetNhanViensEmptyPHBAN(data.Key, data.Value));
        } 
        [HttpPost]
        [Route("InsertNhanVienToPhongBan")]
        public async Task<IActionResult> InsertNhanVienToPhongBan(KeyValuePair<Admin, PhongBan> data)
        {
            DataBase dataBase = new DataBase();
            dataBase.InsertNhanVienToPhongBan(data.Key, data.Value);
            return Ok();
        }
        [HttpPost]
        [Route("DeleteNhanVienFromPhongBan")]
        public async Task<IActionResult> DeleteNhanVienFromPhongBan(KeyValuePair<Admin, PhongBan> data)
        {
            DataBase dataBase = new DataBase();
            dataBase.DeleteNhanVienFromPhongBan(data.Key, data.Value);
            return Ok();
        }
        [HttpPost]
        [Route("InsertPhongBan")]
        public async Task<IActionResult> InsertPhongBan(KeyValuePair<Admin, PhongBan> data)
        {
            DataBase dataBase = new DataBase();
            dataBase.InsertPhongBan(data.Key, data.Value);
            return Ok();
        }
        [HttpPost]
        [Route("DeletePhongBan")]
        public async Task<IActionResult> DeletePhongBan(KeyValuePair<Admin, PhongBan> data)
        {
            DataBase dataBase = new DataBase();
            dataBase.DeletePhongBan(data.Key, data.Value);
            return Ok();
        }
        [HttpPost]
        [Route("UpdatePhongBan")]
        public async Task<IActionResult> UpdatePhongBan(KeyValuePair<Admin, PhongBan> data)
        {
            DataBase dataBase = new DataBase();
            dataBase.UpdatePhongBan(data.Key, data.Value);
            return Ok();
        }
        [HttpPost]
        [Route("GetPhongBans")]
        public async Task<IActionResult> GetPhongBans(KeyValuePair<Admin, PhongBan?> data)
        {
            DataBase dataBase = new DataBase();         
            return Ok(dataBase.GetPhongBans(data.Key, data.Value));
        }
    }
}
