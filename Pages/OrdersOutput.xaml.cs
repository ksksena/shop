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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Shop;
using shop.ApplicationData;
using shop.Pages;

namespace Shop.Pages
{
    public partial class OrdersOutput : Page
    {
        private shopEntities1 context;

        // Класс для отображения деталей заказа с вычисляемым свойством TotalPrice
        public class OrderDetailViewModel
        {
            public OrderDetails OrderDetail { get; set; }
            public Products Products { get; set; }
            public decimal TotalPrice => OrderDetail.quantity * OrderDetail.unitPrice;
        }

        public OrdersOutput()
        {
            InitializeComponent();
            context = new shopEntities1();
            LoadOrders();
        }

        private void LoadOrders()
        {
            var orders = context.Orders.OrderByDescending(o => o.orderDate).ToList();
            listOrders.ItemsSource = orders;
            listOrderDetails.ItemsSource = null; // Очищаем детали при загрузке
        }

        private void listOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listOrders.SelectedItem is Orders selectedOrder)
            {
                var orderDetails = context.OrderDetails
                    .Where(od => od.orderID == selectedOrder.orderID)
                    .Select(od => new OrderDetailViewModel
                    {
                        OrderDetail = od,
                        Products = od.Products
                    })
                    .ToList();
                listOrderDetails.ItemsSource = orderDetails;
            }
            else
            {
                listOrderDetails.ItemsSource = null;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DataOutputAdmin());

        }
    }
}