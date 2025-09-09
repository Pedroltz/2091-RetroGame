using RetroGame2091.Core.Interfaces;
using RetroGame2091.Core.Models;

namespace RetroGame2091.Controllers
{
    public class CombatController
    {
        private readonly ICombatService _combatService;
        private readonly IEnemyService _enemyService;
        private readonly IUIService _uiService;
        private readonly IPlayerSaveService _playerSaveService;
        private readonly IGameConfigService _configService;

        public CombatController(
            ICombatService combatService,
            IEnemyService enemyService,
            IUIService uiService,
            IPlayerSaveService playerSaveService,
            IGameConfigService configService)
        {
            _combatService = combatService;
            _enemyService = enemyService;
            _uiService = uiService;
            _playerSaveService = playerSaveService;
            _configService = configService;
        }

        public string? StartCombat(string enemyId, string? victoryChapter = null, string? defeatChapter = null, string? fleeChapter = null)
        {
            var enemy = _enemyService.LoadEnemy(enemyId);
            if (enemy == null)
            {
                _uiService.WriteWithColor($"Erro: Inimigo '{enemyId}' não encontrado!", _configService.Config.Colors.Error);
                return null;
            }

            var combatState = new CombatState(enemy)
            {
                VictoryChapter = victoryChapter,
                DefeatChapter = defeatChapter,
                FleeChapter = fleeChapter
            };

            return RunCombatLoop(combatState);
        }

        private string? RunCombatLoop(CombatState combatState)
        {
            var player = _playerSaveService.PlayerSave.Character;
            
            // Show initial encounter
            ShowCombatEncounter(combatState.Enemy);
            
            while (combatState.IsActive && combatState.Enemy.IsAlive && !_combatService.IsPlayerDefeated(player))
            {
                CombatResult? result = null;
                
                if (combatState.CurrentTurn == TurnOwner.Player)
                {
                    // Player Turn - Show HUD with combat options integrated
                    var action = GetPlayerCombatActionWithHUD(combatState, player);
                    
                    if (action == null) // Player wants to exit combat/return to menu
                    {
                        break;
                    }
                    
                    // Execute player action
                    result = _combatService.ExecuteAction(combatState, action.Value, player);
                    
                    // Display result
                    ShowCombatResult(result);
                    
                    // Check for combat end
                    if (result.Outcome != CombatOutcome.Continue)
                    {
                        combatState.IsActive = false;
                        
                        // Wait for player to acknowledge result before ending combat
                        _uiService.ShowContinuePrompt();
                        _uiService.SafeReadKey();
                        
                        // Save player progress
                        _playerSaveService.SaveGame();
                        
                        return result.NextChapter;
                    }
                    
                    // Wait for player to continue before enemy turn
                    _uiService.ShowContinuePrompt();
                    _uiService.SafeReadKey();
                }
                else if (combatState.CurrentTurn == TurnOwner.Enemy)
                {
                    // Enemy Turn
                    ShowEnemyTurnIndicator(combatState.Enemy);
                    
                    // Execute enemy action
                    result = _combatService.ExecuteEnemyTurn(combatState, player);
                    
                    // Display result
                    ShowCombatResult(result);
                    
                    // Check for combat end
                    if (result.Outcome != CombatOutcome.Continue)
                    {
                        combatState.IsActive = false;
                        
                        // Wait for player to acknowledge result before ending combat
                        _uiService.ShowContinuePrompt();
                        _uiService.SafeReadKey();
                        
                        // Save player progress
                        _playerSaveService.SaveGame();
                        
                        return result.NextChapter ?? combatState.DefeatChapter;
                    }
                    
                    // Wait for player to continue before next player turn
                    _uiService.ShowContinuePrompt();
                    _uiService.SafeReadKey();
                }
            }

            return null; // Combat was interrupted or no next chapter defined
        }

