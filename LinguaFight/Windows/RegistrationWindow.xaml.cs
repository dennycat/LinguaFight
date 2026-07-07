using System;
using System.Windows;
using LinguaFight.Models;

namespace LinguaFight.Windows
{
    public partial class RegistrationWindow : Window
    {
        private UserManager _userManager;

        public RegistrationWindow(UserManager userManager)
        {
            InitializeComponent();
            _userManager = userManager;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            string nickname = NicknameBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password.Trim();
            string confirmPassword = ConfirmPasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(nickname) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("Будь ласка, заповніть всі поля.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Паролі не співпадають.");
                return;
            }

            if (_userManager.IsNicknameTaken(nickname))
            {
                MessageBox.Show("Користувач з таким нікнеймом вже існує.");
                return;
            }

            try
            {
                var newUser = _userManager.Register(nickname, email, password);

                // Генеруємо код підтвердження
                var rnd = new Random();
                string code = rnd.Next(100000, 999999).ToString();
                newUser.ConfirmationCode = code;
                _userManager.SaveUsers();

                // Поки що показуємо код у MessageBox (замість email)
                MessageBox.Show($"Код підтвердження (тимчасово): {code}");

                // Відкриваємо вікно підтвердження
                ConfirmationWindow confirm = new ConfirmationWindow(_userManager, newUser);
                confirm.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
