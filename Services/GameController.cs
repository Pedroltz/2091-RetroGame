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
                        ShowPlayMenu();
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

        private void ShowPlayMenu()
        {
            bool hasSave = _playerSaveService.HasSaveFile();
            
            if (hasSave)
            {
                string[] playMenuOptions = {
                    "New Game",
                    "Load Game",
                    "Back"
                };
                
                int choice = _uiService.ShowMenu("╔═══════════════════════════════════╗\n║            JOGAR JOGO             ║\n╚═══════════════════════════════════╝", playMenuOptions);
                
                switch (choice)
                {
                    case 0: // New Game
                        if (ConfirmNewGame())
                        {
                            StartNewGame();
                        }
                        break;
                    case 1: // Load Game
                        LoadGame();
                        break;
                    case 2: // Back
                    case -1: // Escape
                        break;
                }
            }
            else
            {
                StartNewGame();
            }
        }

        private bool ConfirmNewGame()
        {
            string[] confirmOptions = {
                "Sim - Começar novo jogo",
                "Não - Voltar"
            };
            
            int choice = _uiService.ShowMenu("╔═══════════════════════════════════╗\n║     CONFIRMAR NOVO JOGO           ║\n║                                   ║\n║ Isso apagará seu progresso atual! ║\n╚═══════════════════════════════════╝", confirmOptions);
            
            return choice == 0;
        }

        private void StartNewGame()
        {
            _playerSaveService.PlayerSave.Character = new Protagonist();
            _playerSaveService.StartNewSession(); // Initialize session timer
            _playerSaveService.SaveGame();
            StartGame(true);
        }

        private void LoadGame()
        {
            _playerSaveService.LoadSave();
            var character = _playerSaveService.PlayerSave.Character;
            
            Console.WriteLine();
            Console.WriteLine();
            
            // Show save info inline
            _uiService.WriteWithColor("╔═══════════════════════════════════╗", _configService.Config.Colors.Title);
            _uiService.WriteWithColor("║         INFORMAÇÕES DO SAVE       ║", _configService.Config.Colors.Title);
            _uiService.WriteWithColor("╚═══════════════════════════════════╝", _configService.Config.Colors.Title);
            Console.WriteLine();
            
            _uiService.WriteWithColor($"Personagem: {character.Name}", _configService.Config.Colors.HighlightedText);
            _uiService.WriteWithColor($"Última sessão: {character.LastPlayed:dd/MM/yyyy HH:mm}", _configService.Config.Colors.NormalText);
            _uiService.WriteWithColor($"Tempo de jogo: {character.PlaytimeMinutes} minutos", _configService.Config.Colors.NormalText);
            Console.WriteLine();
            
            // Show character stats in compact format
            _uiService.WriteWithColor("Status:", _configService.Config.Colors.HighlightedText);
            _uiService.WriteWithColor($"Saúde: {character.Attributes.Saude} | Psicologia: {character.Attributes.Psicologia} | Força: {character.Attributes.Forca}", _configService.Config.Colors.NormalText);
            _uiService.WriteWithColor($"Inteligência: {character.Attributes.Inteligencia} | Conversação: {character.Attributes.Conversacao}", _configService.Config.Colors.NormalText);
            Console.WriteLine();
            
            _uiService.WriteWithColor("Pressione ENTER para continuar ou ESC para voltar...", _configService.Config.Colors.HighlightedText);
            
            var keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                return; // Return to play menu
            }
            
            _playerSaveService.StartNewSession(); // Initialize session timer for loaded game
            StartGame(false);
        }

        private void StartGame(bool isNewGame = true)
        {
            _uiService.ClearScreen();
            
            Chapter? currentChapter;
            string? currentNodeId = null;
            
            if (isNewGame)
            {
                // Show character creation screen
                _characterCreationMenu.ShowCharacterCreation();
                
                // Stop title music after character creation
                _musicService.StopMusic();
                
                _uiService.WriteWithColor($"Welcome to the story, {_playerSaveService.PlayerSave.Character.Name}!", _configService.Config.Colors.HighlightedText);
                Console.WriteLine();
                
                currentChapter = _chapterService.LoadChapter("init_inicio");
                _playerSaveService.UpdateGameProgress("init_inicio", null);
            }
            else
            {
                // Stop title music for loaded game
                _musicService.StopMusic();
                
                var character = _playerSaveService.PlayerSave.Character;
                currentChapter = _chapterService.LoadChapter(character.CurrentChapter);
                currentNodeId = character.CurrentNode;
                
                _uiService.WriteWithColor($"Continuando a história, {character.Name}!", _configService.Config.Colors.HighlightedText);
                Console.WriteLine();
            }
            
            if (currentChapter == null)
            {
                _uiService.WriteWithColor("Erro: Capítulo inicial não encontrado!", _configService.Config.Colors.Error);
                Console.WriteLine("Criando capítulo de exemplo...");
                _chapterService.CreateExampleChapters();
                currentChapter = _chapterService.LoadChapter("init_inicio");
            }
            
            while (currentChapter != null)
            {
                ChapterNode? currentNode = currentChapter.GetCurrentNode(currentNodeId);
                
                if (currentNode == null || currentNode.GameEnd)
                    break;

                // Update progress before showing dialog
                _playerSaveService.UpdateGameProgress(currentChapter.Id ?? "init_inicio", currentNodeId);

                // Use new dialog UI with current node
                _uiService.ShowDialogUI(currentChapter, currentNode);
                
                if (currentNode.Options.Count > 0)
                {
                    int choice;
                    do
                    {
                        choice = _uiService.ShowChapterOptions(currentNode.Options);
                        // If choice is -2, it means F5 was pressed and save was completed
                        // Re-render the dialog and options
                        if (choice == -2)
                        {
                            _uiService.ShowDialogUI(currentChapter, currentNode);
                        }
                    } while (choice == -2);
                    
                    if (choice >= 0 && choice < currentNode.Options.Count)
                    {
                        var selectedOption = currentNode.Options[choice];
                        
                        // Check if this option starts combat
                        if (!string.IsNullOrEmpty(selectedOption.StartCombat))
                        {
                            string? nextDestination = _combatOrchestrationService.StartCombat(
                                selectedOption.StartCombat,
                                selectedOption.VictoryChapter,
                                selectedOption.DefeatChapter,
                                selectedOption.FleeChapter,
                                selectedOption.VictoryNode,
                                selectedOption.DefeatNode,
                                selectedOption.FleeNode);
                            
                            if (!string.IsNullOrEmpty(nextDestination))
                            {
                                // Check if it's a node in current chapter or a new chapter
                                if (currentChapter.Nodes.ContainsKey(nextDestination))
                                {
                                    // It's a node within the same chapter
                                    currentNodeId = nextDestination;
                                }
                                else
                                {
                                    // It's a new chapter
                                    currentChapter = _chapterService.LoadChapter(nextDestination);
                                    currentNodeId = null; // Reset to start node of new chapter
                                }
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
                    HandleContinueWithSave();
                    currentNodeId = currentNode.NextNode;
                }
                else if (!string.IsNullOrEmpty(currentNode.NextChapter))
                {
                    _uiService.ShowContinuePrompt();
                    HandleContinueWithSave();
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

        private void HandleContinueWithSave()
        {
            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                
                if (keyInfo.Key == ConsoleKey.F5)
                {
                    _playerSaveService.SaveGameWithConfirmation(_uiService, _configService);
                    // Force screen refresh by breaking and letting the game re-render
                    return;
                }
                else
                {
                    break; // Any other key continues the game
                }
            }
        }
    }
}