using Billiard4Life.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Configuration;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ListView = System.Windows.Forms.ListView;
namespace Billiard4Life.ViewModel
{

    public class BepViewModel : BaseViewModel
    {
        public ICommand AddCM { get; set; }
        public BepViewModel()
        {
            AddCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                Billiard4Life.View.ThemKhachHang adding = new View.ThemKhachHang();
                adding.Show();
                return;
            });
        }
    }
}
