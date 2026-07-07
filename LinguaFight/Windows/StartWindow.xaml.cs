using System;
using System.Windows;
using LinguaFight.Models;

namespace LinguaFight.Windows
{
    public partial class StartWindow : Window
    {
        private UserManager _userManager;

        public StartWindow()
        {
            InitializeComponent();

            string storagePath = System.IO.Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "users.json");

            _userManager = new UserManager(storagePath);
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Будь ласка, введіть логін та пароль.");
                return;
            }

            // Шукаємо користувача по нікнейму або email
            var user = _userManager.FindByNickname(login) ??
                       _userManager.FindByEmail(login);

            if (user == null)
            {
                MessageBox.Show("Користувача не знайдено. Створіть новий акаунт.");
                return;
            }

            // Перевірка паролю
            bool loginSuccess = _userManager.Login(user.Nickname, password);

            if (!loginSuccess)
            {
                MessageBox.Show("Невірний пароль.");
                return;
            }

            if (_userManager.CurrentUser is null)
            {
                MessageBox.Show("Користувача не знайдено.");
                return;
            }

            if (!_userManager.CurrentUser.IsEmailConfirmed)
            {
                MessageBox.Show("Будь ласка, підтвердіть вашу пошту.");
                return;
            }


            // Успішний вхід → головне меню
            MainMenuWindow menu = new MainMenuWindow(_userManager);
            menu.Show();
            this.Close();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow reg = new RegistrationWindow(_userManager);
            reg.Show();
            this.Close();
        }
    }
}
