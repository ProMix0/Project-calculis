using ClientApp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            MetaWorkItems.Add(new ConcreteMetaWork("name", "Example name",
                @"C:\Users\Миша\source\repos\Project-calculis\ClientNew\Example.JPG", "Description", "Text"));
        }
    }
}
