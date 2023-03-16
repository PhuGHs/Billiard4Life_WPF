using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using LichSuBan.Models;
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

namespace Billiard4Life.ViewModel
{
    public class LichSuBanViewModel : BaseViewModel
    {
        public ICommand DetailCM { get; set; }
        public LichSuBanViewModel()
        {
            DetailCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Billiard4Life.View.ChiTietHoaDon cthd = new View.ChiTietHoaDon();
                cthd.Show();
                return;
            });
        }
    }
}

