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
    public partial class BookDetailPage : Page
    {
        private Books _book;
        public BookDetailPage(Books book)
        {
            InitializeComponent();
            _book = book;
            DataContext = _book;
            // Загрузка отзывов
            LoadReviews();
            // Показать кнопку заморозки, если пользователь — администратор
            if (UserData.CurrentUser != null && UserData.CurrentUser.Roles?.RoleName == "Администратор")
            {
                BtnFreezeBook.Visibility = Visibility.Visible;
            }
        }
        private void LoadReviews()
        {
            var reviews = Core.Context.Reviews.Where(r => r.BookId == _book.Id).Include("Users")  // подгружаем автора отзыва
                .ToList();
            ReviewsList.ItemsSource = reviews;
        }

        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            // Проверка: нет ли уже такой записи
            bool exists = Core.Context.ReadingLists.Any(r => r.UserId == UserData.CurrentUser.Id && r.BookId == _book.Id);
            if (exists)
            {
                MessageBox.Show("Книга уже в вашем списке.");
                return;
            }
            // Добавляем со статусом "В планах" (Id=2)
            var newItem = new ReadingLists
            {
                UserId = UserData.CurrentUser.Id,
                BookId = _book.Id,
                StatusId = 2,
                UpdatedAt = DateTime.Now
            };
            Core.Context.ReadingLists.Add(newItem);
            Core.Context.SaveChanges();
            MessageBox.Show("Книга добавлена в список «В планах».");
        }

        private void BtnComplaintBook_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Жалоба на книгу (пока заглушка).");
        }

        private void BtnComplaintAuthor_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Жалоба на автора (пока заглушка).");
        }

        private void BtnComplaintReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int reviewId = Convert.ToInt32(btn.Tag);
                MessageBox.Show($"Жалоба на отзыв #{reviewId} (пока заглушка).");
            }
        }s
        private void BtnFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Заморозить книгу «{_book.Title}»?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _book.IsFrozen = true;
                Core.Context.SaveChanges();
                MessageBox.Show("Книга заморожена.");
            }
        }
    }
}