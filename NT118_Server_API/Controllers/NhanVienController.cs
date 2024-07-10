using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NT118_Server_API.Models;
using OfficeOpenXml;

namespace NT118_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhanVienController : ControllerBase
    {
        private readonly ILogger<NhanVienController> _logger;
        public NhanVienController(ILogger<NhanVienController> logger)
        {
            _logger = logger;
        }
        [HttpPost]
        public async Task<IActionResult> Insert(NhanVien nhanVien)
        {
            DataBase dataBase = new DataBase();          
            return Ok(dataBase.InsertNhanVien(nhanVien));
        }
        [HttpPost]
        [Route("GetOne")]
        public async Task<IActionResult> GetOne(NhanVien input)
        {
            DataBase dataBase = new DataBase();
            NhanVien? nhanVien = dataBase.GetNhanVien(input.MANV,input.MK);
            if (nhanVien == null) return NoContent();
            return Ok(nhanVien);
        }
        [HttpPost]
        [Route("UploadExcelFile")]
        public async Task<IActionResult> UploadExcelFile(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var package = new ExcelPackage(file.OpenReadStream()))
                {
                    var workbook = package.Workbook;
                    var worksheet = workbook.Worksheets[0]; // Lấy trang tính toán đầu tiên
                    DataBase dataBase = new DataBase();

                    // Đọc dữ liệu từ tệp Excel
                    for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
                    {
                        NhanVien nhanVien = new NhanVien();

                        string? manv = worksheet.Cells[row, 1].Text;
                        if (!string.IsNullOrEmpty(manv))
                        {
                            nhanVien.MANV = manv;
                        }

                        string hoten = worksheet.Cells[row, 2].Text;
                        if (string.IsNullOrEmpty(hoten)) continue;
                        string gioitinh = worksheet.Cells[row, 3].Text;
                        if (string.IsNullOrEmpty(gioitinh)) continue;
                        string ngaysinh = worksheet.Cells[row, 4].Text;
                        if (string.IsNullOrEmpty(ngaysinh)) continue;
                        string ngayvaolam = worksheet.Cells[row, 5].Text;
                        if (string.IsNullOrEmpty(ngayvaolam)) continue;

                        nhanVien.HOTEN = hoten;
                        nhanVien.GIOITINH = gioitinh;

                        string format = "dd/MM/yyyy"; // Định dạng của chuỗi ngày tháng

                        DateTime result_ngaysinh;
                        if (DateTime.TryParseExact(ngaysinh, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result_ngaysinh))
                        {
                            // Chuyển đổi thành công
                            //Console.WriteLine("Ngày tháng: " + result_ngaysinh);
                            nhanVien.NGSINH = result_ngaysinh;
                        }
                        else
                        {
                            // Không chuyển đổi thành công
                            //Console.WriteLine("Không thể chuyển đổi chuỗi thành DateTime.");
                            continue;
                        }
                        DateTime result_ngayvaolam;
                        if (DateTime.TryParseExact(ngayvaolam, format, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result_ngayvaolam))
                        {
                            // Chuyển đổi thành công
                            //Console.WriteLine("Ngày tháng: " + result_ngaysinh);
                            nhanVien.NGVL = result_ngayvaolam;
                        }
                        else
                        {
                            // Không chuyển đổi thành công
                            //Console.WriteLine("Không thể chuyển đổi chuỗi thành DateTime.");
                            continue;
                        }

                        string? diachi = worksheet.Cells[row, 6].Text;
                        if (string.IsNullOrEmpty(diachi)) diachi = null;
                        string? sdt = worksheet.Cells[row, 7].Text;
                        if (string.IsNullOrEmpty(sdt)) sdt = null;
                        string? email = worksheet.Cells[row, 8].Text;
                        if (string.IsNullOrEmpty(email)) email = null;
                        nhanVien.DC = diachi;
                        nhanVien.SDT = sdt;
                        nhanVien.EMAIL = email;

                        string cccd = worksheet.Cells[row, 9].Text;
                        if (string.IsNullOrEmpty(cccd)) continue;
                        nhanVien.CCCD = cccd;

                        string? lcb = worksheet.Cells[row, 10].Text;
                        if (string.IsNullOrEmpty(lcb)) lcb = null;
                        else
                        {
                            lcb = lcb.Replace(".","").Replace("đ","").Trim();
                            nhanVien.LCB = int.Parse(lcb);
                        }

                        string? phb = worksheet.Cells[row, 1].Text;
                        if (!string.IsNullOrEmpty(phb))
                        {
                            nhanVien.PHBAN = phb;
                        }

                        //Console.WriteLine(nhanVien.LCB);
                        if (nhanVien.MANV == null) dataBase.InsertNhanVien(nhanVien);
                        else dataBase.UpdateNhanVien(nhanVien);
                                          
                        //for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                        //{
                        //    string cellValue = worksheet.Cells[row, col].Text;
                        //}
                    }
                    return Ok(worksheet.Dimension.End.Row);
                }
            }
            return Ok(file.Length);
        }
        private bool Authentication(NhanVien nhanVien)
        {
            DataBase db = new DataBase();
            if (db.Login(nhanVien.MANV, nhanVien.MK) != true) return false;
            return true;
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
        [Route("LayDanhSachCongViecTheoTuan")]
        public async Task<IActionResult> LayDanhSachCongViecTheoTuan(NhanVien trph)
        {
            DataBase db = new DataBase();
            if (Authentication(trph))
            {
                return Ok(db.LayDanhSachCongViecTheoTuan(trph.MANV));
            }
            return BadRequest("Authentication failed");
        }
    }
}