        private void ShowCombatEncounter(Enemy enemy)
        {
            _uiService.ClearScreen();
            
            // Fixed header with proper centering
            _uiService.WriteWithColor("╔══════════════════════════════════════════════════════════╗", _configService.Config.Colors.Title);
            string paddedTitle = CenterTextInBox($"ENCONTRO DE COMBATE: {enemy.Name.ToUpper()}", 58);
            _uiService.WriteWithColor($"║{paddedTitle}║", _configService.Config.Colors.Title);
            _uiService.WriteWithColor("╚══════════════════════════════════════════════════════════╝", _configService.Config.Colors.Title);
            Console.WriteLine();
            
            // Create combined text list for proper animation handling
            var encounterText = new List<string>();
            
            // Add enemy description
            encounterText.Add(enemy.Description);
            encounterText.Add(""); // Empty line
            
            // Add encounter dialog
            if (enemy.Dialogs.OnEncounter.Count > 0)
            {
                encounterText.AddRange(enemy.Dialogs.OnEncounter);
                encounterText.Add(""); // Empty line
            }
            
            // Use the chapter text display method for consistent behavior
            _uiService.WriteChapterTextWithEffect(encounterText, _configService.Config.Colors.NormalText);
            
            Console.WriteLine();
            _uiService.WriteWithColor("Pressione qualquer tecla para iniciar o combate...", _configService.Config.Colors.HighlightedText);
            _uiService.SafeReadKey();
        }

        private string CenterTextInBox(string text, int boxWidth)
        {
            if (text.Length >= boxWidth) 
                return text.Substring(0, boxWidth - 3) + "...";
            
            int totalPadding = boxWidth - text.Length;
            int leftPadding = totalPadding / 2;
            int rightPadding = totalPadding - leftPadding;
            
            return new string(' ', leftPadding) + text + new string(' ', rightPadding);
        }



        private void ShowEnemyTurnIndicator(Enemy enemy)
        {
            _uiService.ClearScreen();
            
            // Show enemy turn header
            _uiService.WriteWithColor("═══════════════ TURNO DO INIMIGO ═══════════════", _configService.Config.Colors.Title);
            Console.WriteLine();
            
            _uiService.WriteWithColor($"{enemy.Name} está se preparando para atacar...", _configService.Config.Colors.HighlightedText);
            Console.WriteLine();
            Console.WriteLine();
            
            _uiService.WriteWithColor("Pressione qualquer tecla para ver o ataque...", _configService.Config.Colors.NormalText);
            _uiService.SafeReadKey();
        }

        private CombatAction? GetPlayerCombatActionWithHUD(CombatState combatState, Protagonist player)
        {
_uiService.ClearScreen();
            var enemy = combatState.Enemy;
            
            // Compact improved HUD
            DrawCompactCombatHUD(combatState, player, enemy);
            
            // Opções de combate limpas
            string[] combatOptions = {
                "COMBATE    │ Executar protocolo de neutralização física",
                "HACKEAR    │ Deployar malware de disrupção sistêmica",
                "VARRER     │ Executar scan de vulnerabilidades do target",
                "ESCAPAR    │ Ativar protocolo de exfiltração de emergência"
            };

            // Menu cyberpunk alinhado à esquerda
            _uiService.WriteWithColor("▓▒░ DEPLOY TACTICAL RESPONSE PROTOCOL - SELECT MODULE ░▒▓", _configService.Config.Colors.Title);
            Console.WriteLine();

            int choice = _uiService.ShowMenuWithoutClear("", combatOptions);
            
            return choice switch
            {
                0 => CombatAction.Fight,
                1 => CombatAction.Hack,
                2 => CombatAction.Observe,
                3 => CombatAction.Flee,
                -1 => null, // Escape pressed
                _ => null
            };
        }


        private void ShowCombatResult(CombatResult result)
        {
            _uiService.ClearScreen();
            
            // Show action header without emojis
            string actionName = result.Action switch
            {
                CombatAction.Fight => "ATAQUE",
                CombatAction.Hack => "HACK",
                CombatAction.Observe => "OBSERVAÇÃO",
                CombatAction.Flee => "FUGA",
                _ => "AÇÃO"
            };
            
            _uiService.WriteWithColor($"═══════════ {actionName} ═══════════", _configService.Config.Colors.Title);
            Console.WriteLine();
            
            // Create combined text list for proper animation handling
            var resultText = new List<string>();
            
            // Add main result message
            resultText.Add(result.Message);
            
            if (result.DialogLines.Count > 0)
            {
                resultText.Add(""); // Empty line separator
                resultText.AddRange(result.DialogLines);
            }
            
            // Show damage summary
            if (result.EnemyDamage > 0)
            {
                resultText.Add("");
                resultText.Add($"→ Dano causado ao inimigo: {result.EnemyDamage}");
            }
            
            if (result.PlayerDamage > 0)
            {
                resultText.Add("");
                resultText.Add($"→ Dano recebido: {result.PlayerDamage}");
            }
            
            // Show outcome-specific messages without emojis
            switch (result.Outcome)
            {
                case CombatOutcome.Victory:
                    resultText.Add("");
                    resultText.Add("VITÓRIA!");
                    break;
                case CombatOutcome.Defeat:
                    resultText.Add("");
                    resultText.Add("DERROTA!");
                    break;
                case CombatOutcome.Fled:
                    resultText.Add("");
                    resultText.Add("Você escapou do combate!");
                    break;
            }
            
            // Use the chapter text display method for consistent behavior
            _uiService.WriteChapterTextWithEffect(resultText, _configService.Config.Colors.NormalText);
            
            Console.WriteLine();
        }
        
