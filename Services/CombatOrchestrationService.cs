using RetroGame2091.Core.Interfaces;
using RetroGame2091.Core.Models;

namespace RetroGame2091.Services
{
    public class CombatOrchestrationService : ICombatOrchestrationService
    {
        private readonly ICombatService _combatService;
        private readonly IEnemyService _enemyService;
        private readonly ICombatUIService _combatUIService;
        private readonly IUIService _uiService;
        private readonly IPlayerSaveService _playerSaveService;
        private readonly IGameConfigService _configService;

        public CombatOrchestrationService(
            ICombatService combatService,
            IEnemyService enemyService,
            ICombatUIService combatUIService,
            IUIService uiService,
            IPlayerSaveService playerSaveService,
            IGameConfigService configService)
        {
            _combatService = combatService;
            _enemyService = enemyService;
            _combatUIService = combatUIService;
            _uiService = uiService;
            _playerSaveService = playerSaveService;
            _configService = configService;
        }

        public string? StartCombat(string enemyId, string? victoryChapter = null, string? defeatChapter = null, string? fleeChapter = null, string? victoryNode = null, string? defeatNode = null, string? fleeNode = null)
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
                FleeChapter = fleeChapter,
                VictoryNode = victoryNode,
                DefeatNode = defeatNode,
                FleeNode = fleeNode
            };

            return RunCombatLoop(combatState);
        }

        private string? RunCombatLoop(CombatState combatState)
        {
            var player = _playerSaveService.PlayerSave.Character;
            
            _combatUIService.ShowCombatEncounter(combatState.Enemy);
            
            while (combatState.IsActive && combatState.Enemy.IsAlive && !_combatService.IsPlayerDefeated(player))
            {
                CombatResult? result = null;
                
                if (combatState.CurrentTurn == TurnOwner.Player)
                {
                    var action = _combatUIService.GetPlayerCombatActionWithHUD(combatState, player);
                    
                    if (action == null)
                    {
                        break;
                    }
                    
                    result = _combatService.ExecuteAction(combatState, action.Value, player);
                    
                    _combatUIService.ShowCombatResult(result);
                    
                    if (result.Outcome != CombatOutcome.Continue)
                    {
                        combatState.IsActive = false;
                        
                        _uiService.ShowContinuePrompt();
                        _uiService.SafeReadKeyNoCombatSave();
                        
                        // Não salvar automaticamente - deixar para o jogador decidir
                        
                        // Return node if available, otherwise chapter
                        return GetDestinationFromResult(result, combatState);
                    }
                    
                    _uiService.ShowContinuePrompt();
                    _uiService.SafeReadKeyNoCombatSave();
                }
                else if (combatState.CurrentTurn == TurnOwner.Enemy)
                {
                    _combatUIService.ShowEnemyTurnIndicator(combatState.Enemy);
                    
                    result = _combatService.ExecuteEnemyTurn(combatState, player);
                    
                    _combatUIService.ShowCombatResult(result);
                    
                    if (result.Outcome != CombatOutcome.Continue)
                    {
                        combatState.IsActive = false;
                        
                        _uiService.ShowContinuePrompt();
                        _uiService.SafeReadKeyNoCombatSave();
                        
                        // Não salvar automaticamente - deixar para o jogador decidir
                        
                        // Return node if available, otherwise chapter
                        return GetDestinationFromResult(result, combatState) ?? combatState.DefeatNode ?? combatState.DefeatChapter;
                    }
                    
                    _uiService.ShowContinuePrompt();
                    _uiService.SafeReadKeyNoCombatSave();
                }
            }

            return null;
        }

        private string? GetDestinationFromResult(CombatResult result, CombatState combatState)
        {
            return result.Outcome switch
            {
                CombatOutcome.Victory => combatState.VictoryNode ?? combatState.VictoryChapter,
                CombatOutcome.Defeat => combatState.DefeatNode ?? combatState.DefeatChapter,
                CombatOutcome.Fled => combatState.FleeNode ?? combatState.FleeChapter,
                _ => result.NextChapter
            };
        }
    }
}