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
using System.Windows.Shapes;

namespace UP._01._01_ShutIKrol.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
{
        private Books _book;
        private List<BookGenres> _bookGenres;
        private List<Genres> _availableGenres;
        public EditWindow(Books book)
        {
            InitializeComponent();
            _book = book;
            DataContext = _book;
            RefreshData();
        }
        private void RefreshData()
        {
            _bookGenres = Core.Context.BookGenres.Where(bg => bg.BookId == _book.Id).Include("Genres").ToList();
            ListBoxGenres.ItemsSource = _bookGenres;
            var allGenres = Core.Context.Genres.ToList();
            _availableGenres = allGenres.Where(g => !_bookGenres.Any(bg => bg.GenreId == g.Id)).OrderBy(g => g.Name).ToList();

            CmbGenre.ItemsSource = _availableGenres;
            CmbGenre.SelectedIndex = _availableGenres.Any() ? 0 : -1;
        }
        private void BtnAddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (!(CmbGenre.SelectedItem is Genres selectedGenre)) return;
            if (_bookGenres.Any(bg => bg.GenreId == selectedGenre.Id))
            {
                MessageBox.Show("Этот жанр уже добавлен.");
                return;
            }

            int newId = Core.Context.BookGenres.Any() ? Core.Context.BookGenres.Max(bg => bg.Id) + 1 : 1;
            var newBookGenre = new BookGenres { Id = newId, BookId = _book.Id, GenreId = selectedGenre.Id };
            Core.Context.BookGenres.Add(newBookGenre);
            Core.Context.SaveChanges();

            _bookGenres.Add(newBookGenre);
            ListBoxGenres.ItemsSource = _bookGenres.ToList();

            _availableGenres.Remove(selectedGenre);
            CmbGenre.ItemsSource = null;
            CmbGenre.ItemsSource = _availableGenres;
            CmbGenre.SelectedIndex = _availableGenres.Any() ? 0 : -1;
        }
        private void BtnDeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            var btn = (System.Windows.Controls.Button)sender;
            var bookGenre = btn.DataContext as BookGenres;
            if (bookGenre == null) return;

            Core.Context.BookGenres.Remove(bookGenre);
            Core.Context.SaveChanges();

            _bookGenres.Remove(bookGenre);
            ListBoxGenres.ItemsSource = _bookGenres.ToList();

            var genre = Core.Context.Genres.Find(bookGenre.GenreId);
            if (genre != null)
            {
                _availableGenres.Add(genre);
                _availableGenres = _availableGenres.OrderBy(g => g.Name).ToList();
                CmbGenre.ItemsSource = null;
                CmbGenre.ItemsSource = _availableGenres;
                CmbGenre.SelectedIndex = _availableGenres.Any() ? 0 : -1;
            }
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtBoxCoverPath.Text) ||
                string.IsNullOrWhiteSpace(TxtBoxTitle.Text) ||
                string.IsNullOrWhiteSpace(TxtBoxDescription.Text) ||
                string.IsNullOrWhiteSpace(TxtBoxContent.Text))
            {
                MessageBox.Show("Все поля обязательны для заполнения.");
                return;
            }
            try
            {
                _book.CoverPath = TxtBoxCoverPath.Text;
                _book.Title = TxtBoxTitle.Text;
                _book.Description = TxtBoxDescription.Text;
                _book.Content = TxtBoxContent.Text;
                Core.Context.SaveChanges();
                MessageBox.Show("Изменения сохранены!");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}