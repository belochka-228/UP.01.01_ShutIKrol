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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            DataContext = UserData.CurrentUser;
            LoadDate();        }

        private void LoadDate()
        {
            LoadComplaints();
            LoadUnfreezeRequests();
            LoadAuthorRequests();
            LoadFrozenObjects();
            LoadAllUsers();
        }
        private void LoadComplaints()
        {
            var complaints = Core.Context.Complaints.Where(c => c.IsConfirmed == null).Include("Users").Include("ComplaintTargetTypes").Include("ComplaintReasons").Include("Books").ToList();
            ListBoxComplaints.ItemsSource = complaints;
        }
        private void LoadUnfreezeRequests()
        {
            var requests = Core.Context.UnfreezeApplications.Where(r => r.IsConfirmed == null).Include("Users").Include("Books").ToList();
            ListBoxUnfreeze.ItemsSource = requests;
        }
        private void LoadAuthorRequests()
        {
            var apps = Core.Context.AuthorApplications.Where(a => a.StatusId == 1).Include("Users").ToList();
            ListBoxAuthorApps.ItemsSource = apps;
        }
        private void LoadFrozenObjects()
        {
            ListBoxFrozenBooks.ItemsSource = Core.Context.Books.Where(b => b.IsFrozen).Include("Users").ToList();
            ListBoxFrozenUsers.ItemsSource = Core.Context.Users.Where(u => u.IsFrozen).Include("Roles").ToList();
        }
        private void LoadAllUsers()
        {
            var users = Core.Context.Users.Include("Roles").ToList();
            ListBoxAllUsers.ItemsSource = users;
        }
        // ========== Жалобы ==========
        private void BtnAcceptComplaint_Click(object sender, RoutedEventArgs e)
        {
            var complaint = ((Button)sender).DataContext as Complaints;
            if (complaint == null) return;
            complaint.IsConfirmed = true;
            Core.Context.SaveChanges();
            // Замораживаем книгу, если жалоба на книгу (TargetTypeId == 1)
            if (complaint.TargetTypeId == 1)
            {
                var book = Core.Context.Books.FirstOrDefault(b => b.Id == complaint.BookId);
                if (book != null)
                {
                    book.IsFrozen = true;
                    Core.Context.SaveChanges();
                }
            }
            LoadComplaints();
        }
        private void BtnRejectComplaint_Click(object sender, RoutedEventArgs e)
        {
            var complaint = ((Button)sender).DataContext as Complaints;
            if (complaint == null) return;

            complaint.IsConfirmed = false;
            Core.Context.SaveChanges();
            LoadComplaints();
        }
        // ========== Заявки на разморозку ==========
        private void BtnAcceptUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var request = ((Button)sender).DataContext as UnfreezeApplications;
            if (request == null) return;

            request.IsConfirmed = true;
            Core.Context.SaveChanges();
            // Снимаем заморозку с книги или пользователя
            if (request.BookId != null)            // разморозка книги
            {
                var book = Core.Context.Books.FirstOrDefault(b => b.Id == request.BookId);
                if (book != null)
                {
                    book.IsFrozen = false;
                    Core.Context.SaveChanges();
                }
            }
            else                                   // разморозка пользователя
            {
                var user = Core.Context.Users.FirstOrDefault(u => u.Id == request.UserId);
                if (user != null)
                {
                    user.IsFrozen = false;
                    Core.Context.SaveChanges();
                }
            }
            LoadUnfreezeRequests();
        }
        private void BtnRejectUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var request = ((Button)sender).DataContext as UnfreezeApplications;
            if (request == null) return;

            request.IsConfirmed = false;
            Core.Context.SaveChanges();
            LoadUnfreezeRequests();
        }
        // ========== Заявки на автора ==========
        private void BtnAcceptAuthorApp_Click(object sender, RoutedEventArgs e)
        {
            var app = ((Button)sender).DataContext as AuthorApplications;
            if (app == null) return;

            app.StatusId = 2;                     // Approved
            Core.Context.SaveChanges();

            var user = Core.Context.Users.FirstOrDefault(u => u.Id == app.UserId);
            if (user != null)
            {
                user.RoleId = 2;                  // Автор
                Core.Context.SaveChanges();
            }
            LoadAuthorRequests();
        }
        private void BtnRejectAuthorApp_Click(object sender, RoutedEventArgs e)
        {
            var app = ((Button)sender).DataContext as AuthorApplications;
            if (app == null) return;

            app.StatusId = 3;                     // Rejected
            Core.Context.SaveChanges();
            LoadAuthorRequests();
        }
        // ========== Смена роли пользователя ========
        private void CmbRole_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            // Заполняем роли только один раз
            if (comboBox.Items.Count == 0)
            {
                comboBox.Items.Add("Читатель");
                comboBox.Items.Add("Автор");
                comboBox.Items.Add("Администратор");
            }

            // Получаем текущего пользователя из DataContext
            var user = comboBox.DataContext as Users;
            if (user != null && user.Roles != null)
            {
                comboBox.SelectedItem = user.Roles.RoleName;
            }
        }

        // Обработчик смены роли остается как есть
        private void CmbRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            var user = comboBox.DataContext as Users;
            if (user == null || comboBox.SelectedItem == null) return;

            string newRole = comboBox.SelectedItem.ToString();
            int newRoleId = (newRole == "Автор") ? 2 : (newRole == "Администратор") ? 3 : 1;

            if (user.RoleId != newRoleId)
            {
                user.RoleId = newRoleId;
                Core.Context.SaveChanges();
                // Обновляем отображаемую роль в TextBlock'е через обновление свойства
                var main = Application.Current.MainWindow as MainWindow;
                main?.MainFrame.Refresh();
                LoadAllUsers();                           // перезагружаем весь список
            }
        }
        // ========== Смена пароля ==========
        private void BtnChangePassword_Click(object sender, RoutedEventArgs e)
        {
            var user = ((Button)sender).DataContext as Users;
            if (user == null) return;

            string newPassword = "newpass123";   // в реальности нужен диалог ввода
            user.Password = newPassword;
            Core.Context.SaveChanges();
            MessageBox.Show($"Пароль пользователя {user.DisplayName} изменён на {newPassword}");
        }
    }
}