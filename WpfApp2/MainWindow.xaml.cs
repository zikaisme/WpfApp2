using System.Windows;
using System.Windows.Controls;

namespace WpfApp2
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void shapebotton_checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.Tag != null)
            {
                string selectedShape = rb.Tag.ToString();
                this.Title = $"WPF小畫家v0.1 - 目前工具：{selectedShape}";
            }
        }
    }
}