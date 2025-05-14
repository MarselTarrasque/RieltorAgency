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

namespace Rieltors.Pages.ClientPages
{
    /// <summary>
    /// Логика взаимодействия для RequestPropertyDetails.xaml
    /// </summary>
        public partial class RequestPropertyDetails : Page
        {
            private int _userId;
            private MainWindow _mainWindow;
            private int _propertyId;

            public RequestPropertyDetails(MainWindow mw, int userId, int propertyId)
            {
                InitializeComponent();
                _mainWindow = mw;
                _userId = userId;
                _propertyId = propertyId;
                DataContext = this;
                SelectedRequest = new ADO.Requests
                {
                    ClientID = _userId,
                    RequestDate = DateTime.Now,
                    Status = "Новое"
                };
                InitializeProperty();
            }

            public ADO.Requests SelectedRequest { get; set; }
            public ADO.Properties SelectedProperty { get; set; }

            private void InitializeProperty()
            {
                // Получаем данные о недвижимости по ID
                SelectedProperty = ADO.ConnectionDb.db.Properties.FirstOrDefault(p => p.PropertyID == _propertyId);

                if (SelectedProperty != null)
                {
                    // Заполняем поля обращения данными из недвижимости
                    SelectedRequest.PropertyType = SelectedProperty.PropertyType;
                    SelectedRequest.Location = SelectedProperty.Address;

                    //Привязываем недвижимость к обращению
                    SelectedRequest.RequestProperties.Add(new RequestProperties { PropertyID = _propertyId, RequestID = SelectedRequest.RequestID });
                }
                else
                {
                    MessageBox.Show("Недвижимость не найдена.");
                    _mainWindow.MainFrame.Navigate(new Requests(_mainWindow, _userId)); // Возвращаемся на страницу с обращениями
                    return; // Прерываем дальнейшую инициализацию
                }
            }

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