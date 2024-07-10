using Dapper;
using Microsoft.AspNetCore.Http.Extensions;
using MySql.Data.MySqlClient;
using NT118_Server_API.Models;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using static NT118_Server_API.Models.NhanVien;

namespace NT118_Server_API
{
    public class DataBase
    {
#if DEBUG
        private static string Server0 = "Server=s3.cazo-dev.net; Port=8011; Database=QLNV; UID=server; Password=9vCUVGhmuT!U@Ux3AxJdoUqMT;";
#else
        private static string Server0 = "Server=127.0.0.1; Port=8011; Database=QLNV; UID=server; Password=9vCUVGhmuT!U@Ux3AxJdoUqMT;";
#endif
        public static string ComputeSHA256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
        public class TwoWayEncryptor
        {
            private static readonly string Key = "y12A43ekUgD9N5qX"; // Thay đổi khóa bí mật của bạn ở đây

            public static string Encrypt(string plainText)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                    aesAlg.IV = new byte[16]; // Khởi tạo vector khởi tạo (IV)

                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                swEncrypt.Write(plainText);
                            }
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }

            public static string Decrypt(string cipherText)
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(Key);
                    aesAlg.IV = new byte[16]; // Khởi tạo vector khởi tạo (IV)

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
        public string TestConnection(int server_id = 0)
        {
            if (server_id == 0)
            {
                MySqlConnection connection = new MySqlConnection(Server0);
                try
                {
                    connection.Open();
                    //Console.WriteLine("Kết nối thành công!");

                    // Thực hiện các thao tác với CSDL ở đây

                    connection.Close();
                    return "Database Online";
                }
                catch (MySqlException ex)
                {
                    //Console.WriteLine("Lỗi kết nối: " + ex.Message);
                    return "Database Offline";
                }
            }
            return "Id not found.";
        }
        public string? UpdateNhanVien(NhanVien nhanVien)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    string query = "UPDATE NHANVIEN " +
                                   "SET MK = @MK, HOTEN = @HOTEN, GIOITINH = @GIOITINH, NGSINH = @NGSINH, NGVL = @NGVL, " +
                                   "DC = @DC, SDT = @SDT, EMAIL = @EMAIL, CCCD = @CCCD, LCB = @LCB, PHBAN = @PHBAN " +
                                   "WHERE MANV = @MANV";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MK", nhanVien.MK);
                        command.Parameters.AddWithValue("@HOTEN", nhanVien.HOTEN);
                        command.Parameters.AddWithValue("@GIOITINH", nhanVien.GIOITINH);
                        command.Parameters.AddWithValue("@NGSINH", nhanVien.NGSINH);
                        command.Parameters.AddWithValue("@NGVL", nhanVien.NGVL);
                        command.Parameters.AddWithValue("@DC", nhanVien.DC);
                        command.Parameters.AddWithValue("@SDT", nhanVien.SDT);
                        command.Parameters.AddWithValue("@EMAIL", nhanVien.EMAIL);
                        command.Parameters.AddWithValue("@CCCD", nhanVien.CCCD);
                        command.Parameters.AddWithValue("@LCB", nhanVien.LCB);
                        command.Parameters.AddWithValue("@PHBAN", nhanVien.PHBAN);
                        command.Parameters.AddWithValue("@MANV", nhanVien.MANV);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            // Lệnh UPDATE không tìm thấy MANV trùng khớp
                            throw new Exception("No matching MANV found.");
                        }

                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #region Công việc
        public void ThemCongViec(LichLamViec lich)
        {
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                string query = "INSERT INTO LICHLAMVIEC (TIEUDE, MOTA, NGAYBATDAU, NGAYKETTHUC, PHBAN) VALUES (@TieuDe, @MoTa, @NgayBatDau, @NgayKetThuc, @PhBan)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TieuDe", lich.TieuDe);
                cmd.Parameters.AddWithValue("@MoTa", lich.MoTa);
                cmd.Parameters.AddWithValue("@NgayBatDau", lich.NgayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", lich.NgayKetThuc);
                cmd.Parameters.AddWithValue("@PhBan", lich.PhBan);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void XoaCongViec(LichLamViec lich)
        {
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                string query = "DELETE FROM LICHLAMVIEC WHERE MALV = @MaLV";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLV", lich.MaLV);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void SuaCongViec(LichLamViec lich)
        {
            if (lich.MaLV == null)
            {
                throw new ArgumentException("MaLV là bắt buộc và không thể null.");
            }

            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                List<string> updates = new List<string>();

                if (lich.TieuDe != null)
                    updates.Add("TIEUDE = @TieuDe");
                if (lich.MoTa != null)
                    updates.Add("MOTA = @MoTa");
                if (lich.NgayBatDau != DateTime.MinValue) // Giả sử DateTime.MinValue là giá trị không hợp lệ
                    updates.Add("NGAYBATDAU = @NgayBatDau");
                if (lich.NgayKetThuc != DateTime.MinValue)
                    updates.Add("NGAYKETTHUC = @NgayKetThuc");
                if (lich.PhBan != null)
                    updates.Add("PHBAN = @PhBan");

                if (updates.Count == 0)
                    return; // Không có gì để cập nhật

                string query = $"UPDATE LICHLAMVIEC SET {string.Join(", ", updates)} WHERE MALV = @MaLV";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@MaLV", lich.MaLV);
                if (lich.TieuDe != null)
                    cmd.Parameters.AddWithValue("@TieuDe", lich.TieuDe);
                if (lich.MoTa != null)
                    cmd.Parameters.AddWithValue("@MoTa", lich.MoTa);
                if (lich.NgayBatDau != DateTime.MinValue)
                    cmd.Parameters.AddWithValue("@NgayBatDau", lich.NgayBatDau);
                if (lich.NgayKetThuc != DateTime.MinValue)
                    cmd.Parameters.AddWithValue("@NgayKetThuc", lich.NgayKetThuc);
                if (lich.PhBan != null)
                    cmd.Parameters.AddWithValue("@PhBan", lich.PhBan);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void ThemNhanVienVaoCongViec(int malv, string manv)
        {
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                string query = "INSERT INTO THAMGIALAMVIEC (MALV, MANV) VALUES (@MaLV, @MaNV)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLV", malv);
                cmd.Parameters.AddWithValue("@MaNV", manv);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public void XoaNhanVienKhoiCongViec(int malv, string manv)
        {
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                string query = "DELETE FROM THAMGIALAMVIEC WHERE MALV = @MaLV AND MANV = @MaNV";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLV", malv);
                cmd.Parameters.AddWithValue("@MaNV", manv);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public int LaySoLuongNhanVienTrongCongViec(LichLamViec lich)
        {
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                string query = "SELECT COUNT(*) FROM THAMGIALAMVIEC WHERE MALV = @MaLV";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLV", lich.MaLV);

                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        public List<NhanVien> LayDanhSachNhanVienTrongCongViecCuaPhongBan(LichLamViec lich)
        {
            List<NhanVien> danhSachNhanVien = new List<NhanVien>();
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                string query = @"
            SELECT NV.* 
            FROM NHANVIEN NV 
            JOIN THAMGIALAMVIEC TGV ON NV.MANV = TGV.MANV 
            WHERE TGV.MALV = @MaLV AND NV.PHBAN = @PhBan";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MaLV", lich.MaLV);
                cmd.Parameters.AddWithValue("@PhBan", lich.PhBan);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NhanVien nhanVien = new NhanVien
                        {
                            MANV = reader["MANV"].ToString(),
                            HOTEN = reader["HOTEN"].ToString()
                        };
                        danhSachNhanVien.Add(nhanVien);
                    }
                }
            }
            return danhSachNhanVien;
        }
        public List<LichLamViec> LayDanhSachCongViecCuaPhongBan(LichLamViec lich)
        {
            if (!lich.NgayBatDau.HasValue) lich.NgayBatDau = DateTime.Now;
            if (!lich.NgayKetThuc.HasValue) lich.NgayKetThuc = DateTime.Now;

            List<LichLamViec> danhSachCongViec = new List<LichLamViec>();
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                string query = @"
            SELECT * 
            FROM LICHLAMVIEC 
            WHERE PHBAN = @PhBan AND NGAYBATDAU >= @TuNgay AND NGAYKETTHUC <= @DenNgay";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PhBan", lich.PhBan);
                cmd.Parameters.AddWithValue("@TuNgay", lich.NgayBatDau);
                cmd.Parameters.AddWithValue("@DenNgay", lich.NgayKetThuc);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LichLamViec lichLamViec = new LichLamViec
                        {
                            MaLV = reader.GetInt32(reader.GetOrdinal("MALV")),
                            TieuDe = reader.IsDBNull(reader.GetOrdinal("TIEUDE")) ? null : reader.GetString(reader.GetOrdinal("TIEUDE")),
                            MoTa = reader.IsDBNull(reader.GetOrdinal("MOTA")) ? null : reader.GetString(reader.GetOrdinal("MOTA")),
                            NgayBatDau = reader.IsDBNull(reader.GetOrdinal("NGAYBATDAU")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("NGAYBATDAU")),
                            NgayKetThuc = reader.IsDBNull(reader.GetOrdinal("NGAYKETTHUC")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("NGAYKETTHUC")),
                            PhBan = reader.IsDBNull(reader.GetOrdinal("PHBAN")) ? null : reader.GetString(reader.GetOrdinal("PHBAN"))
                        };
                        danhSachCongViec.Add(lichLamViec);
                    }
                }
            }
            return danhSachCongViec;
        }
        public List<LichLamViec> LayDanhSachCongViecCuaPhongBan(string phBan, int offset = 0)
        {
            List<LichLamViec> danhSachCongViec = new List<LichLamViec>();
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                string query = @"
            SELECT *
            FROM LICHLAMVIEC
            WHERE PHBAN = @PhBan
            LIMIT 10 OFFSET @Offset";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PhBan", phBan);
                cmd.Parameters.AddWithValue("@Offset", offset);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LichLamViec lichLamViec = new LichLamViec
                        {
                            // Điền thông tin của lịch làm việc từ reader
                            // Ví dụ:
                            MaLV = reader.GetInt32(reader.GetOrdinal("MALV")),
                            TieuDe = reader.IsDBNull(reader.GetOrdinal("TIEUDE")) ? null : reader.GetString(reader.GetOrdinal("TIEUDE")),
                            MoTa = reader.IsDBNull(reader.GetOrdinal("MOTA")) ? null : reader.GetString(reader.GetOrdinal("MOTA")),
                            NgayBatDau = reader.GetDateTime(reader.GetOrdinal("NGAYBATDAU")),
                            NgayKetThuc = reader.GetDateTime(reader.GetOrdinal("NGAYKETTHUC")),
                            PhBan = reader.GetString(reader.GetOrdinal("PHBAN")),
                            SoLuongNhanVien = reader.GetInt32(reader.GetOrdinal("SoLuongNhanVien"))
                        };
                        danhSachCongViec.Add(lichLamViec);
                    }
                }
            }
            return danhSachCongViec;
        }
        public (DateTime, DateTime) LayNgayDauVaCuoiTuan()
        {
            DateTime today = DateTime.Today;
            int daysUntilMonday = (int)DayOfWeek.Monday - (int)today.DayOfWeek;
            int daysUntilSunday = (int)DayOfWeek.Sunday - (int)today.DayOfWeek;

            DateTime monday = today.AddDays(daysUntilMonday);
            DateTime sunday = today.AddDays(daysUntilSunday + 7); // Thêm 7 để đảm bảo Chủ nhật của cùng tuần

            return (monday, sunday);
        }
        public List<LichLamViec> LayDanhSachCongViecTheoTuan(string manv)
        {
            var (monday, sunday) = LayNgayDauVaCuoiTuan();
            List<LichLamViec> danhSachCongViec = new List<LichLamViec>();

            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                string query = @"
            SELECT LLV.*
            FROM LICHLAMVIEC LLV
            JOIN THAMGIALAMVIEC TGV ON LLV.MALV = TGV.MALV
            WHERE TGV.MANV = @MANV AND
                  LLV.NGAYBATDAU >= @NgayBatDau AND
                  LLV.NGAYKETTHUC <= @NgayKetThuc";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@MANV", manv);
                cmd.Parameters.AddWithValue("@NgayBatDau", monday);
                cmd.Parameters.AddWithValue("@NgayKetThuc", sunday);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LichLamViec lichLamViec = new LichLamViec
                        {
                            // Điền thông tin của lịch làm việc từ reader
                            // Ví dụ:
                            MaLV = reader.GetInt32(reader.GetOrdinal("MALV")),
                            TieuDe = reader.IsDBNull(reader.GetOrdinal("TIEUDE")) ? null : reader.GetString(reader.GetOrdinal("TIEUDE")),
                            MoTa = reader.IsDBNull(reader.GetOrdinal("MOTA")) ? null : reader.GetString(reader.GetOrdinal("MOTA")),
                            NgayBatDau = reader.GetDateTime(reader.GetOrdinal("NGAYBATDAU")),
                            NgayKetThuc = reader.GetDateTime(reader.GetOrdinal("NGAYKETTHUC")),
                            PhBan = reader.GetString(reader.GetOrdinal("PHBAN"))
                        };
                        danhSachCongViec.Add(lichLamViec);
                    }
                }
            }

            return danhSachCongViec;
        }

        #endregion
        #region Tin nhắn
        public void ThemTinNhanThuong(TinNhan tin)
        {
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                // Kiểm tra MANV và MK
                string queryKiemTra = "SELECT COUNT(*) FROM NHANVIEN WHERE MANV = @MANV AND MK = @MK";
                MySqlCommand cmdKiemTra = new MySqlCommand(queryKiemTra, conn);
                cmdKiemTra.Parameters.AddWithValue("@MANV", tin.NGGUI.MANV);
                cmdKiemTra.Parameters.AddWithValue("@MK", tin.NGGUI.MK);

                conn.Open();
                int soLuongKhop = Convert.ToInt32(cmdKiemTra.ExecuteScalar());
                if (soLuongKhop == 0)
                {
                    throw new UnauthorizedAccessException("Thông tin người gửi không chính xác.");
                }

                // Thêm tin nhắn
                string queryThem = @"
            INSERT INTO TINNHAN (NGGUI, NGNHAN, TG, LTN, ND, TT) 
            VALUES (@NgGui, @NgNhan, @TG, 'Thường', @ND, 'Đã gửi')";
                MySqlCommand cmdThem = new MySqlCommand(queryThem, conn);
                cmdThem.Parameters.AddWithValue("@NgGui", tin.NGGUI.MANV);
                cmdThem.Parameters.AddWithValue("@NgNhan", tin.NGNHAN.MANV);
                cmdThem.Parameters.AddWithValue("@TG", DateTime.Now);
                cmdThem.Parameters.AddWithValue("@ND", tin.ND);

                cmdThem.ExecuteNonQuery();
            }
        }
        public TinNhan LayTinNhan(TinNhan tin)
        {
            TinNhan tinNhan = null;
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                // Kiểm tra MANV và MK của người gửi hoặc người nhận
                string queryKiemTra = @"
            SELECT COUNT(*) 
            FROM NHANVIEN 
            WHERE (MANV = @NgGui AND MK = @MK) OR (MANV = @NgNhan AND MK = @MK1)";
                MySqlCommand cmdKiemTra = new MySqlCommand(queryKiemTra, conn);
                cmdKiemTra.Parameters.AddWithValue("@NgGui", tin.NGGUI);
                cmdKiemTra.Parameters.AddWithValue("@NgNhan", tin.NGNHAN);
                cmdKiemTra.Parameters.AddWithValue("@MK", tin.NGGUI);
                cmdKiemTra.Parameters.AddWithValue("@MK1", tin.NGNHAN);

                conn.Open();
                int soLuongKhop = Convert.ToInt32(cmdKiemTra.ExecuteScalar());
                if (soLuongKhop == 0)
                {
                    throw new UnauthorizedAccessException("Thông tin người gửi hoặc người nhận không chính xác.");
                }

                // Lấy tin nhắn
                string queryLay = @"
            SELECT * FROM TINNHAN 
            WHERE NGGUI = @NgGui AND NGNHAN = @NgNhan AND LTN = 'Thường'";

                if (tin.TG.HasValue)
                {
                    queryLay += " AND TG < @TG";
                }
                else
                {
                    queryLay += " ORDER BY TG DESC LIMIT 1";
                }

                MySqlCommand cmdLay = new MySqlCommand(queryLay, conn);
                cmdLay.Parameters.AddWithValue("@NgGui", tin.NGGUI);
                cmdLay.Parameters.AddWithValue("@NgNhan", tin.NGNHAN);
                if (tin.TG.HasValue)
                {
                    cmdLay.Parameters.AddWithValue("@TG", tin.TG.Value);
                }

                using (MySqlDataReader reader = cmdLay.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tinNhan = new TinNhan
                        {
                            NGGUI = new NhanVien { MANV = reader.GetString(reader.GetOrdinal("NGGUI")) },
                            NGNHAN = new NhanVien { MANV = reader.GetString(reader.GetOrdinal("NGNHAN")) },
                            TG = reader.GetDateTime(reader.GetOrdinal("TG")),
                            LTN = reader.GetString(reader.GetOrdinal("LTN")),
                            ND = reader.GetString(reader.GetOrdinal("ND")),
                            TT = reader.GetString(reader.GetOrdinal("TT"))
                        };
                    }
                }
            }
            return tinNhan;
        }
        public List<NhanVien> LayNguoiNhan(string manv, string mk, int offset = 0)
        {
            List<NhanVien> nguoiNhans = new List<NhanVien>();
            using (MySqlConnection conn = new MySqlConnection(Server0))
            {
                // Kiểm tra MANV và MK
                string queryKiemTra = "SELECT COUNT(*) FROM NHANVIEN WHERE MANV = @MANV AND MK = @MK";
                MySqlCommand cmdKiemTra = new MySqlCommand(queryKiemTra, conn);
                cmdKiemTra.Parameters.AddWithValue("@MANV", manv);
                cmdKiemTra.Parameters.AddWithValue("@MK", mk);

                conn.Open();
                int soLuongKhop = Convert.ToInt32(cmdKiemTra.ExecuteScalar());
                if (soLuongKhop == 0)
                {
                    throw new UnauthorizedAccessException("Thông tin người gửi không chính xác.");
                }

                // Lấy 10 người nhận gần nhất hoặc tiếp theo
                string queryLay = @"
            SELECT DISTINCT NV.MANV, NV.HOTEN, NV.PHBAN
            FROM TINNHAN TN
            JOIN NHANVIEN NV ON TN.NGNHAN = NV.MANV
            WHERE TN.NGGUI = @MANV AND TN.LTN = 'Thường'
            ORDER BY TN.TG DESC
            LIMIT 10 OFFSET @Offset";
                MySqlCommand cmdLay = new MySqlCommand(queryLay, conn);
                cmdLay.Parameters.AddWithValue("@MANV", manv);
                cmdLay.Parameters.AddWithValue("@Offset", offset);

                using (MySqlDataReader reader = cmdLay.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NhanVien nhanVien = new NhanVien
                        {
                            MANV = reader.GetString(reader.GetOrdinal("MANV")),
                            HOTEN = reader.GetString(reader.GetOrdinal("HOTEN")),
                            PHBAN = reader.GetString(reader.GetOrdinal("PHBAN"))
                        };
                        nguoiNhans.Add(nhanVien);
                    }
                }
            }
            return nguoiNhans;
        }



        #endregion
        #region Thông báo
        public void InsertNotification(NotificationManagerData data, string nggui)
        {
            if ((data.Phban != null && data.Ngnhan != null) || (data.Phban == null && data.Ngnhan == null))
            {
                throw new ArgumentException("Chỉ một trong hai trường Phban hoặc Ngnhan được phép khác null.");
            }

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    string sql = "INSERT INTO THONGBAO (NGGUI, PHBAN, NGNHAN, ND, TG) VALUES (@Nggui, @Phban, @Ngnhan, @Content, @Date)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Nggui", nggui);
                        command.Parameters.AddWithValue("@Phban", data.Phban ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Ngnhan", data.Ngnhan ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Content", data.Content);
                        command.Parameters.AddWithValue("@Date", DateTime.Now);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error during database operation: " + ex.Message, ex);
            }
        }
        public List<NotificationManagerData> GetNotificationsForEmployee(string MANV)
        {
            List<NotificationManagerData> notifications = new List<NotificationManagerData>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    // Kiểm tra vai trò của nhân viên
                    bool isManager = false;
                    string checkRoleSql = @"SELECT COUNT(*) FROM PHONGBAN WHERE TRPH = @MANV";
                    using (MySqlCommand checkRoleCmd = new MySqlCommand(checkRoleSql, connection))
                    {
                        checkRoleCmd.Parameters.AddWithValue("@MANV", MANV);
                        isManager = Convert.ToInt32(checkRoleCmd.ExecuteScalar()) > 0;
                    }

                    // Chọn truy vấn dựa trên vai trò
                    string sql;
                    if (isManager)
                    {
                        // Nhân viên là trưởng phòng
                        sql = @"SELECT * FROM THONGBAO 
                            WHERE NGGUI = @MANV 
                            AND TG BETWEEN DATE_SUB(NOW(), INTERVAL 1 WEEK) AND NOW()";
                    }
                    else
                    {
                        // Nhân viên không phải là trưởng phòng
                        sql = @"SELECT * FROM THONGBAO 
                            WHERE PHBAN = (SELECT PHBAN FROM NHANVIEN WHERE MANV = @MANV)
                            AND TG BETWEEN DATE_SUB(NOW(), INTERVAL 1 WEEK) AND NOW()";
                    }

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@MANV", MANV);
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                NotificationManagerData data = new NotificationManagerData
                                {
                                    ID = reader.GetInt32("ID"),
                                    Phban = reader.IsDBNull(reader.GetOrdinal("PHBAN")) ? null : reader.GetString("PHBAN"),
                                    Date = reader.GetDateTime("TG").ToString("dd/MM/yyyy HH:mm"),
                                    Content = reader.GetString("ND"),
                                    Received = reader.IsDBNull(reader.GetOrdinal("SONGNHAN")) ? null : reader.GetInt32("SONGNHAN"),
                                    Seen = reader.IsDBNull(reader.GetOrdinal("SONGXEM")) ? null : reader.GetInt32("SONGXEM"),
                                    IsSeen = HasEmployeeSeenNotification(MANV, reader.GetInt32("ID"))
                                };
                                notifications.Add(data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                throw new Exception("Error: " + ex.Message);
            }

            return notifications;
        }
        public void InsertNgNhanTB(int matb, string manv)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    string sql = "INSERT INTO NGNHANTB (MATB, MANV) VALUES (@Matb, @Manv)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Matb", matb);
                        command.Parameters.AddWithValue("@Manv", manv);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                //throw new Exception("Error: " + ex.Message);
            }
        }
        public void DeleteNotification(int id)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    string sql = "DELETE FROM THONGBAO WHERE ID = @Id";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int affectedRows = command.ExecuteNonQuery();
                        if (affectedRows == 0)
                        {
                            //throw new Exception("Không có thông báo nào được xóa.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }
        public void InsertNgXemTB(int matb, string manv)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    string sql = "INSERT INTO NGXEMTB (MATB, MANV) VALUES (@Matb, @Manv)";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Matb", matb);
                        command.Parameters.AddWithValue("@Manv", manv);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                //throw new Exception("Error: " + ex.Message);
            }
        }
        public bool HasEmployeeSeenNotification(string manv, int matb)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    string sql = "SELECT COUNT(*) FROM NGXEMTB WHERE MANV = @Manv AND MATB = @Matb";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Manv", manv);
                        command.Parameters.AddWithValue("@Matb", matb);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                throw new Exception("Error: " + ex.Message);
            }
        }
        #endregion
        #region Manager Role
        public NhanVien? GetNhanVienByManager(string manv)
        {
            using (var connection = new MySqlConnection(Server0))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM NHANVIEN WHERE MANV = @manv";
                cmd.Parameters.AddWithValue("@manv", manv);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new NhanVien
                        {
                            MANV = reader["MANV"].ToString(),
                            //MK = reader["MK"].ToString(),
                            HOTEN = reader["HOTEN"].ToString(),
                            GIOITINH = reader["GIOITINH"].ToString(),
                            NGSINH = reader.IsDBNull(reader.GetOrdinal("NGSINH")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("NGSINH")),
                            NGVL = reader.IsDBNull(reader.GetOrdinal("NGVL")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("NGVL")),
                            DC = reader["DC"].ToString(),
                            SDT = reader["SDT"].ToString(),
                            EMAIL = reader["EMAIL"].ToString(),
                            CCCD = reader["CCCD"].ToString(),
                            LCB = reader.IsDBNull(reader.GetOrdinal("LCB")) ? 0 : reader.GetDecimal(reader.GetOrdinal("LCB")),
                            PHBAN = reader["PHBAN"].ToString()
                        };
                    }
                }
            }

            return null;
        }
        #endregion
        public void InsertNhanVienToPhongBan(Admin admin, PhongBan phongBan)
        {
            bool? checking = AdminLogin(admin);
            if (checking == null || checking == false)
            {
                throw new Exception("Wrong password or username.");
            }
            using (MySqlConnection connection = new MySqlConnection(Server0))
            {
                string query = "UPDATE NHANVIEN SET PHBAN = @pban WHERE MANV = @manv";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@manv", phongBan.TRPH);
                    command.Parameters.AddWithValue("@pban", phongBan.MAPH);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void DeleteNhanVienFromPhongBan(Admin admin, PhongBan phongBan)
        {
            bool? checking = AdminLogin(admin);
            if (checking == null || checking == false)
            {
                throw new Exception("Wrong password or username.");
            }
            using (MySqlConnection connection = new MySqlConnection(Server0))
            {
                string query = "UPDATE NHANVIEN SET PHBAN = null WHERE MANV = @manv";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@manv", phongBan.TRPH);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public void InsertPhongBan(Admin admin, PhongBan phongBan)
        {
            bool? checking = AdminLogin(admin);
            if (checking == null || checking == false)
            {
                throw new Exception("Wrong password or username.");
            }
            if (string.IsNullOrEmpty(phongBan.TRPH))
                phongBan.TRPH = null;
            using (MySqlConnection connection = new MySqlConnection(Server0))
            {
                connection.Open();
                string query = "INSERT INTO PHONGBAN (TRPH,NGNC) VALUES (@TRPH,@NGNC)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TRPH", phongBan.TRPH);
                    command.Parameters.AddWithValue("@NGNC", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }
        public void UpdatePhongBan(Admin admin, PhongBan phongBan)
        {
            bool? checking = AdminLogin(admin);
            if (checking == null || checking == false)
            {
                throw new Exception("Wrong password or username.");
            }
            if (string.IsNullOrEmpty(phongBan.TRPH))
                phongBan.TRPH = null;
            using (MySqlConnection connection = new MySqlConnection(Server0))
            {
                string query = "UPDATE PHONGBAN SET TRPH = @trph, NGNC = @ngnc WHERE MAPH = @maph;";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@trph", phongBan.TRPH);
                    cmd.Parameters.AddWithValue("@ngnc", DateTime.Now);
                    cmd.Parameters.AddWithValue("@maph", phongBan.MAPH);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeletePhongBan(Admin admin, PhongBan phongBan)
        {
            bool? checking = AdminLogin(admin);
            if (checking == null || checking == false)
            {
                throw new Exception("Wrong password or username.");
            }
            using (MySqlConnection connection = new MySqlConnection(Server0))
            {
                StringBuilder queryBuilder = new StringBuilder("DELETE FROM PHONGBAN WHERE ");

                var conditions = new List<string>();
                var props = typeof(PhongBan).GetProperties().Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime?));
                foreach (var prop in props)
                {
                    var value = prop.GetValue(phongBan);
                    if (value != null && value.ToString() != "")
                    {
                        conditions.Add($"{prop.Name} = @{prop.Name.ToLower()}");
                    }
                }

                if (conditions.Count > 0)
                {
                    queryBuilder.Append(string.Join(" AND ", conditions));
                }
                using (var cmd = new MySqlCommand(queryBuilder.ToString(), connection))
                {
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(phongBan);
                        if (value != null && value.ToString() != "")
                        {
                            cmd.Parameters.AddWithValue($"@{prop.Name.ToLower()}", value);
                        }
                    }
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                //Console.WriteLine(queryBuilder.ToString());
            }
        }
        public List<PhongBan>? GetPhongBans(Admin admin, PhongBan? phongBan)
        {
            bool? checking = AdminLogin(admin);
            if (checking == null || checking == false)
            {
                throw new Exception("Wrong password or username.");
            }
            using (MySqlConnection connection = new MySqlConnection(Server0))
            {
                string query = "SELECT * FROM PHONGBAN";
                if (!string.IsNullOrEmpty(phongBan.MAPH))
                    query += $" WHERE MAPH = \"{phongBan.MAPH}\"";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    List<PhongBan> list = new List<PhongBan>();
                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PhongBan pb = new PhongBan();
                            pb.MAPH = reader["MAPH"].ToString();
                            pb.TRPH = reader["TRPH"].ToString();
                            pb.NGNC = reader.IsDBNull(reader.GetOrdinal("NGNC")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("NGNC"));
                            list.Add(pb);
                        }
                        return list;
                    }
                }
            }
        }
        public string? InsertNhanVien(NhanVien nhanVien)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    string query = "INSERT INTO NHANVIEN (HOTEN, GIOITINH, NGSINH, NGVL, DC, SDT, EMAIL, CCCD, LCB, PHBAN) " +
                                   "VALUES (@HOTEN, @GIOITINH, @NGSINH, @NGVL, @DC, @SDT, @EMAIL, @CCCD, @LCB, @PHBAN)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@HOTEN", nhanVien.HOTEN);
                        command.Parameters.AddWithValue("@GIOITINH", nhanVien.GIOITINH);
                        command.Parameters.AddWithValue("@NGSINH", nhanVien.NGSINH);
                        command.Parameters.AddWithValue("@NGVL", nhanVien.NGVL);
                        command.Parameters.AddWithValue("@DC", nhanVien.DC);
                        command.Parameters.AddWithValue("@SDT", nhanVien.SDT);
                        command.Parameters.AddWithValue("@EMAIL", nhanVien.EMAIL);
                        command.Parameters.AddWithValue("@CCCD", nhanVien.CCCD);
                        command.Parameters.AddWithValue("@LCB", nhanVien.LCB);
                        command.Parameters.AddWithValue("@PHBAN", nhanVien.PHBAN);

                        command.ExecuteNonQuery();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public NhanVien? GetPHBanFromNhanVien(string manv)
        {
            using (var connection = new MySqlConnection(Server0))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM NHANVIEN WHERE MANV = @manv";
                cmd.Parameters.AddWithValue("@manv", manv);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new NhanVien
                        {
                            MANV = reader["MANV"].ToString(),
                            PHBAN = reader["PHBAN"].ToString()
                        };
                    }
                }
            }
            return null;
        }
        public NhanVien? GetNhanVien(string manv, string mk)
        {
            using (var connection = new MySqlConnection(Server0))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT * FROM NHANVIEN WHERE MANV = @manv AND MK = @mk";
                cmd.Parameters.AddWithValue("@manv", manv);
                cmd.Parameters.AddWithValue("@mk", mk);

                connection.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new NhanVien
                        {
                            MANV = reader["MANV"].ToString(),
                            //MK = reader["MK"].ToString(),
                            HOTEN = reader["HOTEN"].ToString(),
                            GIOITINH = reader["GIOITINH"].ToString(),
                            NGSINH = reader.IsDBNull(reader.GetOrdinal("NGSINH")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("NGSINH")),
                            NGVL = reader.IsDBNull(reader.GetOrdinal("NGVL")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("NGVL")),
                            DC = reader["DC"].ToString(),
                            SDT = reader["SDT"].ToString(),
                            EMAIL = reader["EMAIL"].ToString(),
                            CCCD = reader["CCCD"].ToString(),
                            LCB = reader.IsDBNull(reader.GetOrdinal("LCB")) ? 0 : reader.GetDecimal(reader.GetOrdinal("LCB")),
                            PHBAN = reader["PHBAN"].ToString()
                        };
                    }
                }
            }

            return null;
        }
        public string? ChangePassword(string username, string password)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    string query = "UPDATE NHANVIEN " +
                                   "SET MK = @MK " +
                                   "WHERE MANV = @MANV;";

                    //Console.WriteLine(query);

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MANV", username);
                        command.Parameters.AddWithValue("@MK", password);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            // Lệnh UPDATE không tìm thấy MANV trùng khớp
                            throw new Exception("No matching MANV found.");
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public bool? Login(string username, string password)
        {
            string? mk = null;
            using (MySqlConnection connection = new MySqlConnection(Server0))
            {
                connection.Open();

                string query = "SELECT MK FROM NHANVIEN WHERE MANV = @MANV";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MANV", username);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            mk = reader["MK"].ToString();
                        }
                        else return false;
                    }
                }
            }
            if (!string.IsNullOrEmpty(mk))
            {
                if (mk == password) return true; else return false;
            }
            return null;
        }
        public string? CheckIfTRPHExists(string trph)
        {
            using (var connection = new MySqlConnection(Server0))
            {
                connection.Open();
                string query = @"
                    SELECT PHONGBAN.MAPH
                    FROM PHONGBAN
                    WHERE PHONGBAN.TRPH = @manv";
                using (var cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@manv", trph);
                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        return result.ToString();
                    }
                    else return null;
                }
            }
        }
        public string? AdminChangePassword(ChangePasswordData data)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(Server0))
                {
                    connection.Open();

                    string query = "UPDATE ADMIN " +
                                   "SET MK = @MK " +
                                   "WHERE Id = @Id";
                    if (!string.IsNullOrEmpty(data.OldPassword))
                        query += " AND MK = @OldMK;";
                    else query += " AND MK IS NULL;";

                    //Console.WriteLine(query);

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", int.Parse(data.Username));
                        command.Parameters.AddWithValue("@MK", data.NewPassword);
                        command.Parameters.AddWithValue("@OldMK", data.OldPassword);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            // Lệnh UPDATE không tìm thấy MANV trùng khớp
                            throw new Exception("No matching ADMIN found.");
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public bool? AdminLogin(Admin admin)
        {
            using (var connection = new MySqlConnection(Server0))
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT MK FROM ADMIN WHERE Id = @id";
                cmd.Parameters.AddWithValue("@id", admin.Id);

                connection.Open();
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (reader["MK"] == DBNull.Value || reader["MK"].ToString() == "")
                        return null;
                    if (reader["MK"].ToString() == admin.MK)
                        return true;
                }
            }
            return false;
        }
        public string? InsertAdmin(Admin admin)
        {
            try
            {
                using (var connection = new MySqlConnection(Server0))
                {
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO ADMIN (Ten, MK) VALUES (@ten, @mk)";
                    cmd.Parameters.AddWithValue("@ten", admin.Ten);
                    cmd.Parameters.AddWithValue("@mk", admin.MK);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public List<NhanVien>? GetNhanViensEmptyPHBAN(Admin admin, NhanVien nhanVien)
        {
            bool? checking = AdminLogin(admin);
            if (checking == null || checking == false)
            {
                return null;
            }
            StringBuilder queryBuilder = new StringBuilder("SELECT ");
            var checker = nhanVien.Check;
            var fields = new List<string>();

            var properties = typeof(NhanVien.Checker).GetProperties();
            foreach (var prop in properties)
            {
                var value = (bool)prop.GetValue(checker);
                if (value)
                {
                    fields.Add(prop.Name);
                }
            }

            queryBuilder.Append(fields.Count > 0 ? string.Join(", ", fields) : "*");
            queryBuilder.Append(" FROM NHANVIEN WHERE PHBAN is null");

            using (var connection = new MySqlConnection(Server0))
            {
                using (var cmd = new MySqlCommand(queryBuilder.ToString(), connection))
                {

                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        List<NhanVien> res = new List<NhanVien>();
                        while (reader.Read())
                        {
                            var resultNhanVien = new NhanVien();
                            foreach (var field in fields)
                            {
                                var prop = typeof(NhanVien).GetProperty(field);
                                if (prop.PropertyType == typeof(DateTime?))
                                {
                                    prop.SetValue(resultNhanVien, reader.IsDBNull(reader.GetOrdinal(field)) ? null : (DateTime?)reader[field]);
                                }
                                else if (prop.PropertyType == typeof(decimal?))
                                {
                                    prop.SetValue(resultNhanVien, reader.IsDBNull(reader.GetOrdinal(field)) ? null : (decimal?)reader[field]);
                                }
                                else
                                {
                                    prop.SetValue(resultNhanVien, reader[field].ToString());
                                }
                            }
                            resultNhanVien.MK = null;
                            res.Add(resultNhanVien);
                        }
                        return res;
                    }
                }
            }
        }
        public List<NhanVien>? GetNhanViens(Admin admin, NhanVien nhanVien, NhanVien? trph = null)
        {
            if (trph == null)
            {
                bool? checking = AdminLogin(admin);
                if (checking == null || checking == false)
                {
                    return null;
                }
            }


            StringBuilder queryBuilder = new StringBuilder("SELECT ");
            var checker = nhanVien.Check;
            var fields = new List<string>();

            var properties = typeof(NhanVien.Checker).GetProperties();
            foreach (var prop in properties)
            {
                var value = (bool)prop.GetValue(checker);
                if (value)
                {
                    fields.Add(prop.Name);
                }
            }

            queryBuilder.Append(fields.Count > 0 ? string.Join(", ", fields) : "*");
            queryBuilder.Append(" FROM NHANVIEN");

            var conditions = new List<string>();
            var props = typeof(NhanVien).GetProperties().Where(p => p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime?) || p.PropertyType == typeof(decimal?));
            foreach (var prop in props)
            {
                var value = prop.GetValue(nhanVien);
                if (value != null && value.ToString() != "")
                {
                    conditions.Add($"{prop.Name} = @{prop.Name.ToLower()}");
                }
            }

            if (conditions.Count > 0)
            {
                queryBuilder.Append(" WHERE " + string.Join(" AND ", conditions));
            }

            using (var connection = new MySqlConnection(Server0))
            {
                using (var cmd = new MySqlCommand(queryBuilder.ToString(), connection))
                {
                    foreach (var prop in props)
                    {
                        var value = prop.GetValue(nhanVien);
                        if (value != null && value.ToString() != "")
                        {
                            cmd.Parameters.AddWithValue($"@{prop.Name.ToLower()}", value);
                        }
                    }

                    connection.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        List<NhanVien> res = new List<NhanVien>();
                        while (reader.Read())
                        {
                            var resultNhanVien = new NhanVien();
                            foreach (var field in fields)
                            {
                                var prop = typeof(NhanVien).GetProperty(field);
                                if (prop.PropertyType == typeof(DateTime?))
                                {
                                    prop.SetValue(resultNhanVien, reader.IsDBNull(reader.GetOrdinal(field)) ? null : (DateTime?)reader[field]);
                                }
                                else if (prop.PropertyType == typeof(decimal?))
                                {
                                    prop.SetValue(resultNhanVien, reader.IsDBNull(reader.GetOrdinal(field)) ? null : (decimal?)reader[field]);
                                }
                                else
                                {
                                    prop.SetValue(resultNhanVien, reader[field].ToString());
                                }
                            }
                            resultNhanVien.MK = null;
                            res.Add(resultNhanVien);
                        }
                        return res;
                    }
                }
            }
        }
    }
}
