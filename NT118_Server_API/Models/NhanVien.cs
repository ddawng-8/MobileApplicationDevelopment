namespace NT118_Server_API.Models
{
    public class NhanVien
    {
        public NhanVien(string manv) 
        {
            MANV = manv;
        }
        public NhanVien() { }
        public string? MANV { get; set; }
        public string? MK { get; set; }
        public string? HOTEN { get; set; }
        public string? GIOITINH { get; set; }
        public DateTime? NGSINH { get; set; }
        public DateTime? NGVL { get; set; }
        public string? DC { get; set; }
        public string? SDT { get; set; }
        public string? EMAIL { get; set; }
        public string? CCCD { get; set; }
        public decimal? LCB { get; set; }
        public string? PHBAN { get; set; }
        public Checker? Check { get; set; }
        public class Checker
        {
            public bool MANV { get; set; }
            public bool MK { get; set; }
            public bool HOTEN { get; set; }
            public bool GIOITINH { get; set; }
            public bool NGSINH { get; set; } 
            public bool NGVL { get; set; }
            public bool DC { get; set; }
            public bool SDT { get; set; }
            public bool EMAIL { get; set; }
            public bool CCCD { get; set; }
            public bool LCB { get; set; }
            public bool PHBAN { get; set; }
        }
    }
}
