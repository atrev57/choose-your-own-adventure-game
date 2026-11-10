# Spec: Merchant encounter on the South path

Status: ready-for-agent

Vocabulary follows `GLOSSARY.md`. Pricing rationale: ADR 0002 (starting Gold enforces the either/or choice). Localization conventions: ADR 0001.

## Problem Statement

The South path is a dead end. A player who goes South is told that nothing happens and that they "return home, poor, and still bored" — yet the game actually puts them back at the path fork. The path offers nothing, contradicts itself, and gives the player no reason to ever choose it. Meanwhile, every character faces the dragon with exactly the equipment their occupation gave them, so there is no meaningful choice to make before the fight.

## Solution

On a character's first trip South, they meet a **Merchant**. Every character starts with **15 Gold**, and the Merchant sells two items for **10 Gold** each — so the player can afford exactly one:

- **Magical Armor**, which grants an **Armor Bonus** of 5. When the dragon hits, the **Defense Roll** now has three outcomes: a **Full Block** (no damage) at or below Agility, a **Partial Block** (half damage, rounded down) up to Agility + Armor Bonus, and full damage above that.
- A **Magical Weapon**: the player's own weapon, enchanted to the same kind of weapon with +4 maximum damage.

The player may also leave without buying. After the visit, they return to the path fork and can take their purchase North to face the dragon. On later South trips, the Merchant's stall is empty. All of this is available in English, Spanish, French and Italian.

## User Stories

1. As a player, I want my character to start with Gold, so that I have something to spend on my adventure.
2. As a player, I want every character to start with the same amount of Gold, so that I am never locked out of the Merchant by an unlucky roll and never have to restart to get a fair chance.
3. As a player, I want to see my Gold in my character stats, so that I know what I can afford.
4. As a player, I want to meet a Merchant the first time I travel South, so that the South path is worth taking.
5. As a player, I want the Merchant to show me my Gold and the price of each item, so that I can make an informed choice.
6. As a player, I want to choose between Magical Armor and a Magical Weapon, so that I can prepare for the dragon in the way that suits my character.
7. As a player, I want to be able to afford only one of the two items, so that my choice is a real trade-off.
8. As a player, I want to be able to leave the Merchant without buying anything, so that I am never forced to spend Gold.
9. As a player, I want to pick Merchant options by number, so that the menu works the same way as the rest of the game, in any language.
10. As a player, I want an invalid Merchant choice to be rejected and the menu shown again, so that a typo doesn't waste my visit or crash the game.
11. As a player, I want the Merchant to tell me what I bought, my new item's stats and my remaining Gold, so that I know the purchase worked.
12. As a player who buys Magical Armor, I want it to appear in my stats and on the combat stats screen with its Armor Bonus, so that I can see what protects me.
13. As a player who buys Magical Armor, I want dragon hits that I partly block to do half damage, so that the armor makes me tougher without making me invincible.
14. As a player who buys Magical Armor, I want hits I fully block to still do no damage, so that the armor never makes my defense worse.
15. As a player who buys Magical Armor, I want to be told when I made a Partial Block, so that I understand why I took less damage.
16. As a player who buys a Magical Weapon, I want my own weapon enchanted rather than replaced, so that my character keeps the weapon that fits their occupation.
17. As a Thief, I want the Magical Weapon to give me a real boost (dagger from 6 to 10 maximum damage), so that the purchase is worthwhile for a weak weapon.
18. As a player who buys a Magical Weapon, I want its magical name to appear in my stats, in combat, and in attack messages, so that I can see the enchantment.
19. As a player who buys a Magical Weapon, I want it to keep its familiar ASCII art, so that it still feels like my weapon.
20. As a player, I want to return to the path fork after visiting the Merchant, so that I can take my purchase North to the dragon.
21. As a player, I want the South path text to stop saying I went home, so that the story matches what actually happens.
22. As a player who goes South a second time, I want to find the Merchant's stall empty, so that it's clear I can't shop again.
23. As a player who goes South a second time, I want to be returned to the path fork, so that I can still continue my adventure.
24. As a player who starts a new game, I want a fresh character with 15 Gold and a Merchant I haven't met, so that each game offers the same opportunity.
25. As a player who skips the Merchant entirely, I want the dragon fight to work exactly as before, so that the South path is optional.
26. As a player without armor, I want my defense to work exactly as it did before, so that the new rule only affects armored characters.
27. As a Spanish, French or Italian player, I want all Merchant text, item names and messages in my language, so that the game stays fully localized.
28. As a Spanish, French or Italian player, I want magical weapon names to use correct grammatical gender (e.g. *spada magica*, *arco magico*), so that the text reads naturally.
29. As a maintainer, I want the either/or rule to come from prices and starting Gold, so that there is no special-case "one purchase" rule to maintain.
30. As a maintainer, I want the build to fail if any language is missing a new Merchant message, so that untranslated text never ships.
31. As a maintainer, I want the Partial Block boundaries tested with controlled dice, so that the defense rule is verified exactly rather than by chance.

