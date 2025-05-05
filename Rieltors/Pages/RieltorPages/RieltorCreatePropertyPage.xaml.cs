using Microsoft.Win32;
using Rieltors.ADO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Логика взаимодействия для RieltorCreatePropertyPage.xaml
    /// </summary>
    public partial class RieltorCreatePropertyPage : Page
    {
        private MainWindow _mw;
        private int _userId;
        public ObservableCollection<ImageItem> ImageList { get; set; } = new ObservableCollection<ImageItem>();
        private string _propertyImagesDirectory; // Папка для хранения изображений

        public RieltorCreatePropertyPage(MainWindow mw, int userId)
        {
            InitializeComponent();
            _mw = mw;
            _userId = userId;
            DataContext = this;

            // Определяем путь к папке PropertyImages.  Важно использовать AppDomain.CurrentDomain.BaseDirectory для получения базового пути приложения.
            _propertyImagesDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PropertiesImages");

            // Создаем папку, если ее не существует
            if (!Directory.Exists(_propertyImagesDirectory))
            {
                Directory.CreateDirectory(_propertyImagesDirectory);
            }
        }
        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                // Генерируем уникальное имя файла для сохранения в папке проекта
                string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(filePath);
                string destinationPath = System.IO.Path.Combine(_propertyImagesDirectory, fileName);

                // Копируем файл в папку PropertyImages
                try
                {
                    File.Copy(filePath, destinationPath);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(destinationPath, UriKind.Relative); //Указываем относительный Uri
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    ImageList.Add(new ImageItem { ImageSource = bitmap, ImagePath = fileName, Description = "Описание изображения" }); // Сохраняем только имя файла
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании файла: {ex.Message}");
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверки на заполненность полей (добавьте необходимые проверки)
                if (string.IsNullOrEmpty(PropertyTypeComboBox.Text) ||
                    string.IsNullOrEmpty(AddressTextBox.Text) ||
                    string.IsNullOrEmpty(CityTextBox.Text) ||
                    string.IsNullOrEmpty(RegionTextBox.Text) ||
                    string.IsNullOrEmpty(PostalCodeTextBox.Text) ||
                    string.IsNullOrEmpty(PriceTextBox.Text) ||
                    string.IsNullOrEmpty(AreaTextBox.Text) ||
                    string.IsNullOrEmpty(DescriptionTextBox.Text) ||
                    string.IsNullOrEmpty(PropertyStatusComboBox.Text) ||
                    string.IsNullOrEmpty(DealTypeComboBox.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.");
                    return;
                }

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || !decimal.TryParse(AreaTextBox.Text, out decimal area))
                {
                    MessageBox.Show("Некорректный формат цены или площади.  Используйте числовой формат.");
                    return;
                }
                // 1. Создание объекта недвижимости
                var newProperty = new ADO.Properties
                {
                    PropertyType = PropertyTypeComboBox.Text,
                    Address = AddressTextBox.Text,
                    City = CityTextBox.Text,
                    Region = RegionTextBox.Text,
                    PostalCode = PostalCodeTextBox.Text,
                    Price = decimal.Parse(PriceTextBox.Text),
                    Area = decimal.Parse(AreaTextBox.Text),
                    Description = DescriptionTextBox.Text,
                    PropertyStatus = PropertyStatusComboBox.Text,
                    AddedDate = DateTime.Now,
                    RealtorID = _userId, // Заглушка, потом изменить
                    RealEstateTransactions = DealTypeComboBox.Text
                };

                ConnectionDb.db.Properties.Add(newProperty);

                // 2. Сохранение информации об изображениях
                foreach (var imageItem in ImageList)
                {
                    // Создаем запись в таблице PropertyImages
                    var newPropertyImage = new PropertyImages
                    {
                        PropertyID = newProperty.PropertyID,
                        ImageURL = imageItem.ImagePath, // Сохраняем ТОЛЬКО имя файла (относительный путь)
                        Description = imageItem.Description
                    };
                    ConnectionDb.db.PropertyImages.Add(newPropertyImage);
                }
                await ConnectionDb.db.SaveChangesAsync(); // Сохраняем изображения

                MessageBox.Show("Недвижимость успешно добавлена!");
                _mw.MainFrame.NavigationService.Navigate(new PropertiesPage(_mw, _userId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
            }
        }
    }

    public class ImageItem
    {
        public BitmapImage ImageSource { get; set; }
        public string ImagePath { get; set; } // Сохраняем только имя файла (относительный путь).  Важно для переносимости!
        public string Description { get; set; }
    }
}
