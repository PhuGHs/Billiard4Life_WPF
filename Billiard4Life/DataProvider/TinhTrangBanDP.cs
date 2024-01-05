using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Billiard4Life.Models;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using TinhTrangBan.Models;
using Table = TinhTrangBan.Models.Table;

namespace Billiard4Life.DataProvider
{
    public class TinhTrangBanDP : DataProvider
    {
        private static TinhTrangBanDP flag;
        public static TinhTrangBanDP Flag
        {
            get
            {
                if (flag == null) flag = new TinhTrangBanDP();
                return flag;
            }
            set
            {
                flag = value;
            }
        }
        public ObservableCollection<TinhTrangBan.Models.Table> GetTables()
        {
            ObservableCollection<TinhTrangBan.Models.Table> tables = new ObservableCollection<TinhTrangBan.Models.Table>();
            try
            {
                DataTable dt = LoadInitialData("Select * from BAN");
                foreach(DataRow dr in dt.Rows)
                {
                    int soban = Convert.ToInt32(dr["SoBan"]);
                    string trangthai = dr["TrangThai"].ToString();
                    Decimal giamotgio = Convert.ToDecimal(dr["GiaMotGio"]);
                    string loaiban = Convert.ToString(dr["LoaiBan"]);
                    
                    if(soban == null || trangthai == null || giamotgio == null || loaiban == null)
                    {
                        throw new Exception("Co Loi!!");
                    }

                    tables.Add(new Table { ID = soban, NumOfTable = "Bàn " + soban, KindOfTable = loaiban, Price = giamotgio}) ;
                }
            } catch (Exception ex)
            {
                MyMessageBox msb = new MyMessageBox(ex.Message);
                msb.Show();
            } finally
            {
                DBClose();
            }
            return tables;

        }
        public string LoadEachTableStatus(int ID)
        {
            string TableStatus = "";
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Select TrangThai from BAN where SoBan = @SoBan";
                cmd.Parameters.AddWithValue("@SoBan", ID);

                cmd.Connection = SqlCon;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TableStatus = reader.GetString(0);
                }
                return TableStatus;
            }
            finally
            {
                DBClose();
            }
        }
        public int LoadBill(int ID)
        {
            int bill = 0;
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Select SoHD from HOADON where SoBan = @SoBan and TrangThai = N'Chưa trả'";
                cmd.Parameters.AddWithValue("@SoBan", ID);

                cmd.Connection = SqlCon;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    bill = reader.GetInt16(0);
                }
                return bill;
            }
            finally
            {
                DBClose();
            }
        }
        public DateTime LoadBill_startTime(int ID)
        {
            DateTime time = DateTime.Now;
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "Select ThoiDiemTao from HOADON where SoBan = @SoBan and TrangThai = N'Chưa trả'";
                cmd.Parameters.AddWithValue("@SoBan", ID);

                cmd.Connection = SqlCon;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    time = reader.GetDateTime(0);
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
            return time;
        }
        public void UpdateTable(int ID, bool isEmpty)
        {
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;

                cmd.Connection = SqlCon;
                if (isEmpty)
                {
                    cmd.CommandText = "Update BAN set TrangThai = N'Có thể sử dụng', SoHDHienTai = 0 where SoBan = @SoBan";
                    cmd.Parameters.AddWithValue("@SoBan", ID);

                    cmd.ExecuteNonQuery();
                }
                else
                {
                    cmd.CommandText = "Update BAN set TrangThai = N'Đang được sử dụng' where SoBan = @SoBan";
                    cmd.Parameters.AddWithValue("@SoBan", ID);

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                DBClose();
            }
        }
        public bool UpdateBill(int BillID, Decimal tong, TimeSpan SoGio, string makm, string makh, string method, string soban)
        {
            string query = "UPDATE HOADON SET SoGio = @sogio, TriGia = @trigia, NgayHD = @ngayhd," +
                " TrangThai = N'Đã thanh toán', HinhThucThanhToan = @httt ";
            if (!string.IsNullOrEmpty(makm))
            {
                query += ", makm = '" + makm + "'";
            }
            if (!string.IsNullOrEmpty(makh))
            {
                query += ", makh = '" + makh + "'";
            }

            bool next = true;
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = SqlCon;

                cmd.CommandText = query + " where SoHD = @SoHD";
                cmd.Parameters.AddWithValue("@SoHD", BillID);
                cmd.Parameters.AddWithValue("@sogio", SoGio); 
                cmd.Parameters.AddWithValue("@trigia", tong);
                cmd.Parameters.AddWithValue("@httt", method);
                cmd.Parameters.AddWithValue("@ngayhd", DateTime.Now);

                cmd.ExecuteNonQuery();

                cmd.CommandText = "UPDATE BAN SET SoHDHienTai = 0 WHERE SoBan = " + soban;
                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                next = false;
            }
            finally
            {
                DBClose();
            }
            return next;
        }
        public TinhTrangBan.Models.Table AddNewTable(string ID, string LoaiBan)
        {
            string Gia;
            if (LoaiBan == "Líp") Gia = "60000";
            else Gia = "70000";

            DBOpen();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO BAN(SoBan, LoaiBan, GiaMotGio, TrangThai, SoHDHienTai) " +
                "VALUES(@id, @loaiban, @gia, N'Có thể sử dụng', 0)";
            cmd.Parameters.AddWithValue("@id", ID);
            cmd.Parameters.AddWithValue("@loaiban", LoaiBan);
            cmd.Parameters.AddWithValue("@gia", Gia);
            cmd.Connection = SqlCon;
            cmd.ExecuteNonQuery();
            DBClose();

            return new TinhTrangBan.Models.Table { ID = Convert.ToInt16(ID), NumOfTable = "Bàn " + ID, KindOfTable = LoaiBan, Price = Convert.ToDecimal(Gia) };
        }
        public int GetCustomerPoint(string MaKH)
        {
            int point = 0;

            DBOpen();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT SoDiem FROM KHACHHANG WHERE MaKH = @makh";
            cmd.Parameters.AddWithValue("@makh", MaKH);
            cmd.Connection = SqlCon;
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                point = reader.GetInt16(0);
            }

            DBClose();

            return point;
        }
        public void UpdateKhachHangAccumulatedPoint(int SoDiem, string MaKH)
        {
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = SqlCon;
                cmd.Connection = SqlCon;
                cmd.CommandText = "UPDATE KHACHHANG SET SoDiem = SoDiem + @sodiem WHERE MaKH = @makh";
                cmd.Parameters.AddWithValue("@sodiem", SoDiem);
                cmd.Parameters.AddWithValue("@makh", MaKH);

                cmd.ExecuteNonQuery();
            } catch (Exception ex)
            {
                MyMessageBox msb = new MyMessageBox(ex.Message);
                msb.Show();
            } finally
            {
                DBClose();
            }
        }
        public void SwitchTable(int ID, int BillID)
        {
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Connection = SqlCon;

                cmd.CommandText = "Update HOADON set SoBan = @SoBan where SoHD = @SoHD";
                cmd.Parameters.AddWithValue("@SoBan", ID);
                cmd.Parameters.AddWithValue("@SoHD", BillID);

                cmd.ExecuteNonQuery();

                cmd.CommandText = "UPDATE BAN SET SoHDHienTai = @SoHD WHERE SoBan = @SoBan";
                cmd.ExecuteNonQuery();
            }
            finally
            {
                DBClose();
            }
        }
        public void MinusCustomerPoint(string MaKH, int SoDiem)
        {
            try
            {
                DBOpen();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "UPDATE KHACHHANG " +
                    "SET SoDiem = SoDiem - @sodiem " +
                    "WHERE MaKH = @makh";
                cmd.Parameters.AddWithValue("@sodiem", SoDiem);
                cmd.Parameters.AddWithValue("@makh", MaKH);
                cmd.Connection = SqlCon;
                cmd.ExecuteNonQuery();
            } 
            catch (Exception ex)
            {
                MyMessageBox msb = new MyMessageBox("co loi");
                msb.ShowDialog();
            } finally
            {
                DBClose();
            }
        }
        public ObservableCollection<string> ListPhoneCustomer()
        {
            ObservableCollection<string> list = new ObservableCollection<string>();

            DBOpen();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT SDT, TenKH, MaKH FROM KHACHHANG";
            cmd.Connection = SqlCon;
            SqlDataReader reader = cmd.ExecuteReader();
            
            while (reader.Read())
            {
                string SDT = reader.GetString(0);
                list.Add(SDT.Substring(SDT.Length - 4) + "-" + reader.GetString(1) + "-" + reader.GetString(2));
            }
            reader.Close();

            DBClose();

            return list;
        }
        public void StopRecordTimeSpanPlayer(string sohd)
        {
            DBOpen();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "UPDATE HOADON SET NgayHD = @ngayhd WHERE SoHD = @sohd";
            cmd.Parameters.AddWithValue("@ngayhd", DateTime.Now);
            cmd.Parameters.AddWithValue("@sohd", sohd);
            cmd.Connection = SqlCon;
            cmd.ExecuteNonQuery();

            DBClose();
        }
        public string GetMaKH(string sdt)
        {
            try
            {
                return sdt.Substring(sdt.Length - 6);
            }
            catch 
            {
                return "";
            }
        }
    }
}
