using Historia2092.Core.Interfaces;
using Historia2092.Core.Models;

namespace Historia2092.Services
{
    public class GameConfigService : IGameConfigService
    {
        private GameConfig _config;

        public GameConfig Config => _config;

        public GameConfigService()
        {
            _config = new GameConfig();
            LoadConfig();
        }

        public void LoadConfig()
        {
            _config = GameConfig.LoadConfig();
        }

        public void SaveConfig()
        {
            _config.SaveConfig();
        }

        public ConsoleColor GetColor(string colorName)
        {
            return _config.GetColor(colorName);
        }
    }
}