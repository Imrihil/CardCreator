﻿using System.IO;
using System.Windows;
using Microsoft.Win32;
using CardCreator.Features.Cards;
using CardCreator.Features.Drawing;
using CardCreator.Features.Fonts;
using CardCreator.Features.Images;
using MediatR;
using System;

namespace CardCreator
{
    // https://www.codeproject.com/Articles/299436/WPF-Localization-for-Dummies
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IMediator mediator;
        private readonly IFontProvider fontProvider;
        private readonly IImageProvider imageProvider;
        private readonly IPainter painter;
        private readonly ICardBuilder cardBuilder;

        private OpenFileDialog ChooseFileDialog { get; }

        public MainWindow(IMediator mediator, IFontProvider fontProvider, IImageProvider imageProvider, IPainter painter, ICardBuilder cardBuilder)
        {
            this.mediator = mediator;
            this.fontProvider = fontProvider;
            this.imageProvider = imageProvider;
            this.painter = painter;
            this.cardBuilder = cardBuilder;

            ChooseFileDialog = InitializeChooseFileDialog();
            InitializeComponent();
            InitializeFonts();
        }

        private OpenFileDialog InitializeChooseFileDialog()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xls;*xlsx)|*.xls;*xlsx";
#if DEBUG
            openFileDialog.InitialDirectory = Path.GetFullPath(Directory.GetCurrentDirectory() + "../../../../../AppData");
#else
            openFileDialog.InitialDirectory = Path.GetFullPath(Directory.GetCurrentDirectory());
#endif
            return openFileDialog;
        }

        private void InitializeFonts()
        {
            fontProvider.Register(Properties.Resources.Akvaléir_Normal_v2007);
            fontProvider.Register(Properties.Resources.colonna_mt);
            fontProvider.Register(Properties.Resources.runic);
            fontProvider.Register(Properties.Resources.runic_altno);
            fontProvider.Register(Properties.Resources.trebuc);
            fontProvider.Register(Properties.Resources.trebucbd);
            fontProvider.Register(Properties.Resources.trebucbi);
            fontProvider.Register(Properties.Resources.trebucit);
        }

        private void ButtonGenerateCards_Click(object sender, RoutedEventArgs e)
        {
            var result = mediator.Send(new CardGeneratingCommand()).GetAwaiter().GetResult();
            Console.WriteLine(result);
        }

        private void ButtonPreparePdf_Click(object sender, RoutedEventArgs e)
        { }

        private void ButtonChooseFile_Click(object sender, RoutedEventArgs e)
        {
            if (ChooseFileDialog.ShowDialog() == true)
            {
                FileInfo fileInfo = new FileInfo(ChooseFileDialog.FileName);
                Directory_Label.Content = fileInfo.Name;
                ChooseFileDialog.InitialDirectory = fileInfo.Directory.FullName;
            }
        }
    }
}
