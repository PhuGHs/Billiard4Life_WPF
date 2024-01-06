using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using RestaurantManagement.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Billiard4Life.DataProvider;
using System.Drawing.Drawing2D;
using System.Data.SqlTypes;
using System.Collections.Generic;
using OfficeOpenXml.FormulaParsing.Excel.Functions;
using LiveCharts.Defaults;

namespace Billiard4Life.ViewModel
{
    public class ThongKeViewModel : BaseViewModel
    {
        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;
        private string _SumOfPaid;
        public string SumOfPaid { get => _SumOfPaid; set { _SumOfPaid = value; OnPropertyChanged(); } }
        private string _SumOfProfit;
        public string SumOfProfit { get => _SumOfProfit; set { _SumOfProfit = value; OnPropertyChanged(); } }
        private string _DateBegin;
        public string DateBegin
        {
            get => _DateBegin;
            set
            {
                _DateBegin = value;
                if (DateBegin == null || DateEnd == null) return;
                GetRevenue("Ngày");
                OnPropertyChanged();
            }
        }
        private string _DateEnd;
        public string DateEnd
        {
            get => _DateEnd;
            set
            {
                _DateEnd = value;
                if (DateBegin == null || DateEnd == null) return;
                GetRevenue("Ngày");
                OnPropertyChanged();
            }
        }
        private SeriesCollection _SeriesCollectionRevenue;
        public SeriesCollection SeriesCollectionRevenue
        {
            get { return _SeriesCollectionRevenue; }
            set { _SeriesCollectionRevenue = value; }
        }
        private SeriesCollection _SeriesCollectionCrowd;
        public SeriesCollection SeriesCollectionCrowd
        {
            get { return _SeriesCollectionCrowd; }
            set { _SeriesCollectionCrowd = value; }
        }
        private SeriesCollection _SeriesCollectionTypeTable;
        public SeriesCollection SeriesCollectionTypeTable
        {
            get { return _SeriesCollectionTypeTable; }
            set { _SeriesCollectionTypeTable = value; }
        }
        private SeriesCollection _SeriesCollectionStaff;
        public SeriesCollection SeriesCollectionStaff
        {
            get { return _SeriesCollectionStaff; }
            set { _SeriesCollectionStaff = value; }
        }
        public Func<double, string> Formatter { get; set; }
        public Func<double, string> CrowdFormatter { get; set; }
        private string _CrowdMonth;
        public string CrowdMonth { get => _CrowdMonth; 
            set { 
                _CrowdMonth = value;
                GetCrowd();
                OnPropertyChanged(); 
            } }
        private string _StaffMonth;
        public string StaffMonth { get => _StaffMonth; 
            set { 
                _StaffMonth = value;
                GetStaffRevenue();
                OnPropertyChanged(); 
            } }
        private string _TypeTableMonth;
        public string TypeTableMonth { get => _TypeTableMonth; 
            set { 
                _TypeTableMonth = value;
                GetPercentTypeTable();
                OnPropertyChanged(); 
            } }
        private ObservableCollection<string> _ListMonths;
        public ObservableCollection<string> ListMonths { get => _ListMonths; set { _ListMonths = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _ListCrowdMonths;
        public ObservableCollection<string> ListCrowdMonths { get => _ListCrowdMonths; set { _ListCrowdMonths = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _LabelsCrowd;
        public ObservableCollection<string> LabelsCrowd { get => _LabelsCrowd; set { _LabelsCrowd = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _LabelsRevenue;
        public ObservableCollection<string> LabelsRevenue { get => _LabelsRevenue; set { _LabelsRevenue = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _LabelsStaff;
        public ObservableCollection<string> LabelsStaff { get => _LabelsStaff; set { _LabelsStaff = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _Types;
        public ObservableCollection<string> Types { get => _Types; set { _Types = value; OnPropertyChanged(); } }
        private string _TypeSelected;
        public string TypeSelected { get => _TypeSelected; 
            set { 
                _TypeSelected = value; 
                if (TypeSelected == "Theo ngày")
                {
                    DateBeginVisible = "Visible";
                    DateEndVisible = "Visible";
                    ListTimeVisible = "Hidden";
                    DateBegin = DateTime.Now.Month + "/1/" + DateTime.Now.Year;
                    DateEnd = DateTime.Now.ToShortDateString();
                }
                if (TypeSelected == "Theo tháng")
                {
                    DateBeginVisible = "Hidden";
                    DateEndVisible = "Hidden";
                    ListTimeVisible = "Visible";
                    GetListTime("Tháng");
                    TimeSelected = ListTime[ListTime.Count - 1];
                }
                if (TypeSelected == "Theo năm")
                {
                    DateBeginVisible = "Hidden";
                    DateEndVisible = "Hidden";
                    ListTimeVisible = "Visible";
                    GetListTime("Năm");
                    TimeSelected = ListTime[ListTime.Count - 1];
                }
                OnPropertyChanged(); 
            } 
        }
        private string _PercentProOnRevenue;
        public string PercentProOnRevenue { get => _PercentProOnRevenue; set { _PercentProOnRevenue = value; OnPropertyChanged(); } }
        private string _DateBeginVisible;
        public string DateBeginVisible { get => _DateBeginVisible; set { _DateBeginVisible = value; OnPropertyChanged(); } }
        private string _DateEndVisible;
        public string DateEndVisible { get => _DateEndVisible; set { _DateEndVisible = value; OnPropertyChanged(); } }
        private string _ListTimeVisible;
        public string ListTimeVisible { get => _ListTimeVisible; set { _ListTimeVisible = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _ListTime;
        public ObservableCollection<string> ListTime { get => _ListTime; set { _ListTime = value; OnPropertyChanged(); } }
        private string _TimeSelected;
        public string TimeSelected { get => _TimeSelected; 
            set { 
                _TimeSelected = value;
                if (TypeSelected == "Theo tháng") GetRevenue("Tháng");
                if (TypeSelected == "Theo năm") GetRevenue("Năm");
                OnPropertyChanged(); 
            } 
        }

        public ThongKeViewModel()
        {
            OpenConnect();

            ListMonths = new ObservableCollection<string>();
            ListCrowdMonths = new ObservableCollection<string>();
            LabelsCrowd = new ObservableCollection<string>();
            LabelsRevenue = new ObservableCollection<string>();
            LabelsStaff = new ObservableCollection<string>();
            Types = new ObservableCollection<string>();
            ListTime = new ObservableCollection<string>();
            SeriesCollectionRevenue = new SeriesCollection();
            SeriesCollectionCrowd = new SeriesCollection();
            SeriesCollectionStaff = new SeriesCollection();
            SeriesCollectionTypeTable = new SeriesCollection();

            Formatter = value => String.Format("{0:0,0 VND}", Math.Round(value));
            CrowdFormatter = value => Math.Round(value).ToString("G");

            Types.Add("Theo ngày");
            Types.Add("Theo tháng");
            Types.Add("Theo năm");

            TypeSelected = "Theo ngày";
            DateBeginVisible = "Visible";
            DateEndVisible = "Visible";
            ListTimeVisible = "Hidden";
            CrowdMonth = StaffMonth = TypeTableMonth = DateTime.Now.Month + "/" + DateTime.Now.Year;

            GetListMonths();

            CloseConnect();
        }
        public void GetCrowd()
        {
            OpenConnect();

            SeriesCollectionCrowd.Clear();
            LabelsCrowd.Clear();

            if (CrowdMonth == null) return;

            string month = GetMonth(CrowdMonth);
            string year = GetYear(CrowdMonth);
            int[] crowd = new int[DateTime.DaysInMonth(DateTime.Now.Year, int.Parse(month))];
            for (int i = 0; i < crowd.Length; i++)
            {
                crowd[i] = 0;
                LabelsCrowd.Add("Ngày " + (i + 1).ToString());
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT DAY(NgayHD), COUNT(*) FROM HOADON WHERE MONTH(NgayHD) = " + month
                + " AND YEAR(NgayHD) = " + year + " AND TrangThai = N'Đã thanh toán' GROUP BY DAY(NgayHD)";
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int ngay = reader.GetInt32(0);
                int value = reader.GetInt32(1);

                crowd[ngay - 1] = value;
            }
            reader.Close();
            SeriesCollectionCrowd.Add(new LineSeries
            {
                Title = "Thông lượng",
                Values = new ChartValues<int>(crowd)
            });

            CloseConnect();
        }
        public void GetStaffRevenue()
        {
            OpenConnect();

            SeriesCollectionStaff.Clear();
            LabelsStaff.Clear();

            string month = GetMonth(StaffMonth);
            string year = GetYear(StaffMonth);
            string title = "";
            string strQuerry = "";

            if (month == "Tất cả")
            {
                strQuerry = "SELECT SUM(TriGia), hd.MaNV, TenNV FROM HOADON hd JOIN NHANVIEN nv ON hd.MaNV = nv.MaNV" +
                    " WHERE TrangThai = N'Đã thanh toán' GROUP BY hd.MaNV, TenNV";
                title = "Tất cả";
            }
            else
            {
                title = "T" + month;
                strQuerry = "SELECT SUM(TriGia), hd.MaNV, TenNV FROM HOADON hd JOIN NHANVIEN nv ON hd.MaNV = nv.MaNV" +
                    " WHERE MONTH(NgayHD) = " + month + " AND YEAR(NgayHD) = " + year + "" +
                    " AND TrangThai = N'Đã thanh toán' GROUP BY hd.MaNV, TenNV";
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strQuerry;
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();
            List<double> profit = new List<double>();

            while (reader.Read())
            {
                LabelsStaff.Add(reader.GetString(2));
                profit.Add(reader.GetSqlMoney(0).ToDouble());
            }
            reader.Close();
            SeriesCollectionStaff.Add(new ColumnSeries
            {
                Title = title,
                Values = new ChartValues<double>(profit)
            });

            CloseConnect();
        }
        public void GetPercentTypeTable()
        {
            OpenConnect();

            SeriesCollectionTypeTable.Clear();

            string month = GetMonth(TypeTableMonth);
            string year = GetYear(TypeTableMonth);
            string strQuerry = "";

            if (month == "Tất cả")
            {
                strQuerry = "SELECT DISTINCT COUNT(*), LoaiBan FROM HOADON hd JOIN BAN b ON hd.SoBan = b.SoBan " +
                    "WHERE hd.TrangThai = N'Đã thanh toán' GROUP BY LoaiBan";
            }
            else
            {
                strQuerry = "SELECT DISTINCT COUNT(*), LoaiBan FROM HOADON hd JOIN BAN b ON hd.SoBan = b.SoBan " +
                    "WHERE MONTH(NgayHD) = " + month + " AND YEAR(NgayHD) = " + year
                    + " AND hd.TrangThai = N'Đã thanh toán' GROUP BY LoaiBan";
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strQuerry;
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                SeriesCollectionTypeTable.Add(new PieSeries
                {
                    Title = reader.GetString(1),
                    Values = new ChartValues<ObservableValue> { new ObservableValue(reader.GetInt32(0)) },
                    DataLabels = true
                });
            }
            reader.Close();

            CloseConnect(); 
        }
        public void GetRevenue(string type)
        {
            OpenConnect();

            SeriesCollectionRevenue.Clear();
            LabelsRevenue.Clear();

            List<double> paid = new List<double>();
            List<double> profit = new List<double>();

            double sumPaid = 0, sumProfit = 0, percent = 0;
            if (type == "Ngày")
            {
                List<int> index = new List<int>();

                DateTime dt1 = DateTime.Parse(DateBegin);
                DateTime dt2 = DateTime.Parse(DateEnd);
                for (DateTime dt = dt1; dt <= dt2; dt = dt.AddDays(1))
                {
                    index.Add(dt.Day);
                    LabelsRevenue.Add(dt.Day.ToString());
                    paid.Add(0);
                    profit.Add(0);
                }

                // profit
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT DAY(NgayHD), SUM(TriGia) FROM HOADON WHERE TrangThai = N'Đã thanh toán' " +
                    "AND Convert(Date, NgayHD) >= '" + DateBegin + "' AND Convert(Date, NgayHD) <= '" + DateEnd + "' GROUP BY DAY(NgayHD)";
                cmd.Connection = sqlCon;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int dt = reader.GetInt32(0);
                    profit[index.IndexOf(dt)] = reader.GetSqlMoney(1).ToDouble();
                    sumProfit += profit[index.IndexOf(dt)];
                }
                reader.Close();

                // paid
                cmd.CommandText = "SELECT DAY(NgayNhap), SUM(DonGia * SoLuong) FROM CHITIETNHAP " +
                    "WHERE NgayNhap >= '" + DateBegin + "' AND NgayNhap <= '" + DateEnd + "' GROUP BY DAY(NgayNhap)";
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int dt = reader.GetInt32(0);
                    paid[index.IndexOf(dt)] = reader.GetDouble(1);
                    sumPaid += paid[index.IndexOf(dt)];
                }
                reader.Close();

                cmd.CommandText = "SELECT SUM(Gia * SoLuong) FROM CTHD ct JOIN HOADON hd ON ct.SoHD = hd.SoHD " +
                    "JOIN MENU m ON ct.MaMon = m.MaMon WHERE TrangThai = N'Đã thanh toán' " +
                    "AND Convert(Date, NgayHD) >= '" + DateBegin + "' AND Convert(Date, NgayHD) <= '" + DateEnd + "'";
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        percent = Convert.ToDouble(reader.GetDecimal(0));
                }
                reader.Close();
            }

            if (type == "Tháng")
            {
                if (TimeSelected == null) return;
                string month = GetMonth(TimeSelected);
                string year = GetYear(TimeSelected);

                for (int i = 1; i <= DateTime.DaysInMonth(int.Parse(year), int.Parse(month)); i++) 
                {
                    LabelsRevenue.Add(i.ToString());
                    paid.Add(0);
                    profit.Add(0);
                }
                paid.Add(0);
                profit.Add(0);

                //profit 
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT DAY(NgayHD), SUM(TriGia) FROM HOADON WHERE TrangThai = N'Đã thanh toán' " +
                    "AND MONTH(NgayHD) = " + month + " AND YEAR(NgayHD) = " + year + " GROUP BY DAY(NgayHD)";
                cmd.Connection = sqlCon;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    profit[reader.GetInt32(0)] = reader.GetSqlMoney(1).ToDouble();
                    sumProfit += profit[reader.GetInt32(0)];
                }
                reader.Close();

                //paid
                cmd.CommandText = "SELECT DAY(NgayNhap), SUM(DonGia * SoLuong) FROM CHITIETNHAP" +
                    " WHERE MONTH(NgayNhap) = " + month + " AND YEAR(NgayNhap) = " + year + " GROUP BY NgayNhap";
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    paid[reader.GetInt32(0)] = reader.GetDouble(1);
                    sumPaid += paid[reader.GetInt32(0)];
                }
                reader.Close();

                cmd.CommandText = "SELECT SUM(Gia * SoLuong) FROM CTHD ct JOIN HOADON hd ON ct.SoHD = hd.SoHD" +
                    " JOIN MENU m ON ct.MaMon = m.MaMon WHERE TrangThai = N'Đã thanh toán'" +
                    " AND MONTH(NgayHD) = " + month + " AND YEAR(NgayHD) = " + year;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        percent = (double) Math.Round(reader.GetDecimal(0));
                }
                reader.Close();
            }
            if (type == "Năm")
            {
                if (TimeSelected == null) return;
                string year = TimeSelected;
                for (int i = 1; i <= 12; i++)
                {
                    LabelsRevenue.Add(i.ToString() + "/" + year);
                    paid.Add(0);
                    profit.Add(0);
                }

                //profit 
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT MONTH(NgayHD), SUM(TriGia) FROM HOADON WHERE TrangThai = N'Đã thanh toán'" +
                    " AND YEAR(NgayHD) = " + year + " GROUP BY MONTH(NgayHD)";
                cmd.Connection = sqlCon;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    profit[reader.GetInt32(0)] = reader.GetSqlMoney(1).ToDouble();
                    sumProfit += profit[reader.GetInt32(0)];
                }
                reader.Close();

                //paid
                cmd.CommandText = "SELECT MONTH(NgayNhap), SUM(DonGia * SoLuong) FROM CHITIETNHAP" +
                    " WHERE YEAR(NgayNhap) = " + year + " GROUP BY MONTH(NgayNhap)";
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    paid[reader.GetInt32(0)] = reader.GetDouble(1);
                    sumPaid += paid[reader.GetInt32(0)];
                }
                reader.Close();

                cmd.CommandText = "SELECT SUM(Gia * SoLuong) FROM CTHD ct JOIN HOADON hd ON ct.SoHD = hd.SoHD" +
                    " JOIN MENU m ON ct.MaMon = m.MaMon WHERE TrangThai = N'Đã thanh toán' AND YEAR(NgayHD) = " + year;
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        percent = reader.GetDouble(0);
                }
                reader.Close();
            }
            SeriesCollectionRevenue.Add(new LineSeries
            {
                Title = "Thu",
                Values = new ChartValues<double>(profit)
            });
            SeriesCollectionRevenue.Add(new LineSeries
            {
                Title = "Chi",
                Values = new ChartValues<double>(paid)
            });
            SumOfPaid = String.Format("{0:0,0 VND}", Math.Round(sumPaid));
            SumOfProfit = String.Format("{0:0,0 VND}", Math.Round(sumProfit));
            PercentProOnRevenue = "  Sản phẩm\n/Doanh thu\n         " + Math.Round(percent * 100 / sumProfit, 2) + "%";

            CloseConnect();
        }
        public void GetListTime(string month)
        {
            ListTime.Clear();

            if (month == "Tháng")
            {
                for (int i = 6; i <= 12; i++)
                {
                    ListTime.Add(i.ToString() + "/" + (DateTime.Now.Year - 1));
                }
                for (int i = 1; i <= DateTime.Now.Month; i++)
                {
                    ListTime.Add(i.ToString() + "/" + DateTime.Now.Year);
                }
            }
            else
            {
                for (int i = DateTime.Now.Year - 3; i <= DateTime.Now.Year; i++)
                {
                    ListTime.Add(i.ToString());
                }
            }
        }
        public void GetListMonths()
        {
            ListCrowdMonths.Clear();
            ListMonths.Clear();
            for (int i = 6; i <= 12; i++)
            {
                ListMonths.Add(i.ToString() + "/" + (DateTime.Now.Year - 1));
                ListCrowdMonths.Add(i.ToString() + "/" + (DateTime.Now.Year - 1));
            }
            int currentMonth = DateTime.Now.Month;
            for (int i = 1; i <= currentMonth; i++)
            {
                ListMonths.Add(i.ToString() + "/" + DateTime.Now.Year);
                ListCrowdMonths.Add(i.ToString() + "/" + DateTime.Now.Year);
            }
            ListMonths.Add("Tất cả");
        }
        private string GetMonth(string dt)
        {
            int i = 0;
            string month = "";
            while (i < dt.Length && dt[i] != '/')
            {
                month += dt[i];
                i++;
            }
            return month;
        }

        private string GetYear(string dt)
        {
            return dt.Substring(dt.IndexOf("/") + 1);
        }
        
        private void OpenConnect()
        {
            sqlCon = new SqlConnection(strCon);
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
        }
        private void CloseConnect()
        {
            if (sqlCon.State == ConnectionState.Open)
            {
                sqlCon.Close();
            }
        }
    }
}