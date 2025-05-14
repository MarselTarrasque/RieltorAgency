using System;
using System.Windows;
using System.Windows.Controls;
using Rieltors.ADO;

namespace Rieltors.Pages.ClientPages
{
    /// <summary>
    /// Логика взаимодействия для RequestDetails.xaml
    /// </summary>
    public partial class RequestDetails : Page
    {
        private int _userId;
        private MainWindow _mainWindow;

        public RequestDetails(MainWindow mw, int userId)
        {
            InitializeComponent();
            _mainWindow = mw;
            _userId = userId;
            DataContext = this;
            SelectedRequest = new ADO.Requests
            {
                ClientID = _userId,
                RequestDate = DateTime.Now,
                Status = "Новое"
            };
        }

        public ADO.Requests SelectedRequest { get; set; }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ADO.ConnectionDb.db.Requests.Add(SelectedRequest);
                ADO.ConnectionDb.db.SaveChanges();

                MessageBox.Show("Обращение успешно создано!");
                _mainWindow.MainFrame.Navigate(new Requests(_mainWindow, _userId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании обращения: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.Navigate(new Requests(_mainWindow, _userId));
        }
    }
}
