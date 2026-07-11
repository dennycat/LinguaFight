using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace LinguaFight.Windows
{
    public partial class BattleWindow : Window
    {
        public BattleWindow()
        {
            InitializeComponent();
            LoadImages();
        }

        private void LoadImages()
        {
            // Базовий шлях до словника Animals
            string basePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Dictionaries",
                "Animals",
                "Images"
            );

            // Шляхи до робота і монстра
            string robotPath = Path.Combine(basePath, "Robots", "Robot_1", "Robot_1.png");
            string monsterPath = Path.Combine(basePath, "Monsters", "Monster_1", "Monster_1.png");

            // 🔥 Показуємо реальні шляхи (дуже важливо)
            MessageBox.Show("Robot path:\n" + robotPath);
            MessageBox.Show("Monster path:\n" + monsterPath);

            // Завантаження робота
            if (File.Exists(robotPath))
            {
                RobotImage.Source = new BitmapImage(new Uri(robotPath, UriKind.Absolute));
            }
            else
            {
                MessageBox.Show($"Не знайдено картинку робота:\n{robotPath}");
            }

            // Завантаження монстра
            if (File.Exists(monsterPath))
            {
                MonsterImage.Source = new BitmapImage(new Uri(monsterPath, UriKind.Absolute));
            }
            else
            {
                MessageBox.Show($"Не знайдено картинку монстра:\n{monsterPath}");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Answer_Click(sender, new RoutedEventArgs());
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            // тут буде бойова логіка
        }
    }
}
