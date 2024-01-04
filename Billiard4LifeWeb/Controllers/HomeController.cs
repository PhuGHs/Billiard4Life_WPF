using Billiard4LifeWeb.Hubs;
using Billiard4LifeWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Billiard4LifeWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string strCon = "Data Source=DESKTOP-VRFDTBA\\SQLEXPRESS;Initial Catalog=Billiard4Life;Integrated Security=True";
        private readonly IHubContext<ReservationHub> _hub;
        private SqlConnection sqlCon = null;

		public HomeController(ILogger<HomeController> logger, IHubContext<ReservationHub> hub)
        {
            _logger = logger;
            _hub = hub;
        }

        public IActionResult Index()
        {
            ViewData["Active"] = "Home";
            return View();
        }

        public IActionResult Services()
        {
            OpenConnect();

            var menu = new List<MenuItem>();

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT TenMon, Gia FROM MENU WHERE DELETED = 0";
            cmd.Connection = sqlCon;
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var ten = reader.GetString(0);
                var gia = Math.Round(reader.GetDecimal(1)).ToString();

                gia = gia.Insert(gia.Length - 3, ".") + "đ";

                var mon = new MenuItem
                {
                    TenMon = ten,
                    Gia = gia,
                };

                menu.Add(mon);
            }

            CloseConnect();

            ViewData["Active"] = "Services";
            return View(menu);
        }


        [HttpPost]
        public IActionResult Order(Order order)
        {
            OpenConnect();

            var day = order.Ngay?.Substring(0, 2);
            var month = order.Ngay?.Substring(3, 2);
            var year = order.Ngay?.Substring(6, 4);

            var hour = order.Gio?.Substring(0, order.Gio.IndexOf(':'));
            var min = order.Gio?.Substring(order.Gio.IndexOf(':') + 1, 2);
            bool IsPM = order.Gio?.Substring(order.Gio.Length - 2, 2) == "PM";

            if (IsPM)
            {
                hour = (int.Parse(hour ?? "6") + 12).ToString();
            }

            var loaiBan = order.LoaiBan == "1" ? "Carom" :
                          order.LoaiBan == "2" ? "Libre" :
                          "Pool";

            var cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT INTO DATBAN (TenKhachHang, SDT, NgayGio, LoaiBan, DaXacNhan) " +
                $"VALUES(N'{order.TenKhachHang}', '{order.SDT}', '{year}-{day}-{month} {hour}:{min}:00', '{loaiBan}', 0)";
            cmd.Connection = sqlCon;
            cmd.ExecuteNonQuery();

            CloseConnect();

            _hub.Clients.All.SendAsync("NewReservation", "", "");

            TempData["SuccessMessage"] = "Thông tin đặt bàn của bạn đã được ghi nhận. Nhân viên sẽ gọi điện để xác nhận với bạn trong ít phút!";

            return RedirectToAction("Index");
        }

        public IActionResult Prices()
        {
            ViewData["Active"] = "Prices";
            return View();
        }

        public IActionResult PlaceOrder()
        {
            ViewData["Active"] = "PlaceOrder";
            return View();
        }

        public IActionResult Order()
        {
			TempData["SuccessMessage"] = "Thông tin đặt bàn của bạn đã được ghi nhận! Nhân viên sẽ sớm liên lạc với bạn để xác nhận!";

			return Redirect(Request.Headers["Referer"].ToString());
		}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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