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
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
