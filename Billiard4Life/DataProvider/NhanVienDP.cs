using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using OfficeOpenXml.Drawing.Chart;
using Billiard4Life.Models;
using System.Collections.ObjectModel;

namespace Billiard4Life.DataProvider;

public class NhanVienDP : DataProvider
{
    private static NhanVienDP flag;
    public static NhanVienDP Flag
    {
        get
        {
            if (flag == null) flag = new NhanVienDP();
            return flag;
        }
        set
        {
            flag = value;
        }
    }
    public void AddStaff(NhanVien nv)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "INSERT INTO NHANVIEN (MaNV, TenNV, ChucVu, Fulltime, DiaChi, SDT, NgaySinh, NgayVaoLam) " +
            "VALUES (@manv, @ten, @chucvu, @fulltime, @diachi, @sdt, @ngaysinh, @ngayvaolam)";
        cmd.Parameters.AddWithValue("@manv", nv.MaNV);
        cmd.Parameters.AddWithValue("@ten", nv.HoTen);
        cmd.Parameters.AddWithValue("@chucvu", nv.ChucVu);
        cmd.Parameters.AddWithValue("@fulltime", nv.Fulltime);
        cmd.Parameters.AddWithValue("@diachi", nv.DiaChi);
        cmd.Parameters.AddWithValue("@sdt", nv.SDT);
        cmd.Parameters.AddWithValue("@ngaysinh", nv.NgaySinh);
        cmd.Parameters.AddWithValue("@ngayvaolam", nv.NgayVaoLam);
        cmd.Connection = SqlCon;
        cmd.ExecuteNonQuery();

        if (!string.IsNullOrEmpty(nv.TaiKhoan))
        {
            cmd.CommandText = "INSERT INTO TAIKHOAN (ID, MatKhau, Quyen, MaNV) " +
                "VALUES(@taikhoan, @matkhau, @quyen, @manv)";
            cmd.Parameters.AddWithValue("@taikhoan", nv.TaiKhoan);
            cmd.Parameters.AddWithValue("@matkhau", nv.MatKhau);
            cmd.Parameters.AddWithValue("@quyen", "nhanvien");
            cmd.ExecuteNonQuery();
        }
        MyMessageBox msb = new MyMessageBox("Thêm thành công!");
        msb.ShowDialog();

