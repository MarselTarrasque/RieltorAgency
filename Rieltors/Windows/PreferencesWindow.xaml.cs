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
using System.Windows.Shapes;

namespace Rieltors.Windows
{
    /// <summary>
    /// Логика взаимодействия для PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        private int _userId;


        public PreferencesWindow(int userId)
        {
            InitializeComponent();
            _userId = userId;
        
            LoadPreferences(); // Загружаем предпочтения при инициализации окна
        }

        private void LoadPreferences()
        {
            try
            {
                var preferences = ConnectionDb.db.ClientPreferences.FirstOrDefault(p => p.ClientID == _userId);

                if (preferences != null)
                {
                    // Заполняем элементы управления значениями из базы данных

                    // PropertyType
                    if (!string.IsNullOrEmpty(preferences.PropertyType))
                    {
                        // Находим ComboBoxItem с соответствующим Content
                        foreach (ComboBoxItem item in PropertyTypeComboBox.Items)
                        {
                            if (item.Content.ToString() == preferences.PropertyType)
                            {
                                PropertyTypeComboBox.SelectedItem = item;
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(preferences.Status))
                    {
                        // Находим ComboBoxItem с соответствующим Content
                        foreach (ComboBoxItem item in PropertyStatusComboBox.Items)
                        {
                            if (item.Content.ToString() == preferences.Status)
                            {
                                PropertyStatusComboBox.SelectedItem = item;
                                break;
                            }
                        }
                    }
                    // MinPrice
                    MinPriceTextBox.Text = preferences.MinPrice?.ToString();

                    // MaxPrice
                    MaxPriceTextBox.Text = preferences.MaxPrice?.ToString();

                    // MinArea
                    MinAreaTextBox.Text = preferences.MinArea?.ToString();

                    // MaxArea
                    MaxAreaTextBox.Text = preferences.MaxArea?.ToString();

                    // Location
                    LocationTextBox.Text = preferences.Location;

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке предпочтений: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            string propertyType = (PropertyTypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string minPriceText = MinPriceTextBox.Text;
            string maxPriceText = MaxPriceTextBox.Text;
            string minAreaText = MinAreaTextBox.Text;
            string maxAreaText = MaxAreaTextBox.Text;
            string location = LocationTextBox.Text;
            string status = (PropertyStatusComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            // Проверка на пустые строки и некорректный ввод
            if (!ValidateInput(minPriceText, maxPriceText, minAreaText, maxAreaText))
            {
                return; // Прекратить выполнение, если валидация не пройдена
            }

            // Преобразование данных
            decimal? minPrice = string.IsNullOrEmpty(minPriceText) ? (decimal?)null : decimal.Parse(minPriceText);
            decimal? maxPrice = string.IsNullOrEmpty(maxPriceText) ? (decimal?)null : decimal.Parse(maxPriceText);
            double? minArea = string.IsNullOrEmpty(minAreaText) ? (double?)null : double.Parse(minAreaText);
            double? maxArea = string.IsNullOrEmpty(maxAreaText) ? (double?)null : double.Parse(maxAreaText);


            try
            {
                var Preferences = ConnectionDb.db.ClientPreferences.FirstOrDefault(p => p.ClientID == _userId);

                if (Preferences != null)
                {
                    // Обновление существующих предпочтений
                    Preferences.PropertyType = propertyType;
                    Preferences.MinPrice = minPrice;
                    Preferences.MaxPrice = maxPrice;
                    Preferences.MinArea = (decimal?)minArea;
                    Preferences.MaxArea = (decimal?)maxArea;
                    Preferences.Location = location;
                    Preferences.Status = status;

                    ConnectionDb.db.SaveChanges();

                    MessageBox.Show("Предпочтения успешно обновлены.");
                    this.Close();
                }
                else
                {
                    // Обработка ситуации, когда предпочтения не найдены (возможно, стоит создать новые)
                    MessageBox.Show("Предпочтения для данного клиента не найдены.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении предпочтений: {ex.Message}");
            }
        }

        // Метод валидации
        private bool ValidateInput(string minPriceText, string maxPriceText, string minAreaText, string maxAreaText)
        {
            // Проверка на числовой ввод
            if (!string.IsNullOrEmpty(minPriceText) && !decimal.TryParse(minPriceText, out _))
            {
                MessageBox.Show("Минимальная цена должна быть числом.");
                return false;
            }

            if (!string.IsNullOrEmpty(maxPriceText) && !decimal.TryParse(maxPriceText, out _))
            {
                MessageBox.Show("Максимальная цена должна быть числом.");
                return false;
            }

            if (!string.IsNullOrEmpty(minAreaText) && !double.TryParse(minAreaText, out _))
            {
                MessageBox.Show("Минимальная площадь должна быть числом.");
                return false;
            }

            if (!string.IsNullOrEmpty(maxAreaText) && !double.TryParse(maxAreaText, out _))
            {
                MessageBox.Show("Максимальная площадь должна быть числом.");
                return false;
            }

            // Дополнительные проверки (например, min < max) можно добавить здесь
            return true;
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}
