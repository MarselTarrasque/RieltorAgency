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
    /// Логика взаимодействия для EditProfilePage.xaml
    /// </summary>
    public partial class EditProfilePage : Page
    {
        private int _userId;
        private MainWindow _mw;
        private Users _currentUser;  // Добавлено поле для хранения текущего пользователя

        public EditProfilePage(MainWindow mw, int userId)
        {
            InitializeComponent();
            _mw = mw;
            _userId = userId;
            _currentUser = ConnectionDb.db.Users.FirstOrDefault(u => u.UserID == userId); // Получаем пользователя и сохраняем в поле

            if (_currentUser != null)
            {
                // Заполняем элементы управления данными пользователя
                FirstNameTextBox.Text = _currentUser.FirstName;
                LastNameTextBox.Text = _currentUser.LastName;
                EmailTextBox.Text = _currentUser.Email;
                PhoneNumberTextBox.Text = _currentUser.PhoneNumber;
            }
            else
            {
                MessageBox.Show("Пользователь не найден.");
                NavigationService?.GoBack();  // Возвращаемся на предыдущую страницу, если пользователь не найден.
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем данные пользователя
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
                    NavigationService?.GoBack(); // Возвращаемся на предыдущую страницу после сохранения
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
