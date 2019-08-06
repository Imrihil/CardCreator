using System;
using System.Windows;
using System.Windows.Threading;

namespace MyWarCreator.Extensions
{
    public static class UiElementExtensions
    {
        private static readonly Action EmptyDelegate = delegate { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
