using Rieltors.ADO;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rieltors.Pages
{
    public partial class EditProfilePage : Page
    {
        private int _userId;
        private Users _currentUser;

        public EditProfilePage(MainWindow mw, int userId)
        {
            InitializeComponent();

            _userId = userId;
            _currentUser = ConnectionDb.db.Users.FirstOrDefault(u => u.UserID == _userId);
            MessageBox.Show(_currentUser.Email);
        }

        private void EditProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserData();
        }

        private void LoadUserData()
        {
            _currentUser = ConnectionDb.db.Users.FirstOrDefault(u => u.UserID == _userId);

            if (_currentUser != null)
            {
                FirstNameTextBox.Text = _currentUser.FirstName;
                LastNameTextBox.Text = _currentUser.LastName;
                EmailTextBox.Text = _currentUser.Email;
                PhoneNumberTextBox.Text = _currentUser.PhoneNumber;
            }
            else
            {
                MessageBox.Show("Пользователь не найден.");
                NavigationService?.GoBack();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null)
            {
                _currentUser.FirstName = FirstNameTextBox.Text;
                _currentUser.LastName = LastNameTextBox.Text;
                _currentUser.Email = EmailTextBox.Text;
                _currentUser.PhoneNumber = PhoneNumberTextBox.Text;

                try
                {
                    ConnectionDb.db.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены.");
                    NavigationService?.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Пользователь не найден.");
                NavigationService?.GoBack();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
