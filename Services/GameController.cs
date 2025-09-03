using RetroGame2091.Core.Interfaces;
using RetroGame2091.Core.Models;
using RetroGame2091.UI.Menus;

namespace RetroGame2091.Services
{
    public class GameController
    {
        private readonly IUIService _uiService;
        private readonly IPlayerSaveService _playerSaveService;
        private readonly IChapterService _chapterService;
        private readonly IGameConfigService _configService;
        private readonly CharacterCreationMenu _characterCreationMenu;
        private readonly SettingsMenu _settingsMenu;
        private readonly IMusicService _musicService;

        public GameController(
            IUIService uiService,
            IPlayerSaveService playerSaveService,
            IChapterService chapterService,
            IGameConfigService configService,
            CharacterCreationMenu characterCreationMenu,
            SettingsMenu settingsMenu,
            IMusicService musicService)
        {
            _uiService = uiService;
            _playerSaveService = playerSaveService;
            _chapterService = chapterService;
            _configService = configService;
            _characterCreationMenu = characterCreationMenu;
            _settingsMenu = settingsMenu;
            _musicService = musicService;
        }

        public void Run()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;
            
            _musicService.StartTitleMusic();
            _uiService.ShowInitialScreen();
            
            bool play = true;
            while (play)
            {
                string[] mainMenuOptions = {
                    "Play",
                    "Settings", 
                    "Exit"
                };
                
                int choice = _uiService.ShowMenu("╔═══════════════════════════════════╗\n║           MENU PRINCIPAL          ║\n╚═══════════════════════════════════╝", mainMenuOptions);
                
                switch (choice)
                {
                    case 0: // Play
                        StartGame();
                        break;
                    case 1: // Settings
                        _settingsMenu.ShowSettings();
                        break;
                    case 2: // Exit
                    case -1: // Escape
                        play = false;
                        break;
                }
            }
            
            _musicService.StopMusic();
            _uiService.WriteWithColor("\nThank you for playing 2091!\n", _configService.Config.Colors.HighlightedText);
        }

        private void StartGame()
        {
            _uiService.ClearScreen();
            
            // Show character creation screen
            _characterCreationMenu.ShowCharacterCreation();
            
            // Stop title music after character creation
            _musicService.StopMusic();
            
            _uiService.WriteWithColor($"Welcome to the story, {_playerSaveService.PlayerSave.Character.Name}!", _configService.Config.Colors.HighlightedText);
            Console.WriteLine();
            
            Chapter? currentChapter = _chapterService.LoadChapter("init_inicio");
            
            if (currentChapter == null)
            {
                _uiService.WriteWithColor("Erro: Capítulo inicial não encontrado!", _configService.Config.Colors.Error);
                Console.WriteLine("Criando capítulo de exemplo...");
                _chapterService.CreateExampleChapters();
                currentChapter = _chapterService.LoadChapter("init_inicio");
            }
            
            while (currentChapter != null && !currentChapter.GameEnd)
            {
                _uiService.ClearScreen();
                ExecuteChapter(currentChapter);
                
                if (currentChapter.Options.Count > 0)
                {
                    int choice = _uiService.ShowChapterOptions(currentChapter.Options);
                    if (choice >= 0 && choice < currentChapter.Options.Count)
                    {
                        string nextId = currentChapter.Options[choice].NextChapter;
                        currentChapter = _chapterService.LoadChapter(nextId);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(currentChapter.NextChapter))
                {
                    Console.WriteLine();
                    _uiService.WriteWithColor("Pressione qualquer tecla para continuar...", _configService.Config.Colors.NormalText);
                    try
            {
                Console.ReadKey();
            }
            catch (InvalidOperationException)
            {
                Console.Read();
            }
                    currentChapter = _chapterService.LoadChapter(currentChapter.NextChapter);
                }
                else
                {
                    break;
                }
            }
            
            if (currentChapter?.GameEnd == true)
            {
                Console.WriteLine();
                _uiService.WriteWithColor("═══════ FIM DE JOGO ═══════", _configService.Config.Colors.Title);
            }
            
            Console.WriteLine();
            _uiService.WriteWithColor("Pressione qualquer tecla para voltar ao menu...", _configService.Config.Colors.NormalText);
            try
            {
                Console.ReadKey();
            }
            catch (InvalidOperationException)
            {
                Console.Read();
            }
        }

        private void ExecuteChapter(Chapter chapter)
        {
            if (!string.IsNullOrEmpty(chapter.Title))
            {
                Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.Title);
                Console.WriteLine($"═══ {chapter.Title.ToUpper()} ═══");
                Console.ResetColor();
                Console.WriteLine();
            }
            
            // Show skip hint if text animation is enabled and hints are enabled
            if (_configService.Config.Settings.TextSpeed > 0 && _configService.Config.Settings.ShowHints)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("(Press Enter to skip text animation)");
                Console.ResetColor();
                Console.WriteLine();
            }

            _uiService.WriteChapterTextWithEffect(chapter.Text, _configService.Config.Colors.NormalText);
        }

    }
}