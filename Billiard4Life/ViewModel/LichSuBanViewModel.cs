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
using Billiard4Life.DataProvider;
using System.Security;

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
        private ObservableCollection<string> _ListStaff;
        public ObservableCollection<string> ListStaff
        {
            get => _ListStaff;
            set { _ListStaff = value; OnPropertyChanged(); }
        }
        private ObservableCollection<string> _ListStaffID;
        public ObservableCollection<string> ListStaffID
        {
            get => _ListStaffID;
            set { _ListStaffID = value; OnPropertyChanged(); }
        }
        private int _StaffIndexSelected;
        public int StaffIndexSelected
        {
            get => _StaffIndexSelected;
            set { 
                _StaffIndexSelected = value;
                OnPropertyChanged();
                ListViewDisplay(ListStaffID[StaffIndexSelected]);
            }
        }
        private string _DateBegin;
        public string DateBegin
        {
            get => _DateBegin;
            set
            {
                _DateBegin = value;
                OnPropertyChanged();
                ListViewDisplay("Tất cả");
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
                ListViewDisplay("Tất cả");
            }
        }
        public ICommand DetailCM { get; set; }
        public ICommand ExportCM { get; set; }
        public LichSuBanViewModel()
        {
            ListBill = new ObservableCollection<HoaDon>();
            ListStaff = new ObservableCollection<string>();
            ListStaffID = new ObservableCollection<string>();

            GetListStaff();
            StaffIndexSelected = ListStaffID.Count - 1;
            DateBegin = DateTime.Now.Month + "/1/" + DateTime.Now.Year;
            DateEnd = DateTime.Now.ToShortDateString();
            ListViewDisplay("Tất cả");

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
                ExportDetailBill();
            });
        }
        public void ListViewDisplay(string MaNV)
        {
            if (string.IsNullOrEmpty(DateBegin) || string.IsNullOrEmpty(DateEnd)) return;
            ListBill.Clear();
            ListBill = HoaDonDP.Flag.GetBillsFrom(DateBegin, DateEnd, "Tất cả", MaNV);
        }
        public void ExportDetailBill()
        {
            ChoseDate dt = new ChoseDate();
            dt.ShowDialog();
            string dtBegin = dt.GetDate().Item1;
            string dtEnd = dt.GetDate().Item2;

            ObservableCollection<HoaDon> ListBillEx = new ObservableCollection<HoaDon>();
            ListBillEx = HoaDonDP.Flag.GetBillsFrom(dtBegin, dtEnd, "Tất cả", "Tất cả", false);

            DateTime dt1 = DateTime.Parse(dtBegin);
            DateTime dt2 = DateTime.Parse(dtEnd);

            string filePath = "";
            string title = "Chi tiết hóa đơn từ " + dt1.Month + "-" + dt1.Day + "-" + dt1.Year + " đến " + dt2.Month + "-" + dt2.Day + "-" + dt2.Year;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel (*.xlsx)|*.xlsx";
            dialog.FileName = title;

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
                    x.Workbook.Properties.Title = title;

                    x.Workbook.Worksheets.Add("Sheet");

                    ExcelWorksheet ws = x.Workbook.Worksheets[0];

                    ws.Cells.Style.Font.Name = "Times New Roman";


                    string[] columnHeader = { "Số hóa đơn", "Tên khách hàng", "Số giờ", "Ngày hóa đơn", "Tên món", 
                        "Số lượng", "Đơn giá(VNĐ)", "Thành tiền(VNĐ)", "Tổng tiền(VNĐ)" };
                    ws.Column(1).Width = 12;
                    ws.Column(2).Width = 17;
                    ws.Column(3).Width = 12;
                    ws.Column(4).Width = 14;
                    ws.Column(5).Width = 17;
                    ws.Column(6).Width = 10;
                    ws.Column(7).Width = 15;
                    ws.Column(8).Width = 16;
                    ws.Column(9).Width = 16;

                    int countColumn = columnHeader.Count();
                    ws.Cells[1, 1].Value = title;
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

                        ObservableCollection<ChiTietHoaDon> detailBills = HoaDonDP.Flag.GetDetailBill(temp.SoHD);
                        foreach (ChiTietHoaDon cthd in detailBills)
                        {
                            row++;
                            ws.Cells[row, col].Value = cthd.TenSP;
                            ws.Cells[row, col + 1].Value = cthd.SoLuong;
                            ws.Cells[row, col + 2].Value = cthd.DonGia;
                            ws.Cells[row, col + 3].Value = cthd.ThanhTien;
                        }
                    }

                    Byte[] bin = x.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                };
                MyMessageBox msb = new MyMessageBox("Xuất file thành công!");
                msb.ShowDialog();
            }
        }
        public void GetListStaff()
        {
            List<Tuple<string, string>> staffs = new List<Tuple<string, string>>();
            staffs = NhanVienDP.Flag.GetIDAndNameStaff();
            foreach (Tuple<string, string> item in staffs)
            {
                ListStaffID.Add(item.Item1);
                ListStaff.Add(item.Item2);
            }
            ListStaff.Add("Tất cả");
            ListStaffID.Add("Tất cả");
        }
    }
}

