using Rieltors.ADO;
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

namespace Rieltors.Pages.RieltorPages
{
    /// <summary>
    /// Логика взаимодействия для AllClients.xaml
    /// </summary>
    public partial class AllClients : Page
    {
        private MainWindow _mw;
        private int _userId;
        public AllClients(MainWindow mw, int userId)
        {
            InitializeComponent();
            _mw = mw;
            _userId = userId;
            LoadUsers();
        }
        private void LoadUsers()
        {
            try
            {
                var users_filter = ConnectionDb.db.Users
                                  .Where(p => p.UserType == "Клиент") 
                                  .ToList();

                
                var users = users_filter.Select(u => new Users
                {
                    UserID = u.UserID, 
                    FirstName = u.FirstName, 
                    LastName = u.LastName, 
                    Email = u.Email, 
                    PhoneNumber = u.PhoneNumber,
                    IsActive = Convert.ToBoolean(u.IsActive)
                }).ToList();
                UserListView.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (UserListView.SelectedItem is Users selectedUser)
            {
                _mw.MainFrame.NavigationService.Navigate(new ProfilePage(_mw, selectedUser.UserID));
            }
        }
    }
}
