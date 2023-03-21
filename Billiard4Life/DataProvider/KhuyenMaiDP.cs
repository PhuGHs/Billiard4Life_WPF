using Billiard4Life.Models;
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
                DataTable dt = LoadInitialData("Select * from KHUYENMAI");
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
                cmd.CommandText = "INSERT INTO KHUYENMAI VALUES (@makm, @tenkm, @giam, @mucapdung, @ngaybd, @ngaykt, @mota, @trangthai)";
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
            } finally
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
                cmd.CommandText = "DELETE FROM KHUYENMAI WHERE MAKM = @makm";
                cmd.Parameters.AddWithValue("@makm", makm);
                cmd.Connection=SqlCon;

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
            } finally
            {
                DBClose();
            }
        }
    }
}
