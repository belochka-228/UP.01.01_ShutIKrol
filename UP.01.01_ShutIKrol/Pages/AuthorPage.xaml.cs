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
    /// страница автора
    /// </summary>
    public partial class AuthorPage : Page
    {
        private List<Books> _activeBooks;
        private List<Books> _frozenBooks;
        public AuthorPage()
        {
            InitializeComponent();
            LoadData();
        }
        /// <summary>
        /// загрузка активных и замороженных книг текущего автора
        /// </summary>
        private void LoadData()
        {
            int currentUserId = UserData.CurrentUser.Id;
            if (UserData.CurrentUser.IsFrozen)
            {
                BtnAddNewBook.IsEnabled = false;
                BtnAddNewBook.ToolTip = "Добавление книг недоступно (аккаунт заморожен)";
            }
            // загружаем активные
            _activeBooks = Core.Context.Books.Where(b => b.AuthorId == currentUserId && !b.IsFrozen).ToList();
            ListBoxBooks.ItemsSource = _activeBooks;
            // загружаем замороженные
            _frozenBooks = Core.Context.Books.Where(b => b.AuthorId == currentUserId && b.IsFrozen).ToList();
            ListBoxFrozenBooks.ItemsSource = _frozenBooks;
        }
        private void BtnAddNewBook_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new __AddBookPage());
        }
        /// <summary>
        /// открыть окно редактирования
        /// </summary>
        private void BtnEditBook_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            if (btn.DataContext is Books selectedBook)
            {
                var window = new EditWindow(selectedBook);
                window.Owner = Window.GetWindow(this);
                window.ShowDialog();
            }
        }
        /// <summary>
        /// оспорить заморозку книги
        /// </summary>
        private void BtnDefrozeBook_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.DataContext is Books selectedBook)
            {
                MessageBoxResult result = MessageBox.Show($"Оспорить заморозку книги «{selectedBook.Title}»?", "Оспаривание заморозки", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var request = new UnfreezeApplications
                        {
                            UserId = UserData.CurrentUser.Id,
                            Reason = "Оспаривание заморозки книги",
                            StatusId = 1,
                            CreatedAt = DateTime.Now,
                            BookId = selectedBook.Id
                        };
                        Core.Context.UnfreezeApplications.Add(request);
                        Core.Context.SaveChanges();
                        MessageBox.Show("Заявка на разморозку отправлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",   MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}