using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Billiard4Life.Models;
using Billiard4Life.View;
using RestaurantManagement.Models;
using TinhTrangBan.Models;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using OfficeOpenXml.ConditionalFormatting;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Billiard4Life.DataProvider;
using Org.BouncyCastle.Math;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Documment = iTextSharp.text.Document;
using System.IO;

namespace Billiard4Life.ViewModel
{
    public class TinhTrangBanViewModel : BaseViewModel
    {
        string connectstring = ConfigurationManager.ConnectionStrings["QuanLyNhaHang"].ConnectionString;
        public TinhTrangBanViewModel()
        {
            Tables = TinhTrangBanDP.Flag.GetTables();
            StatusOfTableCommand = new RelayCommand<Table>((p) => true, (p) => GetStatusOfTable(p.ID));
            GetPaymentCommand = new RelayCommand<Table>((p) => true, (p) => Payment());
            GetSwitchTableCommand = new RelayCommand<string>((p) => true, (p) => SwitchTable());
            LoadTableStatus();
            LoadEmptyTables();
        }
        #region attributes
        private ObservableCollection<Table> _tables = new ObservableCollection<Table>();
        private ObservableCollection<SelectedMenuItems> _selectedItems = new ObservableCollection<SelectedMenuItems>();
        private ObservableCollection<string> _emptytables = new ObservableCollection<string>();
        private string titleofbill = "";
        private decimal dec_sumofbill = 0;
        private string sumofbill = "0 VND";
        private string selectedtable = "";
        int IDofPaidTable = 0;
        bool isNull = false;
        #endregion
        #region properties
        public ObservableCollection<Table> Tables { get { return _tables; } set { _tables = value; OnPropertyChanged(); } }
        public ObservableCollection<SelectedMenuItems> SelectedItems { get { return _selectedItems; } set { _selectedItems = value; } }
        public ObservableCollection<string> EmptyTables { get { return _emptytables; } set { _emptytables = value; } }
        public string TitleOfBill
        {
            get { return titleofbill; }
            set { titleofbill = value; OnPropertyChanged(); }
        }
        public decimal Dec_sumofbill
        {
            get { return dec_sumofbill; }
            set { dec_sumofbill = value; OnPropertyChanged(); }
        }
        public string SumofBill
        {
            get { return sumofbill; }
            set { sumofbill = value; OnPropertyChanged(); }
        }
        public string SelectedTable
        {
            get { return selectedtable; }
            set { selectedtable = value; OnPropertyChanged(); }
        }
        #endregion
        #region commands
        public ICommand StatusOfTableCommand { get; set; }
        public ICommand GetPaymentCommand { get; set; }
        public ICommand GetSwitchTableCommand { get; set; }
        #endregion
        #region methods
        //public void LoadTables()
        //{
        //    _tables.Add(new Table { NumOfTable = "Bàn 1", ID = 1 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 2", ID = 2 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 3", ID = 3 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 4", ID = 4 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 5", ID = 5 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 6", ID = 6 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 7", ID = 7 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 8", ID = 8 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 9", ID = 9 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 10", ID = 10 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 11", ID = 11 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 12", ID = 12 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 13", ID = 13 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 14", ID = 14 });
        //    _tables.Add(new Table { NumOfTable = "Bàn 15", ID = 15 });

