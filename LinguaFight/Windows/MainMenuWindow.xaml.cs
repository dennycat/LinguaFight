using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using LinguaFight.Models;
using LinguaFight.Windows;

namespace LinguaFight
{
    public partial class MainMenuWindow : Window
    {
        private string? _selectedDictionary;
        private int _timerSeconds = 60;

        private UserManager _userManager;
        //TEST
        // 🔥 Таймер між тренуваннями
        private DispatcherTimer? _trainingTimer;

        public MainMenuWindow(UserManager userManager)
        {
            InitializeComponent();
            _userManager = userManager;

            if (_userManager.CurrentUser != null)
            {
                WelcomeText.Text = $"Вітаю, {_userManager.CurrentUser.Nickname}!";
                CoinsDisplay.Text = $"Монети: {_userManager.CurrentUser.Coins}";
            }
        }

        // Старий конструктор — лишаємо для сумісності
        public MainMenuWindow()
        {
            InitializeComponent();

            string storagePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "users.json"
            );

            _userManager = new UserManager(storagePath);
        }

        private void SelectDictionary_Click(object? sender, RoutedEventArgs? e)
        {
            var window = new DictionarySelectWindow();
            bool? result = window.ShowDialog();

            if (result == true)
            {
                _selectedDictionary = window.Tag as string;
            }
        }

        private void TimerSettings_Click(object? sender, RoutedEventArgs? e)
        {
            var window = new TimerSettingsWindow(_timerSeconds);
            bool? result = window.ShowDialog();

            if (result == true)
            {
                _timerSeconds = window.SelectedSeconds;
            }
        }

        private void StartTraining_Click(object? sender, RoutedEventArgs? e)
        {
            if (_selectedDictionary is null)
            {
                MessageBox.Show("Спочатку виберіть словник.");
                return;
            }

            StartTrainingCycle();
        }

        // 🔥 Запуск першого тренування + підготовка таймера
        private void StartTrainingCycle()
        {
            OpenTrainingWindow();

            // Створюємо таймер, якщо його ще немає
            if (_trainingTimer == null)
            {
                _trainingTimer = new DispatcherTimer();
                _trainingTimer.Interval = TimeSpan.FromSeconds(_timerSeconds);
                _trainingTimer.Tick += (s, e) =>
                {
                    _trainingTimer.Stop();
                    OpenTrainingWindow();
                };
            }
        }

        // 🔥 Відкриття TrainingWindow
        private void OpenTrainingWindow()
        {
            var training = new TrainingWindow(_userManager, _selectedDictionary!, _timerSeconds);

            // Коли тренування завершено → запускаємо таймер
            training.TrainingCompleted += () =>
            {
                _trainingTimer?.Start();
            };

            training.Show();
        }

        private void Achievements_Click(object? sender, RoutedEventArgs? e)
        {
            var window = new AchievementsWindow(_userManager);
            window.ShowDialog();
        }

        private void Settings_Click(object? sender, RoutedEventArgs? e)
        {
            var window = new TimerSettingsWindow(_timerSeconds);
            bool? result = window.ShowDialog();

            if (result == true)
            {
                _timerSeconds = window.SelectedSeconds;
            }
        }

        private void Exit_Click(object? sender, RoutedEventArgs? e)
        {
            this.Close();
        }

        private void Battle_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Бойова система ще в розробці!");
        }

    }
}
