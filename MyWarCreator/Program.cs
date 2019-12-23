using System;
using MyWarCreator.Features.Drawing;
using MyWarCreator.Features.Fonts;
using SimpleInjector;

namespace MyWarCreator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var container = Bootstrap();

            // Any additional other configuration, e.g. of your desired MVVM toolkit.

            RunApplication(container);
        }

        private static Container Bootstrap()
        {
            // Create the container as usual.
            var container = new Container();

            // Register your windows and view models:
            container.Register<MainWindow>();
            container.RegisterSingleton<IFontProvider, FontProvider>();
            container.RegisterSingleton<IPainter, Painter>();

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