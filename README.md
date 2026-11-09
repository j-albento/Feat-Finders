# FeatFinders
**A custom achievement tracking mod for Stardew Valley.**

FeatFinders (To be changed to 'Farmer's Field Guide') is a Stardew Valley mod developed in C# using the [Stardew Modding API (SMAPI)](https://smapi.io/). This mod introduces a customizable achievement system that tracks player progression through various in-game activities.

The goal is to provide additional challenges and milestones that encourage players to explore different aspects of Stardew Valley.

## Features

### Achievement Management
- Load achievement definitions from a JSON configuration file.
- Validate achievement data before processing.
- Track achievement progress using current and target values.
- Detect when achievement requirements have been met.
- Support multiple achievement categories and tracking conditions.

### Fishing Achievement Tracking
- Track the number of unique fish species caught.
- Track total fish caught.
- Update fishing achievement progress automatically.
- Support achievements based on unique fish collections and total catches.
- Inspect individual fishing records through SMAPI console commands.

Current fishing achievements:

| Achievement | Requirement |
|---|---|
| Fisherman | Catch 10 different fish |
| Ol' Mariner | Catch 24 different fish |
| Master Angler | Catch all 72 fish |
| Mother Catch | Catch 100 fish |

### Additional Achievement Categories

The achievement definitions include challenges related to:

- **Earnings:** Reach lifetime earning milestones.
- **Collections:** Complete museum, crafting, cooking, and other collections.
- **Friendship:** Build relationships with villagers.
- **Events:** Complete significant in-game milestones.
- **Skills:** Reach specific skill levels.
- **Mining:** Reach mine depth milestones.
- **Fishing:** Catch different fish species and accumulate catches.
- **Shipping:** Meet shipping and crop production goals.
- **Quests:** Complete Help Wanted requests.

Some of these categories are planned for future implementation.

## Technologies

| Technology | Purpose |
|---|---|
| C# | Primary programming language |
| .NET | Development framework |
| SMAPI | Stardew Valley modding API |
| JSON | Achievement data storage |
| Visual Studio / Visual Studio Code | Development environment |
| Git & GitHub | Version control |


*The structure above represents the main components; additional files may be added as development continues.*

### Core Components

**ModEntry.cs**

The entry point of the mod. Responsible for initializing the achievement manager, subscribing to SMAPI events, and responding to in-game activities.

**Achievement.cs**

Defines the achievement model, including:
- Achievement ID
- Name and description
- Achievement type
- Target value
- Current progress
- Completion status

**AchievementManager.cs**

Responsible for loading, validating, and managing achievement definitions. It also handles progress updates and achievement completion.

**Achievements.json**

Stores achievement definitions separately from the C# implementation, making it easier to add and configure achievements without modifying the core achievement model.

## Console Commands

FeatFinders currently includes the following debugging commands:

| Command | Description |
|---|---|
| `check_achievements` | Displays the currently loaded achievements. |
| `check_fishing_stats` | Displays the player's total fishing statistics and individual fish records. |
| `inspect_fish` | Inspects individual fish records, including fish names, IDs, quantities, and largest recorded catches. |

These commands are useful for testing and debugging achievement tracking.

## Development Roadmap

- [x] Establish the achievement data model.
- [x] Implement JSON achievement loading.
- [x] Implement achievement validation.
- [x] Create the achievement manager.
- [x] Implement lifetime earnings tracking.
- [x] Implement friendship tracking.
- [x] Implement event-based achievement tracking.
- [x] Implement mine depth tracking.
- [x] Implement unique fish tracking.
- [x] Implement total fish caught tracking.
- [x] Add fishing record inspection tools.
- [ ] Implement remaining achievement categories.
- [ ] Implement persistent achievement progress.
- [ ] Develop an in-game achievement tracker UI.
- [ ] Expand testing and compatibility.

## Contributing

This project is currently under active development. Suggestions, bug reports, and feature ideas are welcome.

## License

License information will be added when the project license is selected.

---

**Built with C#, SMAPI, and a love for Stardew Valley.**
