using GameBattleSystem.Interfaces;

namespace GameBattleSystem.Models;

public enum PlayerAction { Attack, Defend, Special }

public enum BattleState { InProgress, PlayerWon, EnemyWon, Quit }

/// <summary>Controls the turns between the player's character and the computer enemy.</summary>
public class Battle
{
    private readonly IBattleLogger _logger;

    public Character Player { get; }
    public Character Enemy { get; }
    public BattleState State { get; private set; } = BattleState.InProgress;
    public int Round { get; private set; } = 1;

    public Battle(Character player, Character enemy, IBattleLogger logger)
    {
        ArgumentNullException.ThrowIfNull(player);
        ArgumentNullException.ThrowIfNull(enemy);
        ArgumentNullException.ThrowIfNull(logger);

        Player = player;
        Enemy = enemy;
        _logger = logger;
        _logger.Log($"BATTLE START: {player.Name} ({player.Type}) vs {enemy.Name} ({enemy.Type})");
    }

    /// <summary>
    /// Runs one round: the player's action, then (if the enemy survives) the enemy's normal attack.
    /// Returns the messages describing what happened.
    /// </summary>
    public List<string> TakeTurn(PlayerAction action)
    {
        if (State != BattleState.InProgress)
            throw new InvalidOperationException("The battle is already over.");

        var messages = new List<string>();

        switch (action)
        {
            case PlayerAction.Attack:
                int dealt = Player.NormalAttack(Enemy);
                messages.Add($"{Player.Name} attacks {Enemy.Name} for {dealt} damage.");
                break;

            case PlayerAction.Defend:
                Player.Defend();
                messages.Add($"{Player.Name} braces for the next attack.");
                break;

            case PlayerAction.Special:
                if (Player.SpecialUsesLeft <= 0)
                {
                    // Not a valid move, so the turn is not consumed.
                    messages.Add($"{Player.Name} has no {Player.SpecialAbilityName} uses left. Choose another action.");
                    return messages;
                }
                messages.Add(Player.UseSpecialAbility(Enemy));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(action), "Unknown action.");
        }

        if (Enemy.IsDefeated)
        {
            Finish(BattleState.PlayerWon, messages);
            return messages;
        }

        int enemyDamage = Enemy.NormalAttack(Player);
        messages.Add($"{Enemy.Name} attacks {Player.Name} for {enemyDamage} damage.");

        if (Player.IsDefeated)
        {
            Finish(BattleState.EnemyWon, messages);
            return messages;
        }

        Round++;
        foreach (string m in messages) _logger.Log(m);
        return messages;
    }

    public void Quit()
    {
        if (State != BattleState.InProgress) return;
        State = BattleState.Quit;
        _logger.Log($"BATTLE END: {Player.Name} quit the battle.");
    }

    private void Finish(BattleState result, List<string> messages)
    {
        State = result;
        string winner = result == BattleState.PlayerWon ? Player.Name : Enemy.Name;
        messages.Add($"{winner} wins the battle!");

        foreach (string m in messages) _logger.Log(m);
        _logger.Log($"BATTLE END: {winner} won. {Player.Name} HP {Player.Health}/{Player.MaxHealth}, " +
                    $"{Enemy.Name} HP {Enemy.Health}/{Enemy.MaxHealth}");
    }
}
