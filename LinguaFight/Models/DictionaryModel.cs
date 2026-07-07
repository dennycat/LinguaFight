using System.Collections.Generic;

namespace LinguaFight.Models
{
    public class DictionaryModel
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public List<WordEntry> Entries { get; set; } = new();
    }
}
