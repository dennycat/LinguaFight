#pragma warning disable CA1416

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Media;
using LinguaFight.Models;
using LinguaFight.Logic;

namespace LinguaFight.Windows
{
    public partial class BattleWindow : Window
    {
        private DictionaryModel? _dictionary;
        private WordEntry? _currentWord;
        private Random _rnd = new Random();

        private HashSet<string> _answeredWords = new();
        private UserManager _userManager;

        private double _monsterHP = 1.0;
        private double _robotHP = 1.0; // залишаємо для майбутньої логіки

        public BattleWindow(UserManager userManager, string dictionaryName)
        {
            InitializeComponent();

            _userManager = userManager;

            LoadImages();
            LoadDictionary(dictionaryName);

            if (_dictionary == null || _dictionary.Entries == null || _dictionary.Entries.Count == 0)
            {
                MessageBox.Show("Словник порожній або не завантажений.");
                Close();
                return;
            }

            UpdateCoinsDisplay();
            ShowNextWord();
        }

        // -----------------------------
        //  ЗАВАНТАЖЕННЯ СЛОВНИКА
        // -----------------------------
        private void LoadDictionary(string dictionaryName)
        {
            string filePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Dictionaries",
                dictionaryName,
                $"{dictionaryName}.json"
            );

            _dictionary = DictionaryLoader.LoadEncrypted(filePath);
        }

        // -----------------------------
        //  ЗАВАНТАЖЕННЯ КАРТИНОК
        // -----------------------------
        private void LoadImages()
        {
            string basePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Dictionaries",
                "Animals",
                "Images"
            );

            string robotPath = Path.Combine(basePath, "Robots", "Robot_1", "Robot_1.png");
            string monsterPath = Path.Combine(basePath, "Monsters", "Monster_1", "Monster_1.png");

            if (File.Exists(robotPath))
                RobotImage.Source = new BitmapImage(new Uri(robotPath, UriKind.Absolute));

            if (File.Exists(monsterPath))
                MonsterImage.Source = new BitmapImage(new Uri(monsterPath, UriKind.Absolute));
        }

        // -----------------------------
        //  ЗВУК МОНЕТКИ
        // -----------------------------
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

        // -----------------------------
        //  ОНОВЛЕННЯ МОНЕТ
        // -----------------------------
        private void UpdateCoinsDisplay()
        {
            if (_userManager.CurrentUser != null)
                CoinsDisplay.Text = $"Монети: {_userManager.CurrentUser.Coins}";
        }

        // -----------------------------
        //  Анімація блимання підказки
        // -----------------------------
        private async Task BlinkCorrectAnswer()
        {
            for (int i = 0; i < 3; i++)
            {
                ResultText.Opacity = 1;
                await Task.Delay(120);
                ResultText.Opacity = 0.2;
                await Task.Delay(120);
            }

            ResultText.Opacity = 1;
        }

        // -----------------------------
        //  ВИБІР НОВОГО СЛОВА
        // -----------------------------
        private void ShowNextWord()
        {
            if (_dictionary == null)
                return;

            var availableWords = _dictionary.Entries
                .Where(w => !_answeredWords.Contains(w.Ukr))
                .ToList();

            if (availableWords.Count == 0)
            {
                MonsterDefeated();
                return;
            }

            _currentWord = availableWords[_rnd.Next(availableWords.Count)];

            WordText.Text = _currentWord.Ukr;
            AnswerBox.Text = "";
            AnswerBox.Focus();
        }

        // -----------------------------
        //  ЛОГІКА ВІДПОВІДІ
        // -----------------------------
        private async void Answer_Click(object sender, RoutedEventArgs e)
        {
            if (_currentWord is null)
                return;

            string userAnswer = AnswerBox.Text.Trim().ToLower();
            string correct = _currentWord.Eng.ToLower();

            // -----------------------------
            //  ПРАВИЛЬНА ВІДПОВІДЬ
            // -----------------------------
            if (userAnswer == correct)
            {
                int coinsEarned = correct.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

                if (_userManager.CurrentUser != null)
                {
                    _userManager.CurrentUser.Coins += coinsEarned;
                    _userManager.SaveUsers();
                    UpdateCoinsDisplay();
                }

                PlayCoinSound();

                _answeredWords.Add(_currentWord.Ukr);

                _monsterHP -= 0.01;
                if (_monsterHP < 0) _monsterHP = 0;

                SetMonsterHP(_monsterHP);

                await Task.Delay(200);

                ResultText.Text = ""; // очищаємо підказку
                ShowNextWord();
            }
            else
            {
                // -----------------------------
                //  НЕПРАВИЛЬНА ВІДПОВІДЬ
                // -----------------------------
                ResultText.Text = $"Правильна відповідь: {correct}";
                ResultText.Foreground = System.Windows.Media.Brushes.Red;

                await BlinkCorrectAnswer();

                await Task.Delay(150);
                ShowNextWord();
            }
        }

        // -----------------------------
        //  HP МОНСТРА
        // -----------------------------
        private void SetMonsterHP(double percent)
        {
            double full = MonsterHPBar.Width;

            MonsterHPGreen.Width = full * percent;
            MonsterHPRed.Width = full * (1 - percent);

            if (percent <= 0)
                MonsterDefeated();
        }

        // -----------------------------
        //  ПЕРЕМОГА
        // -----------------------------
        private void MonsterDefeated()
        {
            MessageBox.Show("Монстра переможено!");
            this.Close();
        }

        // -----------------------------
        //  ENTER → Відповідь
        // -----------------------------
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Answer_Click(sender, new RoutedEventArgs());
        }

        // -----------------------------
        //  ВИХІД З БОЮ
        // -----------------------------
        private void ExitBattleButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

#pragma warning restore CA1416
