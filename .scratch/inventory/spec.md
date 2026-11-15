# Spec: Inventory

Status: ready-for-agent

Vocabulary follows `GLOSSARY.md` (see its Inventory section: **Inventory**, **Wielding**, **Wearing**, **Carrying**). Localization conventions: ADR 0001. Pricing and Gold: ADR 0002 and its 2026-11-15 amendment. This spec builds on the Healing Potion feature (`.scratch/healing-potion/spec.md`).

## Problem Statement

What the player has is scattered and sometimes can't be seen at all. The character stats list Gold in one place and the weapon, armor and Healing Potion lines in a flat block after it, with no grouping. The combat stats screen shows the weapon, armor and Healing Potion lines but not Gold. Most importantly, the character stats can only be viewed right after character creation. Once the player has been South, found a Healing Potion and bought from the Merchant, they come back to the path fork with no way to check what they now have before deciding whether to face the dragon. All they have to go on is the Merchant's purchase messages, which have already scrolled past.

## Solution

The game presents everything the player has as one **Inventory**, grouped under three headings:

- **Wielding**: the weapon the player attacks with, and its maximum damage. This includes a Magician's lightning bolt spell, and a Magical Weapon once enchanted.
- **Wearing**: the Magical Armor and its Armor Bonus, or a line saying the player wears none.
- **Carrying**: Gold, and Healing Potions when the player has any.

The Inventory appears in the same form in the character stats and on the combat stats screen. The path fork gets a new choice, `i`/`inventory`, which shows the Inventory and then asks for a path again, so the player can check what they have at any point before going North. Everything is available in English, Spanish, French and Italian.

## User Stories

1. As a player, I want to see everything I have grouped as an Inventory, so that I can understand my resources at a glance.
2. As a player, I want my weapon shown under a Wielding heading, so that I know what I attack with.
3. As a Magician, I want my lightning bolt spell shown under Wielding, like any other weapon, so that my class is treated consistently.
4. As a player who bought a Magical Weapon, I want Wielding to show my enchanted weapon's higher maximum damage, so that I can see the purchase took effect.
5. As a player wearing Magical Armor, I want it and its Armor Bonus shown under a Wearing heading, so that I know my Partial Block range.
6. As a player without Magical Armor, I want Wearing to say I wear none, so that the Inventory always has the same three sections and I don't wonder whether armor is missing from the display.
7. As a player, I want my Gold shown under a Carrying heading, so that I see it alongside everything else I have.
8. As a player carrying Healing Potions, I want their count shown under Carrying, so that I can plan how many heals I have.
9. As a player with no Healing Potions, I want no Healing Potion line under Carrying, so that the Inventory only lists what I actually have.
10. As a player at the path fork, I want an `i`/`inventory` choice, so that I can check my Inventory at any time before choosing a path.
11. As a player returning from the Merchant, I want to view my Inventory at the fork, so that I can confirm what I bought and how much Gold I have left.
12. As a player who has found the Healing Potion, I want the Inventory at the fork to show it, so that I know I picked it up.
13. As a player, I want viewing my Inventory at the fork to change nothing and ask me for a path again, so that checking is free.
14. As a player, I want to view my Inventory at the fork as many times as I like, so that I'm never penalized for checking.
15. As a player, I want the path prompt to mention the inventory choice, so that I can discover it.
16. As a player, I want the Inventory in the character stats right after character creation, so that I see my starting weapon and 15 Gold before I set off.
17. As a player in combat, I want the combat stats screen to show my Inventory in the same layout, so that I don't have to relearn where things are.
18. As a player in combat, I want to see my Gold on the combat stats screen, so that the screen shows the same Inventory as everywhere else, even though Gold isn't used in combat.
19. As a player in combat, I want my Health Points still shown as current / maximum outside the Inventory, so that health stays with my other stats rather than being treated as something I carry.
20. As a player, I want Strength, Agility and Health Points to stay above the Inventory, so that stats and possessions stay clearly separate.
21. As a Spanish, French or Italian player, I want the Inventory headings, the "wearing nothing" line and the updated path prompt in my language, so that the game stays fully localized.
22. As a player in any language, I want to type `i` or `inventory` at the fork, so that commands stay language-neutral like `n`, `s` and `e`.
23. As a player who enters invalid input at the fork, I want the existing invalid-path message, so that the new choice doesn't change error handling.
24. As a player who never uses the inventory choice, I want the fork, the Merchant and the dragon fight to work exactly as before, so that the feature is purely additive.
25. As a player starting a new game, I want a fresh Inventory (starting weapon, no armor, 15 Gold, no Healing Potions), so that each game starts the same.
26. As a maintainer, I want the character stats and combat stats to build the Inventory from one shared place, so that the two can't drift apart.
27. As a maintainer, I want the build to fail if any language is missing a new Inventory message, so that untranslated text never ships.
28. As a maintainer, I want the Inventory headings to use the glossary terms exactly, so that code, text and docs share one vocabulary.

## Implementation Decisions

