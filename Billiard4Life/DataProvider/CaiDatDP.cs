using Billiard4Life.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Billiard4Life.DataProvider
{
    public class CaiDatDP : DataProvider
    {
        private static CaiDatDP flag;
        public static CaiDatDP Flag
        {
            get
            {
                if (flag == null) flag = new CaiDatDP();
                return flag;
            }
            set
            {
                flag = value;
            }
        }
        public NhanVien GetCurrentEmployee(string MaNV, string pw)
        {
            NhanVien nv = null;
            DataTable dt = new DataTable();
            dt = LoadInitialData("Select * from NhanVien where MaNV = '" + MaNV + "'");
            foreach (DataRow dr in dt.Rows)
            {
                nv = new NhanVien(dr["MaNV"].ToString(), dr["TenNV"].ToString(), dr["ChucVu"].ToString(), dr["DiaChi"].ToString(), (bool)dr["FullTime"], dr["SDT"].ToString(), Convert.ToDateTime(dr["NgayVaoLam"]).ToShortDateString(), Convert.ToDateTime(dr["NgaySinh"]).ToShortDateString());
            }
            nv.MatKhau = pw;
            return nv;
        }
        public void ChangePassword(string pw, string ID)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Update TaiKhoan set MatKhau = @password where ID = @id";
                cmd.Parameters.AddWithValue("@password", pw);
                cmd.Parameters.AddWithValue("@id", ID);
                DBOpen();

                cmd.Connection = SqlCon;
                cmd.ExecuteNonQuery();
            }
            finally
            {
                DBClose();
                MyMessageBox msb = new MyMessageBox("Đổi mật khẩu thành công!");
                msb.Show();
            }

        }

        public void UpdateInfo(string HoTen, string Diachi, string SDT, string NgaySinh, string MaNV)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Exec UPDATEINFO @hoten, @diachi, @manv, @sdt, @ngaysinh";
                cmd.Parameters.AddWithValue("@hoten", HoTen);
                cmd.Parameters.AddWithValue("@diachi", Diachi);
                cmd.Parameters.AddWithValue("@sdt", SDT);
                cmd.Parameters.AddWithValue("@ngaysinh", NgaySinh);
                cmd.Parameters.AddWithValue("@manv", MaNV);
                DBOpen();
                cmd.Connection = SqlCon;
                cmd.ExecuteNonQuery();
            }
            finally
            {
                DBClose();
                MyMessageBox msb = new MyMessageBox("Thay đổi và lưu thông tin thành công");
                msb.Show();
            }
        }
        public void LoadProfileImage(NhanVien nv)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "Select AnhDaiDien from TaiKhoan where MaNV = @manv";
                cmd.Parameters.AddWithValue("@manv", nv.MaNV);
                DBOpen();
                cmd.Connection = SqlCon;
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        nv.AnhDaiDien = Converter.ImageConverter.ConvertByteToBitmapImage((byte[])reader[0]);
                }
            }
            finally
            {
                DBClose();
            }
        }
        public void ChangeProfileImage_SaveToDB(NhanVien nv, string ID)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "Update TaiKhoan set AnhDaiDien = @anhdaidien where ID = @id";
                cmd.Parameters.AddWithValue("@id", ID);
                cmd.Parameters.AddWithValue("@anhdaidien", Converter.ImageConverter.ConvertImageToBytes(nv.AnhDaiDien));

                DBOpen();
                cmd.Connection = SqlCon;
                cmd.ExecuteNonQuery();
            }
            finally
            {
                DBClose();
            }
        }
        #region complementary methods
        public string getAccountIDFromTaiKhoan()
        {
            string ID = "";
            DataTable dt = new DataTable();
            dt = LoadInitialData("Select * from TaiKhoan");
            foreach (DataRow dr in dt.Rows)
            {
                ID = dr["Id"].ToString();
            }
            return ID;
        }
        #endregion
    }
}
