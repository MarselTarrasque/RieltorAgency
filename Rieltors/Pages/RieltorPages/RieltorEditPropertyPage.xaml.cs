using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using Rieltors.ADO;

namespace Rieltors.Pages.RieltorPages
{
    public partial class RieltorEditPropertyPage : Page
    {
        private MainWindow _mw;
        private int _propertyId;
        private ADO.Properties _property;
        private int _userId;
        public ObservableCollection<ImageItem> ImageList { get; set; } = new ObservableCollection<ImageItem>();
        private string _propertyImagesDirectory;

        public RieltorEditPropertyPage(MainWindow mw, int propertyId, int userId)
        {
            InitializeComponent();
            _mw = mw;
            _propertyId = propertyId;
            _userId = userId;
            DataContext = this;

            // Путь к папке PropertiesImages (относительный путь!)
            _propertyImagesDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "PropertiesImages");

            // Создаем папку, если ее не существует
            if (!Directory.Exists(_propertyImagesDirectory))
            {
                Directory.CreateDirectory(_propertyImagesDirectory);
            }

            LoadPropertyData();
        }

        private void LoadPropertyData()
        {
            _property = ConnectionDb.db.Properties.FirstOrDefault(p => p.PropertyID == _propertyId);

            if (_property != null)
            {
                // Заполняем поля формы данными из объекта недвижимости
                PropertyTypeComboBox.Text = _property.PropertyType;
                AddressTextBox.Text = _property.Address;
                CityTextBox.Text = _property.City;
                RegionTextBox.Text = _property.Region;
                PostalCodeTextBox.Text = _property.PostalCode;
                PriceTextBox.Text = _property.Price.ToString();
                AreaTextBox.Text = _property.Area.ToString();
                DescriptionTextBox.Text = _property.Description;
                PropertyStatusComboBox.Text = _property.PropertyStatus;
                DealTypeComboBox.Text = _property.RealEstateTransactions;

                // Заполняем ComboBox'ы
                SetComboBoxSelectedItem(PropertyTypeComboBox, _property.PropertyType);
                SetComboBoxSelectedItem(PropertyStatusComboBox, _property.PropertyStatus);
                SetComboBoxSelectedItem(DealTypeComboBox, _property.RealEstateTransactions);

                // Загрузка изображений из базы данных
                LoadImagesFromDatabase();
            }
            else
            {
                MessageBox.Show("Недвижимость не найдена.");
                _mw.MainFrame.NavigationService.GoBack();
            }
        }

        private void SetComboBoxSelectedItem(ComboBox comboBox, string value)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Content.ToString() == value)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void LoadImagesFromDatabase()
        {
            try
            {
                var propertyImages = ConnectionDb.db.PropertyImages.Where(pi => pi.PropertyID == _propertyId).ToList();

                ImageList.Clear();

                foreach (var propertyImage in propertyImages)
                {
                    // ImageURL хранит только имя файла!
                    string fileName = propertyImage.ImageURL;

                    // Собираем полный путь к файлу
                    string imagePath = System.IO.Path.Combine(_propertyImagesDirectory, fileName);

                    // Проверяем, существует ли файл
                    if (File.Exists(imagePath))
                    {
                        // Создаем BitmapImage
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(imagePath); // Use the full image path
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();

                        // Добавляем ImageItem в ImageList
                        ImageList.Add(new ImageItem { ImageSource = bitmap, ImagePath = fileName, Description = propertyImage.Description });
                    }
                    else
                    {
                        MessageBox.Show($"Файл изображения не найден: {imagePath}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке изображений: {ex.Message}");
            }
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                // Генерируем уникальное имя файла
                string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(filePath);
                string destinationPath = System.IO.Path.Combine(_propertyImagesDirectory, fileName);

                try
                {
                    File.Copy(filePath, destinationPath);
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(destinationPath); // Use the full image path
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImageList.Add(new ImageItem { ImageSource = bitmap, ImagePath = fileName, Description = "Описание изображения" });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при копировании файла: {ex.Message}");
                }
            }
        }

        private void DeleteImageButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранный элемент из ListView
            var selectedImage = ImagesListView.SelectedItem as ImageItem;

            if (selectedImage != null)
            {
                try
                {
                    // 1. Удаление файла с диска
                    string fullPath = System.IO.Path.Combine(_propertyImagesDirectory, selectedImage.ImagePath); // Correct path creation

                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                    else
                    {
                        MessageBox.Show($"Файл не найден: {fullPath}");
                    }

                    // 2. Удаление из ImageList (отображение)
                    ImageList.Remove(selectedImage);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении файла: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите изображение для удаления.");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 1. Обновление объекта недвижимости
                _property.PropertyType = PropertyTypeComboBox.Text;
                _property.Address = AddressTextBox.Text;
                _property.City = _property.City;
                _property.Region = _property.Region;
                _property.PostalCode = _property.PostalCode;
                _property.Price = decimal.Parse(PriceTextBox.Text);
                _property.Area = decimal.Parse(AreaTextBox.Text);
                _property.Description = _property.Description;
                _property.PropertyStatus = PropertyStatusComboBox.Text;
                _property.RealEstateTransactions = DealTypeComboBox.Text;

                ConnectionDb.db.SaveChanges();

                // 2. Обновление изображений
                // Сначала удаляем старые изображения из базы данных
                var oldImages = ConnectionDb.db.PropertyImages.Where(pi => pi.PropertyID == _propertyId).ToList();
                ConnectionDb.db.PropertyImages.RemoveRange(oldImages);
                ConnectionDb.db.SaveChanges();

                // Затем добавляем новые изображения из ImageList
                foreach (var imageItem in ImageList)
                {
                    string fileName = imageItem.ImagePath;  // we store filename in ImagePath

                    var newPropertyImage = new ADO.PropertyImages
                    {
                        PropertyID = _propertyId,
                        ImageURL = fileName, // Сохраняем только имя файла
                        Description = imageItem.Description
                    };
                    ConnectionDb.db.PropertyImages.Add(newPropertyImage);
                }

                ConnectionDb.db.SaveChanges();
                MessageBox.Show("Недвижимость успешно обновлена!");
                _mw.MainFrame.NavigationService.Navigate(new PropertiesPage(_mw, _userId));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении недвижимости: {ex.Message}");
            }
        }
    }
}
