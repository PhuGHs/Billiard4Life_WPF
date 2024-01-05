using Billiard4Life.Models;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;

namespace Billiard4Life.ViewModel
{
    public class DatBanVM : BaseViewModel
    {
        private ObservableCollection<DatBan> _Reservations;
        public ObservableCollection<DatBan> Reservations
        {
            get => _Reservations;
            set
            {
                _Reservations = value;
                OnPropertyChanged();
            }
        }
        private DatBan _Selected;
        public DatBan Selected
        {
            get => _Selected;
            set
            {
                _Selected = value;
                OnPropertyChanged();
            }
        }

        public ICommand ConfirmCM { get; set; }
        public ICommand CancelCM { get; set; }

        private string strCon = ConfigurationManager.ConnectionStrings["Billiard4Life"].ConnectionString;
        private SqlConnection sqlCon = null;

        public DatBanVM()
        {
            Reservations = new ObservableCollection<DatBan>();

            ConfirmCM = new RelayCommand<object>((p) =>
            {
                if (Selected == null)
                {
                    return false;
                }

                return true;
            }, (p) =>
            {
                Confirm();
            });

            CancelCM = new RelayCommand<object>((p) =>
            {
                if (Selected == null)
                {
                    return false;
                }

                return true;
            }, (p) =>
            {
                Cancel();
            });

            GetList();
        }

        public void Confirm()
        {
            OpenConnect();

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlCon;
            cmd.CommandText = $"UPDATE DATBAN SET DaXacNhan = 1 WHERE ID = {Selected.ID}";
            cmd.ExecuteNonQuery();

            GetList();

            CloseConnect();
        }

        public void Cancel()
        {
            OpenConnect();

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlCon;
            cmd.CommandText = $"DELETE FROM DATBAN WHERE ID = {Selected.ID}";
            cmd.ExecuteNonQuery();

            GetList();

            CloseConnect();
        }

        public void GetList()
        {
            _Reservations.Clear();

            OpenConnect();

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT * FROM DATBAN " +
                "WHERE DAY(NgayGio) >= Day(GETDATE()) AND MONTH(NgayGio) >= MONTH(GETDATE()) " +
                "AND YEAR(NgayGio) >= YEAR(GETDATE()) ORDER BY DaXacNhan, ID";
            cmd.Connection = sqlCon;
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var ten = reader.GetString(1);
                var sdt = reader.GetString(2);
                var ngay = reader.GetDateTime(3).ToString();
                var loaiBan = reader.GetString(4);
                var xacnhan = reader.GetBoolean(5);

                Reservations.Add(new DatBan(id, ten, sdt, ngay, loaiBan, xacnhan));
            }

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
    }
}
