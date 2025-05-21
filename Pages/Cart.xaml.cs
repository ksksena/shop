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
using shop.ApplicationData;

namespace Shop.Pages
{
    public partial class Cart : Page
    {
        private shopEntities1 context;
        private int currentUserId;

        public Cart(int userId)
        {
            InitializeComponent();
            context = new shopEntities1();
            currentUserId = userId;
            UpdateCartDisplay();
        }

        private void UpdateCartDisplay()
        {
            listCart.ItemsSource = null;
            listCart.ItemsSource = CartManager.Cart;
            decimal total = CartManager.Cart.Sum(item => item.Quantity * item.Price);
            TextCartTotal.Text = $"Общая сумма: {total:F2}";
        }

        private void RemoveFromCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                int productId = (int)button.Tag;
                var cartItem = CartManager.Cart.FirstOrDefault(c => c.ProductId == productId);
                if (cartItem != null)
                {
                    CartManager.Cart.Remove(cartItem);
                    UpdateCartDisplay();
                    MessageBox.Show($"Товар удален из корзины!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (CartManager.Cart.Count == 0)
            {
                MessageBox.Show("Корзина пуста. Добавьте товары перед оформлением заказа.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var newOrder = new Orders
                {
                    userID = currentUserId,
                    orderDate = DateTime.Now,
                    totalAmount = CartManager.Cart.Sum(item => item.Quantity * item.Price)
                };

                context.Orders.Add(newOrder);
                context.SaveChanges();

                foreach (var item in CartManager.Cart)
                {
                    var orderDetail = new OrderDetails
                    {
                        orderID = newOrder.orderID,
                        prodID = item.ProductId,
                        quantity = item.Quantity,
                        unitPrice = item.Price
                    };
                    context.OrderDetails.Add(orderDetail);
                }

                context.SaveChanges();

                MessageBox.Show("Заказ успешно оформлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                CartManager.Cart.Clear();
                UpdateCartDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DataOutputUser(currentUserId));
        }
    }
}