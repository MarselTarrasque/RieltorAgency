using Rieltors.Pages;
using Rieltors.Pages.RieltorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rieltors.NavPanels
{
    /// <summary>
    /// Логика взаимодействия для RieltorNavPanel.xaml
    /// </summary>
    public partial class RieltorNavPanel : Page
    {
        private MainWindow _mw;
        private int _userId;
        public RieltorNavPanel(MainWindow mw, int userId)
        {
            InitializeComponent();
            _mw = mw;
            _userId = userId;
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.NavigationService.Navigate(new ProfilePage(_mw, _userId));
        }

        private void PropertiesButton_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.NavigationService.Navigate(new Pages.PropertiesPage(_mw, _userId));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.NavigationService.Navigate(null);
            _mw.NavPanel.NavigationService.Navigate(null);
            _mw.RegisterFrame.NavigationService.Navigate(new RegistrationPage(_mw));
        }

        private void CreatePropertyButton_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.NavigationService.Navigate(new RieltorCreatePropertyPage(_mw, _userId));
        }

        private void AllClientsButton_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.NavigationService.Navigate(new AllClients(_mw, _userId));
        }
    }
}
