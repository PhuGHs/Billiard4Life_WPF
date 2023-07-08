using Billiard4Life.Models;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.DataProvider
{
    public class KhuyenMaiDP : DataProvider
    {
        private static KhuyenMaiDP flag;
        public static KhuyenMaiDP Flag
        {
            get
            {
                if (flag == null) flag = new KhuyenMaiDP();
                return flag;
            }
            set
            {
                flag = value;
            }
        }

        public ObservableCollection<KhuyenMai> GetKhuyenMais()
        {
            ObservableCollection<KhuyenMai> KhuyenMais = new ObservableCollection<KhuyenMai>();
            try
            {
                DataTable dt = LoadInitialData("Select * from KHUYENMAI WHERE DELETED = 0");
                foreach (DataRow dr in dt.Rows)
                {
                    string? makm = Convert.ToString(dr["MaKM"]);
                    string? tenkm = Convert.ToString(dr["TenKM"]);
                    int giamgia = Convert.ToInt32(dr["GiamGia"]);
                    Decimal mucapdung = Convert.ToDecimal(dr["MucApDung"]);
                    string trangthai = Convert.ToString(dr["TrangThai"]);
                    string ngaybd = Convert.ToDateTime(dr["BatDau"]).ToShortDateString();
                    string ngaykt = Convert.ToDateTime(dr["KetThuc"]).ToShortDateString();
                    string? mota = Convert.ToString(dr["MoTa"]);

                    if (makm == null
                        || tenkm == null
                        || mota == null
                        || ngaybd == null
                        || ngaykt == null)
                    {
                        throw new ArgumentNullException("Thieu property khong the lay len tu database");
                    }
                    KhuyenMais.Add(new KhuyenMai(makm, tenkm, mucapdung, trangthai, ngaybd, ngaykt, mota, giamgia));
                }
            }
            catch (Exception ex)
            {
                MyMessageBox msb = new MyMessageBox(ex.Message);
                msb.Show();
            }
            finally
            {
                DBClose();
            }
            return KhuyenMais;
        }
        public void AddKhuyenMai(KhuyenMai item)
        {
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "INSERT INTO KHUYENMAI (MaKM, TenKM, GiamGia, MucApDung, BatDau, KetThuc, MoTa, TrangThai) VALUES (@makm, @tenkm, @giam, @mucapdung, @ngaybd, @ngaykt, @mota, @trangthai)";
                cmd.Parameters.AddWithValue("@makm", item.MAKM);
                cmd.Parameters.AddWithValue("@tenkm", item.TenKM);
                cmd.Parameters.AddWithValue("@giam", item.GiamGia);
                cmd.Parameters.AddWithValue("@mucapdung", item.MucApDung);
                cmd.Parameters.AddWithValue("@ngaybd", item.NgayBatDau);
                cmd.Parameters.AddWithValue("@ngaykt", item.NGayKetThuc);
                cmd.Parameters.AddWithValue("@mota", item.MoTa);
                cmd.Parameters.AddWithValue("@trangthai", item.TrangThai);

                cmd.Connection = SqlCon;

                cmd.ExecuteNonQuery();
            }
            finally
            {
                DBClose();
            }
        }
        public void DeleteKhuyenmai(string makm)
        {
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UPDATE KHUYENMAI SET DELETED = 1 WHERE MAKM = @makm";
                cmd.Parameters.AddWithValue("@makm", makm);
                cmd.Connection = SqlCon;

                cmd.ExecuteNonQuery();
            }
            finally { DBClose(); }
        }
        public void UpdateKhuyenMai(KhuyenMai item)
        {
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UPDATE KHUYENMAI SET TenKM = @tenkm, BatDau = @ngaybd, KetThuc = @ngaykt, GiamGia = @giam,  MucApDung = @mucapdung, MoTa = @mota, TrangThai = @trangthai WHERE MaKM = @makm";
                cmd.Parameters.AddWithValue("@tenkm", item.TenKM);
                cmd.Parameters.AddWithValue("@ngaybd", item.NgayBatDau);
                cmd.Parameters.AddWithValue("@ngaykt", item.NGayKetThuc);
                cmd.Parameters.AddWithValue("@giam", item.GiamGia);
                cmd.Parameters.AddWithValue("@mucapdung", item.MucApDung);
                cmd.Parameters.AddWithValue("@mota", item.MoTa);
                cmd.Parameters.AddWithValue("@trangthai", item.TrangThai);
                cmd.Parameters.AddWithValue("@makm", item.MAKM);
                cmd.Connection = SqlCon;

                cmd.ExecuteNonQuery();
            }
            finally
            {
                DBClose();
            }
        }
        public void AutoUpdateStatusKhuyenMai(ObservableCollection<KhuyenMai> kms)
        {
            DateOnly current = DateOnly.FromDateTime(DateTime.Now);

            foreach (KhuyenMai item in kms)
            {
                if (current.CompareTo(DateOnly.FromDateTime(Convert.ToDateTime(item.NgayBatDau))) < 0)
                {
                    UpdateTrangThai(item.MAKM, "Chưa đến kỳ hạn");
                }
                else
                {
                    if (current.CompareTo(DateOnly.FromDateTime(Convert.ToDateTime(item.NGayKetThuc))) <= 0)
                    {
                        UpdateTrangThai(item.MAKM, "Đang diễn ra");
                    }
                    else
                    {
                        UpdateTrangThai(item.MAKM, "Hết hạn");
                    }
                }

            }
        }

        public KhuyenMai GetKhuyenMaisBasedOnMucApDung(Decimal total)
        {
            DataTable dt = new DataTable();
            KhuyenMai? km = null;
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT TOP 1 * FROM KHUYENMAI " +
                                  "WHERE @total >= MucApDung AND TrangThai = @trangthai AND Deleted = 0" +
                                  "ORDER BY GiamGia DESC";
                cmd.Parameters.AddWithValue("@total", total);
                cmd.Parameters.AddWithValue("@trangthai", "Đang diễn ra");
                cmd.Connection = SqlCon;

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string makm = reader.GetString(0);
                    string tenkm = reader.GetString(1);
                    decimal mucapdung = reader.GetDecimal(3);
                    int giamgia = reader.GetInt16(2);
                    string ngaybd = reader.GetDateTime(4).ToShortDateString();
                    string ngaykt = reader.GetDateTime(5).ToShortDateString();
                    string mota = reader.GetString(6);
                    string trangthai = reader.GetString(7);
                    km = new KhuyenMai(makm, tenkm, mucapdung, trangthai, ngaybd, ngaykt, mota, giamgia);
                }
                reader.Close();

            } catch (Exception ex)
            {
                MyMessageBox msb = new MyMessageBox(ex.Message);
                msb.Show();
            } finally
            {
                DBClose();
            }
            return km;
        }

        public void UpdateTrangThai(string makm, string trangthai)
        {
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UPDATE KHUYENMAI SET TRANGTHAI = @trangthai WHERE MaKM = @makm";
                cmd.Parameters.AddWithValue("@trangthai", trangthai);
                cmd.Parameters.AddWithValue("@makm", makm);

                cmd.Connection = SqlCon;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MyMessageBox msb = new MyMessageBox(ex.Message);
                msb.Show();
            }
            finally { DBClose(); }
        }
        public string AutoIDKhuyenMai()
        {
            string ID = "KM001";
            string temp = "";
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT TOP 1 MaKM FROM KHUYENMAI ORDER BY MaKM DESC";
            cmd.Connection = SqlCon;
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                temp = reader.GetString(0);
            }
            reader.Close();
            if (!string.IsNullOrEmpty(temp))
            {
                int num = ExtractNumber(temp) + 1;
                temp = num.ToString();
                while (temp.Length < 3) temp = "0" + temp;
                ID = "KM" + temp;
            }
            DBClose();
            return ID;
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
    }
}
