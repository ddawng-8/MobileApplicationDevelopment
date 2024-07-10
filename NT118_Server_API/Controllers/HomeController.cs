using Microsoft.AspNetCore.Mvc;
using NT118_Server_API.Models;

namespace NT118_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        private bool Authentication(NhanVien nhanVien)
        {
            DataBase db = new DataBase();
            if (db.Login(nhanVien.MANV, nhanVien.MK) != true) return false;
            return true;
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(NhanVien nhanVien)
        {
            DataBase dataBase = new DataBase();
            if (string.IsNullOrEmpty(nhanVien.MANV))
                return BadRequest("Username is required.");
            return Ok(dataBase.Login(nhanVien.MANV,nhanVien.MK));
        }
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordData data)
        {
            if (string.IsNullOrEmpty(data.Username))
                return BadRequest("Username is required.");
            DataBase dataBase = new DataBase();
            bool? checking = dataBase.Login(data.Username, data.OldPassword);
            if (checking == null || checking == true)
            {
                return Ok(dataBase.ChangePassword(data.Username, data.NewPassword));
            }
            return Ok("Wrong password or username.");
        }
        [HttpPost]
        [Route("CheckTRPH")]
        public async Task<IActionResult> CheckTRPH([FromBody] string manv)
        {
            DataBase data = new DataBase();
            string? checker = data.CheckIfTRPHExists(manv);
            if (checker != null)
            {
                return Ok(checker);
            } else return NoContent();
        }
        [HttpGet]
        [Route("CheckServer")]
        public async Task<IActionResult> ById(int id)
        {
            DataBase data = new DataBase();
            return Ok(data.TestConnection(id));
        }
        [HttpPost]
        [Route("InsertNgNhanTB")]
        public async Task<IActionResult> InsertNgNhanTB(KeyValuePair<NhanVien, NotificationManagerData> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key))
            {
                int id = (data.Value.ID == null ? 0 : (int)data.Value.ID);
                db.InsertNgNhanTB(id, data.Key.MANV);
                return Ok();
            }
            return BadRequest("Authentication failed");
        }
        [HttpPost]
        [Route("InsertNgXemTB")]
        public async Task<IActionResult> InsertNgXemTB(KeyValuePair<NhanVien, NotificationManagerData> data)
        {
            DataBase db = new DataBase();
            if (Authentication(data.Key))
            {
                int id = (data.Value.ID == null ? 0 : (int)data.Value.ID);

                db.InsertNgXemTB(id, data.Key.MANV);
                return Ok();
            }
            return BadRequest("Authentication failed");
        }
    }
}
