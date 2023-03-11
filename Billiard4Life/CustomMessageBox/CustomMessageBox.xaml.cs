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

namespace Billiard4Life
{
    /// <summary>
    /// Interaction logic for MyMessageBox.xaml
    /// </summary>
    public partial class MyMessageBox : Window
    {
        public MyMessageBox(string mess, bool YES = false)
        {
            InitializeComponent();
            if (YES)
            {
                Run(mess, true);
            }
            else
            {
                Run(mess);
            }
        }
        private void Run(string Message, bool yesno = false)
        {
            lbMessage.Text = Message;
            if (!yesno)
            {
                btnYES.Visibility = Visibility.Hidden;
                btnNO.Visibility = Visibility.Hidden;
            }
            else
            {
                btnOKcenter.Visibility = Visibility.Hidden;
            }
        }
        private bool Accept = false;
        private void btnOKcenter_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void btnYES_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Accept = true;
        }
        private void btnNO_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            Accept = false;
        }
        public bool ACCEPT()
        {
            return Accept;
        }
    }
}
