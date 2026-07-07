using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LinguaFight.Models;

namespace LinguaFight.Logic
{
    public static class DictionaryLoader
    {
        public static DictionaryModel LoadEncrypted(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл словника не знайдено.", filePath);

            string json = File.ReadAllText(filePath);

            var model = JsonSerializer.Deserialize<DictionaryModel>(json);

            if (model is null)
                throw new InvalidOperationException("Не вдалося розшифрувати словник.");

            return model;
        }

        public static List<string> GetAvailableDictionaries()
        {
            string folder = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Resources",
                "Dictionaries"
            );

            if (!Directory.Exists(folder))
                return new List<string>();

            return Directory.GetFiles(folder, "*.json")
                            .Select(f => Path.GetFileNameWithoutExtension(f))
                            .ToList();
        }
    }
}