        private string GetHealthColor(int health)
        {
            return health switch
            {
                >= 80 => ConsoleColor.Green.ToString(),
                >= 60 => ConsoleColor.Yellow.ToString(),
                >= 40 => ConsoleColor.DarkYellow.ToString(),
                >= 20 => ConsoleColor.Red.ToString(),
                _ => ConsoleColor.DarkRed.ToString()
            };
        }
        
        private void DrawCompactCombatHUD(CombatState combatState, Protagonist player, Enemy enemy)
        {
            // Header
            _uiService.WriteWithColor("═════════════════ NEURAL COMBAT INTERFACE 2091 ═════════════════", _configService.Config.Colors.Title);
            _uiService.WriteWithColor($"                   TURNO: {combatState.TurnCount + 1:D2} -- SYSTEM ONLINE", _configService.Config.Colors.Title);
            Console.WriteLine();
            
            // Draw dual combat boxes
            DrawCombatBoxes(player, enemy);
            
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("              [█] NEURAL LINK ESTABELECIDO - AGUARDANDO INPUT [█]");
            Console.ResetColor();
            Console.WriteLine();
        }

        private void DrawCombatBoxes(Protagonist player, Enemy enemy)
        {
            // Box headers
            WriteColoredText("┌──────────────── AGENTE ────────────────┐", ConsoleColor.Cyan, false);
            Console.Write("  ");
            WriteColoredText("┌──────────────── TARGET ────────────────┐", ConsoleColor.Red, true);
            
            // Names row
            DrawCombatBoxRow(
                $"[{TruncateName(player.Name.ToUpper()),-25}]", ConsoleColor.White,
                $"[{TruncateName(enemy.Name.ToUpper()),-25}]", ConsoleColor.Red,
                ConsoleColor.Cyan, ConsoleColor.Red);
            
            // Health bars row
            DrawHealthRow(player, enemy);
            
            // Second stat row (Psychology vs Enemy Stats)
            DrawSecondStatRow(player, enemy);
            
            // Player stats row
            DrawPlayerStatsRow(player);
            
            // Empty spacing row
            DrawEmptyRow();
            
            // Box footers
            WriteColoredText("└────────────────────────────────────────┘", ConsoleColor.Cyan, false);
            Console.Write("  ");
            WriteColoredText("└────────────────────────────────────────┘", ConsoleColor.Red, true);
        }

        private void WriteColoredText(string text, ConsoleColor color, bool newLine)
        {
            Console.ForegroundColor = color;
            if (newLine) Console.WriteLine(text);
            else Console.Write(text);
            Console.ResetColor();
        }

        private string TruncateName(string name) => name.Length > 25 ? name.Substring(0, 22) + "..." : name;

        private void DrawCombatBoxRow(string leftContent, ConsoleColor leftContentColor, string rightContent, ConsoleColor rightContentColor, ConsoleColor leftBorderColor, ConsoleColor rightBorderColor)
        {
            WriteColoredText("│ ", leftBorderColor, false);
            WriteColoredText(leftContent, leftContentColor, false);
            Console.Write("            ");
            WriteColoredText("│", leftBorderColor, false);
            Console.Write("VS");
            WriteColoredText("│ ", rightBorderColor, false);
            WriteColoredText(rightContent, rightContentColor, false);
            Console.Write("            ");
            WriteColoredText("│", rightBorderColor, true);
        }

