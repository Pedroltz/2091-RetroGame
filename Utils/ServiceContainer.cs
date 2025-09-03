using RetroGame2091.Core.Interfaces;
using RetroGame2091.Services;
using RetroGame2091.UI.Components;
using RetroGame2091.UI.Menus;

namespace RetroGame2091.Utils
{
    public class ServiceContainer
    {
        private readonly Dictionary<Type, object> _services = new();

        public void RegisterServices()
        {
            // Register services
            var configService = new GameConfigService();
            var playerSaveService = new PlayerSaveService();
            var uiService = new UIService(configService, playerSaveService);
            var chapterService = new ChapterService(playerSaveService);
            var musicService = new MusicService();

            // Register menus
            var characterCreationMenu = new CharacterCreationMenu(uiService, playerSaveService, configService);
            var settingsMenu = new SettingsMenu(uiService, playerSaveService, configService);

            // Register game controller
            var gameController = new GameController(uiService, playerSaveService, chapterService, configService, characterCreationMenu, settingsMenu, musicService);

            // Store in container
            _services[typeof(IGameConfigService)] = configService;
            _services[typeof(IPlayerSaveService)] = playerSaveService;
            _services[typeof(IUIService)] = uiService;
            _services[typeof(IChapterService)] = chapterService;
            _services[typeof(IMusicService)] = musicService;
            _services[typeof(CharacterCreationMenu)] = characterCreationMenu;
            _services[typeof(SettingsMenu)] = settingsMenu;
            _services[typeof(GameController)] = gameController;
        }

        public T GetService<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }
            throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
        }
    }
}