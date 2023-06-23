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
    public ObservableCollection<HoaDon> GetBillsFrom(string beginDate, string endDate, string paymethod = "Tất cả")
    {
        ObservableCollection<HoaDon> bills = new ObservableCollection<HoaDon>();

        string query = "SELECT h.*, kh.TenKH FROM HOADON AS h LEFT JOIN KHACHHANG AS kh ON h.MaKH =" +
            " kh.MaKH WHERE NgayHD >= @begin AND NgayHD <= @end AND TrangThai = N'Đã thanh toán'";
        if (paymethod != "Tất cả") query += " AND HinhThucThanhToan = N'" + paymethod + "'";

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
            string sogio = reader.GetTimeSpan(1).TotalMinutes.ToString();
            sogio = ConvertTime(int.Parse(sogio));
            string trigia = reader.GetSqlMoney(2).ToString();
            string manv = reader.GetString(3);
            string soban = reader.GetInt16(6).ToString();
            string ngayhd = reader.GetDateTime(7).ToShortDateString();
            string httt = reader.GetString(9).ToString();
            string tenkh = "";
            if (!reader.IsDBNull(10))
                tenkh = reader.GetString(10);
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
            string ten = reader.GetString(4);
            string soluong = reader.GetInt16(3).ToString();
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
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandType = System.Data.CommandType.Text;
        cmd.CommandText = "SELECT BatDauCa FROM TAIKHOAN WHERE Online = 1";
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        DateTime BatDauCa = DateTime.Now;
        if (reader.Read())
        {
            BatDauCa = reader.GetDateTime(0);
        }
        reader.Close();

        DBClose();

        return HoaDonDP.Flag.GetBillsFrom(BatDauCa.ToString(), DateTime.Now.ToString(), paymethod);
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
