using Microsoft.Win32;
using Rieltors.ADO;
using Rieltors.Windows;
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

namespace Rieltors.Pages
{
    /// <summary>
    /// Логика взаимодействия для RieltorPropertiesPage.xaml
    /// </summary>
    public partial class PropertiesPage : Page
    {
        private MainWindow _mw;
        private int _userId;
        private string _propertyImagesDirectory; // Папка для хранения изображений
        public PropertiesPage(MainWindow mw, int userId)
        {
            InitializeComponent();
            _mw = mw;
            _userId = userId;
            _propertyImagesDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PropertyImages");
            LoadProperties();
        }
        private void LoadProperties()
        {
            try
            {
                var properties = ConnectionDb.db.Properties.ToList();

                var propertiesWithImages = properties.Select(p => new PropertiesWithFirstImage
                {
                    PropertyID = p.PropertyID,
                    PropertyType = p.PropertyType,
                    Address = p.Address,
                    City = p.City,
                    Region = p.Region,
                    PostalCode = p.PostalCode,
                    Price = p.Price,
                    Area = p.Area,
                    FirstImage = GetFirstImage(p.PropertyID)
                }).ToList();

                PropertyListListView.ItemsSource = propertiesWithImages;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private PropertyImages GetFirstImage(int propertyId)
        {
            var images = ConnectionDb.db.PropertyImages.Where(img => img.PropertyID == propertyId).OrderBy(img => img.ImageID).ToList();

            if (images.Any())
            {
                return images.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }
        private void PropertyListListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var selectedProperty = PropertyListListView.SelectedItem as PropertiesWithFirstImage;

            if (selectedProperty != null)
            {

                int propertyId = selectedProperty.PropertyID;

                NavigationService?.Navigate(new PropertyPage(_mw, _userId, propertyId));
            }
        }

        private void PreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            var preferencesWindow = new PreferencesWindow(_userId);
            preferencesWindow.ShowDialog();
            LoadProperties();
        }
    }

    public class PropertiesWithFirstImage : ADO.Properties
    {
        private string _propertyImagesDirectory = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PropertiesImages");
        private PropertyImages _firstImage;
        public PropertyImages FirstImage
        {
            get
            {
                if (_firstImage != null && !string.IsNullOrEmpty(_firstImage.ImageURL))
                {
                    string imagePath = System.IO.Path.Combine(_propertyImagesDirectory, _firstImage.ImageURL);
                    if (File.Exists(imagePath))
                    {   
                        return new PropertyImages { ImageURL = imagePath, Description = _firstImage.Description };
                    }
                    else
                    {
                        return new PropertyImages { ImageURL = "/Images/NoImage.png", Description = "Изображение не найдено" };
                    }

                }
                else
                {
                    return new PropertyImages { ImageURL = "/Images/NoImage.png", Description = "Нет изображения" };
                }
            }
            set
            {
                _firstImage = value;
            }
        }
    }
}
