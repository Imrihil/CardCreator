using CardCreator.Features.Drawing;
using CardCreator.Features.Fonts;
using CardCreator.Features.Preview;
using CardCreator.Settings;
using CardCreator.View;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Text;
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
#if DEBUG
             .AddJsonFile("appsettings.debug.json", optional: false, reloadOnChange: true);
#else
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
#endif

            Configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            services.Configure<AppSettings>(Configuration);

            services.AddTransient<MainWindow>();
            services.AddTransient<ProcessWindow>();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddSingleton<IFontProvider, FontProvider>();
            services.AddSingleton<IImageProvider, ImageProvider>();
            services.AddSingleton<IIconProvider, IconProvider>();
            services.AddSingleton<IPreviewFactory, SafePreviewFactory>();
        }
    }
}
