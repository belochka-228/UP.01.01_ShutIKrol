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
        private int _bookId;

        public ComplaintWindow(int targetTypeId, string targetName, int bookId)
        {
            InitializeComponent();

            _targetTypeId = targetTypeId;
            _targetName = targetName;
            _bookId = bookId;
            string typeText;
            switch (targetTypeId)
            {
                case 1: typeText = "книгу"; break;
                case 2: typeText = "отзыв"; break;
                case 3: typeText = "автора"; break;
                default: typeText = "объект"; break;
            }


            TxtTargetInfo.Text = $"Жалоба на {typeText}";
            TxtTargetName.Text = targetName;

            CmbReasons.ItemsSource = Core.Context.ComplaintReasons.ToList();
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

            var result = MessageBox.Show($"Отправить жалобу на {TxtTargetInfo.Text} «{_targetName}»?\nПричина: {selectedReason.Name}",
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

                MessageBox.Show("Жалоба отправлена.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}