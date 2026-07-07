using System.Windows;
using LinguaFight.Models;

namespace LinguaFight.Windows
{
    public partial class AchievementsWindow : Window
    {
        private UserManager _userManager;

        public AchievementsWindow(UserManager userManager)
        {
            InitializeComponent();

            _userManager = userManager;

            // Відображаємо кількість монет
            if (_userManager.CurrentUser != null)
            {
                CoinsText.Text = _userManager.CurrentUser.Coins.ToString();
            }
            else
            {
                CoinsText.Text = "0";
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
