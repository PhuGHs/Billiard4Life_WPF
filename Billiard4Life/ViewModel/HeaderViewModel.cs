using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Billiard4Life.State.Navigator;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Billiard4Life.ViewModel
{
    public class HeaderViewModel : BaseViewModel
    {
        #region commands
            public ICommand CloseWindowCommand { get; set; }
            public ICommand MinimizeWindowCommand { get; set; }
        #endregion

        public HeaderViewModel()
        {
            CloseWindowCommand = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) => { FrameworkElement window = GetWindowParent(p);
                var windows = window as Window;
                if(windows != null)
                {
                    windows.Close();
                }
            });

            MinimizeWindowCommand  = new RelayCommand<UserControl>((p) => { return p == null ? false : true; }, (p) => {
                FrameworkElement window = GetWindowParent(p);
                var w = window as Window;
                if (w != null)
                {
                    if(w.WindowState != WindowState.Minimized)
                    {
                        w.WindowState = WindowState.Minimized;
                    }
                }
            });
        }

        FrameworkElement GetWindowParent(UserControl p)
        {
            FrameworkElement t = p;

            while(t.Parent != null)
            {
                t = t.Parent as FrameworkElement;
            }

            return t;
        }
    }
}
