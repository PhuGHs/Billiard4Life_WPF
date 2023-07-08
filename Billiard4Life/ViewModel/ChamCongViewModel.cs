using Billiard4Life.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Windows.Input;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using System.Configuration;
using System.Windows.Forms;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace Billiard4Life.ViewModel
{
    public class ChamCongViewModel : BaseViewModel
    {
        private ObservableCollection<string> _ListMonth;
        public ObservableCollection<string> ListMonth { get => _ListMonth; set { _ListMonth = value; OnPropertyChanged(); } }


        private string _MonthSelected;
        public string MonthSelected
        {
            get => _MonthSelected;
            set
            {
                _MonthSelected = value;
                OnPropertyChanged();
                ListViewDisplay();
                GetListDay();
                DaySelected = ListDay[ListDay.Count - 1];
            }
        }
        private ObservableCollection<string> _ListDay;
        public ObservableCollection<string> ListDay { get => _ListDay; set { _ListDay = value; OnPropertyChanged(); } }
        private string _DaySelected;
        public string DaySelected
        {
            get => _DaySelected;
            set
            {
                _DaySelected = value;
                OnPropertyChanged();
                GetListCheck();
            }
        }


        private ObservableCollection<NhanVienCC> _ListStaff;
        public ObservableCollection<NhanVienCC> ListStaff { get => _ListStaff; set { _ListStaff = value; OnPropertyChanged(); } }
        private ObservableCollection<ChamCong> _ListCheck;
        public ObservableCollection<ChamCong> ListCheck { get => _ListCheck; set { _ListCheck = value; OnPropertyChanged(); } }

        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;

        public ICommand CloseCM { get; set; }
        public ICommand ExportCM { get; set; }
        public ICommand SaveCM { get; set; }
        public ChamCongViewModel()
        {
            ListMonth = new ObservableCollection<string>();
            ListDay = new ObservableCollection<string>();
            ListStaff = new ObservableCollection<NhanVienCC>();
            ListCheck = new ObservableCollection<ChamCong>();

            GetListMonth();
            MonthSelected = DateTime.Now.Month + "/" + DateTime.Now.Year;
            ListViewDisplay();


            CloseCM = new RelayCommand<System.Windows.Window>((p) => { return true; }, (p) =>
            {
                if (p == null) return;
                p.Close();
            });

            #region // export command
            ExportCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                OpenConnect();

                string filePath = "";

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Excel (*.xlsx)|*.xlsx";
                dialog.FileName = "Bảng chấm công tháng " + DateTime.Now.Month + "-" + DateTime.Now.Year;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = dialog.FileName;

                    if (string.IsNullOrEmpty(filePath))
                    {
                        MyMessageBox mess = new MyMessageBox("Đường dẫn không hợp lệ!");
                        mess.ShowDialog();
                        return;
                    }

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;

                    using (ExcelPackage x = new ExcelPackage())
                    {
                        x.Workbook.Properties.Title = "Chấm công tháng " + GetMonth(MonthSelected) + "/" + DateTime.Now.Year;

                        x.Workbook.Worksheets.Add("Sheet");

                        ExcelWorksheet ws = x.Workbook.Worksheets[0];

                        ws.Cells.Style.Font.Name = "Times New Roman";


                        string[] columnHeader = { "Họ tên", "Ngày", "Số giờ", "Ghi chú" };
                        ws.Column(1).Width = 15;
                        ws.Column(2).Width = 13;
                        ws.Column(3).Width = 10;
                        ws.Column(4).Width = 15;

                        int countColumn = columnHeader.Count();
                        ws.Cells[1, 1].Value = "Bảng chấm công tháng " + GetMonth(MonthSelected) + "/" + DateTime.Now.Year;
                        ws.Cells[1, 1, 1, countColumn].Merge = true;
                        ws.Cells[1, 1, 1, countColumn].Style.Font.Bold = true;
                        ws.Cells[1, 1, 1, countColumn].Style.Font.Size = 16;
                        ws.Cells[1, 1, 1, countColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        int row = 2;
                        int col = 1;

                        foreach (string column in columnHeader)
                        {
                            var cell = ws.Cells[row, col];
                            cell.Value = column;
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            col++;
                        }

                        cmd.CommandText = "SELECT c.*, n.TenNV FROM CHITIETCHAMCONG AS c JOIN NHANVIEN AS n ON c.MaNV = n.MaNV WHERE MONTH(NgayCC) = " + GetMonth(MonthSelected) + " AND YEAR(NgayCC) = " + DateTime.Now.Year + " ORDER BY MaNV, NgayCC ASC";
                        cmd.Connection = sqlCon;
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            row++;
                            col = 1;
                            string ten = reader.GetString(4);
                            if (ten != ws.Cells[row - 1, 1].Value.ToString()) row++;
                            ws.Cells[row, col++].Value = ten;
                            ws.Cells[row, col++].Value = reader.GetDateTime(1).ToShortDateString();
                            ws.Cells[row, col++].Value = Math.Round(reader.GetDouble(2), 2).ToString();
                            ws.Cells[row, col++].Value = reader.GetString(3);
                        }
                        reader.Close();

                        row += 2;
                        ws.Cells[row, 2].Value = "Tổng số giờ";
                        ws.Cells[row, 2].Style.Font.Bold = true;
                        foreach (NhanVienCC nv in ListStaff)
                        {
                            row++;
                            col = 1;
                            ws.Cells[row, col++].Value = nv.HoTen;
                            ws.Cells[row, col++].Value = Math.Round(nv.TongSoGio, 2);
                        }

                        Byte[] bin = x.GetAsByteArray();
                        File.WriteAllBytes(filePath, bin);
                    };
                    MyMessageBox msb = new MyMessageBox("Xuất file thành công!");
                    msb.ShowDialog();
                }



                CloseConnect();
            });
            #endregion

            #region //save command
            SaveCM = new RelayCommand<object>((p) =>
            {
                foreach (ChamCong nv in ListCheck)
                {
                    if (string.IsNullOrEmpty(nv.SoGioCong)) return false;
                    if (!isFloat(nv.SoGioCong)) return false;
                }
                return true;
            }, (p) =>
            {
                OpenConnect();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM CHITIETCHAMCONG WHERE NgayCC = '" + DaySelected + "'";
                cmd.Connection = sqlCon;
                SqlDataReader reader = cmd.ExecuteReader();

                bool update = reader.Read();
                bool result = true;
                reader.Close();

                cmd.CommandText = "DELETE FROM CHITIETCHAMCONG WHERE NgayCC = '" + DaySelected + "'";
                cmd.ExecuteNonQuery();

                foreach (ChamCong nv in ListCheck)
                {
                    cmd.CommandText = "INSERT INTO CHITIETCHAMCONG VALUES('" + nv.MaNV + "', '" +
                    DaySelected + "', " + nv.SoGioCong + ", N'" + nv.GhiChu + "')";
                    int kq = cmd.ExecuteNonQuery();
                    if (kq == 0) result = false;
                }
                if (result)
                {
                    MyMessageBox msb = new MyMessageBox("Chấm công thành công!");
                    msb.ShowDialog();
                }
                else
                {
                    MyMessageBox msb = new MyMessageBox("Chấm công không thành công!");
                    msb.ShowDialog();
                }
                ListViewDisplay();

                CloseConnect();
            });
            #endregion
        }
        private void ListViewDisplay()
        {
            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM NHANVIEN ORDER BY MaNV ASC";
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();

            ListStaff.Clear();
            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string ten = reader.GetString(1);
                string chucvu = reader.GetString(2);
                string ft;
                if (reader.GetBoolean(3)) ft = "Full-time";
                else ft = "Part-time";

                ListStaff.Add(new NhanVienCC(ma, ten, chucvu, ft));
            }
            reader.Close();

            cmd.CommandText = "SELECT MaNV, SUM(SoGioCong) FROM CHITIETCHAMCONG WHERE MONTH(NgayCC) = " + GetMonth(MonthSelected) + " AND YEAR(NgayCC) = " + DateTime.Now.Year + " GROUP BY MaNV ORDER BY MaNV ASC";
            cmd.Connection = sqlCon;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string ma = reader.GetString(0);
                float tonggio = (float)reader.GetDouble(1);
                foreach (NhanVienCC nv in ListStaff)
                {
                    if (nv.MaNV == ma) nv.TongSoGio = tonggio;
                }
            }
            reader.Close();

            CloseConnect();
        }
        private void GetListMonth()
        {
            ListMonth.Clear();
            int month = 1;
            while (month <= DateTime.Now.Month)
            {
                ListMonth.Add(month.ToString() + "/" + DateTime.Now.Year.ToString());
                month++;
            }
        }
        private void GetListDay()
        {
            ListDay.Clear();
            if (GetMonth(MonthSelected) == DateTime.Now.Month.ToString())
            {
                int day = 1;
                while (day <= DateTime.Now.Day)
                {
                    ListDay.Add(GetMonth(MonthSelected) + "/" + day + "/" + DateTime.Now.Year);
                    day++;
                }
            }
            else
            {
                if (GetMonth(MonthSelected) == "1" || GetMonth(MonthSelected) == "3" || GetMonth(MonthSelected) == "5" || GetMonth(MonthSelected) == "7" || GetMonth(MonthSelected) == "8" || GetMonth(MonthSelected) == "10" || GetMonth(MonthSelected) == "12")
                {
                    int day = 1;
                    while (day <= 31)
                    {
                        ListDay.Add(GetMonth(MonthSelected) + "/" + day + "/" + DateTime.Now.Year);
                        day++;
                    }
                }
                else
                if (GetMonth(MonthSelected) == "4" || GetMonth(MonthSelected) == "6" || GetMonth(MonthSelected) == "9" || GetMonth(MonthSelected) == "11")
                {
                    int day = 1;
                    while (day <= 30)
                    {
                        ListDay.Add(GetMonth(MonthSelected) + "/" + day + "/" + DateTime.Now.Year);
                        day++;
                    }
                }
                else
                {
                    int count;
                    if (isLapYear(DateTime.Now.Year)) count = 29;
                    else count = 28;
                    int day = 1;
                    while (day <= count)
                    {
                        ListDay.Add(day.ToString() + "/" + GetMonth(MonthSelected) + "/" + DateTime.Now.Year);
                        day++;
                    }
                }
            }
        }
        private void GetListCheck()
        {
            OpenConnect();

            ListCheck.Clear();
            foreach (NhanVienCC nv in ListStaff)
            {
                ListCheck.Add(new ChamCong(nv.MaNV));
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM CHITIETCHAMCONG WHERE NgayCC = '" + DaySelected + "' ORDER BY MaNV ASC";
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string ma = reader.GetString(0);
                string ngay = reader.GetDateTime(1).ToShortDateString();
                string gio = reader.GetDouble(2).ToString();
                string note = reader.GetString(3);

                foreach (ChamCong nv in ListCheck)
                {
                    if (nv.MaNV == ma)
                    {
                        nv.Set(ngay, gio, note);
                    }
                }
            }
            reader.Close();

            cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT MaNV, NgayVaoLam FROM NHANVIEN ORDER BY MaNV ASC";
            cmd.Connection = sqlCon;
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string maNV = reader.GetString(0);
                string dateIn = reader.GetDateTime(1).ToShortDateString();
                foreach (ChamCong nv in ListCheck)
                {
                    if (nv.MaNV == maNV)
                    {
                        if (Convert.ToDateTime(dateIn) > Convert.ToDateTime(DaySelected))
                        {
                            nv.Set(DaySelected, "0", "Chưa vào làm");
                        }
                    }
                }
            }
            reader.Close();

            CloseConnect();
        }
        private string GetMonth(string dt)
        {
            string temp = "";
            int i = 0;
            while (dt[i] != '/')
            {
                temp += dt[i];
                i++;
            }
            return temp;
        }
        bool isLapYear(int nInput)
        {
            if ((nInput % 4 == 0 && nInput % 100 != 0) || nInput % 400 == 0)
            {
                return true;
            }
            return false;
        }
        bool isFloat(string s)
        {
            if (s[0] < 48 || s[0] > 57) return false;
            if (s[s.Length - 1] < 48 || s[s.Length - 1] > 57) return false;
            int count = 0;
            int i = 1;
            while (i < s.Length)
            {
                if (s[i] == '.') count++;
                if ((s[i] < 48 || s[i] > 57) && s[i] != '.') return false;
                i++;
            }
            if (count > 1) return false;
            return true;
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
