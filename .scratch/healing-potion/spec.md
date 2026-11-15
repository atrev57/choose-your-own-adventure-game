# Spec: Healing Potion

Status: ready-for-agent

Vocabulary follows `GLOSSARY.md`. Pricing rationale: ADR 0002 and its 2026-11-15 amendment. Localization conventions: ADR 0001.

## Problem Statement

Once the dragon fight starts, the player has only two choices: attack or retreat. Health Points only ever go down. A character who takes a couple of bad hits early can only keep attacking and hope, or give up. Nothing the player does before the fight helps them survive a long one. The Merchant's only offers are the two Magical Items, and a player who wants to play it safe, not hit harder or block better, has nothing to buy.

## Solution

The player can carry **Healing Potions** and drink one during the dragon fight, in place of attacking that round. A Healing Potion restores half of the player's **Maximum Health Points** (the Health Points rolled at character creation), rounded up, and never raises Health Points above that maximum.

Every character finds one Healing Potion beside the road on their first South trip. The Merchant also sells Healing Potions for 5 Gold each, with a stock of 2 per character, after which they are **Sold Out**. With 15 starting Gold, the player still can't afford both Magical Items. Their real choice now is one Magical Item plus one Healing Potion, or two Healing Potions.

In combat, the prompt after each exchange offers attack, heal (showing how many Healing Potions are left) and retreat. Healing when the player has no Healing Potions, or when they are already at full health, is refused without costing the player their turn. Everything is available in English, Spanish, French and Italian.

## User Stories

1. As a player, I want to find a Healing Potion on my first trip South, so that the South path rewards me even if I buy nothing.
2. As a player, I want to be told when I find a Healing Potion, so that I know I have one.
3. As a player, I want to find the Healing Potion only once per character, so that I can't farm the South path for free potions.
4. As a player who goes South a second time, I want to see only the empty stall and no new Healing Potion, so that the world stays consistent.
5. As a player, I want to see how many Healing Potions I carry in my character stats, so that I can plan before going North.
6. As a player with no Healing Potions, I want no Healing Potion line cluttering my stats, so that the stats only show what I actually have, like weapon and armor.
7. As a player, I want the Merchant to offer Healing Potions alongside the Magical Items, so that I can choose survivability.
8. As a player, I want Healing Potions to cost 5 Gold, so that I can buy one alongside a Magical Item or buy two instead of a Magical Item.
9. As a player, I want to still be unable to afford both Magical Items, so that the original either/or choice stays meaningful.
10. As a player, I want to buy Healing Potions one at a time, so that I decide exactly how many to buy.
11. As a player, I want the Merchant to tell me my new Healing Potion count and remaining Gold after each purchase, so that I know the purchase worked.
12. As a player, I want the Merchant to stock only 2 Healing Potions, so that I can't skip the Magical Items for an unbeatable stack of potions.
13. As a player who has bought 2 Healing Potions, I want the Merchant to show them as Sold Out, so that I understand why I can't buy more.
14. As a player who tries to buy a Sold Out Healing Potion, I want a clear message and the menu again, so that I don't lose my visit or my Gold.
15. As a player who can't afford a Healing Potion, I want the usual "cannot afford" message, so that the Merchant behaves consistently.
16. As a player, I want "Leave" to stay the last option in the Merchant menu, so that the menu is predictable.
17. As a player who buys a Magical Item and a Healing Potion, I want to end with 0 Gold and both items, so that the trade-off works as advertised.
18. As a player who buys 2 Healing Potions, I want to be told I can't afford a Magical Item with my remaining 5 Gold, so that the trade-off is clear.
19. As a player in combat, I want a heal option at the prompt after every exchange, so that I can heal whenever I choose.
20. As a player in combat, I want the prompt to show how many Healing Potions I have left, so that I can decide without checking my stats.
21. As a player in combat, I want drinking a Healing Potion to take the place of my attack that round, so that healing is a real trade-off.
22. As a player in combat, I want the dragon to still attack after I drink, so that healing isn't a free action.
23. As a player, I want a Healing Potion to restore half of my Maximum Health Points, rounded up, so that a potion is always worth at least half my health.
24. As a player with very low Maximum Health Points, I want a Healing Potion to restore at least 1 Health Point, so that it is never useless.
25. As a player, I want healing to stop at my Maximum Health Points, so that potions can't push me above the health I was rolled.
26. As a player whose healing would go over my maximum, I want to be told the amount I actually restored, so that I'm not misled.
27. As a player at full health, I want the game to refuse to let me drink and keep my Healing Potion and my turn, so that I don't waste either by mistake.
28. As a player with no Healing Potions, I want the game to say so and let me choose again, so that a mistaken choice doesn't cost me a turn.
29. As a player, I want each Healing Potion I drink to be used up, so that my count goes down as expected.
30. As a player, I want to see my Health Points as current / maximum on the combat stats screen, so that I know how hurt I am and whether healing is worth it.
31. As a player who never heals, I want combat to work exactly as before, so that the new option is purely optional.
32. As a player who never goes South, I want the dragon fight to work exactly as before, with no heal available, so that the South path stays optional.
33. As a Spanish, French or Italian player, I want all Healing Potion text, the potion's name and the Merchant's offer in my language, so that the game stays fully localized.
34. As a player in any language, I want to type `h` or `heal` to drink, so that combat commands stay language-neutral like `a` and `r`.
35. As a player who starts a new game, I want a fresh character with no Healing Potions, an unmet Merchant and a full stock of 2, so that each game offers the same opportunity.
36. As a maintainer, I want the Merchant's stock and prices to keep the either/or rule without a special-case rule, so that the economy stays the single source of truth (ADR 0002).
37. As a maintainer, I want the build to fail if any language is missing a new Healing Potion message, so that untranslated text never ships.
38. As a maintainer, I want heal amounts and combat turn order tested with controlled dice, so that the rules are verified exactly rather than by chance.

