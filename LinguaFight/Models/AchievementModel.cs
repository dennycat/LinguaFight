namespace LinguaFight.Models
{
    public class AchievementModel
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string IconPath { get; set; } = "";
        public bool IsUnlocked { get; set; } = false;
    }
}
