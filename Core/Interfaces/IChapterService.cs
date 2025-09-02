using Historia2092.Core.Models;

namespace Historia2092.Core.Interfaces
{
    public interface IChapterService
    {
        Chapter? LoadChapter(string id);
        void SaveChapter(Chapter chapter);
        void CreateExampleChapters();
    }
}