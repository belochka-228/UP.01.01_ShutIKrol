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
using System.Windows.Shapes;

namespace UP._01._01_ShutIKrol.Pages
{
    /// <summary>
    /// Логика взаимодействия для WindowAddToList.xaml
    /// </summary>
    public partial class WindowAddToList : Window
    {
        private Books _book;
        private List<StatusBooks> _statuses;
        private StatusBooks _selectedStatus;
        private bool _isRadioChecked = false;
        public WindowAddToList(Books book)
        {
            InitializeComponent();
            _book = book;
            DataContext = _book;
            _statuses = Core.Context.StatusBooks.ToList();
            ListBoxStatuses.ItemsSource = _statuses;
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            _isRadioChecked = true;
            var radio = (RadioButton)sender;
            _selectedStatus = (StatusBooks)radio.DataContext;
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (UserData.CurrentUser == null)
            {
                MessageBox.Show("Войдите в аккаунт.");
                return;
            }
            if (!_isRadioChecked || _selectedStatus == null)
            {
                MessageBox.Show("Выберите статус.");
                return;
            }
            try
            {
                bool exists = Core.Context.ReadingLists.Any(r => r.UserId == UserData.CurrentUser.Id && r.BookId == _book.Id);
                if (exists)
                {
                    MessageBox.Show("Эта книга уже есть в вашем списке.");
                    this.DialogResult = false;
                    this.Close();
                    return;
                }
                var newEntry = new ReadingLists
                {
                    UserId = UserData.CurrentUser.Id,
                    BookId = _book.Id,
                    StatusId = _selectedStatus.Id,
                    UpdatedAt = DateTime.Now
                };
                Core.Context.ReadingLists.Add(newEntry);
                Core.Context.SaveChanges();
                MessageBox.Show("Книга успешно сохранена!");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }
    }
}
