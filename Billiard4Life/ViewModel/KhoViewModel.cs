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
        public ICommand DetailCM { get; set; }
        public ICommand AddCM { get; set; }
        public KhoViewModel()
        {
            DetailCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Billiard4Life.View.ChiTietNhapKho ctn = new View.ChiTietNhapKho();
                ctn.Show();
                return;
            });
            AddCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Billiard4Life.View.NhapHangMoi adding = new View.NhapHangMoi();
                adding.Show();
                return;
            });
        }
    }
}
