using Billiard4Life.Models;
using RestaurantManagement.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using OfficeOpenXml.FormulaParsing.ExpressionGraph.FunctionCompilers;
using System.Windows.Forms;

namespace Billiard4Life.ViewModel
{
    public class NhanVienViewModel : BaseViewModel
    {
        public ICommand CheckCM { get; set; }
        public NhanVienViewModel()
        {
            CheckCM = new RelayCommand<object>((p) => { return true; }, (p) =>
            {
                RestaurantManagement.View.ChamCong chamCong = new RestaurantManagement.View.ChamCong();
                chamCong.Show();
                return;
            });
        }
    }
}
