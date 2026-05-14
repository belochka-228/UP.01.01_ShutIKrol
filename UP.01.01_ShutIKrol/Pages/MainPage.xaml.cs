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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            if (UserData.CurrentUser.Roles?.RoleName == "Администратор")
            {
                BtnAdmin.Visibility = Visibility.Visible;
            }
            else if (UserData.CurrentUser.Roles?.RoleName == "Автор")
            {
                BtnAuthor.Visibility = Visibility.Visible;
            }
            if (UserData.CurrentUser.IsFrozen)
            {
                BtnFrozen.Visibility = Visibility.Visible;
            }
        ContentFrame.Navigate(new CatalogPage());
        }
        private void BtnCatalog_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new CatalogPage());
        }
        private void BtnLists_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ReadingListsPage());
        }
        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AdminPage());
        }
        private void BtnAuthor_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AuthorPage());
        }
        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ProfilePage());
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
        private void BtnFrozen_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Ваш аккаунт заморожен. Вы можете оспорить заморозку в профиле.");
        }
    }
}