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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UP._01._01_ShutIKrol.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void AutoButton_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUser.Text.Trim(); 
            string password = txtPassword.Password; 

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Авторизация прошла успешно!");
            }
            else
            {
                MessageBox.Show("Введите логин/пароль");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(Register));  
        }
    }
}
