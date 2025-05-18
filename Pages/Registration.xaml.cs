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

            private void ButtonExit_Click(object sender, RoutedEventArgs e)
            {
                this.NavigationService.Navigate(new Authorization());
            }
        }
    }
