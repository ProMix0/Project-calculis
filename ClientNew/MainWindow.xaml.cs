using Client;
using ClientApp;
using CommonLibrary;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<MetaWork> MetaWorkItems { get; } = new ObservableCollection<MetaWork>();
        private System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        public ConfigurationManager Settings { get; } = ConfigurationManager.Load();

        public MainWindow()
        {
            InitializeComponent();

            notifyIcon.Icon = new System.Drawing.Icon("Ico.ico");
            //notifyIcon.Icon = new System.Drawing.Icon("warn.ico");
            notifyIcon.Visible = true;
            notifyIcon.Click +=
                delegate (object sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };

            DataContext = this;
            listBox.SelectedIndex = 0;
            buttonsPanel.MouseLeftButtonDown += new MouseButtonEventHandler(LayoutRoot_MouseLeftButtonDown);



            TestMethod();
        }

        private void TestMethod()
        {
            MetaWorkItems.Add(new MetaWork("name", "Example name",
                @"E:\Разбор\107_PANA\P1060910.JPG", "My some description",
                "Most\nfull\ndescription", 100));
            MetaWorkItems.Add(new MetaWork("name", "Example name 2",
                @"E:\Разбор\107_PANA\P1070539.JPG", "Description", "Most\nfull\ndescription\n2", 200));
            MetaWorkItems.Add(new MetaWork("name", "Example name 2",
                @"E:\Разбор\107_PANA\P1070539.JPG", "Description", "Most\nfull\ndescription\n2", 200));
            MetaWorkItems.Add(new MetaWork("name", "Example name 2",
                @"E:\Разбор\107_PANA\P1070539.JPG", "Description", "Most\nfull\ndescription\n2", 200));
        }

        void ToTray(object sender, EventArgs e)
        {
            this.Hide();
            notifyIcon.Visible = true;

        }

        void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        void CloseWindow(object sender, EventArgs e)
        {
            Settings.Save();
            Close();
        }

        void CollapseWindow(object sender, EventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        void MaxMinWindow(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    ((Button)sender).Content = "Normalize";
                    WindowState = WindowState.Maximized;
                    break;
                case WindowState.Maximized:
                    ((Button)sender).Content = "Maximize";
                    WindowState = WindowState.Normal;
                    break;
            }
        }
    }

    public class ValuteConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{value} руб.";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
