using RetroGame2091.Core.Interfaces;
using RetroGame2091.Core.Models;

namespace RetroGame2091.UI.Components
{
    public class UIService : IUIService
    {
        private readonly IGameConfigService _configService;
        private readonly IPlayerSaveService _playerSaveService;

        public UIService(IGameConfigService configService, IPlayerSaveService playerSaveService)
        {
            _configService = configService;
            _playerSaveService = playerSaveService;
        }

        public int ShowMenu(string title, string[] options, int startIndex = 0)
        {
            int currentIndex = startIndex;
            
            while (true)
            {
                ClearScreen();
                
                Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.Title);
                Console.WriteLine(title);
                Console.ResetColor();
                Console.WriteLine();
                
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == currentIndex)
                    {
                        Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.HighlightedText);
                        Console.WriteLine($"► {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.Options);
                        Console.WriteLine($"  {options[i]}");
                        Console.ResetColor();
                    }
                }
                
                Console.WriteLine();
                if (_configService.Config.Settings.ShowHints)
                {
                    WriteWithColor("Use ↑↓ para navegar, Enter para selecionar", _configService.Config.Colors.NormalText);
                }
                
                var keyInfo = SafeReadKey(true);
                
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        currentIndex = currentIndex > 0 ? currentIndex - 1 : options.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        currentIndex = currentIndex < options.Length - 1 ? currentIndex + 1 : 0;
                        break;
                    case ConsoleKey.Enter:
                        return currentIndex;
                    case ConsoleKey.Escape:
                        return -1;
                }
            }
        }

        public string GetUserInput(string prompt, string currentValue = "")
        {
            Console.WriteLine();
            Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.Title);
            Console.WriteLine("╔══════════════════════════════════════════════╗");
            Console.WriteLine("║                DIGITE SEU NOME               ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            
            WriteWithColor($"Nome atual: {currentValue}", _configService.Config.Colors.HighlightedText);
            Console.WriteLine();
            WriteWithColor("┌─────────────────────────────────────────────┐", _configService.Config.Colors.NormalText);
            WriteWithColor("│ Digite seu nome (máx: 20 caracteres)        │", _configService.Config.Colors.NormalText);
            WriteWithColor("│ Pressione Enter para confirmar              │", _configService.Config.Colors.NormalText);
            WriteWithColor("│ Pressione Escape para cancelar              │", _configService.Config.Colors.NormalText);
            WriteWithColor("└─────────────────────────────────────────────┘", _configService.Config.Colors.NormalText);
            Console.WriteLine();
            
            Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.HighlightedText);
            Console.Write("Nome: ");
            Console.ResetColor();
            
            string input = "";
            ConsoleKeyInfo keyInfo;
            
            do
            {
                keyInfo = SafeReadKey(true);
                
                if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                    Console.Write("\b \b");
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    return currentValue;
                }
                else if (char.IsLetterOrDigit(keyInfo.KeyChar) || char.IsWhiteSpace(keyInfo.KeyChar))
                {
                    if (input.Length < 20)
                    {
                        input += keyInfo.KeyChar;
                        Console.Write(keyInfo.KeyChar);
                    }
                }
            } while (keyInfo.Key != ConsoleKey.Enter);
            
            return string.IsNullOrWhiteSpace(input) ? currentValue : input.Trim();
        }

        public void WriteWithColor(string text, string color)
        {
            Console.ForegroundColor = _configService.GetColor(color);
            Console.WriteLine(text);
            Console.ResetColor();
        }

        public void WriteTextWithEffect(string text, string color)
        {
            Console.ForegroundColor = _configService.GetColor(color);
            
            if (_configService.Config.Settings.TextSpeed > 0)
            {
                bool skipAnimation = false;
                int charIndex = 0;
                
                while (charIndex < text.Length && !skipAnimation)
                {
                    if (Console.KeyAvailable)
                    {
                        var keyInfo = SafeReadKey(true);
                        if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            skipAnimation = true;
                            Console.Write(text.Substring(charIndex));
                        }
                    }
                    else
                    {
                        Console.Write(text[charIndex]);
                        charIndex++;
                        Thread.Sleep(_configService.Config.Settings.TextSpeed);
                    }
                }
                
                if (!skipAnimation && charIndex < text.Length)
                {
                    Console.Write(text.Substring(charIndex));
                }
            }
            else
            {
                Console.Write(text);
            }
            
            Console.ResetColor();
        }

        public void WriteChapterTextWithEffect(List<string> lines, string color)
        {
            Console.ForegroundColor = _configService.GetColor(color);
            
            if (_configService.Config.Settings.TextSpeed > 0)
            {
                bool skipAnimation = false;
                
                for (int lineIndex = 0; lineIndex < lines.Count && !skipAnimation; lineIndex++)
                {
                    string processedText = ProcessTextVariables(lines[lineIndex]);
                    int charIndex = 0;
                    
                    while (charIndex < processedText.Length && !skipAnimation)
                    {
                        if (Console.KeyAvailable)
                        {
                            var keyInfo = SafeReadKey(true);
                            if (keyInfo.Key == ConsoleKey.Enter)
                            {
                                skipAnimation = true;
                                Console.Write(processedText.Substring(charIndex));
                                Console.WriteLine();
                                
                                for (int remainingIndex = lineIndex + 1; remainingIndex < lines.Count; remainingIndex++)
                                {
                                    string remainingProcessedText = ProcessTextVariables(lines[remainingIndex]);
                                    Console.WriteLine(remainingProcessedText);
                                }
                                break;
                            }
                        }
                        else
                        {
                            Console.Write(processedText[charIndex]);
                            charIndex++;
                            Thread.Sleep(_configService.Config.Settings.TextSpeed);
                        }
                    }
                    
                    if (!skipAnimation)
                    {
                        if (charIndex < processedText.Length)
                        {
                            Console.Write(processedText.Substring(charIndex));
                        }
                        Console.WriteLine();
                    }
                }
            }
            else
            {
                foreach (string line in lines)
                {
                    string processedText = ProcessTextVariables(line);
                    Console.WriteLine(processedText);
                }
            }
            
            Console.ResetColor();
        }

        public void ClearScreen()
        {
            if (_configService.Config.Settings.ClearScreen)
            {
                try
                {
                    Console.Clear();
                }
                catch (IOException)
                {
                    // Ignore clear screen errors in non-interactive environments
                    Console.WriteLine("\n");
                }
            }
        }

        public void ShowInitialScreen()
        {
            ClearScreen();
            
            Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.Title);
            
        string[] asciiArt = {
            "╔════════════════════════════════════════════════════════════════════════════════╗",
            "║                                                                                ║",
            "║                      $$$$$$\\   $$$$$$\\   $$$$$$\\    $$\\                        ║",
            "║                      $$  __$$\\ $$$ __$$\\ $$  __$$\\ $$$$ |                      ║",
            "║                      \\__/  $$ |$$$$\\ $$ |$$ /  $$ |\\_$$ |                      ║",
            "║                      $$$$$$  |$$\\$$\\$$ |\\$$$$$$$ |  $$ |                       ║",
            "║                      $$  ____/ $$ \\$$$$ | \\____$$ |  $$ |                      ║",
            "║                      $$ |      $$ |\\$$$ |$$\\   $$ |  $$ |                      ║",
            "║                      $$$$$$$$\\ \\$$$$$$  /\\$$$$$$  |$$$$$$\\                     ║",
            "║                      \\________| \\______/  \\______/ \\______|                    ║",
            "║                                                                                ║",
            "║                      ░░░░░░░░░░ Another Story ░░░░░░░░░░                       ║",
            "║                                                                                ║",
            "╚════════════════════════════════════════════════════════════════════════════════╝",
        };

            foreach (string line in asciiArt)
            {
                Console.WriteLine(line);
                Thread.Sleep(100);
            }
            
            Console.ResetColor();
            Console.WriteLine();
            WriteWithColor("Pressione qualquer tecla para continuar...", _configService.Config.Colors.NormalText);
            
            SafeReadKey();
        }

        public int ShowChapterOptions(List<Option> options)
        {
            Console.WriteLine();
            WriteWithColor("Suas opções:", _configService.Config.Colors.HighlightedText);
            Console.WriteLine();
            
            // Convert options to string array for the existing ShowMenu method
            string[] optionTexts = new string[options.Count];
            for (int i = 0; i < options.Count; i++)
            {
                optionTexts[i] = ProcessTextVariables(options[i].Text);
            }
            
            // Use the existing ShowMenu functionality but with a custom loop
            int currentIndex = 0;
            
            while (true)
            {
                // Clear from current position down
                int startLine = Console.CursorTop;
                
                // Display options
                for (int i = 0; i < optionTexts.Length; i++)
                {
                    bool meetsRequirement = CheckSkillRequirement(options[i]);
                    
                    if (i == currentIndex)
                    {
                        if (meetsRequirement)
                        {
                            Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.HighlightedText);
                            Console.WriteLine($"► {optionTexts[i]}");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"► {optionTexts[i]} [Requisito: {GetSkillRequirementText(options[i])}]");
                        }
                        Console.ResetColor();
                    }
                    else
                    {
                        if (meetsRequirement)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"  {optionTexts[i]}");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.WriteLine($"  {optionTexts[i]} [Requisito: {GetSkillRequirementText(options[i])}]");
                        }
                        Console.ResetColor();
                    }
                }
                
                Console.WriteLine();
                if (_configService.Config.Settings.ShowHints)
                {
                    WriteWithColor("Use ↑↓ para navegar, Enter para selecionar, 0 para voltar ao menu", _configService.Config.Colors.NormalText);
                    WriteWithColor("Verde = Disponível | Cinza = Não atende requisitos", _configService.Config.Colors.NormalText);
                }
                
                var keyInfo = SafeReadKey(true);
                
                // Clear the displayed options for next iteration
                Console.SetCursorPosition(0, startLine);
                for (int i = 0; i < optionTexts.Length + 3; i++)
                {
                    Console.WriteLine(new string(' ', Math.Min(Console.WindowWidth - 1, 120)));
                }
                Console.SetCursorPosition(0, startLine);
                
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        currentIndex = currentIndex > 0 ? currentIndex - 1 : optionTexts.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        currentIndex = currentIndex < optionTexts.Length - 1 ? currentIndex + 1 : 0;
                        break;
                    case ConsoleKey.Enter:
                        // Only allow selection if requirement is met
                        if (CheckSkillRequirement(options[currentIndex]))
                        {
                            // Clear the options display before returning
                            Console.SetCursorPosition(0, startLine);
                            for (int i = 0; i < optionTexts.Length + 3; i++)
                            {
                                Console.WriteLine();
                            }
                            Console.SetCursorPosition(0, startLine);
                            return currentIndex;
                        }
                        break;
                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.Escape:
                        // Clear the options display before returning
                        Console.SetCursorPosition(0, startLine);
                        for (int i = 0; i < optionTexts.Length + 3; i++)
                        {
                            Console.WriteLine();
                        }
                        Console.SetCursorPosition(0, startLine);
                        return -1;
                }
            }
        }

        private string ProcessTextVariables(string text)
        {
            var character = _playerSaveService.PlayerSave.Character;
            
            // Replace character variables
            text = text.Replace("{protagonist}", character.Name);
            text = text.Replace("{name}", character.Name);
            text = text.Replace("{player}", character.Name);
            
            // Replace attribute variables
            text = text.Replace("{health}", character.Attributes.Saude.ToString());
            text = text.Replace("{psychology}", character.Attributes.Psicologia.ToString());
            text = text.Replace("{strength}", character.Attributes.Forca.ToString());
            text = text.Replace("{intelligence}", character.Attributes.Inteligencia.ToString());
            text = text.Replace("{conversation}", character.Attributes.Conversacao.ToString());
            
            // Replace status variables
            text = text.Replace("{playtime}", character.PlaytimeMinutes.ToString());
            text = text.Replace("{created}", character.CreatedAt.ToString("dd/MM/yyyy"));
            text = text.Replace("{lastplayed}", character.LastPlayed.ToString("dd/MM/yyyy HH:mm"));
            
            // Replace date/time variables
            text = text.Replace("{date}", DateTime.Now.ToString("dd/MM/yyyy"));
            text = text.Replace("{time}", DateTime.Now.ToString("HH:mm"));
            text = text.Replace("{year}", DateTime.Now.Year.ToString());
            
            return text;
        }

        private bool CheckSkillRequirement(Option option)
        {
            if (option.SkillRequirement == null)
                return true;

            var character = _playerSaveService.PlayerSave.Character;
            
            return option.SkillRequirement.Skill.ToLower() switch
            {
                "health" or "saude" => character.Attributes.Saude >= option.SkillRequirement.MinValue,
                "psychology" or "psicologia" => character.Attributes.Psicologia >= option.SkillRequirement.MinValue,
                "strength" or "forca" => character.Attributes.Forca >= option.SkillRequirement.MinValue,
                "intelligence" or "inteligencia" => character.Attributes.Inteligencia >= option.SkillRequirement.MinValue,
                "conversation" or "conversacao" => character.Attributes.Conversacao >= option.SkillRequirement.MinValue,
                _ => true
            };
        }

        private string GetSkillRequirementText(Option option)
        {
            if (option.SkillRequirement == null)
                return "";

            var character = _playerSaveService.PlayerSave.Character;
            int currentValue = option.SkillRequirement.Skill.ToLower() switch
            {
                "health" or "saude" => character.Attributes.Saude,
                "psychology" or "psicologia" => character.Attributes.Psicologia,
                "strength" or "forca" => character.Attributes.Forca,
                "intelligence" or "inteligencia" => character.Attributes.Inteligencia,
                "conversation" or "conversacao" => character.Attributes.Conversacao,
                _ => 0
            };

            string skillName = option.SkillRequirement.Skill.ToLower() switch
            {
                "health" or "saude" => "Saúde",
                "psychology" or "psicologia" => "Psicologia",
                "strength" or "forca" => "Força",
                "intelligence" or "inteligencia" => "Inteligência",
                "conversation" or "conversacao" => "Conversação",
                _ => option.SkillRequirement.Skill
            };

            return $"{skillName} {option.SkillRequirement.MinValue} (Atual: {currentValue})";
        }

        private ConsoleKeyInfo SafeReadKey(bool intercept = false)
        {
            try
            {
                // Check if running in interactive mode
                if (Console.IsInputRedirected || !Environment.UserInteractive)
                {
                    // Non-interactive mode - wait longer and provide escape hatch
                    Thread.Sleep(1000);
                    return new ConsoleKeyInfo((char)27, ConsoleKey.Escape, false, false, false); // ESC key to exit loops
                }
                return Console.ReadKey(intercept);
            }
            catch (InvalidOperationException)
            {
                // Console input redirected - add delay to prevent infinite loops and exit gracefully
                Thread.Sleep(1000);
                return new ConsoleKeyInfo((char)27, ConsoleKey.Escape, false, false, false); // ESC key to exit loops
            }
        }

        private void SafeReadKey()
        {
            try
            {
                // Check if running in interactive mode
                if (Console.IsInputRedirected || !Environment.UserInteractive)
                {
                    // Non-interactive mode - just wait briefly
                    Thread.Sleep(1000);
                    return;
                }
                Console.ReadKey();
            }
            catch (InvalidOperationException)
            {
                // Console input redirected - just wait briefly
                Thread.Sleep(1000);
            }
        }
    }
}