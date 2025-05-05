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

namespace Rieltors.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private MainWindow _mw;
        private int _userId;
        public ProfilePage(MainWindow mw, int userId)
        {
            InitializeComponent();
            _mw = mw;
            _userId = userId;
            var user = ConnectionDb.db.Users.FirstOrDefault(u => u.UserID == _userId);
            FirstName.Text = user.FirstName;
            SecondName.Text = user.LastName;
            DateOfRegister.Text = Convert.ToString(user.RegistrationDate);
            PhoneNumber.Text = user.PhoneNumber;
            Role.Text = user.UserType;
            Email.Text = user.Email;
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.NavigationService.Navigate(new EditProfilePage(_mw, _userId));
        }
    }
}