## Implementation Decisions

- **Maximum Health Points** is a new integer stat on the player, set to the rolled Health Points at character creation. It is not changed by damage. It is shown in the combat stats screen as "current / maximum".
- **Heal amount** is half of Maximum Health Points, rounded up, with a minimum of 1. It is computed from Maximum Health Points, not current Health Points. Health Points after drinking are capped at Maximum Health Points. The reported amount is the amount actually restored.
- **Healing Potion count** is a new integer on the player, starting at 0. It has no cap; the most a player can carry in practice is 3 (1 found plus 2 bought). It appears among the equipment stat lines, in both the character stats and the combat stats, only when greater than 0.
- **Drinking** is a player behavior with three outcomes: drank (with amount restored), refused because there are no Healing Potions, and refused because the player is at full health. Refusals change nothing.
- **Found Healing Potion**: on the first South trip, after the South narrative and before the Merchant intro, the player gains one Healing Potion and is told so. This is tied to the existing "has met the Merchant" fact on the player; nothing new is tracked. Later South trips show only the empty-stall message.
- **Merchant stock** is per character. It starts at 2 and goes down with each Healing Potion bought. Because the Merchant is visited only once per character, the stock is held for that visit (or on the player, if needed so tests can observe it). The found Healing Potion does not count against it.
- **Merchant menu** becomes: 1 Magical Armor (10 Gold), 2 Magical Weapon (10 Gold), 3 Healing Potion (5 Gold, or shown as Sold Out), 4 Leave. Input stays language-neutral numbers (ADR 0001). Choosing a Sold Out Healing Potion prints a Sold Out message and re-prompts. The existing "cannot afford" check is applied per item price, not one shared price.
- **Prices**: Magical Items stay at 10 Gold. Healing Potion is 5 Gold. Starting Gold stays at 15. These numbers and the stock of 2 are load-bearing (ADR 0002 amendment).
- **Combat loop** is restructured so the prompt's choice decides the player's action for the next round. The first round still opens with an automatic player attack followed by the dragon's attack. After that:
  - `a`/`attack`: the player attacks, then the dragon attacks (same as today).
  - `h`/`heal`: the player drinks, then the dragon attacks. There is no player attack that round.
  - `r`/`retreat`: unchanged.
  - Any refused heal (no Healing Potions, full health): message, then re-prompt; no round is played.
  - Invalid input: unchanged.
  - Victory and defeat checks happen at the same points as today, so a dragon attack after drinking can still defeat the player.
