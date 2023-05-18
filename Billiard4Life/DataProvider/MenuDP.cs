using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Billiard4Life.Models;
using TinhTrangBan.Models;

namespace Billiard4Life.DataProvider;

public class MenuDP : DataProvider
{
    private static MenuDP flag;
    public static MenuDP Flag
    {
        get
        {
            if (flag == null) flag = new MenuDP();
            return flag;
        }
        set
        {
            flag = value;
        }
    }
    public async Task<ObservableCollection<MenuItem>> ConvertToCollection()
    {
        ObservableCollection<MenuItem> menuItems = new ObservableCollection<MenuItem>();
        try
        {
            DataTable dt = LoadInitialData("Select * from MENU");
            foreach (DataRow row in dt.Rows)
            {
                string maMon = row["MaMon"].ToString();
                string tenMon = row["TenMon"].ToString();
                BitmapImage anhMon = Converter.ImageConverter.ConvertByteToBitmapImage((byte[])row["AnhMonAn"]);
                Decimal gia = (Decimal)row["Gia"];

                menuItems.Add(new MenuItem(maMon, tenMon, gia, anhMon));
            }
        }
        finally
        {
            DBClose();
        }
        return menuItems;
    }

    public void InformChef(string maMon, int soban, int soluong)
    {
        try
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Exec Inform_Chef_PD @mamon, @soban, @soluong, @ngaycb, @trangthai, @trangthaiban";
            cmd.Parameters.AddWithValue("@mamon", maMon);
            cmd.Parameters.AddWithValue("@soban", soban);
            cmd.Parameters.AddWithValue("@soluong", soluong);
            cmd.Parameters.AddWithValue("@ngaycb", DateTime.Now);
            cmd.Parameters.AddWithValue("@trangthai", "Đang chế biến");
            cmd.Parameters.AddWithValue("@trangthaiban", "Đang được sử dụng");
            DBOpen();
            cmd.Connection = SqlCon;
            cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
    }


    public void PayABill(Int16 soban, Decimal sum, string MaNV)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Exec PAY_A_BILL_PD @trigia, @manv, @soban, @ngayHD, @trangthai";
            cmd.Parameters.AddWithValue("@trigia", sum);
            cmd.Parameters.AddWithValue("@manv", "NV01");
            cmd.Parameters.AddWithValue("@soban", soban);
            cmd.Parameters.AddWithValue("@ngayHD", DateTime.Now);
            cmd.Parameters.AddWithValue("@trangthai", "Chưa trả");
            DBOpen();
            cmd.Connection = SqlCon;

            cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
    }

    public ObservableCollection<Table> GetTables()
    {
        ObservableCollection<Table> tables = new ObservableCollection<Table>();
        try
        {
            DataTable dt = LoadInitialData("Select * from BAN");
            int tinhtrang;
            foreach (DataRow dr in dt.Rows)
            {
                if (String.Compare(dr["TrangThai"].ToString(), "Có thể sử dụng") == 0) tinhtrang = 1;
                else tinhtrang = 0;
                tables.Add(new Table { NumOfTable = dr["SoBan"].ToString(), Status = tinhtrang });
            }
        }
        finally
        {
            DBClose();
        }
        return tables;
    }

    public void AddDish(MenuItem x)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO MENU VALUES (@MaMon, @TenMon, @Gia, @AnhMonAn)";
            cmd.Parameters.AddWithValue("@MaMon", x.ID);
            cmd.Parameters.AddWithValue("@TenMon", x.FoodName);
            cmd.Parameters.AddWithValue("@AnhMonAn", Converter.ImageConverter.ConvertImageToBytes(x.FoodImage));
            cmd.Parameters.AddWithValue("@Gia", x.Price);

            cmd.Connection = SqlCon;
            cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
    }

    public void RemoveDish(string MaMon)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Delete from MENU where MaMon = @mamon";
            cmd.Parameters.AddWithValue("@mamon", MaMon);

            cmd.Connection = SqlCon;
            cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
    }

    public MenuItem GetDishInfo(string MaMon)
    {
        MenuItem X = null;
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Select * from MENU where MaMon = @mamon";
            cmd.Parameters.AddWithValue("@mamon", MaMon);
            cmd.Connection = SqlCon;

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                X = new MenuItem(reader.GetString(0), reader.GetString(1), reader.GetDecimal(2), Converter.ImageConverter.ConvertByteToBitmapImage((byte[])reader[3]));
            }
        }
        finally
        {
            DBClose();
        }
        return X;
    }

    public void EditDishInfo(MenuItem item)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Update MENU set TenMon = @tenmon, AnhMonAn = @anh, Gia = @gia where MaMon = @mamon ";
            cmd.Parameters.AddWithValue("@mamon", item.ID);
            cmd.Parameters.AddWithValue("@anh", Converter.ImageConverter.ConvertImageToBytes(item.FoodImage));
            cmd.Parameters.AddWithValue("@tenmon", item.FoodName);
            cmd.Parameters.AddWithValue("@gia", item.Price);
            cmd.Connection = SqlCon;

            cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
    }

    public ObservableCollection<Kho> GetIngredients()
    {
        ObservableCollection<Kho> NLs = new ObservableCollection<Kho>();
        try
        {
            DataTable dt = LoadInitialData("Select * from KHO");
            foreach (DataRow dr in dt.Rows)
            {
                string tensp = dr["TenSanPham"].ToString();
                float tondu = (float)Convert.ToDouble(dr["TonDu"]);
                string donvi = dr["DonVi"].ToString();
                string dongia = dr["DonGia"].ToString();
                NLs.Add(new Kho(tensp, tondu, donvi, dongia));
            }
        }
        finally
        {
            DBClose();
        }
        return NLs;
    }

    public ObservableCollection<ChiTietMon> getSumIngredients(ObservableCollection<SelectedMenuItem> arr)
    {
        ObservableCollection<ChiTietMon> ctm = new ObservableCollection<ChiTietMon>();
        try
        {
            string fullQuery = string.Empty;
            string outerquery = "select T.TenNL, SUM(T.SoLuong) as Tong from ( ";
            string endquery = " ) as T group by T.TenNL";
            string query = "select TenNL, SoLuong from CHITIETMON where ";
            query += $"MaMon = '{arr[0].ID}'";
            if (arr.Count > 1)
            {
                for (int i = 1; i < arr.Count; i++)
                {
                    query += $" or MaMon = '{arr[i].ID}'";
                }
            }
            fullQuery = outerquery + query + endquery;
            DataTable dt = LoadInitialData(fullQuery);
            foreach (DataRow dr in dt.Rows)
            {
                ctm.Add(new ChiTietMon(dr["TenNL"].ToString(), (float)Convert.ToDouble(dr["Tong"])));
            }
        }
        finally
        {
            DBClose();

        }
        return ctm;
    }

    public ObservableCollection<ChiTietMon> GetIngredientsForDish(string MaMon)
    {
        ObservableCollection<ChiTietMon> Ingredients = new ObservableCollection<ChiTietMon>();
        try
        {
            DataTable dt = LoadInitialData($"Select * from CHITIETMON where MaMon = '{MaMon}'");
            foreach (DataRow dr in dt.Rows)
            {
                Ingredients.Add(new ChiTietMon(dr["TenNL"].ToString(), dr["MaMon"].ToString(), (float)Convert.ToDouble(dr["SoLuong"])));
            }
        }
        finally
        {
            DBClose();
        }
        return Ingredients;
    }

    public void SaveIngredients(ChiTietMon ctm)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Insert into CHITIETMON values (@mamon, @tennl, @soluong)";
            cmd.Parameters.AddWithValue("@mamon", ctm.MaMon);
            cmd.Parameters.AddWithValue("@tennl", ctm.TenNL);
            cmd.Parameters.AddWithValue("@soluong", ctm.SoLuong);
            cmd.Connection = SqlCon;

            cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
    }

    public void UpdateIngredients_Kho(Kho item)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Update KHO set TonDu = @tondu where TenSanPham = @tensanpham";
            cmd.Parameters.AddWithValue("@tondu", item.TonDu);
            cmd.Parameters.AddWithValue("@tensanpham", item.TenSanPham);

            cmd.Connection = SqlCon;
            cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
    }
    public int UpdateIngredients(ChiTietMon ctm)
    {
        int n = 0;
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Update CHITIETMON SET SoLuong = @soluong where TenNL = @tennl and MaMon = @mamon";
            cmd.Parameters.AddWithValue("@soluong", ctm.SoLuong);
            cmd.Parameters.AddWithValue("@tennl", ctm.TenNL);
            cmd.Parameters.AddWithValue("@mamon", ctm.MaMon);
            cmd.Connection = SqlCon;

            n = cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
        return n;
    }
    public void RemoveIngredients(ChiTietMon ctm)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Delete from CHITIETMON where MaMon = @mamon and TenNL = @tennl";
            cmd.Parameters.AddWithValue("@mamon", ctm.MaMon);
            cmd.Parameters.AddWithValue("@tennl", ctm.TenNL);
            cmd.Connection = SqlCon;

            cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
    }

    #region complementary functions
    public Decimal Calculate_Sum(Int16 Soban)
    {
        Decimal sum = 0;
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Exec GET_SUM_OF_PRICE_PD @soban";
            cmd.Parameters.AddWithValue("@soban", Soban);
            cmd.Connection = SqlCon;

            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                sum = reader.GetDecimal(0);
            }
            reader.Close();
            return sum;
        }
        finally
        {
            DBClose();
        }
    }
    public void Fill_CTHD(string MaMon, int SoLuong)
    {
        try
        {
            DBOpen();
            SqlCommand cmd_InsertDetail = new SqlCommand();
            cmd_InsertDetail.CommandText = "Exec INSERT_DETAIL_PD @mamon, @soluong";
            cmd_InsertDetail.Parameters.AddWithValue("@mamon", MaMon);
            cmd_InsertDetail.Parameters.AddWithValue("@soluong", SoLuong);
            DBOpen();
            cmd_InsertDetail.Connection = SqlCon;

            cmd_InsertDetail.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            UpdateCTHD(MaMon, SoLuong);
        }
        finally
        {
            DBClose();
        }

    }
    public void UpdateCTHD(string MaMon, int SoLuong)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "UPDATE CTHD " +
                              "SET SOLUONG = @soluong " +
                              "WHERE SoHD = (SELECT IDENT_CURRENT('HOADON')) AND MaMon = @mamon";
            cmd.Parameters.AddWithValue("@mamon", MaMon);
            cmd.Parameters.AddWithValue("@soluong", SoLuong);

            cmd.Connection = SqlCon;
            cmd.ExecuteNonQuery();

        }
        finally
        {
            DBClose();
        }
    }
    #endregion
}
