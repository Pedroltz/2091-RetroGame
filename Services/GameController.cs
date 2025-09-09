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
        private readonly ICombatOrchestrationService _combatOrchestrationService;

        public GameController(
            IUIService uiService,
            IPlayerSaveService playerSaveService,
            IChapterService chapterService,
            IGameConfigService configService,
            CharacterCreationMenu characterCreationMenu,
            SettingsMenu settingsMenu,
            IMusicService musicService,
            ICombatOrchestrationService combatOrchestrationService)
        {
            _uiService = uiService;
            _playerSaveService = playerSaveService;
            _chapterService = chapterService;
            _configService = configService;
            _characterCreationMenu = characterCreationMenu;
            _settingsMenu = settingsMenu;
            _musicService = musicService;
            _combatOrchestrationService = combatOrchestrationService;
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
            
            string? currentNodeId = null;
            
            while (currentChapter != null)
            {
                ChapterNode? currentNode = currentChapter.GetCurrentNode(currentNodeId);
                
                if (currentNode == null || currentNode.GameEnd)
                    break;

                // Use new dialog UI with current node
                _uiService.ShowDialogUI(currentChapter, currentNode);
                
                if (currentNode.Options.Count > 0)
                {
                    int choice = _uiService.ShowChapterOptions(currentNode.Options);
                    if (choice >= 0 && choice < currentNode.Options.Count)
                    {
                        var selectedOption = currentNode.Options[choice];
                        
                        // Check if this option starts combat
                        if (!string.IsNullOrEmpty(selectedOption.StartCombat))
                        {
                            string? nextChapter = _combatOrchestrationService.StartCombat(
                                selectedOption.StartCombat,
                                selectedOption.VictoryChapter,
                                selectedOption.DefeatChapter,
                                selectedOption.FleeChapter);
                            
                            if (!string.IsNullOrEmpty(nextChapter))
                            {
                                currentChapter = _chapterService.LoadChapter(nextChapter);
                                currentNodeId = null; // Reset to start node of new chapter
                            }
                            else
                            {
                                break; // Exit if no next chapter or combat was cancelled
                            }
                        }
                        // Check if navigating to another node within same chapter
                        else if (!string.IsNullOrEmpty(selectedOption.NextNode))
                        {
                            currentNodeId = selectedOption.NextNode;
                            // Stay in same chapter
                        }
                        // Check if navigating to another chapter
                        else if (!string.IsNullOrEmpty(selectedOption.NextChapter))
                        {
                            currentChapter = _chapterService.LoadChapter(selectedOption.NextChapter);
                            currentNodeId = null; // Reset to start node of new chapter
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(currentNode.NextNode))
                {
                    _uiService.ShowContinuePrompt();
                    _uiService.SafeReadKey();
                    currentNodeId = currentNode.NextNode;
                }
                else if (!string.IsNullOrEmpty(currentNode.NextChapter))
                {
                    _uiService.ShowContinuePrompt();
                    _uiService.SafeReadKey();
                    currentChapter = _chapterService.LoadChapter(currentNode.NextChapter);
                    currentNodeId = null; // Reset to start node of new chapter
                }
                else
                {
                    break;
                }
            }
            
            ChapterNode? finalNode = currentChapter?.GetCurrentNode(currentNodeId);
            if (finalNode?.GameEnd == true)
            {
                Console.WriteLine();
                _uiService.WriteWithColor("═══════ FIM DE JOGO ═══════", _configService.Config.Colors.Title);
            }
            
            Console.WriteLine();
            _uiService.WriteWithColor("Pressione qualquer tecla para voltar ao menu...", _configService.Config.Colors.NormalText);
            _uiService.SafeReadKey();
        }


    }
}