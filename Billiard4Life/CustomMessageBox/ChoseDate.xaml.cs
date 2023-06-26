using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Billiard4Life.CustomMessageBox
{
    /// <summary>
    /// Interaction logic for ChoseDate.xaml
    /// </summary>
    public partial class ChoseDate : Window
    {
        public ChoseDate()
        {
            InitializeComponent();
        }
        public Tuple<string, string> GetDate()
        {
            return new Tuple<string, string>(dtBegin.SelectedDate.ToString(), dtEnd.SelectedDate.ToString());
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (dtBegin.SelectedDate == null || dtEnd.SelectedDate == null) 
            {
                MyMessageBox msg = new MyMessageBox("Vui lòng chọn ngày!");
                msg.ShowDialog();
                return;
            }
            if (dtBegin.SelectedDate > dtEnd.SelectedDate || dtEnd.SelectedDate > DateTime.Now)
            {
                MyMessageBox msg = new MyMessageBox("Vui lòng chọn ngày hợp lệ!");
                msg.ShowDialog();
                return;
            }
            this.Close();
        }
    }
}
