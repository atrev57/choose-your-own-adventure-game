# Spec: Attack resolution

Status: ready-for-agent

Vocabulary follows `GLOSSARY.md` (Attack, Combatant, Defense Roll, Full Block, Partial Block, Armor Bonus, Health Points). The decision and the alternatives that were rejected: ADR 0003. Localization conventions: ADR 0001.

## Problem Statement

Every Attack in the dragon fight is decided by rules that are written twice: once for the player attacking the dragon, once for the dragon attacking the player. Each copy mixes the rules (hit roll, Defense Roll, Full and Partial Block, damage) in with the narration text, and each reaches into the other Combatant to lower its Health Points.

The copies have already drifted apart:

- Only the dragon's Attack checks for a Partial Block.
- Only the dragon's Attack stops Health Points at 0. When the player lands a finishing blow, the game can report the dragon at a negative number, such as "Smolderfang has -4 Health Points", which reads like a bug.

For whoever maintains the game, every rule change has to be made in two places and kept in sync. The open Agility auto-block problem (a Combatant with Agility 20 or more can never be damaged) affects both sides, so today it needs two matching fixes. The rules can only be tested by running a whole fight and searching the English text for fragments, which is slow to write, breaks when wording changes, and ties rule tests to one language.

## Solution

Every Attack, in either direction, is resolved by one Attack resolution module. It decides hit or miss, the defender's Defense Roll (Full Block, Partial Block or no block) and the damage. It lowers the defender's Health Points, never below 0, and reports what happened. The player's and the dragon's Attack narration then only describe that result in the player's chosen language.

For the player, the fight plays exactly as before: same messages, same odds, same order of rolls. The one visible change, delivered as a separate step, is that a defeated dragon shows 0 Health Points rather than a negative number. For the maintainer, each combat rule lives in one place, applies to both Combatants automatically, and can be tested directly without running a fight or reading English text.

## User Stories

