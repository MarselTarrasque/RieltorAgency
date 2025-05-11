using Microsoft.Win32;
using Rieltors.ADO;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Rieltors.Pages.RieltorPages
{
    public partial class RieltorCreatePropertyPage : Page
    {
        private MainWindow _mw;
        private int _userId;
        public ObservableCollection<ImageItem> ImageList { get; set; } = new ObservableCollection<ImageItem>();
        private string _propertyImagesDirectory = @"C:\Users\Марсель\source\repos\MarselTarrasque\RieltorAgency\Rieltors\Images\PropertiesImages\"; // Full path

        public RieltorCreatePropertyPage(MainWindow mw, int userId)
        {
            InitializeComponent();
            _mw = mw;
            _userId = userId;
            DataContext = this;

            // Ensure directory exists
            if (!Directory.Exists(_propertyImagesDirectory))
            {
                Directory.CreateDirectory(_propertyImagesDirectory);
            }

            // Check permissions
            CheckDirectoryAccess();
        }

        private void CheckDirectoryAccess()
        {
            try
            {
                string testFilePath = Path.Combine(_propertyImagesDirectory, "test.txt");
                File.WriteAllText(testFilePath, "Проверка прав доступа");
                File.Delete(testFilePath);
                Console.WriteLine("Права доступа есть!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Нет прав доступа: {ex.Message}");
                MessageBox.Show($"Нет прав доступа для записи в папку {_propertyImagesDirectory}. Обратитесь к администратору.");
            }
        }

        private void AddImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                MessageBox.Show($"Selected File Path: {filePath}");

                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"ERROR: File does not exist: {filePath}");
                    return;
                }

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(filePath);
                string destinationPath = Path.Combine(_propertyImagesDirectory, fileName); // Full path

                MessageBox.Show($"Destination Directory: {_propertyImagesDirectory}");
                MessageBox.Show($"Destination File Path: {destinationPath}");

                try
                {
                    File.Copy(filePath, destinationPath);
                    MessageBox.Show("File copied successfully!");

                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(destinationPath); // Full path
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    ImageList.Add(new ImageItem
                    {
                        ImageSource = bitmap,
                        ImagePath = fileName, // Store just the filename
                        Description = "Описание изображения"
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ERROR copying file: {ex.Message}\nStack Trace: {ex.StackTrace}");
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Basic field validation
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
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || !decimal.TryParse(AreaTextBox.Text, out decimal area))
                {
                    MessageBox.Show("Некорректный формат цены или площади. Используйте числовой формат.");
                    return;
                }

                // Create new property object
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
                    RealtorID = _userId,
                    RealEstateTransactions = DealTypeComboBox.Text
                };

                ConnectionDb.db.Properties.Add(newProperty);

                // Save image info
                foreach (var imageItem in ImageList)
                {
                    var newPropertyImage = new PropertyImages
                    {
                        PropertyID = newProperty.PropertyID,
                        ImageURL = imageItem.ImagePath, // Store only filename
                        Description = imageItem.Description
                    };
                    ConnectionDb.db.PropertyImages.Add(newPropertyImage);
                }

                await ConnectionDb.db.SaveChangesAsync();

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
        public string ImagePath { get; set; } // Store only filename for DB
        public string Description { get; set; }
    }
}
