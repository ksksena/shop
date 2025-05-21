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
using Shop;
using shop.ApplicationData;
using shop.Pages;


namespace Shop.Pages
{
    public partial class OrdersOutput : Page
    {
        private shopEntities1 context;

        public class OrderDetailViewModel
        {
            public OrderDetails OrderDetail { get; set; }
            public Products Products { get; set; }
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
            listOrderDetails.ItemsSource = null;
            TextBlockTotalPrice.Text = "Общая цена: 0";
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

                TextBlockTotalPrice.Text = $"Общая цена: {selectedOrder.totalAmount:F2}";
            }
            else
            {
                listOrderDetails.ItemsSource = null;
                TextBlockTotalPrice.Text = "Общая цена: 0";
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DataOutputAdmin());
        }
    }
}