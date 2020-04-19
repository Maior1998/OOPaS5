using System;
using System.Diagnostics;
using System.Windows;
using OOPaS5.Parser;

namespace OOPaS5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            HtmlEGP egp = new HtmlEGP(new HtmlEGP.XMLDataProvider());
            egp.GetInfo();
            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds/1000.0} seconds");
        }
    }
}
