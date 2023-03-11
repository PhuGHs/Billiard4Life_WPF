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
using Billiard4Life.ViewModel;

namespace Billiard4Life.View
{
    /// <summary>
    /// Interaction logic for User.xaml
    /// </summary>
    public partial class User : Window
    {
        public User()
        {
            InitializeComponent();
        }
        private void ShowCurrentPassword_Checked(object sender, RoutedEventArgs e)
        {
            currentpasswordTxtBox.Text = currentPassword.Password;
            currentPassword.Visibility = Visibility.Collapsed;
            currentpasswordTxtBox.Visibility = Visibility.Visible;
        }

        private void ShowCurrentPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            currentPassword.Password = currentpasswordTxtBox.Text;
            currentpasswordTxtBox.Visibility = Visibility.Collapsed;
            currentPassword.Visibility = Visibility.Visible;
        }

        private void ShowNewPassword_Checked(object sender, RoutedEventArgs e)
        {
            newpasswordTxtBox.Text = newPassword.Password;
            newPassword.Visibility = Visibility.Collapsed;
            newpasswordTxtBox.Visibility = Visibility.Visible;
        }

        private void ShowNewPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            newPassword.Password = newpasswordTxtBox.Text;
            newpasswordTxtBox.Visibility = Visibility.Collapsed;
            newPassword.Visibility = Visibility.Visible;
        }
        private void ShowConfirmPassword_Checked(object sender, RoutedEventArgs e)
        {
            confirmpasswordTxtBox.Text = confirmPassword.Password;
            confirmPassword.Visibility = Visibility.Collapsed;
            confirmpasswordTxtBox.Visibility = Visibility.Visible;
        }

        private void ShowConfirmPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            confirmPassword.Password = confirmpasswordTxtBox.Text;
            confirmpasswordTxtBox.Visibility = Visibility.Collapsed;
            confirmPassword.Visibility = Visibility.Visible;
        }

        private async void doimatkhau_click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(1000);
            confirmpasswordTxtBox.Text = String.Empty;
            newpasswordTxtBox.Text = String.Empty;
            currentpasswordTxtBox.Text = String.Empty;
            confirmPassword.Password = String.Empty;
            newPassword.Password = String.Empty;
            currentPassword.Password = String.Empty;
        }
    }
}