        //    Tables = _tables;
        //}
        public void LoadEmptyTables()
        {
            string numoftable;
            using (SqlConnection con = new SqlConnection(connectstring))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "Select SoBan from BAN where TrangThai = N'Có thể sử dụng'";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        numoftable = reader.GetInt16(0).ToString();
                        _emptytables.Add(numoftable);

                        EmptyTables = _emptytables;
                    }
                    catch
                    {
                        numoftable = "";
                    }
                }
                con.Close();
            }
        }
        public void LoadTableStatus()
        {
            string tablestatus;
            foreach (Table table in _tables)
            {
                tablestatus = TinhTrangBanDP.Flag.LoadEachTableStatus(table.ID);
                if (tablestatus == "Có thể sử dụng")
                {
                    table.Status = 0;
                    table.Coloroftable = "Green";
                }
                else
                {
                    table.Status = 1;
                    table.Coloroftable = "Red";
                }
            }
        }

        public void DisplayBill(int BillID)
        {
            SelectedItems.Clear();
            Dec_sumofbill = 0;
            string FoodName;
            decimal Price;
            int Quantity;
            using (SqlConnection con = new SqlConnection(connectstring))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "Select TenMon, SoLuong, Gia * SoLuong " +
                    "from CTHD inner join MENU on CTHD.MaMon = MENU.MaMon " +
                    "where CTHD.SoHD = @SOHD";
                cmd.Parameters.AddWithValue("@SOHD", BillID);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    try
                    {
                        FoodName = reader.GetString(0);
                        Quantity = reader.GetInt16(1);
                        Price = reader.GetDecimal(2);
                        SelectedMenuItems selected = new SelectedMenuItems(FoodName, Price, Quantity);
                        SelectedItems.Add(selected);

                        Dec_sumofbill += Price;
                        SumofBill = String.Format("{0:0,0 VND}", Dec_sumofbill);
                    }
                    catch
                    {
                        FoodName = "";
                        Quantity = 0;
                        Price = 0;
                    }
                }
                con.Close();
            }

        }
        public void GetStatusOfTable(int ID)
        {
            foreach (Table table in _tables)
            {
                if (table.ID == ID)
                {
                    if (table.Status == 0)
                    {
                        table.Coloroftable = "Green";
                        table.Status = 0;

                    }
                    else
                    {
                        TitleOfBill = table.NumOfTable;
                        table.Bill_ID = TinhTrangBanDP.Flag.LoadBill(table.ID);
                        DisplayBill(table.Bill_ID);
                        IDofPaidTable = table.ID;
                    }
                    break;
                }
            }
        }
        public void PrintBill(int BillID, int TableID)
        {
            using (SqlConnection con = new SqlConnection(connectstring))
            {
                con.Open();
                string strQuery = "Select TenMon, SoLuong, Gia * SoLuong " +
                    "from CTHD inner join MENU on CTHD.MaMon = MENU.MaMon " +
                    "where CTHD.SoHD = " + BillID;

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strQuery;

                SqlDataReader reader = cmd.ExecuteReader();
                List<string> ten = new List<string>();
                List<string> soluong = new List<string>();
                List<string> gia = new List<string>();

                while (reader.Read())
                {
                    ten.Add(reader.GetString(0));
                    soluong.Add(reader.GetInt16(1).ToString());
                    gia.Add(reader.GetDecimal(2).ToString());
                }

                if (ten.Count > 0)
                {
                    DisplayBill(BillID);
                    MyMessageBox yesno = new MyMessageBox("Bạn có muốn in hóa đơn?", true);
                    yesno.ShowDialog();
                    if (yesno.ACCEPT())
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "PDF (*.pdf)|*.pdf";
                        sfd.FileName = "Mã hóa đơn " + BillID.ToString() + " ngày " + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            if (File.Exists(sfd.FileName))
                            {
                                try
                                {
                                    File.Delete(sfd.FileName);
                                }
                                catch (IOException ex)
                                {
                                    MyMessageBox msb = new MyMessageBox("Đã có lỗi xảy ra!");
                                    msb.ShowDialog();
                                }
                            }
                            try
                            {
                                PdfPTable pdfTable = new PdfPTable(3);
                                pdfTable.DefaultCell.Padding = 3;
                                pdfTable.WidthPercentage = 100;
                                pdfTable.HorizontalAlignment = Element.ALIGN_MIDDLE;

                                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\TIMES.TTF", BaseFont.IDENTITY_H, true);
                                Font f = new Font(bf, 16, Font.NORMAL);

                                PdfPCell cell = new PdfPCell(new Phrase("Tên món", f));
                                pdfTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase("Số lượng", f));
                                pdfTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase("Giá", f));
                                pdfTable.AddCell(cell);
                                for (int i = 0; i < ten.Count; i++)
                                {
                                    pdfTable.AddCell(new Phrase(ten[i], f));
                                    pdfTable.AddCell(new Phrase(soluong[i], f));
                                    pdfTable.AddCell(new Phrase(gia[i], f));
                                }

                                using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                                {
                                    Document pdfDoc = new Document(PageSize.A4, 50f, 50f, 40f, 40f);
                                    PdfWriter.GetInstance(pdfDoc, stream);
                                    pdfDoc.Open();
                                    pdfDoc.Add(new Paragraph("                                                 HÓA ĐƠN ", f));
                                    pdfDoc.Add(new Paragraph("    "));
                                    pdfDoc.Add(new Paragraph("Số bàn: " + TableID.ToString() + "                                                                    Mã hóa đơn: " + BillID.ToString(), f));
                                    pdfDoc.Add(new Paragraph("Thời gian: " + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString() + " " + DateTime.Now.TimeOfDay.ToString(), f));
                                    pdfDoc.Add(new Paragraph("    "));
                                    pdfDoc.Add(pdfTable);
                                    pdfDoc.Add(new Paragraph("Tổng cộng:                                                                    " + SumofBill, f));
                                    pdfDoc.Add(new Paragraph("    "));
                                    pdfDoc.Add(new Paragraph("                                      HẸN GẶP LẠI QUÝ KHÁCH", f));
                                    pdfDoc.Close();
                                    stream.Close();
                                }

                                MyMessageBox mess = new MyMessageBox("In thành công!");
                                mess.ShowDialog();
                            }
                            catch (Exception ex)
                            {
                                MyMessageBox msb = new MyMessageBox("Đã có lỗi xảy ra!");
                                msb.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        DisplayBill(BillID);
                    }
                }
                else
                {
                    MyMessageBox mess = new MyMessageBox("Không tồn tại hóa đơn!");
                    mess.ShowDialog();
                }
            }
        }

        public void Payment()
        {
            foreach (Table table in _tables)
            {
                if (table.ID == IDofPaidTable)
                {
                    table.Coloroftable = "Green";
                    table.Status = 0;
                    TinhTrangBanDP.Flag.UpdateTable(table.ID, true);
                    TinhTrangBanDP.Flag.UpdateBillStatus(table.Bill_ID);

                    PrintBill(table.Bill_ID, table.ID);
                    Dec_sumofbill = 0;
                    SumofBill = String.Format("{0:0,0 VND}", Dec_sumofbill);
                    SelectedItems.Clear();
                    TitleOfBill = "";
                    MyMessageBox msb = new MyMessageBox("Đã thanh toán thành công!");
                    msb.Show();
                    break;
                }
            }
        }
        public void SwitchTable()
        {
            foreach (Table table in _tables)
            {
                if (table.ID == IDofPaidTable)
                {
                    if (SelectedTable == "")
                    {
                        MyMessageBox msb = new MyMessageBox("Vui lòng chọn bàn để chuyển đến trong danh sách bàn trống!");
                        msb.Show();
                        isNull = true;
                        break;
                    }
                    else
                    {
                        table.Coloroftable = "Green";
                        table.Status = 0;
                        TinhTrangBanDP.Flag.UpdateTable(table.ID, true);
                        TinhTrangBanDP.Flag.SwitchTable(int.Parse(SelectedTable), table.Bill_ID);
                        TinhTrangBanDP.Flag.UpdateTable(int.Parse(SelectedTable), false);

                        Dec_sumofbill = 0;
                        SumofBill = String.Format("{0:0,0 VND}", Dec_sumofbill);
                        SelectedItems.Clear();
                        TitleOfBill = "";
                        MyMessageBox msb = new MyMessageBox("Đã chuyển bàn thành công!");
                        msb.Show();
                        break;
                    }
                }
            }
            if (IDofPaidTable == 0)
            {
                MyMessageBox msb = new MyMessageBox("Vui lòng ấn chọn 1 bàn cần chuyển trước khi nhấn nút Chuyển bàn!");
                msb.Show();
                isNull = true;
            }
            foreach (Table table in _tables)
            {
                if (isNull)
                {
                    break;
                }
                else if (table.ID == int.Parse(SelectedTable))
                {
                    table.Coloroftable = "Red";
                    table.Status = 1;
                }
            }
            EmptyTables.Clear();
            LoadEmptyTables();
        }
#endregion
    }
}