using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Rieltors.ADO;

namespace Rieltors.Pages
{
    /// <summary>
    /// Логика взаимодействия для PropertyPage.xaml
    /// </summary>
    public partial class PropertyPage : Page
    {
        private MainWindow _mw;
        private int _userId;
        private int _propertyId;
        private List<string> _imageUrls; // Список URL изображений
        private int _currentImageIndex = 0; // Индекс текущего изображения
        private string _propertyImagesDirectory;
        private ADO.Properties _property;
        private List<BitmapImage> _images;
        private ADO.Users _rieltor;

        public PropertyPage(MainWindow mw, int userId, int propertyId)
        {
            InitializeComponent();
            _mw = mw;
            _userId = userId;
            _propertyId = propertyId;
            _images = new List<BitmapImage>();
            LoadButtons();
            _propertyImagesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PropertyImages");

            LoadPropertyData();
            LoadImages();
            UpdateImageDisplay();
        }

        private void LoadButtons()
        {
            var _user = ConnectionDb.db.Users.FirstOrDefault(u => u.UserID == _userId);
            if(_user.UserType == "Клиент")
            {
                EditPanel.Visibility = Visibility.Hidden;
                DeletePanel.Visibility = Visibility.Hidden;
            }
        }
        private void LoadPropertyData()
        {
            try
            {
                // Получаем данные о недвижимости из базы данных
                _property = ConnectionDb.db.Properties
                    .FirstOrDefault(p => p.PropertyID == _propertyId);

                if (_property == null)
                {
                    MessageBox.Show("Не удалось найти информацию о недвижимости", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    _mw.MainFrame.GoBack();
                    return;
                }

                // Устанавливаем контекст данных для привязок
                DataContext = _property;

                // Обновляем заголовок страницы

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _mw.MainFrame.GoBack();
            }
        }

        private void LoadImages()
        {
            // Получаем список имен файлов изображений для данной недвижимости из базы данных
            _imageUrls = ConnectionDb.db.PropertyImages
                .Where(pi => pi.PropertyID == _propertyId)
                .OrderBy(pi => pi.ImageID) // Сортируем по ImageID (как и при выводе в ListView)
                .Select(pi => pi.ImageURL)
                .ToList();

            if (_imageUrls == null || _imageUrls.Count == 0)
            {
                _imageUrls = new List<string> { "/Images/NoImage.png" };
            }

            foreach (var imageUrl in _imageUrls)
            {
                string imagePath = imageUrl.StartsWith("/") ? imageUrl : Path.Combine(_propertyImagesDirectory, imageUrl);
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    _images.Add(bitmap);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}");
                    _images.Add(new BitmapImage(new Uri("/Images/NoImage.png", UriKind.Relative)));
                }
            }
        }

        private void UpdateImageDisplay()
        {
            if (_images.Count > 0)
            {
                MainImage.Source = _images[_currentImageIndex];
                // Обновляем Tag у Image для отображения номера
                MainImage.Tag = $"{_currentImageIndex + 1}/{_images.Count}";
            }
            else
            {
                MainImage.Source = null;
                MainImage.Tag = "0/0";
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (_images.Count > 0)
            {
                _currentImageIndex = (_currentImageIndex - 1 + _images.Count) % _images.Count;
                UpdateImageDisplay();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_images.Count > 0)
            {
                _currentImageIndex = (_currentImageIndex + 1) % _images.Count;
                UpdateImageDisplay();
            }
        }

        private void ContactButton_Click(object sender, RoutedEventArgs e)
        {
            // Здесь должен быть код для связи с риэлтором
            _rieltor = ConnectionDb.db.Users
                    .FirstOrDefault(p => p.UserID == _property.RealtorID);
            MessageBox.Show($"Контактная информация:\nТелефон: {_rieltor.PhoneNumber} \nEmail:  {_rieltor.Email}",
                          "Контактная информация",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.GoBack();
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            _mw.MainFrame.NavigationService.Navigate(new RieltorPages.RieltorEditPropertyPage(_mw, _propertyId, _userId));
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var PROP = ConnectionDb.db.Properties
                .FirstOrDefault(p => p.PropertyID == _property.PropertyID);

            if (PROP != null)
            {
                // Удаляем пользователя
                ConnectionDb.db.Properties.Remove(PROP);
                // Сохраняем изменения
                ConnectionDb.db.SaveChanges();
                _mw.MainFrame.NavigationService.Navigate(new PropertiesPage(_mw, _userId));
            }
        }
    }
}