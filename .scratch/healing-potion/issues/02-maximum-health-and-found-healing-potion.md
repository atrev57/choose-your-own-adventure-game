# 02: Maximum Health Points and the found Healing Potion

**What to build:** The player gains **Maximum Health Points**, set to the Health Points rolled at character creation, and the combat stats screen shows health as current / maximum. On the first South trip, after the South narrative and before the Merchant intro, the player finds one **Healing Potion** and is told so. Their character stats and combat stats then show a "Healing Potions" line, which is hidden when the count is 0. Later South trips show only the empty stall and add nothing. The Healing Potion can't be drunk yet; that comes in 03.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

Spec: `../spec.md`. Localization: ADR 0001.

- [ ] Maximum Health Points is set at character creation and does not change when the player takes damage.
- [ ] The combat stats screen shows health as current / maximum.
- [ ] Through the game's start method: the first South trip prints the found-potion message before the Merchant intro, and stats then show "Healing Potions: 1".
- [ ] The Healing Potions stats line is absent when the count is 0 (a character who never went South).
- [ ] A second South trip shows the empty stall and does not add a Healing Potion; stats still show 1.
- [ ] A new character starts with 0 Healing Potions.
- [ ] The found-potion message, the stats line, current / maximum health, and the Healing Potion's display name (*pocion curativa*, *potion de soin*, *pozione curativa*) exist in all four languages without accents; key-parity passes, and one check confirms the display name exists in every language.
- [ ] A full Italian playthrough through the game's start method: South → find the Healing Potion → leave the Merchant → stats show the count → exit.

## Comments