## Implementation Decisions

- **Gold** is a new integer stat on the player, set to 15 when a character is created. It is not rolled. It is shown in the player stats display.
- **Item prices** are 10 Gold each. 15 starting Gold with 10-Gold items is what limits the player to one purchase (ADR 0002); there is no separate "already bought" rule.
- **Merchant state is per character.** Whether the character has met the Merchant is stored on the player, not on the game and not as a static, so that a new character always gets a fresh Merchant.
- **Armor** is a new nullable property on the player, mirroring the way the weapon is held: a type/name plus an Armor Bonus (5 for Magical Armor). It appears in the player stats display and the combat stats screen, with its name translated for display.
- **Magical Weapon** is a new weapon value that replaces the player's weapon: same ASCII art, maximum damage +4, and its own type string per base weapon ("magical long sword", "magical lightning bolt spell", "magical dagger", "magical long bow"). No "is magical" flag and no "magical {0}" format string, because those cannot respect grammatical gender in Spanish, French and Italian.
- **Defense Roll** in the dragon's attack gains a Partial Block band. With roll R, Agility A, Armor Bonus B (0 without armor) and damage D: R ≤ A → no damage; R ≤ A + B → floor(D / 2) damage; otherwise D damage. The player's health still floors at 0. The calculation may be factored into a helper, but it is tested through the dragon's attack (see Testing Decisions).
- **Die** becomes overridable (its roll method made virtual) so that tests can supply scripted rolls. Production behavior is unchanged.
- **South path flow** in the game: on the first trip for a character, show the (rewritten) South narrative and the Merchant; on later trips, show an empty-stall message. In both cases control returns to the path fork.
- **Merchant menu** shows the player's Gold and the numbered options 1 (Magical Armor, 10 Gold), 2 (Magical Weapon, 10 Gold), 3 (Leave). Input is language-neutral numbers, per ADR 0001. Invalid input re-prompts.
- **Purchase result message**: after buying, the Merchant prints what was bought, its key stat (Armor Bonus or new maximum damage) and the remaining Gold. This doubles as the observable outcome for tests.
- **Localization**: every new message key (Merchant intro, menu, purchase results, leave, empty stall, Partial Block, Gold stat, Armor stat), the rewritten South narrative, the four magical weapon names and the armor name are added to all four languages. Magical weapon and armor names are hand-translated entries in the display maps. No accented characters, per ADR 0001.

## Testing Decisions

- **What makes a good test:** tests drive the game the way a player does and assert only on what the player sees (console output) or on public results of a public method. They do not reach into private state or assert on how a calculation is factored.
- **Seam 1 — the game's start method with scripted console input** (existing). Tests supply input lines and capture output. Covers: 15 Gold shown at character creation; South → Merchant → buy Armor, buy Weapon, or Leave → back to the fork; invalid Merchant input re-prompts; purchase result messages (Armor Bonus 5, Thief damage 6 → 10, magical weapon name, 5 Gold remaining; 15 Gold kept on Leave); second South trip shows the empty stall; a full Italian playthrough. Tests end with exit, so the random character-creation rolls do not affect assertions.
- **Seam 2 — the dragon's attack with a scripted Die** (existing constructor injection; new: overridable roll and a scripted-die test helper). Tests set the player's Agility and Armor, then script the attack, defense and damage rolls. Covers: Full Block at R = A; Partial Block at R = A + 1 and R = A + 5; full damage at R = A + 6; half damage rounds down for odd damage; damage 1 on a Partial Block → 0; an unarmored player at R = A + 1 takes full damage (unchanged behavior).
- **Existing localization test** that every language has the same keys as English covers all new keys automatically. Add one check that each magical weapon type has a display entry in every language.
- **Prior art:** the Italian language test class (full playthrough through the game's start method with console input), the PBI-18 game tests (console input around the dragon encounter), the dragon tests (constructing a dragon with a Die), and the Messages tests (key parity across languages).

## Out of Scope

- Ways to earn more Gold (dragon's hoard, selling items). Tracked separately in `.scratch/earn-gold/`. Must respect ADR 0002.
- The bug where Agility of 20 or more blocks every attack (for both the player and the dragon), which makes Magical Armor worthless for high-Agility Elves and Halflings. Tracked separately in `.scratch/agility-auto-block/`; deliberately not worked around here.
- Buying both items, repeat Merchant visits with stock, selling items back, or a general inventory.
- Translating command input (letters/numbers stay language-neutral).
- Switching languages to UTF-8 accented text.

## Further Notes

- Implementation is split into vertical-slice tickets under `issues/`: 01 prefactor — dice that tests can control; 02 Gold and a Merchant who sells the Magical Weapon; 03 Magical Armor with Partial Block (blocked by 01 and 02).
- Spanish, French and Italian messages about the player keep the existing masculine-default limitation described in ADR 0001.
- The two follow-up issues were found during design and should be triaged before any balance changes to the Merchant.
