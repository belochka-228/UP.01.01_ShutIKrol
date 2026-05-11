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
        }
        // При изменении статуса
        private void ListBoxStatuses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }
        // Поиск
        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateList();
        }
        // Сортировка
        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateList();
        }
        private void UpdateList()
        {
            if (_allUserBooks == null) return;

            var filtered = _allUserBooks.AsEnumerable();

            // Фильтр по статусу
            if (ListBoxStatuses.SelectedItem is StatusBooks selectedStatus && selectedStatus.Id != 0)
                filtered = filtered.Where(r => r.StatusId == selectedStatus.Id);
            // Поиск
            string text = TxtSearch.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(text))
                filtered = filtered.Where(r => r.Books.Title.ToLower().Contains(text) || r.Books.Users.DisplayName.ToLower().Contains(text));
            // Сортировка
            switch (CmbSort.SelectedIndex)
            {
                case 1: // По названию
                    filtered = filtered.OrderBy(r => r.Books.Title);
                    break;
                case 2: // По рейтингу
                    filtered = filtered.OrderByDescending(r =>
                        r.Books.Reviews.Any() ? r.Books.Reviews.Average(rev => rev.Rating) : 0);
                    break;
            }
            ListBoxBooks.ItemsSource = filtered.ToList();
        }
        // Переместить книгу
        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.DataContext is ReadingLists selectedEntry)
            {
                var window = new MoveBookWindow(selectedEntry);
                window.Owner = Window.GetWindow(this);
                window.ShowDialog();
                // Перезагружаем данные после закрытия окна
                LoadData();
                // Восстанавливаем выделение статуса (если возможно)
                var currentStatus = ListBoxStatuses.SelectedItem as StatusBooks;
                if (currentStatus != null)
                    ListBoxStatuses.SelectedItem = _statuses.FirstOrDefault(s => s.Id == currentStatus.Id);
                else
                    ListBoxStatuses.SelectedIndex = 0;
            }
        }
        // Открытие книги
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
