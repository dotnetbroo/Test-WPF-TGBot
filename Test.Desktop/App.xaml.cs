using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Data;
using System.Windows;
using Test.Data.DbContexts;
using Test.Data.Repositories;
using Test.Service.Interfaces;
using Test.Service.Meppers;
using Test.Service.Services;

namespace Test.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            var serviceCollection = new ServiceCollection();
            //ConfigureServices(serviceCollection);
            
        }
        protected override void OnStartup(StartupEventArgs e)
        {
            //base.OnStartup(e);

            //// Initialize DI container
            //var serviceProvider = ConfigureServices();

            //// Resolve MainWindow with dependencies
            //var mainWindow = serviceProvider.GetRequiredService<MainWindow>();

            //// Show MainWindow
            //mainWindow.Show();
            //base.OnStartup(e);

        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<AppDbContext>(ServiceLifetime.Transient);
            services.AddSingleton<Controller>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductService, ProductService>();
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapperProfile>(); // Add your AutoMapper profile(s)
            });
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            // Register services


            // Register MainWindow (optional if using implicit registration)
            services.AddTransient<MainWindow>();

            // Build service provider
            return services.BuildServiceProvider();
        }
        private void ConfigureServicesss(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(ServiceLifetime.Transient);
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductService, ProductService>();
            services.AddTransient<MainWindow>();

        }
    }
}