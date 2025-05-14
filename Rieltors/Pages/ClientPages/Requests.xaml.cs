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

namespace Rieltors.Pages.ClientPages
{
    /// <summary>
    /// Логика взаимодействия для Requests.xaml
    /// </summary>
    public partial class Requests : Page
    {
        private MainWindow _mw;
        private int _userId;

        public Requests(MainWindow mw, int userId)
        {
            InitializeComponent();
            _mw = mw;
            _userId = userId;
            LoadUserRequests(); // Загружаем данные при инициализации страницы
        }

        private void LoadUserRequests()
        {
            try
            {
                // Получаем все обращения, связанные с этим пользователем (ClientID)
                var requests = ADO.ConnectionDb.db.Requests
                    .Where(r => r.ClientID == _userId)
                    .ToList();

                // Устанавливаем ItemsSource для ListView
                RequestsListView.ItemsSource = requests;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке обращений: {ex.Message}");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Реализовать логику удаления обращения
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Реализовать логику редактирования обращения
        }
    }
}
