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
using System.Windows.Data;
using System.ComponentModel;
using KhuyenMai = Billiard4Life.Models.KhuyenMai;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;

namespace Billiard4Life.ViewModel
{
    public class TinhTrangBanViewModel : BaseViewModel
    {
        string connectstring = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        public TinhTrangBanViewModel()
        {
            Tables = TinhTrangBanDP.Flag.GetTables();
            ListPhoneCustomer = new ObservableCollection<string>();
            GetListPhoneCustomer();
            Update();
            LoadTableStatus();
            LoadEmptyTables();
            LoadOptions();
            _tables_view = new CollectionViewSource();
            _tables_view.Source = Tables;
            StatusOfTableCommand = new RelayCommand<Table>((p) => true, (p) =>
            {
                CustomerPhoneNumber = "";
                GetStatusOfTable(p.ID);
            });
            GetPaymentCommand = new RelayCommand<Table>((p) =>
            {
                if (string.IsNullOrEmpty(TitleOfBill)) return false;
                return true;
            }, (p) =>
            {
                if (PaymentMethodSelected == null)
                {
                    MyMessageBox msb = new MyMessageBox("Vui lòng chọn phương thức thanh toán!");
                    msb.ShowDialog();
                }
                else
                {
                    Payment();
                }
            });
            GetSwitchTableCommand = new RelayCommand<string>((p) => true, (p) => SwitchTable());
            AddNewTableCM = new RelayCommand<object>((p) => true, (p) =>
            {
                ChiTietBan newTable = new ChiTietBan();
                newTable.DataContext = this;
                AddTableID = Convert.ToString(Tables.Count() + 1);
                newTable.ShowDialog();
            });
            AddTableCM = new RelayCommand<object>((p) =>
            {
                if (AddTableType == null) return false;
                return true;
            }, (p) =>
            {
                Tables.Add(TinhTrangBanDP.Flag.AddNewTable(AddTableID, AddTableType));
                AddTableID = Convert.ToString(Tables.Count() + 1);
                MyMessageBox msb = new MyMessageBox("Thêm thành công!");
                msb.ShowDialog();
            });
            SortingFeature_Command = new RelayCommand<object>((p) => true, (p) =>
            {
                TableView.Filter = item =>
                {
                    if (SelectedOption == "Tất cả")
                    {
                        return true;
                    }
                    return ((Table)item).KindOfTable == selectedOption;
                };
            });
            PrintBillCM = new RelayCommand<Table>((p) =>
            {
                if (string.IsNullOrEmpty(TitleOfBill)) return false;
                return true;
            }, (p) =>
            {
                TimeStopRecord = DateTime.Now;
                TinhTrangBanDP.Flag.StopRecordTimeSpanPlayer(MenuDP.Flag.GetCurrentBillIDForThisTable(IDofPaidTable));
                PrintBill(Convert.ToInt16(MenuDP.Flag.GetCurrentBillIDForThisTable(IDofPaidTable)), IDofPaidTable);
            });
            OpenPaymentCM = new RelayCommand<Table>((p) =>
            {
                if (string.IsNullOrEmpty(TitleOfBill)) return false;
                return true;
            }, (p) =>
            {
                var window = new ThanhToan();
                window.DataContext = this;
                window.Show();
            });
        }
        #region attributes
        private ObservableCollection<Table> _tables = new ObservableCollection<Table>();
        private ObservableCollection<SelectedMenuItems> _selectedItems = new ObservableCollection<SelectedMenuItems>();
        private KhuyenMai km = new KhuyenMai();
        private ObservableCollection<string> _emptytables = new ObservableCollection<string>();
        private ObservableCollection<string> _combobox_option_kindoftables = new ObservableCollection<string>();
        private CollectionViewSource _tables_view;
        private ObservableCollection<string> _ListPhoneCustomer;
        private string customerPhoneNumber;
        private string titleofbill = "";
        private string selectedOption = "Tất cả";
        private decimal dec_sumofbill = 0;
        private string sumofbill = "0 VND";
        private string selectedtable = "";
        private decimal totalDiscount = 0;
        private decimal overallBill = 0;
        private string s_totaldiscount;
        private string s_overallBill;
        private decimal s_memberdiscount = 0;
        private string _MemberDiscount;
        private string _PaymentMethodSelected;
        private string _AddTableID;
        private string _AddTableType;
        int IDofPaidTable = 0;
        bool isNull = false;
        private TimeSpan timeSpanPlayer;
        private DateTime TimeStopRecord;
        private bool BillIsPaid;
        private Decimal tienBan;
        private string str_tienBan;
        #endregion
        #region properties
        public ObservableCollection<Table> Tables { get { return _tables; } set { _tables = value; OnPropertyChanged(); } }
        public KhuyenMai KM { get { return km; } set { km = value; OnPropertyChanged(); } }
        public string CustomerPhoneNumber 
        { 
            get { return customerPhoneNumber; } 
            set 
            { 
                customerPhoneNumber = value;
                if (!string.IsNullOrEmpty(CustomerPhoneNumber))
                {
                    string MaKH = TinhTrangBanDP.Flag.GetMaKH(CustomerPhoneNumber);
                    if (TinhTrangBanDP.Flag.GetCustomerPoint(MaKH) >= 100)
                    {
                        D_MemberDiscount = (Dec_sumofbill + TienBan) * 10 / 100;
                        MemberDiscount = "-" + String.Format("{0:0,0 VND}", D_MemberDiscount);
                        D_OverAllBill -= D_MemberDiscount;
                        OverallBill = String.Format("{0:0,0 VND}", D_OverAllBill);
                    }
                }
                else MemberDiscount = "-00 VNĐ";
                OnPropertyChanged(); 
            } 
        }
        public ObservableCollection<SelectedMenuItems> SelectedItems { get { return _selectedItems; } set { _selectedItems = value; } }
        public ObservableCollection<string> EmptyTables { get { return _emptytables; } set { _emptytables = value; } }
        public ObservableCollection<string> Combobox_Option_KindOfTables { get { return _combobox_option_kindoftables; } set { _emptytables = value; } }
        public ICollectionView TableView { get { return this._tables_view.View; }}
        public TimeSpan TimeSpanPlayer { get { return timeSpanPlayer; } set { timeSpanPlayer = value; OnPropertyChanged(); } }
        public Decimal TienBan { get { return tienBan; } set { tienBan = value; OnPropertyChanged(); } }
        public string S_TienBan { get { return str_tienBan; } set { str_tienBan = value; OnPropertyChanged(); } }
        public string TitleOfBill
        {
            get { return titleofbill; }
            set { titleofbill = value; OnPropertyChanged(); }
        }
        public string PaymentMethodSelected
        {
            get => _PaymentMethodSelected;
            set { _PaymentMethodSelected = value; OnPropertyChanged(); }
        }
        public ObservableCollection<string> ListPhoneCustomer
        {
            get =>  _ListPhoneCustomer;
            set
            {
                _ListPhoneCustomer = value;
                OnPropertyChanged();
            }
        }
        public Decimal D_TotalDiscount { get { return totalDiscount; } set { totalDiscount = value; OnPropertyChanged();} }
        public Decimal D_OverAllBill { get { return overallBill; } set { overallBill = value; OnPropertyChanged(); } }
        public Decimal D_MemberDiscount { get => s_memberdiscount; set { s_memberdiscount = value; OnPropertyChanged(); } }
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
        public string SelectedOption
        {
            get { return selectedOption; }
            set { selectedOption = value; OnPropertyChanged(); }
        }
        public string TotalDiscount
        {
            get
            {
                return s_totaldiscount;
            }
            set
            {
                s_totaldiscount = value;
                OnPropertyChanged();
            }
        }

