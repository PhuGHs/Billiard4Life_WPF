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
            DataTable dt = LoadInitialData("Select * from MENU WHERE DELETED = 0");
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
    public void UpdateKho(string MaMon, int SoLuong)
    {
        List<Tuple<string, float>> ctm = new List<Tuple<string, float>>();

        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "SELECT TenNL, SoLuong FROM CHITIETMON WHERE MaMon = @mamon";
        cmd.Parameters.AddWithValue("@mamon", MaMon);
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            ctm.Add(new Tuple<string, float>(reader.GetString(0), (float)reader.GetDouble(1)));
        }
        reader.Close();

        for (int i = 0; i < ctm.Count; i++)
        {
            cmd.CommandText = "UPDATE KHO SET TonDu = TonDu - @soluong WHERE TenSanPham = @ten";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@soluong", ctm[i].Item2);
            cmd.Parameters.AddWithValue("@ten", ctm[i].Item1);
            cmd.ExecuteNonQuery();
        }

        DBClose();
    }
    public void PayABill(Int16 soban, Decimal sum, string MaNV)
    {
        string SoHD = Flag.GetCurrentBillIDForThisTable(soban);

        try
        {
            DBOpen();
            if (SoHD != "0")
            {
                return;
            }
            else
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "INSERT INTO HOADON(MaNV, TriGia, SoBan, ThoiDiemTao, TrangThai) " +
                    "VALUES(@manv, @trigia, @soban, @thoidiemtao, @trangthai)";
                cmd.Parameters.AddWithValue("@trigia", sum);
                cmd.Parameters.AddWithValue("@manv", MaNV);
                cmd.Parameters.AddWithValue("@soban", soban);
                cmd.Parameters.AddWithValue("@thoidiemtao", DateTime.Now);
                cmd.Parameters.AddWithValue("@trangthai", "Chưa trả");
                DBOpen();
                cmd.Connection = SqlCon;

                cmd.ExecuteNonQuery();

                cmd.CommandText = "UPDATE BAN SET TrangThai = N'Đang sử dụng', SoHDHienTai = IDENT_CURRENT('HOADON') WHERE SoBan = " + soban;
                cmd.ExecuteNonQuery();
            }
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
            cmd.CommandText = "INSERT INTO MENU (MaMon, TenMon, AnhMonAn, Gia) VALUES (@MaMon, @TenMon, @AnhMonAn, @Gia)";
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
            cmd.CommandText = "UPDATE MENU SET Deleted = 1 WHERE MaMon = @mamon";
            cmd.Parameters.AddWithValue("@mamon", MaMon);

            cmd.Connection = SqlCon;
            cmd.ExecuteNonQuery();
        }
        finally
        {
            DBClose();
        }
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
            DataTable dt = LoadInitialData("Select * from KHO WHERE Xoa = 0");
            foreach (DataRow dr in dt.Rows)
            {
                string tensp = dr["TenSanPham"].ToString();
                float tondu = (float)Convert.ToDouble(dr["TonDu"]);
                //float MucBaoNhap = (float)Convert.ToDouble(dr["MucBaoNhap"]);
                string nhom = dr["NhomSanPham"].ToString();
                string donvi = dr["DonVi"].ToString();
                NLs.Add(new Kho(tensp, tondu, donvi, nhom));
            }
        }
        finally
        {
            DBClose();
        }
        return NLs;
    }
    public string AutoIDMenu()
    {
        string ID = "MA001";
        string temp = "";
        DBOpen();
        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "SELECT TOP 1 MaMon FROM MENU ORDER BY MaMon DESC";
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
            ID = "MA" + temp;
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
    public void RemoveIngredients(string MaMon)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Delete from CHITIETMON where MaMon = @mamon";
            cmd.Parameters.AddWithValue("@mamon", MaMon);
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
    public void Fill_CTHD(string SoHD, string MaMon, int SoLuong)
    {
        try
        {
            DBOpen();
            SqlCommand cmd_InsertDetail = new SqlCommand();
            cmd_InsertDetail.CommandText = "INSERT INTO CTHD(SoHD, MaMon, SoLuong) VALUES(@sohd, @mamon, @soluong)";
            cmd_InsertDetail.Parameters.AddWithValue("@mamon", MaMon);
            cmd_InsertDetail.Parameters.AddWithValue("@sohd", SoHD);
            cmd_InsertDetail.Parameters.AddWithValue("@soluong", SoLuong);
            DBOpen();
            cmd_InsertDetail.Connection = SqlCon;

            cmd_InsertDetail.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            UpdateCTHD(SoHD, MaMon, SoLuong);
        }
        finally
        {
            DBClose();
        }

    }
    public string GetCurrentBillIDForThisTable(int SoBan)
    {
        DBOpen();

        SqlCommand cmd = new SqlCommand();
        cmd.CommandText = "SELECT SoHDHienTai FROM BAN WHERE SoBan = " + SoBan;
        cmd.Connection = SqlCon;
        SqlDataReader reader = cmd.ExecuteReader();

        string SoHD = "0";
        if (reader.Read())
        {
            SoHD = reader.GetString(0);
        }

        DBClose();
        return SoHD;
    }
    public void UpdateCTHD(string SoHD, string MaMon, int SoLuong)
    {
        try
        {
            DBOpen();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "UPDATE CTHD " +
                              "SET SOLUONG = SOLUONG + @soluong " +
                              "WHERE SoHD = @sohd AND MaMon = @mamon";
            cmd.Parameters.AddWithValue("@mamon", MaMon);
            cmd.Parameters.AddWithValue("@sohd", SoHD);
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
