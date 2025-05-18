using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
using shop.ApplicationData;


namespace Shop.Pages
{
    public partial class EditProduct : Page
    {
        private shopEntities1 context = new shopEntities1();
        private Products product;
        public event Action ProductUpdated;

        public EditProduct(Products product)
        {
            InitializeComponent();
            this.product = product ?? new Products();
            LoadSuppliers();
            LoadProductData();
        }

        private void LoadSuppliers()
        {
            var suppliers = context.Suppliers.ToList();
            cmbSuppliers.ItemsSource = suppliers;
            cmbSuppliers.DisplayMemberPath = "FIO";
            cmbSuppliers.SelectedValuePath = "suppID";
            cmbSuppliers.SelectedValue = product.suppID;
        }

        private void LoadProductData()
        {
            txtName.Text = product.name ?? string.Empty;
            txtDescription.Text = product.description ?? string.Empty;
            txtPrice.Text = product.price.HasValue ? product.price.Value.ToString("F2") : string.Empty;
            txtPhoto.Text = product.imageE ?? string.Empty;
            cmbSuppliers.SelectedValue = product.suppID;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPrice.Text))
                {
                    MessageBox.Show("Название и цена обязательны для заполнения!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
                {
                    MessageBox.Show("Цена должна быть положительным числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                product.name = txtName.Text.Trim();
                product.description = txtDescription.Text.Trim();
                product.price = price;
                product.imageE = txtPhoto.Text.Trim();
                product.suppID = (int?)cmbSuppliers.SelectedValue;

                if (product.prodID == 0) // Новый товар
                {
                    context.Products.Add(product);
                }
                context.SaveChanges();
                ProductUpdated?.Invoke();
                MessageBox.Show("Товар успешно сохранен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void LoadImageButton(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                dialog.InitialDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Images"));

                dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*";
                dialog.Title = "Выберите изображение";

                if (dialog.ShowDialog() == true)
                {
                    string photoName = System.IO.Path.GetFileName(dialog.FileName);
                    product.imageE = photoName;
                    MessageBox.Show("Изображение загружено: " + photoName, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Изображение не выбрано.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке изображения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}