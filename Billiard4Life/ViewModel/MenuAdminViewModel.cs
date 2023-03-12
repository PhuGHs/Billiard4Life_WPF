using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Billiard4Life.DataProvider;
using Billiard4Life.Models;
using Billiard4Life.View;
using System.Windows.Data;
using System.ComponentModel;
using Diacritics.Extensions;
using System.Data.SqlClient;
using System.Diagnostics;
using Button = System.Windows.Controls.Button;

namespace Billiard4Life.ViewModel
{
    public class MenuAdminViewModel : BaseViewModel
    {
        public MenuAdminViewModel()
        {
            AddOneMenuDish = new RelayCommand<Object>((p) => true, (p) =>
            {
                MenuAdmin_ThemMon window = new MenuAdmin_ThemMon();
                window.DataContext = this;
                window.ShowDialog();
            });
        }
        #region Commands
            public ICommand AddOneMenuDish { get; set; }
        #endregion

    }
}
