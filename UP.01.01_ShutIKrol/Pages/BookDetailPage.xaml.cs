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
        private List<ComplaintTargetTypes> _complaintTargetTypes;
        private List<Reviews> _reviews;

        public BookDetailPage(Books book)
        {
            InitializeComponent();

            _book = book;
            DataContext = _book;

            double avgRating = _book.Reviews.Any() ? _book.Reviews.Average(r => r.Rating) : 0;
            TxtRating.Text = avgRating.ToString("0");

            _complaintTargetTypes = Core.Context.ComplaintTargetTypes.ToList();
            LoadReviews();

            if (UserData.CurrentUser != null && UserData.CurrentUser.Roles?.RoleName == "Администратор")
            {
                BtnFreezeBook.Visibility = Visibility.Visible;
            }
        }
        private void LoadReviews()
        {
            _reviews = Core.Context.Reviews.Where(r => r.BookId == _book.Id).Include("Users").ToList();
            ReviewsList.ItemsSource = _reviews;
        }
        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_book.Content))
            {
                WindowReadText window = new WindowReadText(_book.Content);
                window.Owner = Window.GetWindow(this);
                window.Show();
            }
            else
            {
                MessageBox.Show("У этой книги еще нет текста для чтения.");
            }
        }
        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowAddToList(_book);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
            NavigationService?.Navigate(new CatalogPage());
        }
        private void BtnComplaintBook_Click(object sender, RoutedEventArgs e)
        {
            var window = new ComplaintWindow(1, _book.Title, _book.Id);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
        private void BtnComplaintAuthor_Click(object sender, RoutedEventArgs e)
        {
            var window = new ComplaintWindow(3, _book.Users?.DisplayName ?? "Автор", _book.Id);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
        private void BtnComplaintReview_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                int reviewId = Convert.ToInt32(btn.Tag);
                var window = new ComplaintWindow(2, $"Отзыв #{reviewId}", _book.Id);
                window.Owner = Window.GetWindow(this);
                window.ShowDialog();
            }
        }
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
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}