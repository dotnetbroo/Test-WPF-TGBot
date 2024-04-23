using Microsoft.Win32;
using System.Windows;
using Test.Desktop.DbContexts;
using Test.Domain.Entities;

namespace Test.Desktop
{
    public partial class ProdcutUpdateWindow : Window
    {
        public delegate void UpdateMainWindow();
        public UpdateMainWindow UpdateDelegate { get; set; }
        public Product product { get; set; }
        public ProdcutUpdateWindow()
        {
            InitializeComponent();
        }
        public void SetData(Product product)
        {
            this.product = product;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ProductNameTextBox.Text = product.Name;
            ProductDescriptionTextBox.Text = product.Description;
        }

        private void SelectVideo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video files (*.mp4;*.avi)|*.mp4;*.avi|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                this.product.Video = openFileDialog.FileName;
            }
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductNameTextBox.Text.Length > 0 && ProductDescriptionTextBox.Text.Length > 0
                    && this.product.Video.Length > 0)
            {
                try
                {

                    var newProduct = new Product
                    {
                        Id = this.product.Id,
                        Name = ProductNameTextBox.Text,
                        Description = ProductDescriptionTextBox.Text,
                        Video = this.product.Video,
                        SortNumber = this.product.SortNumber,
                    };

                    using (var dbContext = new AppDbContext())
                    {
                        dbContext.Products.Update(newProduct);
                        var result = dbContext.SaveChanges();
                        if (result > 0)
                        {
                            MessageBox.Show("Mahsulot tahrirlandi");
                            UpdateDelegate.Invoke();
                            this.Close();
                        }

                    }
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
    }
}
