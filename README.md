# Game Character & Battle System (Scenario 1)

A console-based, turn-based battle game written in C# (.NET 10, Visual Studio 2026).
The player manages a roster of characters (Knight, Archer, Mage), can search and sort them,
and can fight a randomly chosen computer enemy.

## How to run
1. Open `GameBattleSystem.slnx` in Visual Studio 2026.
2. Set `GameBattleSystem` as the startup project and press F5 (or Ctrl+F5).
3. Run the tests with Test > Run All Tests (project `GameBattleSystem.Tests`).

Data files are in `GameBattleSystem/Data/` and are copied next to the executable:
`characters.json` (roster) and `battle_log.txt` (battle actions, appended each battle).

## Gameplay
- Attack: damage = attacker's Attack - half of the target's Defence (minimum 1).
- Defend: halves the damage of the enemy's next attack.
- Special (3 uses per battle): Knight - Shield Bash (1.5x + 5), Archer - Power Shot (2x), Mage - Heal (+30 HP).
- After every player action the enemy makes a normal attack. Battles use a copy of the character, so the saved roster is not changed by fighting.

## Design choices
| Area | Choice |
|---|---|
| Inheritance / polymorphism | Abstract `Character`; `Knight`, `Archer`, `Mage` override the special ability |
| Interfaces | `ISpecialAbility`, `IBattleLogger`, `ICharacterRepository` |
| Encapsulation | Stats have private setters; changed only through `TakeDamage`, `Heal`, `Defend` |
| Factory pattern | `CharacterFactory` creates characters and enemies |
| Strategy pattern | `CharacterComparer` subclasses are interchangeable sort strategies |
| Template method | `Character.UseSpecialAbility` enforces the use limit, subclasses supply the effect |
| Algorithms | Linear and binary search by ID, linear search by name, manual bubble sort |
| Persistence | `FileManager` saves/loads polymorphic JSON; bad or missing files fall back to defaults with a warning |
| Logging | `BattleLogger` appends timestamped lines to a TXT file; a logging failure never crashes the game |
# GameBattleSystem