1. As a player, I want the dragon fight to play exactly as it did before the refactor, so that nothing I'm used to changes by accident.
2. As a player, I want my Attack to hit with the same odds as before (a d20 at or below my Strength), so that my character's Strength still matters the same way.
3. As a player, I want the dragon's Attack to hit with the same odds as before, so that the fight isn't suddenly harder or easier.
4. As a player, I want the dragon to block my Attack with a Defense Roll at or below its Agility, as before, so that the dragon's Agility still matters.
5. As a player, I want the dragon to taunt me after it blocks my Attack, as before, so that the fight keeps its personality.
6. As a player, I want the dragon to reply after I damage it, as before, so that the fight keeps its personality.
7. As a player, I want a Full Block on my Defense Roll to stop all of the dragon's damage, as before, so that high Agility still protects me.
8. As a player Wearing Magical Armor, I want a Partial Block to halve the dragon's damage, rounded down, as before, so that my Merchant purchase still pays off.
9. As a player without Magical Armor, I want a Defense Roll above my Agility to take full damage, as before, so that buying Magical Armor stays a real choice.
10. As a player, I want my Health Points never to drop below 0, as before, so that the defeat message reads sensibly.
11. As a player, I want a defeated dragon to show 0 Health Points, never a negative number, so that the victory reads sensibly.
12. As a player, I want the victory to happen at exactly the same moment as before, so that the clamp at 0 changes only what's shown, not who wins.
13. As a player, I want my Attack to show the same intro, weapon art, hit, miss, damage and Health Points messages as before, so that the fight looks familiar.
14. As a player with a Magical Weapon, I want my damage rolled up to its higher maximum, as before, so that the enchantment still counts.
15. As a player in Spanish, French or Italian, I want every Attack message to stay in my language, so that the refactor doesn't break localization.
16. As a player who drinks a Healing Potion in a round, I want that round to play exactly as before (no Attack from me, the dragon still attacks), so that healing keeps its trade-off.
17. As a player who retreats, I want retreat to work exactly as before, so that the refactor doesn't trap me in the fight.
18. As a player, I want the combat stats screen to show the same information as before, so that I can still judge the fight.
19. As a maintainer, I want the hit roll, Defense Roll, blocks and damage decided in one place, so that a rule change applies to both the player and the dragon.
20. As a maintainer, I want the player and the dragon to be interchangeable as attacker and defender, so that one set of rules serves both directions.
21. As a maintainer, I want a rule meant for one side only to come from what that Combatant reports (such as an Armor Bonus of 0), so that the rules never need to ask who is attacking.
22. As a maintainer, I want the Attack resolution to lower the defender's Health Points itself, never below 0, so that no caller can forget to apply damage or skip the clamp.
23. As a maintainer, I want the Attack resolution to report whether it hit, the kind of block, the damage actually dealt and whether the defender was defeated, so that narration and tests can read the result without redoing the rules.
24. As a maintainer, I want the narration to describe the result in Messages without any combat rules of its own, so that wording changes can't break the rules and rule changes can't break the wording.
25. As a maintainer, I want to test an Attack by checking its result and the defender's Health Points, so that tests don't depend on English text.
26. As a maintainer, I want to test an Attack with small stand-in Combatants, so that a test only sets up the numbers that matter.
27. As a maintainer, I want the order of Die rolls to stay the same (hit roll, Defense Roll, damage, then any narration roll), so that every existing scripted-dice test keeps working.
28. As a maintainer, I want the attacker's Die to keep rolling the defender's Defense Roll, as now, so that existing scripted-dice tests keep working.
29. As a maintainer, I want the existing combat, dragon and Healing Potion tests to pass without edits after the refactor, so that I know it changed no behavior.
30. As a maintainer, I want an unarmed player turned away before any Die is rolled, as now, so that the Attack resolution can assume the attacker has a weapon.
31. As a maintainer, I want the leftover roll helpers on the player and the dragon removed once nothing uses them, so that there's one obvious place to look for the rules.
32. As a maintainer, I want the Agility auto-block fix to become one change, so that it can't be applied to one side and forgotten on the other.
33. As a maintainer, I want the dragon to get a Partial Block automatically if it is ever given an Armor Bonus, so that new content doesn't need new rules.
34. As a maintainer, I want the refactor and the visible clamp fix delivered as separate steps, so that a behavior change is never hidden inside a refactor.
35. As a maintainer, I want the new terms (Attack, Combatant) in the glossary and the decision recorded in an ADR, so that future work uses the same language and doesn't undo the design.

## Implementation Decisions

- **Combatant interface.** The player and the dragon both implement one shared Combatant interface exposing:
  - Strength
  - Agility
  - Armor Bonus
  - the maximum damage of the weapon they attack with
  - Health Points, which the Attack resolution can lower

  The player's Armor Bonus comes from the Magical Armor they are Wearing, or is 0 without it; the player's maximum damage comes from the weapon they are Wielding. The dragon's Armor Bonus is always 0, so its Partial Block range is empty, which keeps today's behavior. Two real adapters (the player and the dragon) justify the seam.
- **Attack resolution module.** A new module with one operation: resolve an Attack given an attacker, a defender and a Die. In order:
  1. Hit roll: a d20; the Attack hits at or below the attacker's Strength. A miss ends the Attack with no further rolls.
  2. Defense Roll, on a hit: a d20 for the defender. Full Block at or below the defender's Agility ends the Attack with no damage roll. Partial Block is above Agility but at or below Agility plus Armor Bonus.
  3. Damage: a roll up to the attacker's maximum damage, halved and rounded down on a Partial Block.
  4. Lower the defender's Health Points by the damage, never below 0.
  5. Report the outcome.

  It has no Messages and no console output.
- **Attack outcome.** A small immutable result reporting:
  - whether the Attack hit
  - the block (none, Full or Partial)
  - the damage actually dealt
  - whether the defender was defeated (Health Points at 0)

  The defender's new Health Points are read from the defender, not copied into the outcome. This follows the existing Healing Potion drink outcome.
