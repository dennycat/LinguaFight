using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media;
using LinguaFight.Models;
using LinguaFight.Logic;
using System.Media;
using System.IO;

namespace LinguaFight.Windows
{
    public partial class TrainingWindow : Window
    {
        private DictionaryModel _dictionary;
        private int _timerSeconds;

        private WordEntry? _currentWord;
        private Random _rnd = new Random();

        private bool _waitingForCorrectAnswer = true;

        private UserManager _userManager;

        // Подія завершення тренування
        public event Action? TrainingCompleted;

        public TrainingWindow(UserManager userManager, string dictionaryName, int timerSeconds)
        {
            InitializeComponent();

            _userManager = userManager;
            _timerSeconds = timerSeconds;

            string filePath = Path.Combine(
      AppDomain.CurrentDomain.BaseDirectory,
      "Resources",
      "Dictionaries",
      dictionaryName,
      $"{dictionaryName}.json"
  );


            _dictionary = DictionaryLoader.LoadEncrypted(filePath)!;

            UpdateCoinsDisplay();
            ShowNextWord();

            PlayTrainingStartSound();
        }

        private void UpdateCoinsDisplay()
        {
            if (_userManager.CurrentUser != null)
                CoinsDisplay.Text = $"Монети: {_userManager.CurrentUser.Coins}";
        }

        private void PlayCoinSound()
        {
            try
            {
                string soundPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Resources",
                    "Sounds",
                    "coin.wav"
                );

                if (!File.Exists(soundPath))
                    return;

                SoundPlayer player = new SoundPlayer(soundPath);
                player.Load();
                player.Play();
            }
            catch { }
        }

        private void PlayTrainingStartSound()
        {
            try
            {
                string soundPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Resources",
                    "Sounds",
                    "training_start.wav"
                );

                if (!File.Exists(soundPath))
                    return;

                SoundPlayer player = new SoundPlayer(soundPath);
                player.Load();
                player.Play();
            }
            catch { }
        }

        private void PulseCoinsDisplay()
        {
            var scale = new ScaleTransform(1.0, 1.0);
            CoinsDisplay.RenderTransformOrigin = new Point(0.5, 0.5);
            CoinsDisplay.RenderTransform = scale;

            var animation = new DoubleAnimation
            {
                From = 1.0,
                To = 1.3,
                Duration = TimeSpan.FromMilliseconds(150),
                AutoReverse = true,
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
        }

        private void ShowCoinsGainAnimation(int amount)
        {
            CoinsGainText.Text = $"+{amount}";
            CoinsGainText.Opacity = 1;

            var fade = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1.2),
                BeginTime = TimeSpan.FromSeconds(0.3)
            };

            CoinsGainText.BeginAnimation(OpacityProperty, fade);
        }

        private void ShowNextWord()
        {
            _waitingForCorrectAnswer = true;

            if (_dictionary?.Entries == null || _dictionary.Entries.Count == 0)
            {
                MessageBox.Show("Словник порожній або не завантажений.");
                return;
            }

            _currentWord = _dictionary.Entries[_rnd.Next(_dictionary.Entries.Count)];
            WordText.Text = _currentWord!.Ukr;
            ResultText.Text = "";
            AnswerBox.Text = "";
            AnswerBox.Focus();
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWord is null)
                return;

            string userAnswer = AnswerBox.Text.Trim().ToLower();
            string correct = _currentWord.Eng.ToLower();

            if (userAnswer == correct)
            {
                int coinsEarned = 0;
                int delay = 0; // миттєве закриття за замовчуванням

                // Монети тільки якщо відповідь правильна з першого разу
                if (_waitingForCorrectAnswer && _userManager.CurrentUser != null)
                {
                    coinsEarned = correct.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

                    _userManager.CurrentUser.Coins += coinsEarned;
                    _userManager.SaveUsers();

                    UpdateCoinsDisplay();
                    ShowCoinsGainAnimation(coinsEarned);
                    PulseCoinsDisplay();
                    PlayCoinSound();

                    delay = 1500; // пауза тільки коли є анімація монет
                }

                Task.Delay(delay).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        TrainingCompleted?.Invoke();
                        this.Close();
                    });
                });
            }
            else
            {
                // ❗ Помилка → монети більше не даємо
                _waitingForCorrectAnswer = false;

                ResultText.Text = $"Правильна відповідь: {correct}";
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Answer_Click(sender, new RoutedEventArgs());
            }
        }
    }
}
