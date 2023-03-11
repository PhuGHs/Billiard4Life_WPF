using Billiard4Life.Command;
using Billiard4Life.ViewModel;
using Billiard4Life.State;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Billiard4Life.State.Navigator
{
    public class Navigator : ObservableObject, INavigator
    {
        public Navigator(string role)
        {
            ViewAuthorization = new ViewAuthorization(role);
            _adminView = ViewAuthorization.AdminView;
            _employeeView = ViewAuthorization.EmployeeView;
        }
        private Visibility _adminView;
        private Visibility _employeeView;
        private ViewAuthorization _authorization;
        public ViewAuthorization ViewAuthorization { get { return _authorization; } set { _authorization = value; OnPropertyChanged(_authorization.ToString()); } }
        private BaseViewModel _currentViewModel;
        private string _currentTitle;
        public BaseViewModel CurrentViewModel { 
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }
        public string CurrentTitle
        {
            get
            {
                return _currentTitle;
            }
            set
            {
                _currentTitle = value;
                OnPropertyChanged(nameof(CurrentTitle));
            }
        }

        public ICommand SelectViewModelCommand => new SelectViewModelCommand(this, this);
        public Visibility AdminView { get => _adminView; set { _adminView = value; OnPropertyChanged(_adminView.ToString()); } }
        public Visibility EmployeeView { get => _employeeView; set { _employeeView = value; OnPropertyChanged(_employeeView.ToString()); } }
    }
}
