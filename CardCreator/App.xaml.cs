using CardCreator.Features.Cards;
using CardCreator.Features.Drawing;
using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace CardCreator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(MainWindow));

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddSingleton<IFontProvider, FontProvider>();
            services.AddSingleton<IImageProvider, ImageProvider>();

            services.AddSingleton<IPainter, Painter>();

            services.AddSingleton<ICardBuilder, CardBuilder>();
        }
    }
}
