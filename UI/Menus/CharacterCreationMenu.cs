using Historia2092.Core.Interfaces;
using Historia2092.Core.Models;

namespace Historia2092.UI.Menus
{
    public class CharacterCreationMenu
    {
        private readonly IUIService _uiService;
        private readonly IPlayerSaveService _playerSaveService;
        private readonly IGameConfigService _configService;

        public CharacterCreationMenu(IUIService uiService, IPlayerSaveService playerSaveService, IGameConfigService configService)
        {
            _uiService = uiService;
            _playerSaveService = playerSaveService;
            _configService = configService;
        }

        public void ShowCharacterCreation()
        {
            _uiService.ClearScreen();
            
            // Configure name
            string newName = _uiService.GetUserInput("Configure seu personagem:", _playerSaveService.PlayerSave.Character.Name);
            if (!string.IsNullOrWhiteSpace(newName))
            {
                _playerSaveService.PlayerSave.Character.Name = newName;
            }
            
            Console.WriteLine();
            _uiService.WriteWithColor("═══════ DISTRIBUIÇÃO DE ATRIBUTOS ═══════", _configService.Config.Colors.Title);
            Console.WriteLine();
            
            _uiService.WriteWithColor("Você tem 50 pontos para distribuir entre seus atributos.", _configService.Config.Colors.HighlightedText);
            _uiService.WriteWithColor("Cada atributo começa com 50 pontos (máximo: 100).", _configService.Config.Colors.NormalText);
            Console.WriteLine();
            
            // Reset attributes to base values
            _playerSaveService.PlayerSave.Character.Attributes.Saude = 50;
            _playerSaveService.PlayerSave.Character.Attributes.Psicologia = 50;
            _playerSaveService.PlayerSave.Character.Attributes.Forca = 50;
            _playerSaveService.PlayerSave.Character.Attributes.Inteligencia = 50;
            _playerSaveService.PlayerSave.Character.Attributes.Conversacao = 50;
            _playerSaveService.PlayerSave.Character.Attributes.PointsToDistribute = 50;
            
            ShowAttributeDistributionInterface();
            
            _uiService.ClearScreen();
            _uiService.WriteWithColor($"Personagem {_playerSaveService.PlayerSave.Character.Name} criado com sucesso!", _configService.Config.Colors.HighlightedText);
            ShowFinalAttributeStatus();
            Console.WriteLine();
            _uiService.WriteWithColor("Pressione qualquer tecla para começar a aventura...", _configService.Config.Colors.NormalText);
            try
            {
                Console.ReadKey();
            }
            catch (InvalidOperationException)
            {
                Console.Read();
            }
            _playerSaveService.SaveGame();
        }

        private void ShowAttributeDistributionInterface()
        {
            int selectedAttribute = 0;
            bool configuring = true;
            
            while (configuring)
            {
                _uiService.ClearScreen();
                
                Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.Title);
                Console.WriteLine("╔════════════════════════════════════════════════╗");
                Console.WriteLine("║           DISTRIBUIÇÃO DE ATRIBUTOS            ║");
                Console.WriteLine("╚════════════════════════════════════════════════╝");
                Console.ResetColor();
                Console.WriteLine();
                
                Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.Title);
                Console.WriteLine($"═══ PERSONAGEM: {_playerSaveService.PlayerSave.Character.Name.ToUpper()} ═══");
                Console.ResetColor();
                Console.WriteLine();
                
                _uiService.WriteWithColor($"Pontos disponíveis: {_playerSaveService.PlayerSave.Character.Attributes.PointsToDistribute}", _configService.Config.Colors.HighlightedText);
                Console.WriteLine();
                
                string[] attributes = { "Saúde", "Psicologia", "Força", "Inteligência", "Conversação" };
                int[] values = { 
                    _playerSaveService.PlayerSave.Character.Attributes.Saude,
                    _playerSaveService.PlayerSave.Character.Attributes.Psicologia,
                    _playerSaveService.PlayerSave.Character.Attributes.Forca,
                    _playerSaveService.PlayerSave.Character.Attributes.Inteligencia,
                    _playerSaveService.PlayerSave.Character.Attributes.Conversacao
                };
                
                for (int i = 0; i < attributes.Length; i++)
                {
                    string bar = CreateProgressBar(values[i], 100);
                    
                    if (i == selectedAttribute)
                    {
                        Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.HighlightedText);
                        Console.WriteLine($"► {attributes[i]}: {values[i]}/100 {bar}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.NormalText);
                        Console.WriteLine($"  {attributes[i]}: {values[i]}/100 {bar}");
                        Console.ResetColor();
                    }
                }
                
