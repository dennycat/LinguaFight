using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LinguaFight.Models
{
    public class UserManager
    {
        private readonly string _storagePath;

        // Поточний залогінений користувач
        public UserProfile? CurrentUser { get; private set; }

        // Всі зареєстровані користувачі
        private List<UserProfile> _users = new();

        public UserManager(string storagePath)
        {
            _storagePath = storagePath;
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (!File.Exists(_storagePath))
            {
                _users = new List<UserProfile>();
                return;
            }

            var json = File.ReadAllText(_storagePath);
            _users = JsonSerializer.Deserialize<List<UserProfile>>(json) ?? new List<UserProfile>();
        }

        public void SaveUsers()
        {
            var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_storagePath, json);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Перевірка зайнятості нікнейму
        public bool IsNicknameTaken(string nickname)
        {
            return _users.Any(u => u.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase));
        }

        // Пошук по нікнейму
        public UserProfile? FindByNickname(string nickname)
        {
            return _users.FirstOrDefault(u =>
                u.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase));
        }

        // Пошук по email
        public UserProfile? FindByEmail(string email)
        {
            return _users.FirstOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        // Реєстрація нового користувача
        public UserProfile Register(string nickname, string email, string password)
        {
            if (IsNicknameTaken(nickname))
                throw new InvalidOperationException("Користувач з таким нікнеймом вже існує.");

            if (IsEmailTaken(email))
                throw new InvalidOperationException("Ця електронна пошта вже використовується.");

            var user = new UserProfile
            {
                Nickname = nickname,
                Email = email,
                PasswordHash = HashPassword(password),
                IsEmailConfirmed = false,
                DictionaryProgress = new Dictionary<string, int>(),
                Achievements = new List<string>(),
                Coins = 0
            };

            _users.Add(user);
            SaveUsers();

            return user;
        }


        // Логін через нікнейм
        public bool Login(string nickname, string password)
        {
            var user = FindByNickname(nickname);
            if (user == null)
                return false;

            var hash = HashPassword(password);
            if (!string.Equals(user.PasswordHash, hash, StringComparison.Ordinal))
                return false;

            CurrentUser = user;
            return true;
        }

        // Підтвердження email
        public void ConfirmEmail(string nickname)
        {
            var user = FindByNickname(nickname);
            if (user == null) return;

            user.IsEmailConfirmed = true;
            SaveUsers();
        }

        // Збереження прогресу словника
        public void SaveDictionaryProgress(string dictionaryName, int progress)
        {
            if (CurrentUser == null)
                return;

            CurrentUser.DictionaryProgress[dictionaryName] = progress;
            SaveUsers();
        }

        // Метод перевірки email:
        public bool IsEmailTaken(string email)
        {
            return _users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

    }
}