- **No new domain object.** The Inventory is a view of facts the player already has: weapon, armor, Gold and Healing Potion count. Nothing new is stored.
- **The shared equipment stat lines become the Inventory lines.** The existing player method that returns the weapon, armor and Healing Potion lines (shared by the character stats and combat stats) is renamed to say Inventory and now returns, in order:
  - An Inventory header.
  - **Wielding** heading, then the weapon and maximum damage lines (existing labels).
  - **Wearing** heading, then either the armor and Armor Bonus lines (existing labels) or a single "nothing" line.
  - **Carrying** heading, then the Gold line (existing label), then the Healing Potions line only when the count is above 0.
  
  The section labels are fixed in that order and always appear, even when a section is empty.
- **Character stats** stop printing Gold on their own. Gold appears only under Carrying. Name, Race, Occupation, Strength, Agility and Health Points stay above the Inventory, unchanged.
- **Combat stats** keep the player's Name, Strength, Agility and current / maximum Health Points above the Inventory. They now include Gold because it is part of Carrying. The dragon's section is unchanged; the dragon has no Inventory.
- **Weapon-less player** (only possible in tests; every occupation is assigned a weapon): the Wielding section shows the same "nothing" line as an empty Wearing section.
- **Path fork**: `i` or `inventory` (case-insensitive, trimmed) shows the player's Inventory and re-prompts for a path. It does not count as a South visit, changes no state, and can be repeated. The path-choice validator accepts the new inputs. Input stays language-neutral (ADR 0001).
- **Localization**: new message keys in all four languages, without accents (ADR 0001):
  - Inventory header
  - Wielding, Wearing and Carrying headings
  - "Nothing" line for an empty section
  - Updated path prompt offering `i` for inventory
  
  Existing labels for weapon, damage, armor, Armor Bonus, Gold and Healing Potions are reused as they are.
- The glossary already defines the four Inventory terms. Code comments and new message keys use those terms and avoid the listed synonyms (equipment, gear, items, bag, backpack, holding, equipped).

## Testing Decisions

- **What makes a good test:** tests drive the game the way a player does and assert only on what the player sees (console output) or on what public methods return. They don't inspect private state or check how lines are assembled internally.
- **Seam 1: the game's start method with scripted console input** (existing). Covers:
  - `i` at the fork before going South shows Wielding (the starting weapon), Wearing nothing, and Carrying 15 Gold with no Healing Potion line.
  - After the first South trip with purchases, `i` shows the updated Inventory: Magical Weapon damage or Magical Armor with its Armor Bonus, the remaining Gold, and the Healing Potion count.
  - `inventory` (full word, mixed case) works the same as `i`.
  - Viewing the Inventory re-prompts for a path and doesn't trigger the South visit or the empty stall.
  - Invalid input at the fork still shows the invalid-path message.
  - One full Italian playthrough that views the Inventory.
  
  Tests end with exit, so random character-creation rolls don't affect assertions.
- **Seam 2: the character stats and combat stats display** (existing public methods). Covers:
  - The order is header, Wielding, Wearing, Carrying, and all three headings are always present.
  - Gold appears exactly once in the character stats (under Carrying).
  - Combat stats now include Gold, and still show current / maximum Health Points outside the Inventory.
  - Healing Potion line present only when the count is above 0.
  - Magical Armor and its Armor Bonus appear under Wearing when worn, and the "nothing" line otherwise.
  
  Existing combat stats and Merchant tests that assert on these lines are updated where the layout changed.
- **Seam 3: Messages** (existing). The key-parity test across languages covers all new keys automatically.
- **Prior art:**
  - Merchant tests and Italian language tests: full playthroughs through the game's start method with console input.
  - Combat tests: building a test player and dragon and asserting on the combat stats display.
  - Healing Potion tests: stat-line presence depending on the count.
  - Messages tests: key parity.

## Out of Scope

- Using, dropping, swapping or selling anything from the Inventory, including drinking a Healing Potion outside combat.
- Wielding more than one weapon, wearing more than one piece of armor, or any carrying capacity or weight.
- New things to carry beyond Gold and Healing Potions.
- Showing the Inventory at the dragon encounter prompt or as a combat action. The combat stats screen at the start of the fight is enough.
- Showing the dragon's weapon as an Inventory.
- Ways to earn Gold (`.scratch/earn-gold/`), the Agility auto-block bug (`.scratch/agility-auto-block/`), and negative starting Health Points (`.scratch/negative-starting-health/`).
- Translating command input.

## Further Notes

- The Inventory glossary section was added to `GLOSSARY.md` before this spec and is not yet committed. It should be committed with this feature.
- The Healing Potion and Merchant specs listed "a general inventory" as out of scope. This spec stays within that limit: it adds a grouped view and a way to open it, not a general system for picking up and managing things.
- Suggested vertical slices for `issues/`:
  1. Inventory lines grouped under Wielding / Wearing / Carrying in both stats screens, with Gold moved under Carrying. Not blocked.
  2. `i`/`inventory` at the path fork, plus the updated path prompt. Blocked by 01.
