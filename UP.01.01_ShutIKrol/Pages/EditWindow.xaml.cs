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
    /// окно редактирования книги
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
        /// <summary>
        /// обновление списков жанров и доступных жанров
        /// </summary>
        private void RefreshData()
        {
            _bookGenres = Core.Context.BookGenres.Where(bg => bg.BookId == _book.Id).Include("Genres").ToList(); // получаем жанры книги из базы

            var allGenres = Core.Context.Genres.ToList(); // обновляем список доступных жанров
            _availableGenres = allGenres.Where(g => !_bookGenres.Any(bg => bg.GenreId == g.Id)).OrderBy(g => g.Name).ToList();

            // обновляем элементы
            ListBoxGenres.ItemsSource = _bookGenres;
            CmbGenre.ItemsSource = _availableGenres;
            CmbGenre.SelectedIndex = _availableGenres.Any() ? 0 : -1;
        }
        /// <summary>
        /// добавление жанра книге
        /// </summary>
        private void BtnAddGenre_Click(object sender, RoutedEventArgs e)
        {
            if (CmbGenre.SelectedItem == null) return;

            Genres selectedGenre = (Genres)CmbGenre.SelectedItem; //берем выбранный жанр

            if (_bookGenres.Any(bg => bg.GenreId == selectedGenre.Id))
            {
                MessageBox.Show("Этот жанр уже добавлен.");
                return;
            }

            int newId = 1; //находим следующий ID для записи
            if (Core.Context.BookGenres.Any())
            {
                newId = Core.Context.BookGenres.Max(bg => bg.Id) + 1; 
            }
            var newBookGenre = new BookGenres  //связь книга-жанр
            { 
                Id = newId, 
                BookId = _book.Id, 
                GenreId = selectedGenre.Id 
            };

            Core.Context.BookGenres.Add(newBookGenre);
            Core.Context.SaveChanges();

            // обновляем списки и интерфейс
            _bookGenres.Add(newBookGenre);
            RefreshData();
        }
        /// <summary>
        /// удаление жанра у книги
        /// </summary>  
        private void BtnDeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            BookGenres bookGenre = (BookGenres)btn.DataContext;
            if (bookGenre == null) return;

            Core.Context.BookGenres.Remove(bookGenre);
            Core.Context.SaveChanges();

            _bookGenres.Remove(bookGenre);
            RefreshData();           
        }
        /// <summary>
        /// сохранение изменений книги
        /// </summary>
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