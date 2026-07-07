using System.Windows;

namespace LinguaFight.Windows
{
    public partial class TimerSettingsWindow : Window
    {
        public int SelectedSeconds { get; private set; }

        public TimerSettingsWindow(int currentSeconds)
        {
            InitializeComponent();

            // Якщо таймер ще не встановлено — ставимо дефолт 1 хвилину
            if (currentSeconds <= 0)
            {
                HoursBox.Text = "00";
                MinutesBox.Text = "01";
                SecondsBox.Text = "00";
            }
            else
            {
                HoursBox.Text = (currentSeconds / 3600).ToString("00");
                MinutesBox.Text = ((currentSeconds % 3600) / 60).ToString("00");
                SecondsBox.Text = (currentSeconds % 60).ToString("00");
            }
        }

        private void SetTimer_Click(object? sender, RoutedEventArgs? e)
        {
            // Безпечний парсинг
            if (!int.TryParse(HoursBox.Text, out int h) ||
                !int.TryParse(MinutesBox.Text, out int m) ||
                !int.TryParse(SecondsBox.Text, out int s))
            {
                MessageBox.Show("Введіть коректні числові значення.");
                return;
            }

            // Обмеження як у цифрового будильника
            if (h < 0 || h > 23)
            {
                MessageBox.Show("Години мають бути в межах 0–23.");
                return;
            }

            if (m < 0 || m > 59)
            {
                MessageBox.Show("Хвилини мають бути в межах 0–59.");
                return;
            }

            if (s < 0 || s > 59)
            {
                MessageBox.Show("Секунди мають бути в межах 0–59.");
                return;
            }

            SelectedSeconds = h * 3600 + m * 60 + s;

            if (SelectedSeconds <= 0)
            {
                MessageBox.Show("Таймер має бути більше 0.");
                return;
            }

            MessageBox.Show("Таймер встановлено!");
            this.DialogResult = true;
            this.Close();
        }

        private void Close_Click(object? sender, RoutedEventArgs? e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
