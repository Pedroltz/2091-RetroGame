# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

2091 is a text-based cyberpunk adventure game built with .NET 8.0, featuring JSON-based narrative structure, turn-based combat, and character progression. The game follows a corporate worker in Night City navigating a dystopian future.

## Common Commands

### Building and Running
```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the game
dotnet run
```

### Development Workflow
- The project uses .NET 8.0 with C# and Newtonsoft.Json for serialization
- No test framework is currently configured
- The game runs in a console/terminal environment with UTF-8 encoding

## Architecture

### Dependency Injection Pattern
The project uses a custom DI container (`ServiceContainer` in `Utils/ServiceContainer.cs`). All services are manually registered and resolved. When adding new services:
1. Create the interface in `Core/Interfaces/`
2. Implement the service in `Services/`
3. Register both in `ServiceContainer.RegisterServices()`
4. Inject via constructor in consuming classes

### Story System Architecture

**Chapter Structure (Two Formats)**:
- **Legacy format**: Single chapter with `Text`, `Options`, `GameEnd` at root level
- **New format**: Multi-node chapters with `StartNode` and `Nodes` dictionary
- Both formats are supported via `Chapter.GetCurrentNode()` method
- Chapters live in `Chapters/Prologo/` directory as JSON files

**Node Navigation**:
- `NextNode`: Navigate within the same chapter
- `NextChapter`: Load a different chapter file
- Combat options use `StartCombat`, `VictoryNode/Chapter`, `DefeatNode/Chapter`, `FleeNode/Chapter`

**Variable Substitution**: Story text supports placeholders like `{name}`, `{health}`, `{psychology}`, `{strength}`, `{intelligence}`, `{conversation}`, `{playtime}`, `{created}`, `{lastplayed}`, `{date}`, `{time}`

### Combat System Architecture

Combat is orchestrated through three layers:
1. **CombatOrchestrationService**: Main entry point, handles combat loop and outcome routing
2. **CombatService**: Core combat logic (actions, turn management, damage calculation)
3. **CombatUIService**: Display layer for combat screens and animations

**Combat Flow**:
- Triggered via `Option.StartCombat` with enemy ID
- Uses turn-based system with `CombatState` tracking current state
- Enemy data loaded from `Enemies/*.json` files
- Results route to Victory/Defeat/Flee nodes or chapters
- Player stats from `Attributes` model affect combat outcomes

### Service Layer Organization

**Core Services**:
- `ChapterService`: Loads/saves story content, manages progression
- `PlayerSaveService`: Handles save/load, session tracking, player data persistence
- `GameConfigService`: Manages `Config/config.json` (colors, text speed, settings)
- `MusicService`: Optional FFmpeg-based audio system with graceful degradation
- `CombatOrchestrationService`: Combat system entry point

**UI Services**:
- `UIService`: Core UI operations (menus, dialogs, colored text output)
- `CombatUIService`: Combat-specific UI (HUD, action selection, combat results)

**Menu Services**:
- `CharacterCreationMenu`: 50-point attribute distribution system
- `SettingsMenu`: In-game settings management

### Game Flow (GameController)

Main loop: Title screen → Main menu → Play menu → Game loop
- New game: Character creation → Story from "init_inicio"
- Load game: Resume from `PlayerSave.CurrentChapter` and `CurrentNode`
- Navigation: Chapter/node system with combat interruptions
- Save system: F5 during gameplay, auto-save on progress updates

### Data Models

**PlayerSave** (`Saves/player.json`):
- Contains `Protagonist` with `Attributes` (Saude, Psicologia, Forca, Inteligencia, Conversacao)
- Tracks `CurrentChapter`, `CurrentNode`, `PlaytimeMinutes`, session timing
- Save/load handled by `PlayerSaveService`

**Chapter**:
- Supports both legacy (flat) and new (node-based) structures
- Nodes contain dialog text, options with skill requirements, combat triggers
- Options can specify `SkillRequirement` (minimum attribute value needed)

**Enemy** (`Enemies/*.json`):
- Stats: Health, Defense, Attack, HackingDefense, FleeThreshold
- Dialogs for various combat events (encounter, attack, hacked, flee, defeat, victory)
- `IsObserved` flag for tactical information

## Content Directories

The project copies these directories to output during build:
- `Chapters/` - Story content JSON files
- `Enemies/` - Enemy definitions JSON files
- `Config/` - Game configuration JSON
- `Saves/` - Player save files
- `Sounds/` - Audio files (optional, requires FFmpeg)

## Important Patterns

### Adding New Story Content
1. Create JSON file in `Chapters/Prologo/` (or appropriate subfolder)
2. Use new node-based format with `StartNode` and `Nodes` dictionary
3. Reference nodes via `NextNode` (same chapter) or `NextChapter` (different file)
4. For combat, specify enemy ID and victory/defeat/flee destinations

### Adding New Enemies
1. Create JSON in `Enemies/` directory following `Enemy` model structure
2. Define stats, dialogs for all combat events
3. Reference by filename (without .json) in chapter options `StartCombat` field

### F5 Save System
During gameplay, F5 triggers save with visual confirmation. The system:
- Saves via `PlayerSaveService.SaveGameWithConfirmation()`
- Returns special code (-2) to force UI refresh
- Does NOT auto-save during combat to prevent save-scumming

### Session Tracking
- `StartNewSession()` called when starting new game or loading save
- `UpdateGameProgress()` called on each node/chapter transition
- Playtime tracked in minutes, updated on save

## Language and Localization

Currently Portuguese (Brazilian) is the primary language for:
- UI strings and menus
- Story content
- Combat dialogs
- Error messages

English is used only in code comments and some system messages.

## FFmpeg Audio System

The game attempts to install FFmpeg automatically on startup (Windows/Linux). Audio is optional - the game gracefully continues without it if unavailable. `FFmpegInstaller` handles detection and installation with app restart if needed.
