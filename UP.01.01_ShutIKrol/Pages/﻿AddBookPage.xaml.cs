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
    /// Логика взаимодействия для __AddBookPage.xaml
    /// </summary>
    public partial class __AddBookPage : Page
    {
        private List<Genres> _selectedGenres;
        private List<Genres> _availableGenres;
        public __AddBookPage()
        {
            InitializeComponent();
            _selectedGenres = new List<Genres>();
            RefreshGenres();
        }

        private void RefreshGenres()
        {
            var allGenres = Core.Context.Genres.OrderBy(g => g.Name).ToList();
            _availableGenres = allGenres.Where(g => !_selectedGenres.Any(sg => sg.Id == g.Id)).ToList();
            CmbGenre.ItemsSource = _availableGenres;
            CmbGenre.SelectedIndex = _availableGenres.Any() ? 0 : -1;
            ListBoxGenres.ItemsSource = _selectedGenres.ToList();
        }

        private void BtnAddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (CmbGenre.SelectedItem is Genres selectedGenre)
            {
                if (_selectedGenres.Any(g => g.Id == selectedGenre.Id))
                {
                    MessageBox.Show("Этот жанр уже выбран.");
                    return;
                }
                _selectedGenres.Add(selectedGenre);
                RefreshGenres();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            string coverPath = TxtCoverPath.Text.Trim();
            string title = TxtTitle.Text.Trim();
            string description = TxtDescription.Text.Trim();
            string content = TxtContent.Text.Trim();

            if (string.IsNullOrWhiteSpace(coverPath) ||
                string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("Все поля обязательны для заполнения.");
                return;
            }

            try
            {
                var newBook = new Books
                {
                    CoverPath = coverPath,
                    Title = title,
                    Description = description,
                    Content = content,
                    AuthorId = UserData.CurrentUser.Id,
                    IsFrozen = false
                };
                Core.Context.Books.Add(newBook);
                Core.Context.SaveChanges();

                if (_selectedGenres.Any())
                {
                    int maxId = Core.Context.BookGenres.Any() ? Core.Context.BookGenres.Max(bg => bg.Id) : 0;
                    foreach (var genre in _selectedGenres)
                    {
                        maxId++;
                        Core.Context.BookGenres.Add(new BookGenres
                        {
                            Id = maxId,
                            BookId = newBook.Id,
                            GenreId = genre.Id
                        });
                    }
                    Core.Context.SaveChanges();
                }

                MessageBox.Show("Книга успешно добавлена!");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении книги: {ex.Message}");
            }
        }
    }
}
