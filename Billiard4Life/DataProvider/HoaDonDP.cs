using Billiard4Life.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.DataProvider;
public class HoaDonDP : DataProvider
{
    private static HoaDonDP flag;
    public static HoaDonDP Flag
    {
        get
        {
            if (flag == null) flag = new HoaDonDP();
            return flag;
        }
        set
        {
            flag = value;
        }
    }
    public ObservableCollection<HoaDon> GetBillsFrom(string beginDate, string endDate, string paymethod, string MaNV, bool staff = false)
    {
        ObservableCollection<HoaDon> bills = new ObservableCollection<HoaDon>();

        string query = "SELECT h.*, kh.TenKH FROM HOADON AS h LEFT JOIN KHACHHANG AS kh ON h.MaKH =" +
            " kh.MaKH WHERE TrangThai = N'Đã thanh toán' ";

        if (staff)
        {
            query += " AND NgayHD >= @begin AND NgayHD <= @end";
        }
        else query += " AND CONVERT(Date, NgayHD) >= @begin AND CONVERT(Date, NgayHD) <= @end";
        if (paymethod != "Tất cả") query += " AND HinhThucThanhToan = N'" + paymethod + "'";
        if (MaNV != "Tất cả") query += " And MaNV = '" + MaNV + "'";

        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = query;
        cmd.Parameters.AddWithValue("@begin", beginDate);
        cmd.Parameters.AddWithValue("@end", endDate);
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string ma = reader.GetInt16(0).ToString();
            string sogio = Math.Round(reader.GetTimeSpan(1).TotalMinutes).ToString();
            sogio = ConvertTime(int.Parse(sogio));
            string trigia = String.Format("{0:0,0 VND}", Math.Round((decimal)reader.GetSqlMoney(2)));
            string manv = reader.GetString(3);
            string soban = reader.GetInt16(6).ToString();
            string ngayhd = reader.GetDateTime(7).ToString();
            string httt = reader.GetString(9).ToString();
            string tenkh = "";
            if (!reader.IsDBNull(11))
                tenkh = reader.GetString(11);
            else tenkh = "Khách vãng lai";
            string makm = "";
            if (!reader.IsDBNull(5))
                makm = reader.GetString(5);

            bills.Add(new HoaDon(ma, sogio, trigia, manv, soban, ngayhd, httt, tenkh, makm));
        }

        DBClose();

        return bills;
    }
    public ObservableCollection<ChiTietHoaDon> GetDetailBill(string SoHD)
    {
        ObservableCollection<ChiTietHoaDon> detailBills = new ObservableCollection<ChiTietHoaDon>();

        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = "SELECT ct.*, m.TenMon, m.Gia FROM CTHD AS ct JOIN MENU AS m ON ct.MaMon = m.MaMon WHERE SoHD = " + SoHD;
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            string ten = reader.GetString(3);
            string soluong = reader.GetInt16(2).ToString();
            string gia = reader.GetSqlMoney(4).ToString();
            float tien = float.Parse(soluong) * float.Parse(gia);

            detailBills.Add(new ChiTietHoaDon(ten, soluong, gia, tien.ToString()));
        }
        reader.Close();

        DBClose();

        return detailBills;
    }
    public ObservableCollection<HoaDon> GetBillsShift(string paymethod)
    {
        string start = NhanVienDP.Flag.StaffOnline().Item2;
        return HoaDonDP.Flag.GetBillsFrom(start, DateTime.Now.ToString(), paymethod, "Tất cả", true);
    }
    public string TotalBillPerMethod(string paymethod)
    {
        string start = NhanVienDP.Flag.StaffOnline().Item2;

        DBOpen();

        string query = "SELECT SUM(TriGia) FROM HOADON WHERE NgayHD >= '" + start + "' ";
        if (paymethod != "Tất cả") query += " AND HinhThucThanhToan = N'" + paymethod + "'";

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = query;
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        string total = "0 VNĐ";
        if (reader.Read())
        {
            if (!reader.IsDBNull(0)) total = String.Format("{0:0,0 VND}", Math.Round((decimal)reader.GetSqlMoney(0)));
        }
        reader.Close();

        DBClose();

        return total;
    }
    #region Support Method
    private string ConvertTime(int h)
    {
        int gio = h / 60;
        int du = h % 60;
        return gio.ToString() + "h" + du.ToString() + "m";
    }
    #endregion
}
