namespace RetroGame2091.Core.Models
{
    public class Protagonist
    {
        public string Name { get; set; } = "Alex";
        public Attributes Attributes { get; set; } = new Attributes();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastPlayed { get; set; } = DateTime.Now;
        public int PlaytimeMinutes { get; set; } = 0;
        public Dictionary<string, object> CustomData { get; set; } = new Dictionary<string, object>();
    }
}