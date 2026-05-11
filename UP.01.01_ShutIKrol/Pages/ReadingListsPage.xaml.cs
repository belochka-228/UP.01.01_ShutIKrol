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
    /// Логика взаимодействия для ReadingListsPage.xaml
    /// </summary>
    public partial class ReadingListsPage : Page
    {
        private List<ReadingLists> _allUserBooks;
        private List<StatusBooks> _statuses;
        private List<Genres> _genres;
        public ReadingListsPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            int userId = UserData.CurrentUser.Id;
            _allUserBooks = Core.Context.ReadingLists.Where(r => r.UserId == userId).Include("Books.Users").ToList();

            _statuses = Core.Context.StatusBooks.ToList();
            _statuses.Insert(0, new StatusBooks { Id = 0, Name = "Все" });
            ListBoxStatuses.ItemsSource = _statuses;
            ListBoxStatuses.SelectedIndex = 0;

            _genres = Core.Context.Genres.ToList();
            _genres.Insert(0, new Genres { Id = 0, Name = "Все жанры" });
            CmbGenres.ItemsSource = _genres;
            CmbGenres.DisplayMemberPath = "Name";
            CmbGenres.SelectedIndex = 0;
        }
        private void CmbGenres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }
        private void ListBoxStatuses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList();
        }
        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }
        private void UpdateList()
        {
            if (_allUserBooks == null) return;

            var filtered = _allUserBooks.AsEnumerable();

            if (ListBoxStatuses.SelectedItem is StatusBooks selectedStatus && selectedStatus.Id != 0)
                filtered = filtered.Where(r => r.StatusId == selectedStatus.Id);

            string text = TxtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(text))
                filtered = filtered.Where(r => r.Books.Title.ToLower().Contains(text) || r.Books.Users.DisplayName.ToLower().Contains(text));

            if (CmbGenres.SelectedItem is Genres selectedGenre && selectedGenre.Id != 0)
                filtered = filtered.Where(r => r.Books.BookGenres.Any(bg => bg.GenreId == selectedGenre.Id));

            switch (CmbSort.SelectedIndex)
            {
                case 1:
                    filtered = filtered.OrderBy(r => r.Books.Title);
                    break;
                case 2:
                    filtered = filtered.OrderByDescending(r =>
                        r.Books.Reviews.Any() ? r.Books.Reviews.Average(rev => rev.Rating) : 0);
                    break;
            }
            ListBoxBooks.ItemsSource = filtered.ToList();
        }
        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.DataContext is ReadingLists selectedEntry)
            {
                var window = new MoveBookWindow(selectedEntry);
                window.Owner = Window.GetWindow(this);
                window.ShowDialog();
                LoadData();
                var currentStatus = ListBoxStatuses.SelectedItem as StatusBooks;
                if (currentStatus != null)
                    ListBoxStatuses.SelectedItem = _statuses.FirstOrDefault(s => s.Id == currentStatus.Id);
                else
                    ListBoxStatuses.SelectedIndex = 0;
            }
        }
        private void ListBoxBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxBooks.SelectedItem is ReadingLists entry)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.MainFrame.Navigate(new BookDetailPage(entry.Books));
            }
        }
    }
}
