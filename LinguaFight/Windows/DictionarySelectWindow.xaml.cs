using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LinguaFight.Logic;

namespace LinguaFight.Windows
{
    public partial class DictionarySelectWindow : Window
    {
        private string? _selectedDictionary;

        public DictionarySelectWindow()
        {
            InitializeComponent();
            LoadDictionaries();

            CurrentDictionaryText.Text = "(не вибрано)";
            CurrentDictionaryText.Foreground = Brushes.Red;
        }

        private void LoadDictionaries()
        {
            var dictionaries = DictionaryLoader.GetAvailableDictionaries();

            if (dictionaries == null || dictionaries.Count == 0)
            {
                MessageBox.Show("Не знайдено жодного словника у Resources/Dictionaries.");
                DictionaryList.ItemsSource = null;
                return;
            }

            DictionaryList.ItemsSource = dictionaries;
            DictionaryList.SelectionChanged += DictionaryList_SelectionChanged;
        }

        private void DictionaryList_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            if (DictionaryList.SelectedItem is string selected)
            {
                _selectedDictionary = selected;
                CurrentDictionaryText.Text = selected;
                CurrentDictionaryText.Foreground = Brushes.DeepSkyBlue;
            }
            else
            {
                _selectedDictionary = null;
                CurrentDictionaryText.Text = "(не вибрано)";
                CurrentDictionaryText.Foreground = Brushes.Red;
            }
        }

        private void Select_Click(object? sender, RoutedEventArgs? e)
        {
            if (_selectedDictionary is string selected)
            {
                this.Tag = selected;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Будь ласка, виберіть словник.");
            }
        }

        private void Back_Click(object? sender, RoutedEventArgs? e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
