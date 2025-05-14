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
        public partial class CreateRequestPage : Page
        {
            private int _clientId;
            private MainWindow _mw;
            public CreateRequestPage(MainWindow mw, int clientId)
            {
                InitializeComponent();
                _clientId = clientId;
                _mw = mw;
            }

            private void CreateButton_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    // Создание нового обращения
                    var newRequest = new ADO.Requests()
                    {

                        RequestDate = RequestDatePicker.SelectedDate ?? DateTime.Now, // Если дата не выбрана, используем текущую
                        RequestType = RequestTypeTextBox.Text,
                        PropertyType = PropertyTypeTextBox.Text,
                        Location = LocationTextBox.Text,
                        PriceMin = Convert.ToDecimal(string.IsNullOrEmpty(PriceMinTextBox.Text) ? "0" : PriceMinTextBox.Text),
                        PriceMax = Convert.ToDecimal(string.IsNullOrEmpty(PriceMaxTextBox.Text) ? "0" : PriceMaxTextBox.Text),
                        AreaMin = Convert.ToDecimal(string.IsNullOrEmpty(AreaMinTextBox.Text) ? "0" : AreaMinTextBox.Text),
                        AreaMax = Convert.ToDecimal(string.IsNullOrEmpty(AreaMaxTextBox.Text) ? "0" : AreaMaxTextBox.Text),
                        Description = DescriptionTextBox.Text,
                        Status = StatusTextBox.Text,
                        Notes = NotesTextBox.Text
                    };

                    // Добавление в базу данных и сохранение изменений
                    ConnectionDb.db.Requests.Add(newRequest);
                    ConnectionDb.db.SaveChanges();

                    MessageBox.Show("Обращение успешно создано!");
                    NavigationService?.GoBack(); // Возвращаемся на предыдущую страницу
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании обращения: {ex.Message}");
                }
            }

            private void CancelButton_Click(object sender, RoutedEventArgs e)
            {
                NavigationService?.GoBack(); // Возвращаемся на предыдущую страницу
            }
        }
    }
