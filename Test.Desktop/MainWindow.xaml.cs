using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Test.Data.Repositories;
using Test.Desktop.DbContexts;
using Test.Domain.Entities;
using Test.Service.Commons.Exceptions;
using Test.Service.Interfaces;

namespace Test.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly IProductService _productServiceClient;
        private readonly IRepository<Product> _repository;
        private readonly IMapper _mapper;

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductVideoPath { get; set; }
        public long SortNumber { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            RefreshDatagrid();
        }
        public async void RefreshDatagrid()
        {
            await using (var dbContext = new AppDbContext())
            {
                var products = await dbContext.Products.ToListAsync();
                if (products.Count > 0)
                {
                    ProductsDataGrid.ItemsSource = products;
                }
                else
                {
                    MessageBox.Show("Not found product");
                }
            };
        }

        private async void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductNameTextBox.Text.Length > 0 && ProductDescriptionTextBox.Text.Length > 0
                    && ProductVideoPath.Length > 0)
            {
                try
                {
                    Random random = new Random();
                    int randomNumber = random.Next(99999999);

                    var newProduct = new Product
                    {
                        Name = ProductNameTextBox.Text,
                        Description = ProductDescriptionTextBox.Text,
                        Video = ProductVideoPath,
                        SortNumber = randomNumber
                    };

                    using (var dbContext = new AppDbContext())
                    {
                        dbContext.Add(newProduct);
                        var result = dbContext.SaveChanges();
                        if (result > 0)
                        {
                            MessageBox.Show("Yangi Maxsulot qo'shildi");
                            ProductNameTextBox.Text = "";
                            ProductDescriptionTextBox.Text = "";
                            ProductVideoPath = "";
                            RefreshDatagrid();
                        }

                    }
                    MessageBox.Show("Mahsulot muvaffaqiyatli saqlandi!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Xatolik: {ex.Message}");
                }

            }
            else
            {
                MessageBox.Show("Ma'lumotni to'liq kiriting!!");
            }
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductsDataGrid.SelectedItem != null)
                {
                    var selectedItem = ProductsDataGrid.SelectedItem as Product;
                    using (var dbContext = new AppDbContext())
                    {
                        var product = await dbContext.Products
                            .Where(ea => ea.Id == selectedItem.Id)
                            .AsNoTracking()
                            .FirstOrDefaultAsync();

                        if (product is null)
                            throw new CustomException(404, "Product is not found");

                        var entity = await dbContext.Products.FirstOrDefaultAsync(e => e.Id.Equals(selectedItem.Id));
                        dbContext.Products.Remove(entity);
                        var result = await dbContext.SaveChangesAsync() > 0;

                        if (result)
                        {
                            MessageBox.Show("Mahsulot muvaffaqiyatli o'chirildi!");
                            spModifiers.Visibility = Visibility.Collapsed;
                            RefreshDatagrid();
                        }
                        else
                            MessageBox.Show("Xatolik: Mahsulot o'chirilmadi.");
                    }


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik: {ex.Message}");
            }
        }

        private async void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductsDataGrid.SelectedItem != null)
                {
                    var selectedItem = ProductsDataGrid.SelectedItem as Product;
                    var updatedProduct = new Product
                    {
                        Id = selectedItem.Id,
                        Name = selectedItem.Name,
                        Description = selectedItem.Description,
                        Video = selectedItem.Video,
                        SortNumber = selectedItem.SortNumber,
                    };
                    ProdcutUpdateWindow prodcutUpdateWindow = new ProdcutUpdateWindow();
                    prodcutUpdateWindow.SetData(updatedProduct);
                    prodcutUpdateWindow.UpdateDelegate = RefreshDatagrid;
                    prodcutUpdateWindow.ShowDialog();

                }

                MessageBox.Show("Mahsulot muvaffaqiyatli yangilandi!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik: {ex.Message}");
            }
        }

        private async Task<IFormFile> ConvertToByteArray(string filePath)
        {
            byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
            MemoryStream memoryStream = new MemoryStream(fileBytes);
            return new FormFile(memoryStream, 0, fileBytes.Length, null, Path.GetFileName(filePath));
        }


        private void SelectVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video files (*.mp4;*.avi)|*.mp4;*.avi|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                ProductVideoPath = openFileDialog.FileName;
            }
        }

        private async void SearchProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchQuery = SearchTextBox.Text;
                using (var dbContext = new AppDbContext())
                {
                    var product = await dbContext.Products
                       .Where(p => EF.Functions.Like(p.Name.ToLower(), $"%{searchQuery}%")
                       || EF.Functions.Like(p.Description.ToLower(), $"%{searchQuery}%"))
                       .ToListAsync();

                    ShowSearchResults(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ShowSearchResults(List<Product> products)
        {
            ProductsDataGrid.ItemsSource = null;
            ProductsDataGrid.ItemsSource = products;
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                if (sender is DataGrid dataGrid)
                {
                    if (dataGrid.SelectedItem != null)
                    {
                        var selectedItem = dataGrid.SelectedItem as Product;
                        spModifiers.Visibility = Visibility.Visible;
                    }


                }
            }
        }
    }
}
