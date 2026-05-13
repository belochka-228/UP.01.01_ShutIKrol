using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            Loaded += ProfilePage_Loaded;
        }
        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            var user = UserData.CurrentUser;
            if (user.Roles?.RoleName == "Читатель")
                BtnAuthorRequest.Visibility = Visibility.Visible;
            else
                BtnAuthorRequest.Visibility = Visibility.Collapsed;
            if (user.IsFrozen)
            {
                string reason = "Причина не указана";
                var lastComplaint = Core.Context.Complaints.Where(c => c.TargetTypeId == 3 && c.IsConfirmed == true && c.Books.AuthorId == user.Id).OrderByDescending(c => c.CreatedAt).FirstOrDefault();

                if (lastComplaint != null && lastComplaint.ComplaintReasons != null)
                    reason = lastComplaint.ComplaintReasons.Name;

                MessageBoxResult result = MessageBox.Show($"Ваш аккаунт заморожен.\nПричина: {reason}\n\nХотите подать заявку на разморозку?", "Заморожен", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    var request = new UnfreezeApplications
                    {
                        UserId = user.Id,
                        Reason = "Оспаривание заморозки аккаунта",
                        StatusId = 1,
                        CreatedAt = DateTime.Now,
                        BookId = null
                    };
                    Core.Context.UnfreezeApplications.Add(request);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Заявка на разморозку отправлена!");
                }
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.MainFrame.Navigate(new MainPage());
                return;
            }
            DataContext = user;

            var reviews = Core.Context.Reviews.Where(r => r.UserId == user.Id).Include("Books").OrderByDescending(r => r.CreatedAt).ToList();
            ListBoxReviews.ItemsSource = reviews;
        }
        private void BtnAuthorRequest_Click(object sender, RoutedEventArgs e)
        {
            var user = UserData.CurrentUser;
            bool alreadyRequested = Core.Context.AuthorApplications.Any(a => a.UserId == user.Id);
            if (alreadyRequested)
            {
                MessageBox.Show("Вы уже подавали заявку на роль автора.");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Подать заявку на получение роли «Автор»?","Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var request = new AuthorApplications
                    {
                        UserId = user.Id,
                        StatusId = 1,
                        CreatedAt = DateTime.Now
                    };
                    Core.Context.AuthorApplications.Add(request);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Заявка принята!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }
    }
}