using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace UP._01._01_ShutIKrol.Pages
{
    /// <summary>
    /// Логика взаимодействия для WindowReadText.xaml
    /// </summary>
    public partial class WindowReadText : Window
    {
        public WindowReadText(string text)
        {
            InitializeComponent();
            this.DataContext = new { Content = text };

        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
