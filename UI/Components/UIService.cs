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
            int rightColumnWidth = 26;
            int leftColumnWidth = Console.WindowWidth - rightColumnWidth - 2;
            
            // Calculate available space for options, ensuring minimum space
            int minRequiredLines = options.Count + 5; // Options + hints + spacing
            int currentRow = Console.CursorTop;
            int availableSpace = Console.WindowHeight - currentRow - 2;
            
            // If not enough space, start options from a safe position
            if (availableSpace < minRequiredLines)
            {
                currentRow = Math.Max(0, Console.WindowHeight - minRequiredLines - 2);
                Console.SetCursorPosition(0, currentRow);
            }
            
            // Add spacing before options
            Console.WriteLine();
            currentRow++;
            
            Console.SetCursorPosition(0, currentRow);
            Console.WriteLine(); // Linha extra para separar do texto
            currentRow++;
            
            Console.SetCursorPosition(0, currentRow);
            Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.HighlightedText);
            Console.Write("Suas opções:");
            Console.ResetColor();
            currentRow += 2;
            
            // Convert options to string array
            string[] optionTexts = new string[options.Count];
            for (int i = 0; i < options.Count; i++)
            {
                string optionText = ProcessTextVariables(options[i].Text);
                // Truncate if necessary to fit in the left column
                if (optionText.Length > leftColumnWidth - 4)
                {
                    optionText = optionText.Substring(0, leftColumnWidth - 7) + "...";
                }
                optionTexts[i] = optionText;
            }
            
            int currentIndex = 0;
            int startOptionsLine = currentRow;
            
            while (true)
            {
                // Clear options area with enhanced safety check
                int maxClearLine = Math.Min(startOptionsLine + optionTexts.Length + 5, Console.WindowHeight - 1);
                for (int clearLine = startOptionsLine; clearLine < maxClearLine; clearLine++)
                {
                    if (clearLine >= 0 && clearLine < Console.WindowHeight && clearLine <= Console.BufferHeight - 1)
                    {
                        try
                        {
                            Console.SetCursorPosition(0, clearLine);
                            Console.Write(new string(' ', Math.Min(leftColumnWidth, Console.WindowWidth)));
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            // Skip this line if cursor position is invalid
                            continue;
                        }
                    }
                }
                
                // Display options in simple format with safety checks
                int optionLine = startOptionsLine;
                for (int i = 0; i < optionTexts.Length && optionLine < Console.WindowHeight - 2; i++)
                {
                    if (optionLine >= 0 && optionLine < Console.WindowHeight)
                    {
                        try
                        {
                            Console.SetCursorPosition(0, optionLine);
                            bool meetsRequirement = CheckSkillRequirement(options[i]);
                            
                            if (i == currentIndex)
                            {
                                if (meetsRequirement)
                                {
                                    Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.HighlightedText);
                                    Console.Write($"► {optionTexts[i]}");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.Write($"► {optionTexts[i]} [Requisito não atendido]");
                                }
                                Console.ResetColor();
                            }
                            else
                            {
                                if (meetsRequirement)
                                {
                                    Console.ForegroundColor = ConsoleColor.Green;
                                    Console.Write($"  {optionTexts[i]}");
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.Write($"  {optionTexts[i]} [Requisito não atendido]");
                                }
                                Console.ResetColor();
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            // Skip this option if cursor position is invalid
                        }
                    }
                    optionLine++;
                }
                
                if (_configService.Config.Settings.ShowHints && optionLine < Console.WindowHeight - 3)
                {
                    optionLine++;
                    Console.SetCursorPosition(0, optionLine);
                    Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.NormalText);
                    Console.WriteLine("Use ↑↓ para navegar, Enter para selecionar, 0 para voltar ao menu");
                    optionLine++;
                    Console.SetCursorPosition(0, optionLine);
                    Console.WriteLine("Verde = Disponível | Cinza = Não atende requisitos");
                    Console.ResetColor();
                }
                
                var keyInfo = SafeReadKey(true);
                
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        currentIndex = currentIndex > 0 ? currentIndex - 1 : optionTexts.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        currentIndex = currentIndex < optionTexts.Length - 1 ? currentIndex + 1 : 0;
                        break;
                    case ConsoleKey.Enter:
                        if (CheckSkillRequirement(options[currentIndex]))
                        {
                            // Clear options area before returning with enhanced safety
                            int maxClear = Math.Min(startOptionsLine + optionTexts.Length + 5, Console.WindowHeight - 1);
                            for (int clearLine = startOptionsLine; clearLine < maxClear; clearLine++)
                            {
                                if (clearLine >= 0 && clearLine < Console.WindowHeight && clearLine <= Console.BufferHeight - 1)
                                {
                                    try
                                    {
                                        Console.SetCursorPosition(0, clearLine);
                                        Console.Write(new string(' ', Math.Min(leftColumnWidth, Console.WindowWidth)));
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        // Skip clearing this line if cursor position is invalid
                                        continue;
                                    }
                                }
                            }
                            return currentIndex;
                        }
                        break;
                    case ConsoleKey.D0:
                    case ConsoleKey.NumPad0:
                    case ConsoleKey.Escape:
                        // Clear options area before returning with enhanced safety
                        int maxClearEsc = Math.Min(startOptionsLine + optionTexts.Length + 5, Console.WindowHeight - 1);
                        for (int clearLine = startOptionsLine; clearLine < maxClearEsc; clearLine++)
                        {
                            if (clearLine >= 0 && clearLine < Console.WindowHeight && clearLine <= Console.BufferHeight - 1)
                            {
                                try
                                {
                                    Console.SetCursorPosition(0, clearLine);
                                    Console.Write(new string(' ', Math.Min(leftColumnWidth, Console.WindowWidth)));
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    // Skip clearing this line if cursor position is invalid
                                    continue;
                                }
                            }
                        }
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

        public void ShowStatusPanel()
        {
            ShowInlineStatusPanel(0);
        }
        
        private void ShowInlineStatusPanel(int startLine)
        {
            var character = _playerSaveService.PlayerSave.Character;
            
            // Save current cursor position
            int currentLeft = Console.CursorLeft;
            int currentTop = Console.CursorTop;
            
            // Calculate position for elegant HUD
            int hudStartPos = Math.Max(0, Console.WindowWidth - 35);
            
            // Create an elegant HUD with cyberpunk styling
            Console.SetCursorPosition(hudStartPos, startLine);
            
            // Agent name with cyberpunk brackets
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[ ");
            Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.HighlightedText);
            string nameText = character.Name.Length > 10 ? character.Name.Substring(0, 10) : character.Name;
            Console.Write($"{nameText}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(" ] ");
            
            // Health with heart symbol and bar
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("♥ ");
            Console.ForegroundColor = GetHealthColor(character.Attributes.Saude);
            DrawMiniBar(character.Attributes.Saude, 6);
            Console.ForegroundColor = GetHealthColor(character.Attributes.Saude);
            Console.Write($" {character.Attributes.Saude}");
            
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(" | ");
            
            // Sanity with brain/mind symbol and bar
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("◈ ");
            Console.ForegroundColor = GetHealthColor(character.Attributes.Psicologia);
            DrawMiniBar(character.Attributes.Psicologia, 6);
            Console.ForegroundColor = GetHealthColor(character.Attributes.Psicologia);
            Console.Write($" {character.Attributes.Psicologia}");
            
            Console.ResetColor();
            
            // Restore cursor position only if we moved it
            if (currentLeft != 0 || currentTop != 0)
            {
                Console.SetCursorPosition(currentLeft, currentTop);
            }
        }
        
        private void DrawMiniBar(int value, int maxWidth)
        {
            int filled = (int)((double)value / 100 * maxWidth);
            ConsoleColor barColor = GetHealthColor(value);
            
            Console.ForegroundColor = barColor;
            Console.Write(new string('█', filled));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', maxWidth - filled));
        }
        
        private ConsoleColor GetHealthColor(int value)
        {
            return value switch
            {
                >= 90 => ConsoleColor.Cyan,        // Excelente - Ciano brilhante
                >= 75 => ConsoleColor.Green,       // Muito bom - Verde
                >= 60 => ConsoleColor.Yellow,      // Bom - Amarelo  
                >= 40 => ConsoleColor.DarkYellow,  // Preocupante - Amarelo escuro
                >= 20 => ConsoleColor.Red,         // Crítico - Vermelho
                _ => ConsoleColor.DarkRed          // Extremamente crítico - Vermelho escuro
            };
        }

        public void ShowDialogUI(Chapter chapter)
        {
            ClearScreen();
            ShowEnhancedDialogUI(chapter);
        }
        
        private void ShowEnhancedDialogUI(Chapter chapter)
        {
            // Simplified dialog with clean layout
            int rightColumnWidth = 26; // Compact width
            int leftColumnWidth = Console.WindowWidth - rightColumnWidth - 2;
            
            // Draw simplified HUD
            ShowSimplifiedRightColumnHUD(rightColumnWidth);
            
            // Show main content without frames
            ShowLeftColumnContent(chapter, 0, leftColumnWidth);
        }
        
        private void ShowLeftColumnContent(Chapter chapter, int columnStart, int columnWidth)
        {
            int currentLine = 0;
            
            // Simple chapter title
            if (!string.IsNullOrEmpty(chapter.Title))
            {
                Console.SetCursorPosition(columnStart, currentLine);
                Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.Title);
                Console.WriteLine($"═══ {chapter.Title.ToUpper()} ═══");
                Console.ResetColor();
                currentLine += 2;
            }
            
            // Simple hint
            if (_configService.Config.Settings.TextSpeed > 0 && _configService.Config.Settings.ShowHints)
            {
                Console.SetCursorPosition(columnStart, currentLine);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("(Pressione Enter para pular animação)");
                Console.ResetColor();
                currentLine += 2;
            }
            
            // Clean text content without frames
            WriteColumnTextFixed(chapter.Text, columnStart, currentLine, columnWidth);
        }
        
        private void ShowSimplifiedRightColumnHUD(int columnWidth)
        {
            var character = _playerSaveService.PlayerSave.Character;
            int columnStart = Console.WindowWidth - columnWidth;
            int currentLine = 0;
            
            // Simplified HUD frame
            Console.SetCursorPosition(columnStart, currentLine);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"╔{new string('═', columnWidth - 2)}╗");
            
            // Agent name
            Console.SetCursorPosition(columnStart, ++currentLine);
            Console.Write("║ ");
            Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.HighlightedText);
            string nameText = character.Name.Length > columnWidth - 12 ? character.Name.Substring(0, columnWidth - 15) + "..." : character.Name;
            Console.Write($"AGENTE: {nameText}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(new string(' ', Math.Max(0, columnWidth - 11 - nameText.Length)));
            Console.WriteLine("║");
            
            // Separator
            Console.SetCursorPosition(columnStart, ++currentLine);
            Console.WriteLine($"╠{new string('─', columnWidth - 2)}╣");
            
            // Health with number
            Console.SetCursorPosition(columnStart, ++currentLine);
            Console.Write("║ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("♥ SAÚDE ");
            Console.Write("[");
            Console.ForegroundColor = GetHealthColor(character.Attributes.Saude);
            int healthFilled = (int)((double)character.Attributes.Saude / 100 * 8);
            Console.Write(new string('█', healthFilled));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', 8 - healthFilled));
            Console.ResetColor();
            Console.Write("] ");
            Console.ForegroundColor = GetHealthColor(character.Attributes.Saude);
            Console.Write($"{character.Attributes.Saude}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ║  ");
            
            // Psychology with number
            Console.SetCursorPosition(columnStart, ++currentLine);
            Console.Write("║ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("◈ MENTE ");
            Console.Write("[");
            Console.ForegroundColor = GetHealthColor(character.Attributes.Psicologia);
            int psyFilled = (int)((double)character.Attributes.Psicologia / 100 * 8);
            Console.Write(new string('█', psyFilled));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', 8 - psyFilled));
            Console.ResetColor();
            Console.Write("] ");
            Console.ForegroundColor = GetHealthColor(character.Attributes.Psicologia);
            Console.Write($"{character.Attributes.Psicologia}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  ║  ");
            
            // Bottom frame
            Console.SetCursorPosition(columnStart, ++currentLine);
            Console.WriteLine($"╚{new string('═', columnWidth - 2)}╝");
            Console.ResetColor();
        }
        
        private void WriteColumnTextFixed(List<string> lines, int startX, int startY, int maxWidth)
        {
            int currentY = startY;
            bool skipAllAnimation = false;
            
            // Preparar todas as linhas processadas antecipadamente
            List<string> allWrappedLines = new List<string>();
            List<int> linePositions = new List<int>();
            
            foreach (string line in lines)
            {
                string processedLine = ProcessTextVariables(line);
                
                if (string.IsNullOrEmpty(processedLine))
                {
                    allWrappedLines.Add("");
                    linePositions.Add(currentY);
                    currentY++;
                    continue;
                }
                
                // Quebrar texto para caber na coluna
                var wrappedLines = WrapTextForColumn(processedLine, maxWidth);
                
                foreach (string wrappedLine in wrappedLines)
                {
                    allWrappedLines.Add(wrappedLine);
                    linePositions.Add(currentY);
                    currentY++;
                }
            }
            
            // Se não há animação, exibir tudo de uma vez
            if (_configService.Config.Settings.TextSpeed <= 0)
            {
                for (int i = 0; i < allWrappedLines.Count; i++)
                {
                    Console.SetCursorPosition(startX, linePositions[i]);
                    Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.NormalText);
                    Console.Write(allWrappedLines[i]);
                    Console.ResetColor();
                }
                return;
            }
            
            // Animação com possibilidade de pular tudo
            Console.ForegroundColor = _configService.GetColor(_configService.Config.Colors.NormalText);
            
            for (int lineIndex = 0; lineIndex < allWrappedLines.Count && !skipAllAnimation; lineIndex++)
            {
                string wrappedLine = allWrappedLines[lineIndex];
                
                if (string.IsNullOrEmpty(wrappedLine))
                {
                    continue;
                }
                
                Console.SetCursorPosition(startX, linePositions[lineIndex]);
                
                for (int charIndex = 0; charIndex < wrappedLine.Length && !skipAllAnimation; charIndex++)
                {
                    // Verificar se o usuário pressionou Enter
                    if (Console.KeyAvailable)
                    {
                        var keyInfo = SafeReadKey(true);
                        if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            skipAllAnimation = true;
                            break;
                        }
                    }
                    
                    Console.Write(wrappedLine[charIndex]);
                    Thread.Sleep(_configService.Config.Settings.TextSpeed);
                }
                
                // Se a animação não foi pulada, completar a linha atual
                if (!skipAllAnimation && Console.CursorLeft < startX + wrappedLine.Length)
                {
                    Console.SetCursorPosition(startX, linePositions[lineIndex]);
                    Console.Write(wrappedLine);
                }
            }
            
            // Se a animação foi pulada, exibir todo o resto instantaneamente
            if (skipAllAnimation)
            {
                for (int i = 0; i < allWrappedLines.Count; i++)
                {
                    if (!string.IsNullOrEmpty(allWrappedLines[i]))
                    {
                        Console.SetCursorPosition(startX, linePositions[i]);
                        Console.Write(allWrappedLines[i]);
                    }
                }
            }
            
            Console.ResetColor();
        }
        
        private List<string> WrapTextForColumn(string text, int maxWidth)
        {
            List<string> lines = new List<string>();
            
            if (string.IsNullOrEmpty(text))
            {
                lines.Add("");
                return lines;
            }
            
            string[] words = text.Split(' ');
            string currentLine = "";
            
            foreach (string word in words)
            {
                if (currentLine.Length + word.Length + 1 <= maxWidth)
                {
                    if (!string.IsNullOrEmpty(currentLine))
                        currentLine += " ";
                    currentLine += word;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }
                    else
                    {
                        lines.Add(word);
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }
            
            return lines;
        }
    }
}