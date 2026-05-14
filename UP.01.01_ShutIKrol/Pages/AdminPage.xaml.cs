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
            ListBoxFrozenReviews.ItemsSource = Core.Context.Reviews.Where(r => r.IsFrozen).Include("Users").Include("Books").ToList();
        }
        private void LoadAllUsers()
        {
            var users = Core.Context.Users.Include("Roles").ToList();
            ListBoxAllUsers.ItemsSource = users;
        }
        private void BtnAcceptComplaint_Click(object sender, RoutedEventArgs e)
        {
            var complaint = ((Button)sender).DataContext as Complaints;
            if (complaint == null) return;

            complaint.IsConfirmed = true;
            Core.Context.SaveChanges();

            switch (complaint.TargetTypeId)
            {
                case 1: // Book — заморозка книги
                    var book = Core.Context.Books.FirstOrDefault(b => b.Id == complaint.BookId);
                    if (book != null)
                    {
                        book.IsFrozen = true;
                        Core.Context.SaveChanges();
                    }
                    break;

                case 3: // Author — заморозка автора книги
                    var targetBook = Core.Context.Books.FirstOrDefault(b => b.Id == complaint.BookId);
                    if (targetBook != null)
                    {
                        var author = Core.Context.Users.FirstOrDefault(u => u.Id == targetBook.AuthorId);
                        if (author != null)
                        {
                            author.IsFrozen = true;
                            Core.Context.SaveChanges();
                        }
                    }
                    break;

                case 2: // Review — просто подтверждаем, без заморозки
                default:
                    break;
            }

            LoadComplaints();
        }
        private void BtnUnfreezeBook_Click(object sender, RoutedEventArgs e)
        {
            var book = ((Button)sender).DataContext as Books;
            if (book != null)
            {
                book.IsFrozen = false;
                Core.Context.SaveChanges();
                LoadFrozenObjects();
            }
        }

        private void BtnUnfreezeUser_Click(object sender, RoutedEventArgs e)
        {
            var user = ((Button)sender).DataContext as Users;
            if (user != null)
            {
                user.IsFrozen = false;
                Core.Context.SaveChanges();
                LoadFrozenObjects();
            }
        }

        private void BtnUnfreezeReview_Click(object sender, RoutedEventArgs e)
        {
            var review = ((Button)sender).DataContext as Reviews;
            if (review != null)
            {
                review.IsFrozen = false;
                Core.Context.SaveChanges();
                LoadFrozenObjects();
            }
        }
        private void BtnRejectComplaint_Click(object sender, RoutedEventArgs e)
        {
            var complaint = ((Button)sender).DataContext as Complaints;
            if (complaint == null) return;

            complaint.IsConfirmed = false;
            Core.Context.SaveChanges();
            LoadComplaints();
        }
        private void BtnAcceptUnfreeze_Click(object sender, RoutedEventArgs e)
        {
            var request = ((Button)sender).DataContext as UnfreezeApplications;
            if (request == null) return;

            request.IsConfirmed = true;
            Core.Context.SaveChanges();
            if (request.BookId != null)
            {
                var book = Core.Context.Books.FirstOrDefault(b => b.Id == request.BookId);
                if (book != null)
                {
                    book.IsFrozen = false;
                    Core.Context.SaveChanges();
                }
            }
            else
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
        private void BtnAcceptAuthorApp_Click(object sender, RoutedEventArgs e)
        {
            var app = ((Button)sender).DataContext as AuthorApplications;
            if (app == null) return;

            app.StatusId = 2;
            Core.Context.SaveChanges();

            var user = Core.Context.Users.FirstOrDefault(u => u.Id == app.UserId);
            if (user != null)
            {
                user.RoleId = 2;
                Core.Context.SaveChanges();
            }
            LoadAuthorRequests();
        }
        private void BtnRejectAuthorApp_Click(object sender, RoutedEventArgs e)
        {
            var app = ((Button)sender).DataContext as AuthorApplications;
            if (app == null) return;

            app.StatusId = 3;
            Core.Context.SaveChanges();
            LoadAuthorRequests();
        }
        private void CmbRole_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = (ComboBox)sender;
            if (comboBox.Items.Count == 0)
            {
                comboBox.Items.Add("Читатель");
                comboBox.Items.Add("Автор");
                comboBox.Items.Add("Администратор");
            }
            var user = comboBox.DataContext as Users;
            if (user != null && user.Roles != null)
            {
                comboBox.SelectedItem = user.Roles.RoleName;
            }
        }
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
                var main = Application.Current.MainWindow as MainWindow;
                main?.MainFrame.Refresh();
                LoadAllUsers();
            }
        }
        private void BtnOpenPasswordWindow_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var user = btn.DataContext as Users;
            if (user == null) return;
            var window = new ChangePasswordWindow(user);
            window.ShowDialog();
        }
    }
}