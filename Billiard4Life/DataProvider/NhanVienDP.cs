using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using OfficeOpenXml.Drawing.Chart;

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
    #endregion
}
