﻿using Billiard4Life.DataProvider;
using Billiard4Life.Models;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Billiard4Life.ViewModel
{
    public class LichSuCaViewModel : BaseViewModel
    {
        private string _TimeStart;
        public string TimeStart
        {
            get => _TimeStart;
            set
            {
                _TimeStart = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<HoaDon> _ListBill;
        public ObservableCollection<HoaDon> ListBill { get => _ListBill; set { _ListBill = value; OnPropertyChanged(); } }
        private HoaDon _BillSelected;
        public HoaDon BillSelected
        {
            get => _BillSelected;
            set { _BillSelected = value; OnPropertyChanged(); }
        }
        private ObservableCollection<string> _PayMethods;
        public ObservableCollection<string> PayMethods
        {
            get => _PayMethods;
            set
            {
                _PayMethods = value;
                OnPropertyChanged();
            }
        }
        private string _PayMethodSelected;
        public string PayMethodSelected
        {
            get => _PayMethodSelected;
            set
            {
                _PayMethodSelected = value;
                ListViewDisplay(PayMethodSelected);
                TotalBill = HoaDonDP.Flag.TotalBillPerMethod(PayMethodSelected);
                OnPropertyChanged();
            }
        }
        private string _TotalBill;
        public string TotalBill
        {
            get => _TotalBill;
            set
            {
                _TotalBill = value;
                OnPropertyChanged();
            }
        }
        public ICommand DetailCM { get; set; }
        public ICommand ExportCM { get; set; }
        public LichSuCaViewModel()
        {
            //initialize
            PayMethods = new ObservableCollection<string>();
            ListBill = new ObservableCollection<HoaDon>();
            TimeStart = NhanVienDP.Flag.StaffOnline().Item2;
            GetPayMethods();
            ListViewDisplay("Tất cả");
            TotalBill = HoaDonDP.Flag.TotalBillPerMethod("Tất cả");
            DetailCM = new RelayCommand<object>((p) =>
            {
                if (BillSelected == null) return false;
                return true;
            }, (p) =>
            {
                Billiard4Life.View.ChiTietHoaDon cthd = new View.ChiTietHoaDon(BillSelected.SoHD);
                cthd.Show();
                return;
            });
            ExportCM = new RelayCommand<object>((p) =>
            {
                return true;
            }, (p) =>
            {
                ExportDetailShift();
            });
        }
        #region Method
        public void GetPayMethods()
        {
            PayMethods.Add("Tiền mặt");
            PayMethods.Add("Thẻ ngân hàng");
            PayMethods.Add("Chuyển khoản ngân hàng");
            PayMethods.Add("Chuyển MOMO");
            PayMethods.Add("Tất cả");
            PayMethodSelected = "Tất cả";
        }
        public void ListViewDisplay(string paymethod)
        {
            ListBill = HoaDonDP.Flag.GetBillsShift(paymethod);
        }
        public void ExportDetailShift()
        {
            string filePath = "";

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Excel (*.xlsx)|*.xlsx";
            dialog.FileName = "Chi tiết ca từ " + NhanVienDP.Flag.StaffOnline().Item2 + " đến " + DateTime.Now.ToString()
                + " của " + NhanVienDP.Flag.StaffOnline().Item1.MaNV + "-" + NhanVienDP.Flag.StaffOnline().Item1.HoTen;

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
                    x.Workbook.Properties.Title = "Chi tiết ca từ " + NhanVienDP.Flag.StaffOnline().Item2 + " đến " + DateTime.Now.ToString()
                + " của " + NhanVienDP.Flag.StaffOnline().Item1.MaNV + "-" + NhanVienDP.Flag.StaffOnline().Item1.HoTen;

                    x.Workbook.Worksheets.Add("Sheet");

                    ExcelWorksheet ws = x.Workbook.Worksheets[0];

                    ws.Cells.Style.Font.Name = "Times New Roman";


                    string[] columnHeader = { "Tiền mặt", "Thẻ ngân hàng", "Chuyển khoản ngân hàng", "Chuyển khoản MOMO"};

                    int countColumn = columnHeader.Count();
                    ws.Cells[1, 1].Value = "Tổng kết ca";
                    ws.Cells[1, 1, 1, countColumn].Merge = true;
                    ws.Cells[1, 1, 1, countColumn].Style.Font.Bold = true;
                    ws.Cells[1, 1, 1, countColumn].Style.Font.Size = 16;
                    ws.Cells[1, 1, 1, countColumn].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    int row = 2;
                    int col = 1;
                    int maxRow = 2;

                    foreach (string column in columnHeader)
                    {
                        var cell = ws.Cells[row, row, col, col + 2];
                        cell.Merge = true;
                        cell.Value = column;
                        cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        ObservableCollection<HoaDon> Bills = new ObservableCollection<HoaDon>();
                        Bills = HoaDonDP.Flag.GetBillsShift(column);

                        foreach (HoaDon hd in Bills)
                        {
                            ws.Cells[row, col].Value = "Bàn " + hd.SoBan;
                            ws.Cells[row, col + 1].Value = hd.NgayHD;
                            ws.Cells[row, col + 2].Value = hd.TriGia;
                            row++;
                        }
                        ws.Cells[row, col].Value = "Tổng";
                        ws.Cells[row, col + 1].Value = HoaDonDP.Flag.TotalBillPerMethod(column);

                        col += 3;
                        if (row > maxRow) { maxRow = row; }
                        row = 2;
                    }
                    maxRow++;
                    ws.Cells[maxRow, 1].Value = "Tổng tất cả:";
                    ws.Cells[maxRow, 2].Value = HoaDonDP.Flag.TotalBillPerMethod("Tất cả");

                    Byte[] bin = x.GetAsByteArray();
                    File.WriteAllBytes(filePath, bin);
                };
                MyMessageBox msb = new MyMessageBox("Xuất file thành công!");
                msb.ShowDialog();
            }
        }
        #endregion
    }
}