- **The Attack resolution changes Health Points itself** rather than leaving it to the caller, as drinking a Healing Potion changes the Healing Potion count itself.
- **Order of Die rolls is unchanged:** hit roll, Defense Roll, damage, then any narration roll (the dragon's taunt after blocking the player, or its reply after taking damage). The attacker's Die rolls the defender's Defense Roll, as today.
- **Narration.** The player's and the dragon's Attack keep their names, entry points and output. Each one:
  1. prints its intro and weapon art
  2. asks the Attack resolution module for the outcome
  3. describes the outcome with the same message keys as today

  The dragon's taunt and reply stay with the dragon as narration.
- **Unarmed player.** The "no weapon" check stays in the player's narration, before resolution, so no Die is rolled. The Attack resolution assumes the attacker has a weapon.
- **Removed.** The separate attack-roll, hit-check and damage-roll helpers on the player and the dragon are deleted; nothing else calls them. The dragon's taunt and reply pickers stay.
- **Combat is unchanged.** The fight loop, combat prompt, Healing Potion round, retreat and stats screens keep calling the same entry points.
- **Two steps.** Step one is the pure refactor. The dragon's Health Points are still lowered exactly as today, without stopping at 0, so every existing test passes unedited. Step two makes the clamp at 0 apply to both Combatants, which is the only visible change.
- **Domain model.** The glossary gains Attack and Combatant. Defense Roll and Full Block are reworded to apply to either defender. ADR 0003 records the decision.

## Testing Decisions

- **What makes a good test:** it drives a module through its interface and checks what comes out (the Attack outcome, and the defender's Health Points), never how the rules are split up inside. A Die scripted with exact rolls makes each rule deterministic. A scripted Die that runs out of rolls fails the test, which proves no extra roll happened.
- **Seam 1 (new): the Attack resolution module's interface.** Tested directly with a scripted Die and small stand-in Combatants, with no Messages and no console. Covers:
  - A miss: no Defense Roll, no damage, Health Points unchanged.
  - A hit with no block: full damage, Health Points lowered by it.
  - A Full Block: no damage, and no damage roll is used.
  - A Partial Block: damage halved, rounded down (e.g. 7 → 3).
  - An Armor Bonus of 0: a Defense Roll just above Agility does full damage.
  - The edges: a hit roll exactly at Strength hits; a Defense Roll exactly at Agility is a Full Block; a Defense Roll exactly at Agility plus Armor Bonus is a Partial Block.
  - Health Points stop at 0 when the player is the defender.
  - Health Points stop at 0 when the dragon is the defender (added in step two; fails before it).
  - Defender defeated is reported exactly when Health Points reach 0.
- **Seam 2 (existing, unchanged): Combat driven by a scripted Die and scripted console input**, plus the full playthroughs through the game's start method. No new tests are added here. All existing combat, dragon, Healing Potion and language playthrough tests must pass without edits after step one. That is the proof the refactor changed nothing the player sees. In step two, an existing test may need updating only if it expected a negative dragon Health Points value, noted in the commit.
- **Prior art:**
  - the Healing Potion drink tests (an outcome record checked directly)
  - the dragon tests (exact scripted roll sequences through the Defense Roll)
  - the combat tests (test players and dragons built with fixed stats)
  - the scripted-die helper used throughout

## Out of Scope

- The Agility auto-block fix (`.scratch/agility-auto-block/`). This work makes it a one-place change, but it is decided and delivered separately.
- Giving the dragon armor, or any other change to Partial Block.
- Translating the "no weapon" message, which can't currently appear because every Occupation gets a weapon.
- Changing the fight loop, the combat prompt, Healing Potion rounds, retreat or the stats screens.
- Changing the order of Die rolls or which Die rolls the Defense Roll.
- A console seam for prompts and output (a separate candidate from the architecture review).

## Further Notes

- Issues under `issues/`:
  1. One Attack resolution module for both Combatants: the refactor, the Combatant interface, the new Attack resolution tests, and this session's glossary updates and ADR 0003. No visible change. Not blocked.
  2. The dragon's Health Points stop at 0: the one visible change. Blocked by 01.
- Work happens on the branch `refactor/attack-resolution`, started from `feature/healing-potion`, because the player and the dragon have unmerged changes there.
- This was the top recommendation of the 2026-11-15 architecture review.
