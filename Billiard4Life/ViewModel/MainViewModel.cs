using Billiard4Life.DataProvider;
using Billiard4Life.State.Navigator;
using RestaurantManagement.View;
using RestaurantManagement.ViewModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace Billiard4Life.ViewModel
{

    public class MainViewModel : BaseViewModel
    {
        private int _ReservationCount;
        public int ReservationCount
        {
            get => _ReservationCount;
            set
            {
                _ReservationCount = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            ReservationCount = GetReservationCount();

            LoadWindowCommand = new RelayCommand<Window>((p) => true, (p) =>
            {
                if (p == null)
                {
                    return;
                }
                p.Hide();
                LoginWindow window = new LoginWindow();
                window.ShowDialog();
                var loginVM = window.DataContext as LoginWindowVM;
                if (loginVM == null)
                {
                    return;
                }
                if (loginVM.IsLoggedIn)
                {
                    Navigator = new Navigator(loginVM.Role);
                    if (loginVM.Role != "admin")
                    {
                        ExitTitle = "Kết thúc ca";
                        if (NhanVienDP.Flag.IsOnline(LoginWindowVM.MaNV) == false)
                        {
                            NhanVienDP.Flag.SetAccountOnline(LoginWindowVM.MaNV);
                            NhanVienDP.Flag.StartTimeKeeping(LoginWindowVM.MaNV);
                        }
                    }
                    CaiDatViewModel = new CaiDatViewModel(LoginWindowVM.MaNV, loginVM.UserName, loginVM.Password, loginVM.Role);
                    p.Show();
                }
                else
                {
                    p.Close();
                }
            });

            LogOutCommand = new RelayCommand<Window>((p) => true, (p) =>
            {
                if (p == null)
                {
                    return;
                }
                if (NhanVienDP.Flag.IsStaff(LoginWindowVM.MaNV))
                {
                    MyMessageBox msb = new MyMessageBox("Kết thúc ca làm?", true);
                    msb.ShowDialog();
                    if (msb.ACCEPT())
                    {
                        NhanVienDP.Flag.SetAccountOffline(LoginWindowVM.MaNV);
                        NhanVienDP.Flag.StopTimeKeepingAndRecord(LoginWindowVM.MaNV);
                    }
                }
                System.Windows.Forms.Application.Restart();
                p.Close();
            });
            Navigator = new Navigator("admin");
            HeaderViewModel = new HeaderViewModel();
            bep = new BepViewModel();
        }

        private int GetReservationCount()
        {
            OpenConnect();

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT COUNT(*) FROM DATBAN WHERE DAY(NgayGio) >= Day(GETDATE()) " +
                "AND MONTH(NgayGio) >= MONTH(GETDATE()) AND YEAR(NgayGio) >= YEAR(GETDATE())";
            cmd.Connection = sqlCon;
            var reader = cmd.ExecuteReader();

            int num = 0;

            while (reader.Read())
            {
                num = reader.GetInt32(0);
            }

            CloseConnect();
            return num;
        }

        CaiDatViewModel caiDatViewModel;
        HeaderViewModel headerViewModel;
        Navigator navigator;
        BepViewModel bep;
        public string MaNV;
        private string _ExitTitle = "Đăng xuất";

        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;

        public ICommand LoadWindowCommand { get; set; }
        public ICommand LogOutCommand { get; set; }
        public CaiDatViewModel CaiDatViewModel
        {
            get { return caiDatViewModel; }
            set { caiDatViewModel = value; OnPropertyChanged(); }
        }

        public HeaderViewModel HeaderViewModel
        {
            get { return headerViewModel; }
            set { headerViewModel = value; OnPropertyChanged(); }
        }

        public Navigator Navigator
        {
            get { return navigator; }
            set { navigator = value; OnPropertyChanged(); }
        }
        public string ExitTitle
        {
            get => _ExitTitle;
            set { _ExitTitle = value; OnPropertyChanged(); }
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
    }
}
