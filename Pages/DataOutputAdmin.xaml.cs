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
using System.Diagnostics;

namespace Shop.Pages
{
    public partial class DataOutputAdmin : Page
    {
        private shopEntities1 context;
        private List<Products> allProducts;
        private Products selectedProduct;

        public DataOutputAdmin()
        {
            InitializeComponent();
            context = new shopEntities1();
            ComboFilter.SelectedIndex = 0;
            ComboSort.SelectedIndex = 0;

            allProducts = context.Products.ToList();
            listProducts.ItemsSource = allProducts;

            var suppliers = context.Suppliers.ToList();
            foreach (var supplier in suppliers)
            {
                ComboFilter.Items.Add(new ComboBoxItem { Content = supplier.FIO, Tag = supplier.suppID });
            }
            UpdateFoundCount(allProducts.Count);
        }

        private void listProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedProduct = listProducts.SelectedItem as Products;
            if (selectedProduct != null)
            {
                Debug.WriteLine($"Выбран товар: {selectedProduct.name}");
            }
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProductList();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProductList();
        }

        private void TextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProductList();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (listProducts.SelectedItem is Products selectedProduct)
            {
                EditProduct editPage = new EditProduct(selectedProduct);
                editPage.ProductUpdated += UpdateProductList;
                NavigationService.Navigate(editPage);
            }
            else
            {
                MessageBox.Show("Выберите товар для редактирования!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Products newProduct = new Products();
            EditProduct editPage = new EditProduct(newProduct);
            editPage.ProductUpdated += UpdateProductList;
            NavigationService.Navigate(editPage);
        }

        private void UpdateProductList()
        {
            string searchText = TextSearch.Text.ToLower();
            string selectedSupplier = (ComboFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (allProducts == null)
            {
                UpdateFoundCount(0);
                return;
            }
            var filteredProducts = allProducts.Where(product =>
                product != null &&
                product.name != null &&
                product.name.ToLower().Contains(searchText) &&
                (selectedSupplier == "Все поставщики" ||
                 (product.Suppliers != null && product.Suppliers.FIO == selectedSupplier)))
                .ToList();

            List<Products> sortedProducts;
            if (ComboSort.SelectedItem is ComboBoxItem selectedItem)
            {
                string sortBy = selectedItem.Content.ToString();
                switch (sortBy)
                {
                    case "Сортировать по названию (А-Я)":
                        sortedProducts = filteredProducts.OrderBy(product => product.name).ToList();
                        break;
                    case "Сортировать по цене":
                        sortedProducts = filteredProducts.OrderBy(product => product.price).ToList();
                        break;
                    case "Не сортировать":
                    default:
                        sortedProducts = filteredProducts;
                        break;
                }
            }
            else
            {
                sortedProducts = filteredProducts;
            }
            listProducts.ItemsSource = sortedProducts;
            UpdateFoundCount(sortedProducts.Count);
        }

        private void UpdateFoundCount(int count)
        {
            TextFoundCount.Text = $"Найдено: {count}";
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersOutput());
        }
    }
}