                Console.WriteLine();
                if (_playerSaveService.PlayerSave.Character.Attributes.PointsToDistribute > 0)
                {
                    _uiService.WriteWithColor($"ATENÇÃO: Ainda há {_playerSaveService.PlayerSave.Character.Attributes.PointsToDistribute} pontos para distribuir!", _configService.Config.Colors.Error);
                    Console.WriteLine();
                }
                _uiService.WriteWithColor("↑↓ Navegar | ←→ Ajustar pontos | Enter Finalizar | Esc Cancelar", _configService.Config.Colors.Options);
                
                ConsoleKeyInfo keyInfo;
                try
                {
                    keyInfo = Console.ReadKey(true);
                }
                catch (InvalidOperationException)
                {
                    keyInfo = new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false);
                }
                
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedAttribute = selectedAttribute > 0 ? selectedAttribute - 1 : attributes.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedAttribute = selectedAttribute < attributes.Length - 1 ? selectedAttribute + 1 : 0;
                        break;
                    case ConsoleKey.LeftArrow:
                        DecreaseAttribute(selectedAttribute);
                        break;
                    case ConsoleKey.RightArrow:
                        IncreaseAttribute(selectedAttribute);
                        break;
                    case ConsoleKey.Enter:
                        if (_playerSaveService.PlayerSave.Character.Attributes.PointsToDistribute == 0)
                        {
                            configuring = false;
                        }
                        else
                        {
                            // Show warning message but don't wait for key press - just continue the loop
                            // This prevents the infinite loop issue
                        }
                        break;
                    case ConsoleKey.Escape:
                        configuring = false;
                        break;
                }
            }
        }

        private string CreateProgressBar(int current, int max)
        {
            int barLength = 20;
            int filled = (int)((double)current / max * barLength);
            string bar = "[" + new string('█', filled) + new string('░', barLength - filled) + "]";
            return bar;
        }

        private void IncreaseAttribute(int attributeIndex)
        {
            if (_playerSaveService.PlayerSave.Character.Attributes.PointsToDistribute <= 0) return;
            
            int currentValue = GetAttributeValueByIndex(attributeIndex);
            if (currentValue >= 100) return;
            
            SetAttributeValueByIndex(attributeIndex, currentValue + 1);
            _playerSaveService.PlayerSave.Character.Attributes.PointsToDistribute--;
        }
        
        private void DecreaseAttribute(int attributeIndex)
        {
            int currentValue = GetAttributeValueByIndex(attributeIndex);
            if (currentValue <= 50) return;
            
            SetAttributeValueByIndex(attributeIndex, currentValue - 1);
            _playerSaveService.PlayerSave.Character.Attributes.PointsToDistribute++;
        }

        private int GetAttributeValueByIndex(int index)
        {
            return index switch
            {
                0 => _playerSaveService.PlayerSave.Character.Attributes.Saude,
                1 => _playerSaveService.PlayerSave.Character.Attributes.Psicologia,
                2 => _playerSaveService.PlayerSave.Character.Attributes.Forca,
                3 => _playerSaveService.PlayerSave.Character.Attributes.Inteligencia,
                4 => _playerSaveService.PlayerSave.Character.Attributes.Conversacao,
                _ => 50
            };
        }
        
        private void SetAttributeValueByIndex(int index, int value)
        {
            switch (index)
            {
                case 0:
                    _playerSaveService.PlayerSave.Character.Attributes.Saude = value;
                    break;
                case 1:
                    _playerSaveService.PlayerSave.Character.Attributes.Psicologia = value;
                    break;
                case 2:
                    _playerSaveService.PlayerSave.Character.Attributes.Forca = value;
                    break;
                case 3:
                    _playerSaveService.PlayerSave.Character.Attributes.Inteligencia = value;
                    break;
                case 4:
                    _playerSaveService.PlayerSave.Character.Attributes.Conversacao = value;
                    break;
            }
        }

        private void ShowFinalAttributeStatus()
        {
            Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.Title);
            Console.WriteLine("═══════ STATUS FINAL ═══════");
            Console.ResetColor();
            
            _uiService.WriteWithColor($"💚 Saúde: {_playerSaveService.PlayerSave.Character.Attributes.Saude}", _configService.Config.Colors.HighlightedText);
            _uiService.WriteWithColor($"🧠 Psicologia: {_playerSaveService.PlayerSave.Character.Attributes.Psicologia}", _configService.Config.Colors.HighlightedText);
            _uiService.WriteWithColor($"💪 Força: {_playerSaveService.PlayerSave.Character.Attributes.Forca}", _configService.Config.Colors.HighlightedText);
            _uiService.WriteWithColor($"🧮 Inteligência: {_playerSaveService.PlayerSave.Character.Attributes.Inteligencia}", _configService.Config.Colors.HighlightedText);
            _uiService.WriteWithColor($"💬 Conversação: {_playerSaveService.PlayerSave.Character.Attributes.Conversacao}", _configService.Config.Colors.HighlightedText);
        }
    }
}