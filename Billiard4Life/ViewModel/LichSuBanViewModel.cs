using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using Microsoft.Office.Interop.Excel;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using LicenseContext = OfficeOpenXml.LicenseContext;
using System.IO;
using System.Windows;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using DataTable = System.Data.DataTable;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography;
using MaterialDesignThemes.Wpf;
using System.Windows.Documents;
using System.Security.Permissions;
using Billiard4Life.Models;
using Billiard4Life.CustomMessageBox;
using iTextSharp.text;

namespace Billiard4Life.ViewModel
{
    public class LichSuBanViewModel : BaseViewModel
    {
        private ObservableCollection<HoaDon> _ListBill;
        public ObservableCollection<HoaDon> ListBill { get => _ListBill; set { _ListBill = value; OnPropertyChanged(); } }
        private HoaDon _Selected;
        public HoaDon Selected
        {
            get => _Selected;
            set { _Selected = value; OnPropertyChanged(); }
        }

        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;
        private string _DateBegin;
        public string DateBegin
        {
            get => _DateBegin;
            set
            {
                _DateBegin = value;
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
                OnPropertyChanged();
                ListViewDisplay();
                OnPropertyChanged();
            }
        }
        public ICommand DetailCM { get; set; }
        public ICommand ExportCM { get; set; }
        public LichSuBanViewModel()
        {
            OpenConnect();

            ListBill = new ObservableCollection<HoaDon>();
            DateBegin = DateTime.Now.Month + "/1/" + DateTime.Now.Year;
            DateEnd = DateTime.Now.ToShortDateString();

            DetailCM = new RelayCommand<object>((p) => 
            {
                if (Selected == null) return false;
                return true; 
            }, (p) =>
            {
                Billiard4Life.View.ChiTietHoaDon cthd = new View.ChiTietHoaDon(Selected.SoHD);
                cthd.Show();
                return;
            });
            ExportCM = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                OpenConnect();

                ChoseDate dt = new ChoseDate();
                dt.ShowDialog();
                string dtBegin = "", dtEnd = "";
                string s = dt.GetDate();
                int i = 0;
                while (s[i] != ' ') dtBegin += s[i++];
                while (s[i] != 'M') i++;
                i += 2;
                while (s[i] != ' ') dtEnd += s[i++];

                List<HoaDon> ListBillEx = new List<HoaDon>();

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT h.*, kh.TenKH FROM HOADON AS h LEFT JOIN KHACHHANG AS kh ON h.MaKH = kh.MaKH WHERE NgayHD >= '" + dtBegin + "' AND NgayHD <= '" + dtEnd + "' AND TrangThai = N'Đã thanh toán'";
                cmd.Connection = sqlCon;
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
                    string tenkh = "";
                    if (!reader.IsDBNull(9))
                        tenkh = reader.GetString(9);
                    else tenkh = "Khách vãng lai";
                    string makm = "";
                    if (!reader.IsDBNull(5))
                        makm = reader.GetString(5);

                    ListBillEx.Add(new HoaDon(ma, sogio, trigia, manv, soban, ngayhd, tenkh, makm));
                }
                reader.Close();

                DateTime dt1 = DateTime.Parse(dtBegin);
                DateTime dt2 = DateTime.Parse(dtEnd);

                string filePath = "";

                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Excel (*.xlsx)|*.xlsx";
                dialog.FileName = "Chi tiết hóa đơn từ " + dt1.Month + "-" + dt1.Day + "-" + dt1.Year + " đến " + dt2.Month + "-" + dt2.Day + "-" + dt2.Year;

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

                    using (ExcelPackage x = new ExcelPackage())
                    {
                        x.Workbook.Properties.Title = "Chi tiết hóa đơn từ " + dtBegin + " đến " + dtEnd;

                        x.Workbook.Worksheets.Add("Sheet");

                        ExcelWorksheet ws = x.Workbook.Worksheets[0];

                        ws.Cells.Style.Font.Name = "Times New Roman";


                        string[] columnHeader = { "Số hóa đơn", "Tên khách hàng", "Số giờ", "Ngày hóa đơn", "Tên món", "Số lượng", "Đơn giá(VNĐ)", "Thành tiền(VNĐ)", "Tổng tiền(VNĐ)"};

                        int countColumn = columnHeader.Count();
                        ws.Cells[1, 1].Value = "Chi tiết hóa đơn từ " + dtBegin + " đến " + dtEnd;
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

                        foreach (HoaDon temp in ListBillEx)
                        {
                            col = 1;
                            row++;
                            ws.Cells[row, col++].Value = temp.SoHD;
                            ws.Cells[row, col++].Value = temp.TenKH;
                            ws.Cells[row, col++].Value = temp.SoGio;
                            ws.Cells[row, col++].Value = temp.NgayHD;
                            ws.Cells[row, countColumn].Value = temp.TriGia;

                            cmd.CommandText = "SELECT ct.*, m.TenMon, m.Gia FROM CTHD AS ct JOIN MENU AS m ON ct.MaMon = m.MaMon WHERE SoHD = " + temp.SoHD;
                            cmd.Connection = sqlCon;
                            reader = cmd.ExecuteReader();

                            while (reader.Read()) 
                            {
                                row++;
                                string ten = reader.GetString(4);
                                string soluong = reader.GetInt16(3).ToString();
                                string gia = reader.GetSqlMoney(4).ToString();
                                float tien = float.Parse(soluong) * float.Parse(gia);

                                ws.Cells[row, col++].Value = ten;
                                ws.Cells[row, col++].Value = soluong;
                                ws.Cells[row, col++].Value = gia;
                                ws.Cells[row, col++].Value = tien;
                            }
                            reader.Close();
                        }

                        Byte[] bin = x.GetAsByteArray();
                        File.WriteAllBytes(filePath, bin);
                    };
                    MyMessageBox msb = new MyMessageBox("Xuất file thành công!");
                    msb.ShowDialog();
                }
                CloseConnect();
            });

            CloseConnect();
        }
        public void ListViewDisplay()
        {
            OpenConnect();

            if (string.IsNullOrEmpty(DateBegin) || string.IsNullOrEmpty(DateEnd)) return;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT h.*, kh.TenKH FROM HOADON AS h LEFT JOIN KHACHHANG AS kh ON h.MaKH = kh.MaKH WHERE NgayHD >= '" + DateBegin + "' AND NgayHD <= '" + DateEnd + "' AND TrangThai = N'Đã thanh toán'";
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();

            ListBill.Clear();
            while (reader.Read())
            {
                string ma = reader.GetInt16(0).ToString();
                string sogio = reader.GetTimeSpan(1).TotalMinutes.ToString();
                sogio = ConvertTime(int.Parse(sogio));
                string trigia = reader.GetSqlMoney(2).ToString();
                string manv = reader.GetString(3);
                string soban = reader.GetInt16(6).ToString();
                string ngayhd = reader.GetDateTime(7).ToShortDateString();
                string tenkh = "";
                if (!reader.IsDBNull(9))
                    tenkh = reader.GetString(9);
                else tenkh = "Khách vãng lai";
                string makm = "";
                if (!reader.IsDBNull(5))
                    makm = reader.GetString(5);

                ListBill.Add(new HoaDon(ma, sogio, trigia, manv, soban, ngayhd, tenkh, makm));
            }

            CloseConnect();
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
        private string ConvertTime(int h)
        {
            int gio = h / 60;
            int du = h % 60;
            return gio.ToString() + "h" + du.ToString() + "m";
        }
    }
}

