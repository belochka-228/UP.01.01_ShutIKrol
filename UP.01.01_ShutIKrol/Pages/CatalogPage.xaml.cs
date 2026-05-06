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
using UP._01._01_ShutIKrol; 

namespace UP._01._01_ShutIKrol.Pages
{
    public partial class CatalogPage : Page
    {
        private List<Books> _allBooks;
        private List<Genres> _genres;
        private ListBox BookListBox => this.FindName("ListBoxBooks") as ListBox;

        public CatalogPage()
        {
            InitializeComponent();
            Loaded += CatalogPage_Loaded;
        }
        private void CatalogPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            _allBooks = Core.Context.Books.Include("Users").Where(b => !b.IsFrozen).ToList();

            _genres = Core.Context.Genres.ToList();
            _genres.Insert(0, new Genres { Id = 0, Name = "Все жанры" });

            CmbGenres.ItemsSource = _genres;
            CmbGenres.DisplayMemberPath = "Name";
            CmbGenres.SelectedValuePath = "Id";
            CmbGenres.SelectedIndex = 0;

            UpdateBookList(_allBooks);
        }
        private void UpdateBookList(List<Books> books)
        {
            if (BookListBox != null)
                BookListBox.ItemsSource = books;
        }
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = TxtSearch.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(searchText))
            {
                UpdateBookList(_allBooks);
                return;
            }
            var filtered = _allBooks.Where(b => b.Title.ToLower().Contains(searchText)
                         || (b.Users != null && b.Users.DisplayName.ToLower().Contains(searchText))).ToList();
            UpdateBookList(filtered);
        }
        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbSort.SelectedItem == null) return;
            string sortType = ((ComboBoxItem)CmbSort.SelectedItem).Content.ToString();

            List<Books> sortedList;
            switch (sortType)
            {
                case "По названию (А-Я)":
                    sortedList = _allBooks.OrderBy(b => b.Title).ToList();
                    break;
                case "По рейтингу (убыв.)":
                    sortedList = _allBooks.OrderByDescending(b =>
                        b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0).ToList();
                    break;
                default:
                    sortedList = _allBooks;
                    break;
            }
            if (!string.IsNullOrWhiteSpace(TxtSearch.Text))
            {
                string searchText = TxtSearch.Text.Trim().ToLower();
                sortedList = sortedList.Where(b => b.Title.ToLower().Contains(searchText)
                             || (b.Users != null && b.Users.DisplayName.ToLower().Contains(searchText))).ToList();
            }
            UpdateBookList(sortedList);
        }
        private void CmbGenres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CmbGenres.SelectedItem == null) return;
            var selectedGenre = CmbGenres.SelectedItem as Genres;
            if (selectedGenre == null || selectedGenre.Id == 0)
            {
                UpdateBookList(_allBooks);
                return;
            }
            var filtered = _allBooks.Where(b => b.BookGenres.Any(bg => bg.GenreId == selectedGenre.Id)).ToList();
            UpdateBookList(filtered);
        }
        private void ListBoxBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (BookListBox?.SelectedItem is Books selectedBook)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.MainFrame.Navigate(new BookDetailPage(selectedBook));
            }
        }
    }
}