- **Combat prompt** text is updated in every language to offer heal and show the remaining Healing Potion count.
- **Player's Die becomes injectable** (prefactor), mirroring how the dragon takes a Die. Production behavior is unchanged: a player created without a Die uses a normal one.
- **Localization**: every new message key is added to all four languages, without accents (ADR 0001):
  - Healing Potion found
  - Drank and restored N
  - No Healing Potions
  - Already at full health
  - Sold Out
  - Merchant menu (renumbered)
  - Purchase result
  - Healing Potions stat line
  - Updated combat prompt
  - Health shown as current / maximum
  
  The Healing Potion's display name is hand-translated: *pocion curativa* (Spanish), *potion de soin* (French), *pozione curativa* (Italian).

## Testing Decisions

- **What makes a good test:** tests drive the game the way a player does and assert only on what the player sees (console output) or on public results of public methods. They do not reach into private state or assert on how a calculation is factored.
- **Seam 1: Combat with a scripted Die and scripted console input.** Uses the existing combat entry point plus the new injectable player Die and the existing scripted-die test helper. Covers:
  - Heal amount is half of Maximum Health Points, rounded up: an odd maximum (17 → 9), an even maximum (16 → 8), and the minimum of 1.
  - The cap: at 15/17, drinking reports 2 restored and health 17.
  - Full health: refused, the Healing Potion count is unchanged, and the dragon does not attack (no round is played).
  - No Healing Potions: refused, and no round is played.
  - The count goes down by one per drink.
  - In a heal round the player does not attack but the dragon does, and the dragon's attack can still defeat the player.
  - Attack-only fights produce the same sequence as before.
  - The combat prompt shows the remaining count.
  - The combat stats show current / maximum health.
- **Seam 2: the game's start method with scripted console input** (existing). Covers:
  - The first South trip shows the found Healing Potion, and stats show a count of 1.
  - Buying one Healing Potion: 15 → 10 Gold, count 2.
  - Buying two, then a third attempt shows Sold Out and Gold is unchanged.
  - Magical Item plus Healing Potion leaves 0 Gold.
  - Two Healing Potions, then a Magical Item attempt shows "cannot afford".
  - Leave is option 4.
  - The second South trip finds no Healing Potion.
  - A full Italian playthrough.
  
  Existing Merchant tests are updated for the renumbered menu. Tests end with exit, so random character-creation rolls do not affect assertions.
- **Seam 3: Messages** (existing). The key-parity test across languages covers all new keys automatically. Add one check that the Healing Potion display name exists in every language.
- **Prior art:** the merchant tests and Italian language tests (full playthroughs through the game's start method with console input); the dragon tests (scripted Die with exact roll sequences for the Defense Roll); the combat tests (constructing a test player and dragon); the Messages tests (key parity).

## Out of Scope

- Starting Health Points of 0 or below (e.g. a Halfling rolling 1 then 4 gets -3). Tracked separately in `.scratch/negative-starting-health/`. This feature only guarantees a minimum heal of 1.
- Using Healing Potions outside combat (at the path fork, before the fight, or at the dragon encounter prompt).
- Other potion types, a general inventory, selling items back, or restocking the Merchant.
- Ways to earn Gold (`.scratch/earn-gold/`). Any new Gold source must re-check ADR 0002 and its amendment.
- The Agility auto-block bug (`.scratch/agility-auto-block/`).
- Translating command input.

## Further Notes

- Implementation is split into vertical-slice tickets under `issues/`:
  1. Prefactor: the player's Die becomes injectable. Not blocked.
  2. Maximum Health Points and the found Healing Potion. Not blocked, so it can run alongside 01.
  3. Drinking a Healing Potion in combat. Blocked by 01 and 02.
  4. The Merchant sells Healing Potions: 5 Gold, stock of 2, Sold Out, renumbered menu. Blocked by 03.
- Spanish, French and Italian messages about the player keep the existing masculine-default limitation described in ADR 0001. The Healing Potion's name is feminine in all three languages.
