namespace LinguaFight.Models
{
    public class WordEntry
    {
        public string Ukr { get; set; }
        public string Eng { get; set; }
        public string Pol { get; set; }

        public WordEntry(string ukr, string eng, string pol)
        {
            Ukr = ukr;
            Eng = eng;
            Pol = pol;
        }
    }
}
