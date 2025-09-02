using Newtonsoft.Json;

namespace Historia2092.Core.Models
{
    public class SkillRequirement
    {
        public string Skill { get; set; } = "";
        public int MinValue { get; set; } = 0;
    }

    public class Option
    {
        public string Text { get; set; } = "";
        public string NextChapter { get; set; } = "";
        public Dictionary<string, object>? Conditions { get; set; }
        public SkillRequirement? SkillRequirement { get; set; }
    }

    public class Chapter
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public List<string> Text { get; set; } = new List<string>();
        public List<Option> Options { get; set; } = new List<Option>();
        public bool GameEnd { get; set; } = false;
        public string? NextChapter { get; set; }

        public static Chapter? LoadChapter(string id)
        {
            try
            {
                string chapterPath = Path.Combine("Chapters", "Capitulo1", $"{id}.json");
                if (File.Exists(chapterPath))
                {
                    string json = File.ReadAllText(chapterPath);
                    return JsonConvert.DeserializeObject<Chapter>(json);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading chapter {id}: {ex.Message}");
            }
            
            return null;
        }

        public void SaveChapter()
        {
            try
            {
                string chapterPath = Path.Combine("Chapters", "Capitulo1", $"{Id}.json");
                Directory.CreateDirectory(Path.GetDirectoryName(chapterPath)!);
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(chapterPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving chapter {Id}: {ex.Message}");
            }
        }
    }
}