        DBClose();
    }
    public void DeleteStaff(string MaNV)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "UPDATE NHANVIEN SET Xoa = 1 WHERE MaNV = @manv";
        cmd.Parameters.AddWithValue("@manv", MaNV);
        cmd.Connection = SqlCon;
        cmd.ExecuteNonQuery();

        DBClose();
    }
    public string AutoIDStaff()
    {
        string ID = "NV001", temp = "";

        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT TOP (1) MaNV FROM NHANVIEN ORDER BY MaNV DESC";
        cmd.Connection = SqlCon;

        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            temp = reader.GetString(0);
        }
        reader.Close();

        if (!string.IsNullOrEmpty(temp))
        {
            int num = ExtractNumber(temp) + 1;
            temp = num.ToString();
            while (temp.Length < 3) temp = "0" + temp;
            ID = "NV" + temp;
        }

        DBClose();

        return ID;
    }

    public ObservableCollection<NhanVien> GetAllStaff(string query)
    {
        ObservableCollection<NhanVien> ListStaff = new ObservableCollection<NhanVien>();

        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = query;
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string id = reader.GetString(0);
            string ten = reader.GetString(1);
            string chucvu = reader.GetString(2);
            bool ftime = reader.GetBoolean(3);
            string diachi = reader.GetString(4);
            string sdt = reader.GetString(5);
            string ngsinh = reader.GetDateTime(6).ToShortDateString();
            string ngvl = reader.GetDateTime(7).ToShortDateString();
            string tk = "";
            if (!reader.IsDBNull(9))
                tk = reader.GetString(9);
            string mk = "";
            if (!reader.IsDBNull(10))
                mk = reader.GetString(10);

            ListStaff.Add(new NhanVien(id, ten, chucvu, diachi, ftime, sdt, ngvl, ngsinh, tk, mk));
        }

        DBClose();

        return ListStaff;
    }
    public void UpdateInfoStaff(NhanVien nv)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "UPDATE NhanVien SET TenNV = @ten, ChucVu = @chucvu, FullTime = @fulltime, DiaChi = @diachi, " +
            "SDT = @sdt, NgaySinh = @ngaysinh, NgayVaoLam = @nvl WHERE MaNV = @manv";
        cmd.Parameters.AddWithValue("@manv", nv.MaNV);
        cmd.Parameters.AddWithValue("@ten", nv.HoTen);
        cmd.Parameters.AddWithValue("@chucvu", nv.ChucVu);
        cmd.Parameters.AddWithValue("@fulltime", nv.Fulltime);
        cmd.Parameters.AddWithValue("@diachi", nv.DiaChi);
        cmd.Parameters.AddWithValue("@sdt", nv.SDT);
        cmd.Parameters.AddWithValue("@ngaysinh", nv.NgaySinh);
        cmd.Parameters.AddWithValue("@nvl", nv.NgayVaoLam);
        cmd.Connection = SqlCon;
        cmd.ExecuteNonQuery();

        MyMessageBox msb = new MyMessageBox("Sửa thành công!");
        msb.ShowDialog();
        DBClose();
    }
    public List<Tuple<string, string>> GetIDAndNameStaff()
    {
        List<Tuple<string, string>> temp = new List<Tuple<string, string>>();

        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT MaNV, TenNV FROM NHANVIEN WHERE Xoa = 0";
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            temp.Add(new Tuple<string, string>(reader.GetString(0), reader.GetString(1)));
        }
        reader.Close();

        DBClose();

        return temp;
    }
    public bool IsStaff(string MaNV)
    {
        DBOpen();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT Quyen FROM TAIKHOAN WHERE MaNV = @manv";
        cmd.Parameters.AddWithValue("@manv", MaNV);
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        bool isStaff = false;
        if (reader.Read())
        {
            if (reader.GetString(0) == "nhanvien") isStaff = true;
        }
        reader.Close();

        DBClose();
        return isStaff;
    }
    public void StartTimeKeeping(string MaNV)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "UPDATE TAIKHOAN SET BatDauCa = @batdauca WHERE MaNV = @manv";
        cmd.Parameters.AddWithValue("@batdauca", DateTime.Now);
        cmd.Parameters.AddWithValue("@manv", MaNV);
        cmd.Connection = SqlCon;
        cmd.ExecuteNonQuery();

        DBClose();
    }
    public void SetAccountOnline(string MaNV)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "UPDATE TAIKHOAN SET Online = 1 WHERE MaNV = @manv";
        cmd.Parameters.AddWithValue("@manv", MaNV);
        cmd.Connection = SqlCon;
        cmd.ExecuteNonQuery();

        DBClose();
    }
    public void SetAccountOffline(string MaNV)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "UPDATE TAIKHOAN SET Online = 0 WHERE MaNV = @manv";
        cmd.Parameters.AddWithValue("@manv", MaNV);
        cmd.Connection = SqlCon;
        cmd.ExecuteNonQuery();

        DBClose();
    }
    public void StopTimeKeepingAndRecord(string MaNV)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT BatDauCa FROM TAIKHOAN WHERE MaNV = @manv";
        cmd.Parameters.AddWithValue("@manv", MaNV);
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();
        DateTime BatDauCa = DateTime.Now;
        if (reader.Read())
        {
            BatDauCa = reader.GetDateTime(0);
        }
        reader.Close();
        double SoGio = (DateTime.Now - BatDauCa).TotalSeconds;
        if (SoGio < 60) return;

        if (IsChecked(MaNV) == false) cmd.CommandText = "INSERT INTO CHITIETCHAMCONG VALUES(@manv, @ngaycc, @sogiocong, N'')";
        else cmd.CommandText = "UPDATE CHITIETCHAMCONG SET SoGioCong = SoGioCong + @sogiocong WHERE MaNV = @manv AND NgayCC = @ngaycc";
        
        DBOpen();
        
        //cmd.Parameters.AddWithValue("@manv", MaNV);
        cmd.Parameters.AddWithValue("@ngaycc", BatDauCa.Date);
        cmd.Parameters.AddWithValue("@sogiocong", ConvertTotalSecondToHour(SoGio));
        cmd.Connection = SqlCon;
        cmd.ExecuteNonQuery();

        DBClose();
    }
    public Tuple<NhanVien, string> StaffOnline()
    {
        Tuple<NhanVien, string> temp = null;

        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT n.*, BatDauCa FROM NHANVIEN AS n JOIN TAIKHOAN AS t ON n.MaNV = t.MaNV WHERE Online = 1";
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();
        string start = "";
        if (reader.Read())
        {
            temp = new Tuple<NhanVien, string>(new NhanVien(reader.GetString(0), reader.GetString(1)), reader.GetDateTime(9).ToString());
        }
        reader.Close();

        DBClose();

        return temp;
    }
    #region Support Method
    public double ConvertTotalSecondToHour(double total)
    {
        double result = total / 3600;
        total = total % 3600;
        return Math.Round(result + total / 3600, 2);
    }
    public bool IsOnline(string MaNV)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT Online FROM TAIKHOAN WHERE MaNV = @manv";
        cmd.Parameters.AddWithValue("@manv", MaNV);
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();
        bool isOnline = false;
        if (reader.Read())
        {
            isOnline = reader.GetBoolean(0);
        }
        reader.Close();

        DBClose();

        return isOnline;
    }
    public bool IsAnyStaffOnline()
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "SELECT * FROM TAIKHOAN WHERE Online = 1";
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        bool temp = false;
        if (reader.Read()) { temp = true; }

        DBClose();

        return temp;
    }
    public bool IsChecked(string MaNV)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = "SELECT * FROM CHITIETCHAMCONG WHERE MaNV = @manv AND NgayCC = @ngay";
        cmd.Parameters.AddWithValue("@manv", MaNV);
        cmd.Parameters.AddWithValue("@ngay", DateTime.Now.ToShortDateString());
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        bool isChecked = false;
        if (reader.Read()) isChecked = true;
        reader.Close();

        DBClose();

        return isChecked;
    }
    private int ExtractNumber(string input)
    {
        string output = string.Empty;
        foreach (char c in input)
        {
            if (char.IsDigit(c))
            {
                output += c;
            }
        }
        return int.Parse(output);
    }
    #endregion
}
