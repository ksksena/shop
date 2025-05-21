using shop.ApplicationData;
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
using shop;
using shop.Pages;

namespace Shop.Pages
{
    public partial class DataOutputUser : Page
    {
        private shopEntities1 context;
        private int currentUserId;

        public DataOutputUser(int userId)
        {
            InitializeComponent();
            context = new shopEntities1();
            currentUserId = userId;
            LoadData();
            LoadFilters();
        }

        private void LoadData()
        {
            var products = context.Products.ToList();
            ApplyFiltersAndSort(products);
        }

        private void LoadFilters()
        {
            var suppliers = context.Suppliers.ToList();
            foreach (var supplier in suppliers)
            {
                ComboFilter.Items.Add(new ComboBoxItem { Content = supplier.FIO, Tag = supplier.suppID });
            }
        }

        private void ApplyFiltersAndSort(List<Products> products)
        {
            if (!string.IsNullOrWhiteSpace(TextSearch.Text))
            {
                products = products.Where(p => p.name.ToLower().Contains(TextSearch.Text.ToLower())).ToList();
            }

            var selectedFilter = (ComboFilter.SelectedItem as ComboBoxItem)?.Tag as int?;
            if (selectedFilter.HasValue && selectedFilter != 0)
            {
                products = products.Where(p => p.suppID == selectedFilter.Value).ToList();
            }

            var selectedSort = (ComboSort.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (selectedSort)
            {
                case "Сортировать по названию (А-Я)":
                    products = products.OrderBy(p => p.name).ToList();
                    break;
                default:
                    break;
            }

            listProducts.ItemsSource = products;
            TextFoundCount.Text = $"Найдено: {products.Count}";
        }

        private void TextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadData();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadData();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                int productId = (int)button.Tag;
                var product = context.Products.FirstOrDefault(p => p.prodID == productId);

                if (product != null)
                {
                    var cartItem = CartManager.Cart.FirstOrDefault(c => c.ProductId == productId);
                    if (cartItem != null)
                    {
                        cartItem.Quantity++;
                    }
                    else
                    {
                        CartManager.Cart.Add(new CartManager.CartItem
                        {
                            ProductId = product.prodID,
                            ProductName = product.name,
                            Quantity = 1,
                            Price = product.price ?? 0m
                        });
                    }

                    //MessageBox.Show($"Товар '{product.name}' добавлен в корзину!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ViewCart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Cart(currentUserId));
        }
    }
}