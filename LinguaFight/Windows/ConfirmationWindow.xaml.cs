using System.Windows;
using LinguaFight.Models;

namespace LinguaFight.Windows
{
    public partial class ConfirmationWindow : Window
    {
        private UserManager _userManager;
        private UserProfile _user;

        public ConfirmationWindow(UserManager userManager, UserProfile user)
        {
            InitializeComponent();
            _userManager = userManager;
            _user = user;
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            string enteredCode = CodeBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(enteredCode))
            {
                MessageBox.Show("Введіть код.");
                return;
            }

            if (enteredCode != _user.ConfirmationCode)
            {
                MessageBox.Show("Невірний код підтвердження.");
                return;
            }

            // Код правильний → підтверджуємо email
            _user.IsEmailConfirmed = true;
            _user.ConfirmationCode = "";
            _userManager.SaveUsers();

            MessageBox.Show("Пошта успішно підтверджена!");

            // Повертаємось у StartWindow
            StartWindow start = new StartWindow();
            start.Show();
            this.Close();
        }
    }
}
