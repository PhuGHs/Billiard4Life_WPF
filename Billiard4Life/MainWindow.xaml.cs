using Billiard4Life.View;
using Billiard4Life.ViewModel;
using Microsoft.AspNetCore.SignalR.Client;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace Project;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    HubConnection connection;

    public MainWindow()
    {
        InitializeComponent();

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
                var vm = this.DataContext as MainViewModel;

                vm.ReservationCount++;

                SystemSounds.Beep.Play();
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

    private void Border_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            this.DragMove();
        }
    }
    private void ShowUser_Click(object sender, RoutedEventArgs e)
    {
        User userSettingPage = new User();
        userSettingPage.ShowDialog();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var reservation = new Reservation();
        reservation.Show();
    }
}
