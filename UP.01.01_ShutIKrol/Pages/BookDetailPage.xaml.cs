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
            if (UserData.CurrentUser.Roles != null)
            {
                if (UserData.CurrentUser.Roles.RoleName == "Администратор")
                {
                    BtnFreezeBook.Visibility = Visibility.Visible;
                }
            }
        }
        private void LoadReviews()
        {
            ReviewsList.Items.Clear();
            var reviews = Core.Context.Reviews.Where(r => r.BookId == _book.Id && !r.IsFrozen).Include("Users").ToList();
            foreach (var review in reviews)
            {
                ReviewsList.Items.Add(review);
            }
        }
        private void BtnFreezeReview_Loaded(object sender, RoutedEventArgs e)
        {
            if (UserData.CurrentUser?.Roles?.RoleName == "Администратор")
                ((Button)sender).Visibility = Visibility.Visible;
        }
        private void BtnRead_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_book.Content))
            {
                WindowReadText window = new WindowReadText(_book.Content);
                window.Owner = Window.GetWindow(this);
                window.Show();
            }
        }
        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            var window = new WindowAddToList(_book);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
        private void BtnComplaintBook_Click(object sender, RoutedEventArgs e)
        {
            ComplaintWindow window = new ComplaintWindow(1, _book.Title, _book.Id);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
        private void BtnComplaintAuthor_Click(object sender, RoutedEventArgs e)
        {
            string authorName = "Автор";
            if (_book.Users != null && _book.Users.DisplayName != null)
            {
                authorName = _book.Users.DisplayName;
            }
            ComplaintWindow window = new ComplaintWindow(3, authorName, _book.Id);
            window.Owner = Window.GetWindow(this);
            window.ShowDialog();
        }
        private void BtnComplaintReview_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Tag != null)
            {
                int reviewId = Convert.ToInt32(btn.Tag);
                ComplaintWindow window = new ComplaintWindow(2, $"Отзыв #{reviewId}", _book.Id);
                window.Owner = Window.GetWindow(this);
                window.ShowDialog();
            }
        }
        private void BtnSubmitReview_Click(object sender, RoutedEventArgs e)
        {
            string text = TxtNewReview.Text.Trim();
            if (text == "")
            {
                MessageBox.Show("Введите текст отзыва.");
                return;
            }
            ComboBoxItem selectedItem = (ComboBoxItem)CmbRating.SelectedItem;
            int rating = int.Parse(selectedItem.Content.ToString());
            Reviews newReview = new Reviews
            {
                BookId = _book.Id,
                UserId = UserData.CurrentUser.Id,
                Text = text,
                Rating = rating,
                CreatedAt = DateTime.Now,
                IsFrozen = false
            };
            Core.Context.Reviews.Add(newReview);
            Core.Context.SaveChanges();
            TxtNewReview.Clear();
            CmbRating.SelectedIndex = 0;
            LoadReviews();
            UpdateAverageRating();
        }
        private void UpdateAverageRating()
        {
            double avgRating = 0;
            if (_book.Reviews.Count > 0)
            {
                double sum = 0;
                foreach (var review in _book.Reviews)
                {
                    sum += review.Rating;
                }
                avgRating = sum / _book.Reviews.Count;
            }
            TxtRating.Text = Math.Round(avgRating).ToString();
        }
        private void BtnFreezeBook_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"Заморозить книгу «{_book.Title}»?","Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _book.IsFrozen = true;
                Core.Context.SaveChanges();
                MessageBox.Show("Книга заморожена.");
            }
        }
        private void BtnFreezeReview_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            int reviewId = (int)btn.Tag;
            var review = Core.Context.Reviews.FirstOrDefault(r => r.Id == reviewId);

            if (review == null)
            {
                MessageBox.Show("Отзыв не найден.");
                return;
            }
            if (review.IsFrozen)
            {
                MessageBox.Show("Этот отзыв уже заморожен.");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Заморозить этот отзыв?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                review.IsFrozen = true;
                Core.Context.SaveChanges();
                LoadReviews();
            }
        }
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}