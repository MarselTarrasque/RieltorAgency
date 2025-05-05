using Rieltors;
using Rieltors.ADO;
using Rieltors.NavPanels;
using Rieltors.Pages;
using Rieltors.Pages.RieltorPages;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private MainWindow _mw;

        public RegistrationPage(MainWindow mw)
        {
            InitializeComponent();
            _mw = mw;

            // Валидация регистрации
            Binding emailBinding = new Binding("Email") { Source = this };
            emailBinding.ValidationRules.Add(new EmailValidationRule());
            BindingOperations.SetBinding(RegisterEmailTextBox, TextBox.TextProperty, emailBinding);

            Binding firstNameBinding = new Binding("FirstName") { Source = this };
            firstNameBinding.ValidationRules.Add(new RequiredValidationRule());
            BindingOperations.SetBinding(RegisterFirstNameTextBox, TextBox.TextProperty, firstNameBinding);

            Binding lastNameBinding = new Binding("LastName") { Source = this };
            lastNameBinding.ValidationRules.Add(new RequiredValidationRule());
            BindingOperations.SetBinding(RegisterLastNameTextBox, TextBox.TextProperty, lastNameBinding);

            Binding phoneBinding = new Binding("Phone") { Source = this };
            phoneBinding.ValidationRules.Add(new PhoneValidationRule());
            BindingOperations.SetBinding(RegisterPhoneTextBox, TextBox.TextProperty, phoneBinding);

            // Валидация входа
            Binding loginEmailBinding = new Binding("LoginEmail") { Source = this };
            loginEmailBinding.ValidationRules.Add(new RequiredValidationRule());
            loginEmailBinding.ValidationRules.Add(new EmailValidationRule());
            BindingOperations.SetBinding(LoginEmailTextBox, TextBox.TextProperty, loginEmailBinding);
        }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string LoginEmail { get; set; }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            if (Validation.GetHasError(LoginEmailTextBox))
            {
                isValid = false;
            }

            if (string.IsNullOrEmpty(LoginPasswordBox.Text))
            {
                isValid = false;
                MessageBox.Show("Пожалуйста, введите пароль.");
            }

            if (isValid)
            {
                var user = ConnectionDb.db.Users.FirstOrDefault(u => u.Email == LoginEmailTextBox.Text);
                if (user == null)
                {
                    MessageBox.Show("Пользователь с таким email не найден.");
                }
                else if (user.PasswordHash != LoginPasswordBox.Text)
                {
                    MessageBox.Show("Неверный пароль");
                }
                else
                {
                    _mw.RegisterFrame.NavigationService.Navigate(null);
                    MessageBox.Show($"Вход выполнен успешно! Добро пожаловать, {user.FirstName} {user.LastName}!");
                    if (user.UserType == "Клиент")
                    {
                        _mw.MainFrame.NavigationService.Navigate(new ProfilePage(_mw, user.UserID));
                        _mw.NavPanel.NavigationService.Navigate(new ClientNavPanel(_mw, user.UserID));
                    }
                    else if (user.UserType == "Риэлтор")
                    {
                        _mw.MainFrame.NavigationService.Navigate(new PropertiesPage(_mw, user.UserID));
                        _mw.NavPanel.NavigationService.Navigate(new RieltorNavPanel(_mw, user.UserID));
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, исправьте ошибки в полях.");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            if (Validation.GetHasError(RegisterEmailTextBox) || 
                Validation.GetHasError(RegisterFirstNameTextBox) || 
                Validation.GetHasError(RegisterLastNameTextBox) || 
                Validation.GetHasError(RegisterPhoneTextBox))
            {
                isValid = false;
            }

            if (string.IsNullOrEmpty(RegisterPasswordBox.Text))
            {
                isValid = false;
                MessageBox.Show("Пожалуйста, введите пароль.");
            }

            if (string.IsNullOrEmpty(RegisterConfirmPasswordBox.Text))
            {
                isValid = false;
                MessageBox.Show("Пожалуйста, подтвердите пароль.");
            }

            if (RegisterPasswordBox.Text != RegisterConfirmPasswordBox.Text)
            {
                isValid = false;
                MessageBox.Show("Пароли не совпадают!");
            }

            if (isValid)
            {
                if (ConnectionDb.db.Users.Any(u => u.Email == RegisterEmailTextBox.Text))
                {
                    MessageBox.Show("Пользователь с таким email уже зарегистрирован.");
                }
                else if (ConnectionDb.db.Users.Any(u => u.PhoneNumber == RegisterPhoneTextBox.Text))
                {
                    MessageBox.Show("Пользователь с таким номером телефона уже зарегистрирован.");
                }
                else
                {
                    var newUser = new Users
                    {
                        FirstName = RegisterFirstNameTextBox.Text,
                        LastName = RegisterLastNameTextBox.Text,
                        Email = RegisterEmailTextBox.Text,
                        PhoneNumber = RegisterPhoneTextBox.Text,
                        PasswordHash = RegisterConfirmPasswordBox.Text,
                        RegistrationDate = DateTime.Now,
                        IsActive = true,
                        UserType = ((ComboBoxItem)RegisterRoleComboBox.SelectedItem).Content.ToString()
                    };

                    var newClientPreferences = new ClientPreferences
                    {
                        ClientID = newUser.UserID
                    };

                    ConnectionDb.db.Users.Add(newUser);
                    ConnectionDb.db.ClientPreferences.Add(newClientPreferences);
                    ConnectionDb.db.SaveChanges();
                    MessageBox.Show("Пользователь успешно зарегистрирован!");
                    ClearRegistrationFields();
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, исправьте ошибки в полях.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearRegistrationFields();
        }

        private void ClearRegistrationFields()
        {
            RegisterFirstNameTextBox.Text = string.Empty;
            RegisterLastNameTextBox.Text = string.Empty;
            RegisterEmailTextBox.Text = string.Empty;
            RegisterPasswordBox.Text = string.Empty;
            RegisterConfirmPasswordBox.Text = string.Empty;
            RegisterPhoneTextBox.Text = string.Empty;
            RegisterRoleComboBox.SelectedIndex = -1;
        }

        public class EmailValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                string email = (string)value;
                if (string.IsNullOrEmpty(email))
                {
                    return new ValidationResult(false, "Email обязателен для заполнения.");
                }

                if (!IsValidEmail(email))
                {
                    return new ValidationResult(false, "Неверный формат email.");
                }

                return ValidationResult.ValidResult;
            }

            private bool IsValidEmail(string email)
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(email);
                    return addr.Address == email;
                }
                catch
                {
                    return false;
                }
            }
        }

        public class RequiredValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                string str = (string)value;
                if (string.IsNullOrEmpty(str))
                {
                    return new ValidationResult(false, "Это поле обязательно для заполнения.");
                }
                return ValidationResult.ValidResult;
            }
        }

        public class PhoneValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                string phone = (string)value;
                if (string.IsNullOrEmpty(phone))
                {
                    return new ValidationResult(false, "Телефон обязателен для заполнения.");
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d+$"))
                {
                    return new ValidationResult(false, "Телефон должен содержать только цифры.");
                }

                return ValidationResult.ValidResult;
            }
        }
    }
}