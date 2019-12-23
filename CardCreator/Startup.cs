using CardCreator.Features.Cards;
using CardCreator.Features.Drawing;
using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using Container = SimpleInjector.Container;

namespace CardCreator
{
    public class Startup
    {
        public void Run()
        {
            var container = Bootstrap();

            RunApplication(container);
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }

        private static Container Bootstrap()
        {
            var container = new Container();

            container.Register<MainWindow>();
            container.RegisterSingleton<IFontProvider, FontProvider>();
            container.RegisterSingleton<IImageProvider, ImageProvider>();

            container.RegisterSingleton<IPainter, Painter>();

            container.RegisterSingleton<ICardBuilder, CardBuilder>();

            container.Verify();

            return container;
        }

        private static void RunApplication(Container container)
        {
            try
            {
                var app = new App();
                app.InitializeComponent();
                var mainWindow = container.GetInstance<MainWindow>();
                app.Run(mainWindow);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
