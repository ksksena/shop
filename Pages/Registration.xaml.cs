using shop.ApplicationData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using shop;


namespace shop.Pages
{
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
            public Registration()
            {
                InitializeComponent();
                ButtonRegist.IsEnabled = false;
            }

            private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
            {
                if (PBpassword.Password != TBPassword.Text)
                {
                    ButtonRegist.IsEnabled = false;
                    PBpassword.Background = Brushes.LightCoral;
                    PBpassword.BorderBrush = Brushes.Red;
                }
                else
                {
                    ButtonRegist.IsEnabled = true;
                    PBpassword.Background = Brushes.LightGreen;
                    PBpassword.BorderBrush = Brushes.Green;
                }
            }

            private void ButtonRegist_Click(object sender, RoutedEventArgs e)
            {
                string fio = TBName.Text;
                string login = TBLogin.Text;
                string password = TBPassword.Text;
                string passwordRepeat = PBpassword.Password;
                string email = TBEmail.Text;
                string telephone = TBTelephone.Text;

                if (string.IsNullOrWhiteSpace(fio) ||
                    string.IsNullOrWhiteSpace(login) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(passwordRepeat) ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(telephone))
                {
                    MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (password != passwordRepeat)
                {
                    MessageBox.Show("Пароли не совпадают!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                try
                {
                    using (var context = new shopEntities1())
                    {
                        var existingUser = context.Users.FirstOrDefault(u => u.login == login);
                        if (existingUser != null)
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var userRole = context.Roles.FirstOrDefault(r => r.role == "Клиент");
                        if (userRole == null)
                        {
                            MessageBox.Show("Роль 'Клиент' не найдена в системе!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var newUser = new Users
                        {
                            FIO = fio,
                            login = login,
                            password = password,
                            emailU = email,
                            phone = telephone,
                            roleID = userRole.roleID
                        };

                        context.Users.Add(newUser);
                        context.SaveChanges();

                        MessageBox.Show("Регистрация прошла успешно!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Очистка полей после успешной регистрации
                        TBName.Clear();
                        TBLogin.Clear();
                        TBPassword.Clear();
                        PBpassword.Clear();
                        TBEmail.Clear();
                        TBTelephone.Clear();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении данных: " + ex.Message, "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        private void TBTelephone_TextChanged(object sender, TextChangedEventArgs e)
        {
            string digitsOnly = new string(TBTelephone.Text.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length > 0 && digitsOnly[0] != '7' && digitsOnly[0] != '8')
            {
                digitsOnly = "7" + digitsOnly;
            }
            string formattedPhone = "+7 (";
            if (digitsOnly.Length > 1)
                formattedPhone += digitsOnly.Substring(1, Math.Min(3, digitsOnly.Length - 1));
            if (digitsOnly.Length > 4)
                formattedPhone += ") " + digitsOnly.Substring(4, Math.Min(3, digitsOnly.Length - 4));
            if (digitsOnly.Length > 7)
                formattedPhone += "-" + digitsOnly.Substring(7, Math.Min(2, digitsOnly.Length - 7));
            if (digitsOnly.Length > 9)
                formattedPhone += "-" + digitsOnly.Substring(9, Math.Min(2, digitsOnly.Length - 9));
            TBTelephone.TextChanged -= TBTelephone_TextChanged;
            TBTelephone.Text = formattedPhone;
            TBTelephone.CaretIndex = formattedPhone.Length;
            TBTelephone.TextChanged += TBTelephone_TextChanged;
        }

        private void TBTelephone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }
            string currentText = TBTelephone.Text.Replace("+7 (", "").Replace(")", "").Replace("-", "").Replace(" ", "");
            if (currentText.Length >= 11)
            {
                e.Handled = true;
            }
        }
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }
        private void ButtonExit_Click(object sender, RoutedEventArgs e)
            {
                this.NavigationService.Navigate(new Authorization());
            }
        }
    }
