using Billiard4Life.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Billiard4Life.State.Navigator
{
    public enum TypeOfView
    {
        NhanVien,
        Kho,
        Menu,
        KhuyenMai,
        MenuAdmin,
        LichSuBan,
        TinhTrangBan,
        CaiDat,
        DangXuat,
        ThongKe,
        Bep,
        LichSuCa
    }
    public interface INavigator
    {
        BaseViewModel CurrentViewModel { get; set; }
        string CurrentTitle { get; set; }
        Visibility AdminView { get; set; }
        Visibility EmployeeView { get; set; }
        ICommand SelectViewModelCommand { get;  }
    }
}
