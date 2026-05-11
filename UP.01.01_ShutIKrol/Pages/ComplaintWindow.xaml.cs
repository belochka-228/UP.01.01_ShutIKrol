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
    /// Логика взаимодействия для ComplaintWindow.xaml
    /// </summary>
    public partial class ComplaintWindow : Window
    {
        private int _targetTypeId;
        private string _targetName;
        private int _bookId;         // обязательный BookId
        private List<ComplaintReasons> _reasons;

        public ComplaintWindow(int targetTypeId, string targetName, int bookId)
        {
            InitializeComponent();

            _targetTypeId = targetTypeId;
            _targetName = targetName;
            _bookId = bookId;

            string typeText;
            if (targetTypeId == 1)
                typeText = "книгу";
            else if (targetTypeId == 2)
                typeText = "отзыв";
            else if (targetTypeId == 3)
                typeText = "автора";
            else
                typeText = "объект";

            TxtTargetInfo.Text = $"Жалоба на {typeText}";
            TxtTargetName.Text = targetName;
            _reasons = Core.Context.ComplaintReasons.ToList();
            CmbReasons.ItemsSource = _reasons;
            CmbReasons.SelectedIndex = 0;
        }
        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            if (UserData.CurrentUser == null)
            {
                MessageBox.Show("Войдите в аккаунт.");
                return;
            }
            if (CmbReasons.SelectedItem == null)
            {
                MessageBox.Show("Выберите причину жалобы.");
                return;
            }
            var selectedReason = (ComplaintReasons)CmbReasons.SelectedItem;
            var result = MessageBox.Show(
                $"Отправить жалобу на {TxtTargetInfo.Text} «{_targetName}»?\nПричина: {selectedReason.Name}",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;
            try
            {
                var complaint = new Complaints
                {
                    UserId = UserData.CurrentUser.Id,
                    TargetTypeId = _targetTypeId,
                    ReasonId = selectedReason.Id,
                    BookId = _bookId,
                    CreatedAt = DateTime.Now
                };
                Core.Context.Complaints.Add(complaint);
                Core.Context.SaveChanges();

                MessageBox.Show("Жалоба отправлена.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}