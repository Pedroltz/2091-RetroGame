using Historia2092.Core.Models;

namespace Historia2092.Core.Interfaces
{
    public interface IGameConfigService
    {
        GameConfig Config { get; }
        void SaveConfig();
        void LoadConfig();
        ConsoleColor GetColor(string colorName);
    }
}