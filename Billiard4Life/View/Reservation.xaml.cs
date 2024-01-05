using Billiard4Life.ViewModel;
using Microsoft.AspNetCore.SignalR.Client;
using System.Windows;
using System.Windows.Input;

namespace Billiard4Life.View
{
    /// <summary>
    /// Interaction logic for Reservation.xaml
    /// </summary>
    public partial class Reservation : Window
    {
        HubConnection connection;

        public Reservation()
        {
            InitializeComponent();
            this.DataContext = new DatBanVM();

            connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7004/reservationhub")
            .WithAutomaticReconnect()
            .Build();

            Connect();
        }

        private async void Connect()
        {
            connection.On<string, string>("NewReservation", (user, mess) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var vm = this.DataContext as DatBanVM;

                    vm.GetList();
                });
            });

            try
            {
                await connection.StartAsync();
            }
            catch
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
