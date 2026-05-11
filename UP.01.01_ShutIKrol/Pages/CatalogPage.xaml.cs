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
        private ComboBox _cmbSort;
        private ComboBox _cmbGenres;
        public CatalogPage()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            _allBooks = Core.Context.Books.Where(b => !b.IsFrozen).ToList();
            ListBoxBooks.ItemsSource = _allBooks;

            _cmbSort = new ComboBox { Width = 150, Height = 26 };
            _cmbSort.Items.Add(new ComboBoxItem { Content = "Без" });
            _cmbSort.Items.Add(new ComboBoxItem { Content = "По названию" });
            _cmbSort.Items.Add(new ComboBoxItem { Content = "По рейтингу" });
            _cmbSort.SelectedIndex = 0;
            _cmbSort.SelectionChanged += CmbSort_SelectionChanged;
            SortPlaceholder.Content = _cmbSort;

            var genres = Core.Context.Genres.ToList();
            genres.Insert(0, new Genres { Id = 0, Name = "Все жанры" });

            _cmbGenres = new ComboBox
            {
                Width = 150,
                Height = 26,
                DisplayMemberPath = "Name"
            };
            _cmbGenres.ItemsSource = genres;
            _cmbGenres.SelectedIndex = 0;
            _cmbGenres.SelectionChanged += CmbGenres_SelectionChanged;
            GenrePlaceholder.Content = _cmbGenres;
        }
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void CmbGenres_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ApplyFilters()
        {
            if (_allBooks == null) return;
            var filtered = _allBooks.AsEnumerable();

            string text = TxtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(text))
                filtered = filtered.Where(b => b.Title.ToLower().Contains(text) || b.Users.DisplayName.ToLower().Contains(text));

            if (_cmbGenres?.SelectedItem is Genres genre && genre.Id != 0)
                filtered = filtered.Where(b => b.BookGenres.Any(bg => bg.GenreId == genre.Id));

            switch (_cmbSort?.SelectedIndex)
            {
                case 1:
                    filtered = filtered.OrderBy(b => b.Title);
                    break;
                case 2:
                    filtered = filtered.OrderByDescending(b => b.Reviews.Any() ? b.Reviews.Average(r => r.Rating) : 0);
                    break;
            }
            ListBoxBooks.ItemsSource = filtered.ToList();
        }
        private void ListBoxBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxBooks.SelectedItem is Books selectedBook)
            {
                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.MainFrame.Navigate(new BookDetailPage(selectedBook));
            }
        }
        private void BtnAddToList_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Books selectedBook)
            {
                var window = new WindowAddToList(selectedBook);
                window.Owner = Window.GetWindow(this);
                window.ShowDialog();
            }
        }
    }
}

