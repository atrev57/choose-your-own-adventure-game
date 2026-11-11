# 03: Magical Armor with Partial Block

**What to build:** Complete the Merchant's either/or choice by adding Magical Armor. The Merchant menu becomes 1. Magical Armor (10 Gold), 2. Magical Weapon (10 Gold), 3. Leave. Armor grants an Armor Bonus of 5: when the dragon hits, a Defense Roll above Agility but within Agility + Armor Bonus is a Partial Block and does half damage, rounded down. Players without armor defend exactly as before.

**Blocked by:** 01 (scriptable dice), 02 (Gold and the Merchant)

**Status:** ready-for-agent

Spec: `../spec.md`. Pricing: ADR 0002.

- [x] The Merchant menu offers Magical Armor, Magical Weapon and Leave in that order (all 4 languages); ticket 02's tests are updated for the new numbering.
- [x] Buying Magical Armor sets the player's Armor (Armor Bonus 5), deducts 10 Gold, and prints what was bought, the Armor Bonus and the remaining Gold.
- [x] Armor's translated name and bonus appear in the player stats and on the combat stats screen.
- [x] Defense Roll with a scripted die: roll = Agility → no damage; Agility + 1 and Agility + 5 with armor → half damage, rounded down, with a Partial Block message; Agility + 6 → full damage; odd damage rounds down; damage 1 on a Partial Block → 0.
- [x] An unarmored player at Agility + 1 still takes full damage (ticket 01's tests still pass).
- [x] A full Italian playthrough through the game's start method: South → buy armor → back at the fork → exit.
- [x] Key-parity test passes.

## Comments