        private void DrawHealthRow(Protagonist player, Enemy enemy)
        {
            WriteColoredText("│ SAUDE: ", ConsoleColor.Cyan, false);
            DrawWideHealthBar(player.Attributes.Saude, 100, 20, GetHealthColor(player.Attributes.Saude));
            WriteColoredText($" {player.Attributes.Saude:D3}", ConsoleColor.Green, false);
            Console.Write("      ");
            WriteColoredText("│", ConsoleColor.Cyan, false);
            Console.Write("  ");
            WriteColoredText("│ VIDA: ", ConsoleColor.Red, false);
            int enemyHealthPercent = enemy.CurrentHealth * 100 / enemy.Stats.Health;
            DrawWideHealthBar(enemy.CurrentHealth, enemy.Stats.Health, 20, GetHealthColor(enemyHealthPercent));
            WriteColoredText($" {enemy.CurrentHealth:D3}", ConsoleColor.Red, false);
            Console.Write("       ");
            WriteColoredText("│", ConsoleColor.Red, true);
        }

        private void DrawSecondStatRow(Protagonist player, Enemy enemy)
        {
            WriteColoredText("│ MENTE: ", ConsoleColor.Cyan, false);
            DrawWideHealthBar(player.Attributes.Psicologia, 100, 20, GetHealthColor(player.Attributes.Psicologia));
            WriteColoredText($" {player.Attributes.Psicologia:D3}", ConsoleColor.Magenta, false);
            Console.Write("      ");
            WriteColoredText("│", ConsoleColor.Cyan, false);
            Console.Write("  ");
            
            if (enemy.IsObserved)
            {
                WriteColoredText("│ STATS: ", ConsoleColor.Red, false);
                WriteColoredText($"ATK:{enemy.Stats.Attack:D2} ", ConsoleColor.Red, false);
                WriteColoredText($"DEF:{enemy.Stats.Defense:D2} ", ConsoleColor.Blue, false);
                WriteColoredText($"FW:{enemy.Stats.HackingDefense:D2} ", ConsoleColor.Magenta, false);
                WriteColoredText($"ESC:{enemy.Stats.FleeThreshold:D2}%", ConsoleColor.Green, false);
                Console.Write(" ");
                WriteColoredText("│", ConsoleColor.Red, true);
            }
            else
            {
                WriteColoredText("│ ", ConsoleColor.Red, false);
                WriteColoredText("[███████ ENCRYPTED DATA ███████]", ConsoleColor.DarkGray, false);
                Console.Write("       ");
                WriteColoredText("│", ConsoleColor.Red, true);
            }
        }

        private void DrawPlayerStatsRow(Protagonist player)
        {
            WriteColoredText("│ STATS: ", ConsoleColor.Cyan, false);
            WriteColoredText($"FORCA:{player.Attributes.Forca:D2} ", ConsoleColor.Red, false);
            WriteColoredText($"INT:{player.Attributes.Inteligencia:D2} ", ConsoleColor.Blue, false);
            WriteColoredText($"CONV:{player.Attributes.Conversacao:D2}", ConsoleColor.Yellow, false);
            Console.Write("         ");
            WriteColoredText("│", ConsoleColor.Cyan, false);
            Console.Write("  ");
            WriteColoredText("│", ConsoleColor.Red, false);
            Console.Write(new string(' ', 40));
            WriteColoredText("│", ConsoleColor.Red, true);
        }

        private void DrawEmptyRow()
        {
            WriteColoredText("│", ConsoleColor.Cyan, false);
            Console.Write(new string(' ', 40));
            WriteColoredText("│", ConsoleColor.Cyan, false);
            Console.Write("  ");
            WriteColoredText("│", ConsoleColor.Red, false);
            Console.Write(new string(' ', 40));
            WriteColoredText("│", ConsoleColor.Red, true);
        }

        private void DrawWideHealthBar(int currentValue, int maxValue, int barWidth, string colorString)
        {
            ConsoleColor color = Enum.Parse<ConsoleColor>(colorString);
            double percentage = (double)currentValue / maxValue;
            int filledBars = (int)(percentage * barWidth);
            
            Console.Write("[");
            Console.ForegroundColor = color;
            
            // Barra mais larga e detalhada
            Console.Write(new string('█', filledBars));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('─', barWidth - filledBars));
            
            Console.ResetColor();
            Console.Write("]");
        }

    }
}