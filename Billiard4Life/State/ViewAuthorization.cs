using Billiard4Life.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Billiard4Life.State
{
    public class ViewAuthorization : BaseViewModel
    {
        public ViewAuthorization(string role)
        {
            getCurrentRole(role);
        }
        private bool adminRole;
        private Visibility _adminView;
        private Visibility _employeeView;

        public Visibility AdminView { get { return _adminView; } set { _adminView = value; OnPropertyChanged(); } }
        public Visibility EmployeeView { get { return _employeeView; } set { _employeeView = value; OnPropertyChanged(); } }
        public bool AdminRole { get { return adminRole; } set { adminRole = value; OnPropertyChanged(); } }
        public void getCurrentRole(string role)
        {
            if (String.Compare(role, "admin") == 0)
            {
                AdminRole = true;
            }
            else
            {
                AdminRole = false;
            }

            ClassifyAdmin(AdminRole);
        }
        public void ClassifyAdmin(bool AdminRole)
        {
            if (AdminRole)
            {
                AdminView = Visibility.Visible;
                EmployeeView = Visibility.Collapsed;
            }
            else
            {
                AdminView = Visibility.Collapsed;
                EmployeeView = Visibility.Visible;
            }
        }
    }
}
