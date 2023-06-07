using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billiard4Life.Models;
using System.Windows.Input;
using Billiard4Life.DataProvider;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;
using Billiard4Life.ViewModel;
using Billiard4Life;

namespace Billiard4Life.ViewModel
{
    public class CaiDatViewModel : BaseViewModel
    {
        public CaiDatViewModel(string MaNV, string ID, string password, string role)
        {
            Role = role;
            NhanVien = CaiDatDP.Flag.GetCurrentEmployee(MaNV, password);
            CaiDatDP.Flag.LoadProfileImage(NhanVien);
            UpdateInfoCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                CaiDatDP.Flag.UpdateInfo(NhanVien.HoTen, NhanVien.DiaChi, NhanVien.SDT, NhanVien.NgaySinh, NhanVien.MaNV);
            });
            CancelCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                NhanVien = CaiDatDP.Flag.GetCurrentEmployee(MaNV, password);
                CaiDatDP.Flag.LoadProfileImage(NhanVien);
            });
            ChangeProfileImage = new RelayCommand<object>((p) => true, (p) =>
            {
                OpenFileDialog open = new OpenFileDialog();
                open.Title = "Thay ảnh đại diện";
                open.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" + "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" + "Portable Network Graphic (*.png)|*.png";

                if (open.ShowDialog() == DialogResult.OK)
                {
                    BitmapImage bmi = new BitmapImage();
                    bmi.BeginInit();
                    bmi.CacheOption = BitmapCacheOption.OnLoad;
                    bmi.UriSource = new Uri(open.FileName);
                    bmi.EndInit();
                    NhanVien.AnhDaiDien = bmi;

                    MyMessageBox msb = new MyMessageBox("Đã thay đổi ảnh đại diện!");
                    msb.Show();
                }
                CaiDatDP.Flag.ChangeProfileImage_SaveToDB(NhanVien, ID);
            });
            CurrentPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => true, (p) =>
            {
                CurrentPassword = p.Password;
            });
            NewPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => true, (p) =>
            {
                NewPassword = p.Password;
            });
            ConfirmPasswordChangedCommand = new RelayCommand<PasswordBox>((p) => true, (p) =>
            {
                ConfirmPassword = p.Password;
            });
            ChangePassword = new RelayCommand<object>((p) => true, (p) =>
            {
                if (PasswordValidation())
                {
                    NhanVien = CaiDatDP.Flag.GetCurrentEmployee(MaNV, NewPassword);
                    CaiDatDP.Flag.LoadProfileImage(NhanVien);
                    CaiDatDP.Flag.ChangePassword(NewPassword, ID);
                }
                return;
            });
            CloseWindowCommand = new RelayCommand<Window>((p) => true, (p) =>
            {
                p.Close();
            });
        }
        #region attributes
        private NhanVien _nhanVien;
        private string _currentPassword;
        private string _newPassword;
        private string _confirmPassword;
        #endregion
        #region variables
        public NhanVien NhanVien { get { return _nhanVien; } set { _nhanVien = value; OnPropertyChanged(); } }
        public string LoaiNhanVien_Str
        {
            get
            {
                if (_nhanVien.Fulltime)
                {
                    return "Full-time";
                }
                return "Part-time";
            }
            set
            {
                if (value == "Full-time")
                {
                    _nhanVien.Fulltime = true;
                }
                else
                {
                    _nhanVien.Fulltime = false;
                }
            }
        }
        public string CurrentPassword
        {
            get => _currentPassword;
            set
            {
                _currentPassword = value; OnPropertyChanged();
            }
        }
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                _newPassword = value; OnPropertyChanged();
            }
        }
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value; OnPropertyChanged();
            }
        }
        public string Role { get; set; }
        #endregion
        #region commands
        public ICommand UpdateInfoCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand ChangeProfileImage { get; set; }
        public ICommand ChangePassword { get; set; }
        public ICommand CurrentPasswordChangedCommand { get; set; }
        public ICommand NewPasswordChangedCommand { get; set; }
        public ICommand ConfirmPasswordChangedCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }
        #endregion
        #region complementary functions
        public bool PasswordValidation()
        {
            if (String.IsNullOrEmpty(CurrentPassword) || String.IsNullOrEmpty(NewPassword) || String.IsNullOrEmpty(ConfirmPassword))
            {
                MyMessageBox msb = new MyMessageBox("Hãy nhập đầy đủ mật khẩu!");
                msb.Show();
                return false;
            }
            else if (ConfirmPassword != NewPassword)
            {
                MyMessageBox msb = new MyMessageBox("Mật khẩu xác nhận và mật khẩu mới không trùng với nhau!");
                msb.Show();
                return false;
            }
            else if (CurrentPassword != NhanVien.MatKhau)
            {
                MyMessageBox msb = new MyMessageBox("Mật khẩu sai!");
                msb.Show();
                return false;
            }
            return true;
        }
        #endregion
    }
}