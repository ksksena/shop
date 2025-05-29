using Shop.Pages;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace shop.Pages
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Page
    {
        public Authorization()
        {
            InitializeComponent();
            TBLogin.Focus();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
                try
                {
                var userObj = ApplicationData.AppConnect.Model1.Users.FirstOrDefault(x => x.login == TBLogin.Text && x.password == PBPassword.Password);
                if (userObj == null)
                {
                    MessageBox.Show("Такого пользователя нет", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (userObj != null)
                {
                   
                    string fio = $"{userObj.FIO}";
                    var role = ApplicationData.AppConnect.Model1.Roles.FirstOrDefault(r => r.roleID == userObj.roleID)?.role ?? "Не определена";
                    MessageBox.Show($"Здравствуйте, {fio}. Ваша роль {role}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (role == "Администратор")
                    {
                        NavigationService.Navigate(new DataOutputAdmin());
                    }
                    else if (role == "Клиент")
                    {
                        NavigationService.Navigate(new DataOutputUser(userObj.userID));
                    }
                    else
                    {
                        MessageBox.Show("Неизвестная роль. Обратитесь к администратору.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка" + ex.Message.ToString(), "Критическая ошибка приложения", MessageBoxButton.OK, MessageBoxImage.Information);
                }


        }

        private void ButtonRegistr_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Registration());
        }

    }

}
