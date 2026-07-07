using System.Collections.Generic;

namespace LinguaFight.Models
{
    public class UserProfile
    {
        // --- Основні дані користувача ---
        public string Nickname { get; set; } = "";
        public string Email { get; set; } = "";

        // --- Статистика ---
        public int TotalCorrectAnswers { get; set; } = 0;
        public int TotalAttempts { get; set; } = 0;
        public List<string> CompletedDictionaries { get; set; } = new();

        // --- Реєстрація та безпека ---
        public string PasswordHash { get; set; } = "";
        public bool IsEmailConfirmed { get; set; } = false;

        // --- Код підтвердження email ---
        public string ConfirmationCode { get; set; } = "";

        // --- Прогрес словників ---
        public Dictionary<string, int> DictionaryProgress { get; set; } = new();

        // --- Досягнення ---
        public List<string> Achievements { get; set; } = new();
        // --- Монети ---
        public int Coins { get; set; } = 0;

    }
}
