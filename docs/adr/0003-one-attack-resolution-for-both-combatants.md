---
status: accepted
date: 2026-11-15
---

# One Attack resolution for both Combatants

The player's and the dragon's Attacks were each written out in full, in `Player.Attack` and `Dragon.Attack`, with the rules (hit roll, Defense Roll, Full and Partial Block, damage) mixed into the narration. The two copies drifted: only the dragon's Attack checked for a Partial Block, and only the dragon's Attack stopped the player's Health Points at 0. We decided to resolve every Attack in one module, `Attack.Resolve(attacker, defender, die)`, written against a shared **Combatant** interface that both Player and Dragon implement. It applies the damage to the defender and returns an `AttackOutcome` (hit, block kind, damage, defender defeated). `Player.Attack` and `Dragon.Attack` keep their names and only turn that outcome into Messages, the same split that `DrinkHealingPotion` → `HealingPotionDrink` already uses.

## Considered Options

- **One method per direction** (`PlayerAttacksDragon`, `DragonAttacksPlayer`) taking the concrete types: rejected, because it keeps two copies of the rules, and the copies are what drifted.
- **Return the outcome only and let `Combat` apply the damage**: rejected, because every caller would have to remember to apply it and to stop Health Points at 0. The resolution module changes Health Points itself, as `DrinkHealingPotion` changes the Healing Potion count.
- **Leave the rules where they were**: rejected, because the open Agility auto-block fix (`.scratch/agility-auto-block/`) would have to be made twice.

## Consequences

- **A rule changed in `Attack.Resolve` applies to both Combatants.** A rule meant for one side only has to be expressed through what the Combatant reports, as Partial Block is: the dragon's Armor Bonus is 0, so its Partial Block range is empty. Don't add a "who is attacking?" branch inside the resolution module.
- **The order of Die rolls is fixed**: hit roll, Defense Roll, damage, then any narration roll (the dragon's taunt or reply). The attacker's Die rolls the defender's Defense Roll. Tests built on `ScriptedDie` depend on both facts.
- An unarmed player is turned away by the narration before any Die is rolled, so `Attack.Resolve` can assume the attacker has a weapon.
