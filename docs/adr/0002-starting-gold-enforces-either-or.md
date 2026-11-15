---
status: accepted
date: 2026-11-10
---

# Starting Gold enforces the Merchant's either/or choice

The Merchant on the South path offers Magical Armor or a Magical Weapon, and the player may buy only one. We enforce that through the economy rather than a rule: every character starts with a fixed **15 Gold** and each item costs **10 Gold**, so a second purchase is simply unaffordable. Starting Gold is fixed rather than rolled so that no player is locked out of the Merchant by a bad roll.

## Considered Options

- **Rolled starting Gold** (e.g. d20): rejected — a low roll could leave a player unable to buy anything, forcing restarts.
- **Rolled Gold with a guaranteed minimum** (e.g. 10 + d6): rejected — adds variance without adding a decision, since everyone still affords exactly one item.
- **A hard-coded "one purchase only" rule with no currency**: rejected — "buy" would be meaningless, and Gold gives room for future ways to earn it.

## Consequences

- **The 15/10 numbers are load-bearing.** Anyone who adds a way to earn Gold before the first South trip, lowers item prices, or raises starting Gold can silently let a player buy both items. If buying both should stay impossible, re-check this ADR first.
- The Merchant appears only once per character, so Gold earned later cannot be spent at this Merchant as currently designed.

## Amendment (2026-11-15): Healing Potions

The Merchant also sells **Healing Potions** for **5 Gold**, with a stock of **2 per character**. This keeps the either/or: 10 + 10 > 15, so two Magical Items are still unaffordable. It also adds a second trade-off: one Magical Item plus one Healing Potion, or two Healing Potions with 5 Gold left over. The stock limit only matters because of this price. At 5 Gold, 15 Gold could otherwise buy three potions. Considered and rejected: **10 Gold**, which would make the potion a third either/or choice and leave the stock limit with nothing to do; **3 Gold**, which lets a player afford a Magical Item plus potions while the stock limit alone holds back hoarding.

- **The 15/10/5 numbers and the stock of 2 are now all load-bearing.** Re-check this ADR before changing any of them.