        public string OverallBill
        {
            get
            {
                return s_overallBill;
            }
             set
            {
                s_overallBill = value;
                OnPropertyChanged();
            }
        }
        public string MemberDiscount
        {
            get => _MemberDiscount;
            set
            {
                _MemberDiscount = value;
                OnPropertyChanged();
            }
        }
        public string AddTableID
        {
            get => _AddTableID;
            set { _AddTableID = value; OnPropertyChanged(); }
        }
        public string AddTableType
        {
            get => _AddTableType;
            set { _AddTableType = value; OnPropertyChanged(); }
        }
        #endregion
        #region commands
        public ICommand StatusOfTableCommand { get; set; }
        public ICommand GetPaymentCommand { get; set; }
        public ICommand GetSwitchTableCommand { get; set; }
        public ICommand SortingFeature_Command { get; set; }
        public ICommand PrintBillCM { get; set; }
        public ICommand OpenPaymentCM { get; set; }
        public ICommand AddNewTableCM { get; set; }
        public ICommand AddTableCM { get; set; }
        #endregion
        #region methods
        public void LoadOptions()
        {
            foreach (Table t in _tables)
            {
                if(!Combobox_Option_KindOfTables.Contains(t.KindOfTable))
                {
                    Combobox_Option_KindOfTables.Add(t.KindOfTable);
                }
            }
            Combobox_Option_KindOfTables.Add("Tất cả");
        }

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
                    table.Coloroftable = "#05BFDB";
                }
                else
                {
                    table.Status = 1;
                    table.Coloroftable = "#FF6D60";
                }
            }
        }

        public void DisplayBill(int BillID)
        {
            SelectedItems.Clear();
            Dec_sumofbill = 0;
            D_TotalDiscount = 0;
            D_OverAllBill = 0;
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
            Update();
            if (KM == null)
            {
                D_TotalDiscount = 0;
            }
            else
            {
                D_TotalDiscount = (Dec_sumofbill + TienBan) * KM.GiamGia / 100;
            }
            D_OverAllBill = Dec_sumofbill - D_TotalDiscount + TienBan;
            SumofBill = String.Format("{0:0,0 VND}", Dec_sumofbill + TienBan);
            TotalDiscount = "-" + String.Format("{0:0,0 VND}", D_TotalDiscount);
            OverallBill = String.Format("{0:0,0 VND}", D_OverAllBill);
        }
        public void GetListPhoneCustomer()
        {
            ListPhoneCustomer = TinhTrangBanDP.Flag.ListPhoneCustomer();
        }
        public void GetStatusOfTable(int ID)
        {
            foreach (Table table in _tables)
            {
                if (table.ID == ID)
                {
                    if (table.Status == 0)
                    {
                        table.Coloroftable = "#05BFDB";
                        table.Status = 0;

                    }
                    else
                    {
                        TitleOfBill = table.NumOfTable;
                        if (TimeStopRecord <= DateTime.MinValue)
                        {
                            TimeSpanPlayer = DateTime.Now - TinhTrangBanDP.Flag.LoadBill_startTime(table.ID);
                        }
                        else
                        {
                            TimeSpanPlayer = TimeStopRecord - TinhTrangBanDP.Flag.LoadBill_startTime(table.ID);
                        }
                        TienBan = Convert.ToDecimal(TimeSpanPlayer.TotalSeconds) * table.Price / 3600;
                        S_TienBan = String.Format("{0:0,0 VND}", Decimal.Round(TienBan));
                        table.Bill_ID = TinhTrangBanDP.Flag.LoadBill(table.ID);
                        DisplayBill(table.Bill_ID);
                        IDofPaidTable = table.ID;
                    }
                    break;
                }
            }
        }

        public void Update()
        {
            KM = KhuyenMaiDP.Flag.GetKhuyenMaisBasedOnMucApDung(Dec_sumofbill + TienBan);
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

                if (true)
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
                                if (ten.Count > 0)
                                {
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
                                    pdfDoc.Add(new Paragraph("Thời gian chơi: " + TimeSpanPlayer.ToString(@"hh\:mm\:ss"), f));
                                    pdfDoc.Add(new Paragraph("    "));
                                    pdfDoc.Add(pdfTable);
                                    pdfDoc.Add(new Paragraph("Tổng cộng:                                                                    " + OverallBill, f));
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
                    //MyMessageBox mess = new MyMessageBox("Không tồn tại hóa đơn!");
                    //mess.ShowDialog();
                }
            }
        }

        public void Payment()
        {
            foreach (Table table in _tables)
            {
                if (table.ID == IDofPaidTable)
                {
                    string MaKM, MaKH;
                    if (KM == null) MaKM = ""; else MaKM = KM.MAKM;
                    if (string.IsNullOrEmpty(CustomerPhoneNumber)) MaKH = ""; else MaKH = TinhTrangBanDP.Flag.GetMaKH(CustomerPhoneNumber);
                    bool success = TinhTrangBanDP.Flag.UpdateBill(table.Bill_ID, D_OverAllBill, TimeSpanPlayer, MaKM, MaKH, PaymentMethodSelected, table.ID.ToString());
                    if (success)
                    {
                        if (!string.IsNullOrEmpty(MemberDiscount))
                        {
                            TinhTrangBanDP.Flag.MinusCustomerPoint(MaKH, 100);
                        }
                        if (!string.IsNullOrEmpty(MaKH))
                        {
                            TinhTrangBanDP.Flag.UpdateKhachHangAccumulatedPoint(((int)D_OverAllBill / 50000) * 20, MaKH);
                        }
                        table.Coloroftable = "#05BFDB";
                        table.Status = 0;
                        TinhTrangBanDP.Flag.UpdateTable(table.ID, true);

                        Dec_sumofbill = 0;
                        D_TotalDiscount = 0;
                        D_OverAllBill = 0;
                        D_MemberDiscount = 0;
                        SumofBill = String.Format("{0:0,0 VND}", Dec_sumofbill);
                        TotalDiscount = String.Format("{0:0,0 VND}", D_TotalDiscount);
                        OverallBill = String.Format("{0:0,0 VND}", D_OverAllBill);
                        MemberDiscount = String.Format("{0:0,0 VND}", D_MemberDiscount);

                        SelectedItems.Clear();
                        TitleOfBill = "";
                        PaymentMethodSelected = "";
                        CustomerPhoneNumber = "";
                        MyMessageBox msb = new MyMessageBox("Đã thanh toán thành công!");
                        msb.Show();
                        break;
                    }

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
                        isNull = false;
                        table.Coloroftable = "#05BFDB";
                        table.Status = 0;
                        TinhTrangBanDP.Flag.UpdateTable(table.ID, true);
                        TinhTrangBanDP.Flag.SwitchTable(int.Parse(SelectedTable), table.Bill_ID);
                        TinhTrangBanDP.Flag.UpdateTable(int.Parse(SelectedTable), false);

                        Dec_sumofbill = 0;
                        D_TotalDiscount = 0;
                        D_OverAllBill = 0;
                        SumofBill = String.Format("{0:0,0 VND}", Dec_sumofbill);
                        TotalDiscount = String.Format("{0:0,0 VND}", D_TotalDiscount);
                        OverallBill = String.Format("{0:0,0 VND}", D_OverAllBill);
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
                    table.Coloroftable = "#FF6D60";
                    table.Status = 1;
                }
            }
            EmptyTables.Clear();
            LoadEmptyTables();
        }
#endregion
    }
}