using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using Billiard4Life.Models;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Document = iTextSharp.text.Document;

namespace Billiard4Life.ViewModel
{
    public class KhoViewModel : BaseViewModel
    {
        private ObservableCollection<Kho> _ListWareHouse;
        public ObservableCollection<Kho> ListWareHouse { get => _ListWareHouse; set { _ListWareHouse = value; OnPropertyChanged(); } }
        private ObservableCollection<string> _Groups;
        public ObservableCollection<string> Groups { get => _Groups; set { _Groups = value; OnPropertyChanged(); } }
        private Kho _Selected;
        public Kho Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
                OnPropertyChanged();
            }
        }
        #region // Search bar
        private string _Search;
        public string Search
        {
            get => _Search;
            set
            {
                _Search = value;
                string strQuery;
                OnPropertyChanged();
                string temp;
                if (GroupSelected == "Tất cả") temp = "";
                else temp = " AND NhomSanPham = N'" + GroupSelected + "'";
                if (!String.IsNullOrEmpty(Search))
                {
                    strQuery = "SELECT * FROM KHO WHERE Xoa = 0 AND TenSanPham LIKE N'%" + Search + "%'" + temp;
                }
                else
                    strQuery = "SELECT * FROM KHO WHERE Xoa = 0" + temp;
                ListViewDisplay(strQuery);
            }
        }
        #endregion

        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;
        private string _GroupSelected;
        public string GroupSelected
        {
            get => _GroupSelected;
            set
            {
                _GroupSelected = value;
                if (!string.IsNullOrEmpty(GroupSelected))
                {
                    if (GroupSelected == "Tất cả")
                        ListViewDisplay("SELECT * FROM KHO WHERE Xoa = 0");
                    else
                    ListViewDisplay("SELECT * FROM KHO WHERE Xoa = 0 AND NhomSanPham = N'" + GroupSelected + "'");
                }
                OnPropertyChanged();
            }
        }
        public ICommand DetailCM { get; set; }
        public ICommand AddCM { get; set; }
        public ICommand DeleteCM { get; set; }
        public ICommand CheckCM { get; set; }
        public KhoViewModel()
        {
            OpenConnect();

            Groups = new ObservableCollection<string>();
            Groups.Add("Nước uống");
            Groups.Add("Đồ ăn");
            Groups.Add("Khác");
            Groups.Add("Tất cả");
            ListWareHouse = new ObservableCollection<Kho>();
            ListViewDisplay("SELECT * FROM KHO WHERE Xoa = 0");
            GroupSelected = "Tất cả";


            DetailCM = new RelayCommand<object>((p) => 
            {
                if (Selected == null) return false;
                return true; 
            }, (p) =>
            {
                Billiard4Life.View.ChiTietNhapKho ctn = new View.ChiTietNhapKho(Selected.TenSanPham);
                ctn.Show();
                return;
            });
            AddCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Billiard4Life.View.NhapHangMoi adding;
                if (Selected != null)
                {
                    MyMessageBox yn = new MyMessageBox("Bạn có muốn nhập thêm sản phẩm đang chọn?", true);
                    yn.ShowDialog();
                    if (yn.ACCEPT())
                    {
                        adding = new View.NhapHangMoi(Selected);
                        if (adding.ShowDialog() == true) { }
                        ListViewDisplay("SELECT * FROM KHO WHERE Xoa = 0");
                        return;
                    }
                    adding = new View.NhapHangMoi();
                    if (adding.ShowDialog() == true) { }
                    ListViewDisplay("SELECT * FROM KHO WHERE Xoa = 0");
                    return;
                }
                adding = new View.NhapHangMoi();
                if (adding.ShowDialog() == true) { }
                ListViewDisplay("SELECT * FROM KHO WHERE Xoa = 0");
                return;
            });
            #region // delete command
            DeleteCM = new RelayCommand<object>((p) =>
            {
                if (Selected == null) return false;
                return true;
            }, (p) =>
            {
                bool delete = false;

                if (Selected.TonDu > 0)
                {
                    MyMessageBox yn = new MyMessageBox("Sản phẩm này đang còn trong kho!\n   Bạn có chắc chắn xóa?", true);
                    yn.ShowDialog();
                    if (yn.ACCEPT())
                    {
                        delete = true;
                    }
                }
                else
                    delete = true;

                if (delete)
                {
                    OpenConnect();

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE KHO SET Xoa = 1 WHERE TenSanPham = N'" + Selected.TenSanPham + "'";
                    cmd.Connection = sqlCon;

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MyMessageBox mess = new MyMessageBox("Xóa thành công!");
                        mess.ShowDialog();
                    }
                    else
                    {
                        MyMessageBox mess = new MyMessageBox("Xóa không thành công!");
                        mess.ShowDialog();
                    }
                    ListViewDisplay("SELECT * FROM KHO WHERE Xoa = 0");

                    CloseConnect();
                }
            });
            #endregion

            #region // check command
            CheckCM = new RelayCommand<object>((p) =>
            {
                if (ListWareHouse == null) return false;
                return true;
            }, (p) =>
            {
                OpenConnect();

                string strQuery = "SELECT * FROM KHO WHERE TonDu <= MucBaoNhap AND Xoa = 0";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strQuery;
                cmd.Connection = sqlCon;

                SqlDataReader reader = cmd.ExecuteReader();
                List<string> ten = new List<string>();
                List<string> soluong = new List<string>();
                List<string> donvi = new List<string>();

                while (reader.Read())
                {
                    ten.Add(reader.GetString(0));
                    soluong.Add(reader.GetDouble(1).ToString());
                    donvi.Add(reader.GetString(2));
                }

                if (ten.Count > 0)
                {
                    ListViewDisplay(strQuery);
                    MyMessageBox yesno = new MyMessageBox("Bạn có muốn in danh sách?", true);
                    yesno.ShowDialog();
                    if (yesno.ACCEPT())
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.Filter = "PDF (*.pdf)|*.pdf";
                        sfd.FileName = "Danh sách cần nhập " + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
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
                                pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

                                BaseFont bf = BaseFont.CreateFont(Environment.GetEnvironmentVariable("windir") + @"\fonts\TIMES.TTF", BaseFont.IDENTITY_H, true);
                                Font f = new Font(bf, 16, Font.NORMAL);

                                PdfPCell cell = new PdfPCell(new Phrase("Tên sản phẩm", f));
                                pdfTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase("Tồn dư", f));
                                pdfTable.AddCell(cell);
                                cell = new PdfPCell(new Phrase("Đơn vị", f));
                                pdfTable.AddCell(cell);
                                for (int i = 0; i < ten.Count; i++)
                                {
                                    pdfTable.AddCell(new Phrase(ten[i], f));
                                    pdfTable.AddCell(new Phrase(soluong[i], f));
                                    pdfTable.AddCell(new Phrase(donvi[i], f));
                                }

                                using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                                {
                                    Document pdfDoc = new Document(PageSize.A4, 50f, 50f, 40f, 40f);
                                    PdfWriter.GetInstance(pdfDoc, stream);
                                    pdfDoc.Open();
                                    pdfDoc.Add(new Paragraph("              DANH SÁCH SẢN PHẨM CẦN NHẬP THÊM " + DateTime.Now.ToShortDateString(), f));
                                    pdfDoc.Add(new Paragraph("    "));
                                    pdfDoc.Add(pdfTable);
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
                        ListViewDisplay("SELECT * FROM KHO WHERE Xoa = 0");
                    }
                    else
                        ListViewDisplay("SELECT * FROM KHO WHERE Xoa = 0");
                }
                else
                {
                    MyMessageBox mess = new MyMessageBox("Chưa có sản phẩm nào \n      cần nhập thêm!");
                    mess.ShowDialog();
                }

                CloseConnect();
            });
            #endregion

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

        private void ListViewDisplay(string strQuery)
        {
            OpenConnect();

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strQuery;
            cmd.Connection = sqlCon;
            SqlDataReader reader = cmd.ExecuteReader();
            ListWareHouse.Clear();
            while (reader.Read())
            {
                string ten = reader.GetString(0);
                float tondu = (float)reader.GetDouble(1);
                string donvi = reader.GetString(3);
                string nhom = reader.GetString(4);
                ListWareHouse.Add(new Kho(ten, tondu, donvi, nhom));
            }

            CloseConnect();
        }
    }
}
