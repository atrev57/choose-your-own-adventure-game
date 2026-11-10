# 02: Gold and a Merchant who sells the Magical Weapon

**What to build:** A playable Merchant on the South path. Every character starts with 15 Gold. On their first South trip they meet the Merchant, who shows their Gold and offers a Magical Weapon for 10 Gold or the chance to leave. Buying enchants the player's own weapon (same kind, same art, +4 maximum damage, magical name). Afterwards the player returns to the path fork; later South trips find the stall empty. Fully localized in all four languages.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

Spec: `../spec.md`. Pricing: ADR 0002. Localization conventions: ADR 0001.

- [ ] A new character has 15 Gold, shown in the character stats at creation.
- [ ] The South path narrative no longer says the player returns home (all 4 languages).
- [ ] First South trip per character shows the Merchant with the player's Gold and numbered options: Magical Weapon (10 Gold), Leave.
- [ ] Buying the Magical Weapon replaces the weapon with its magical counterpart (same art, max damage +4, own type string), deducts 10 Gold, and prints what was bought, the new max damage and the remaining Gold.
- [ ] Leaving keeps all 15 Gold and changes nothing.
- [ ] Invalid Merchant input shows an error and re-prompts.
- [ ] After the Merchant, the player is back at the path fork and can go North.
- [ ] A second South trip by the same character shows an empty-stall message and returns to the fork. Merchant state lives on the player, so a new character meets a fresh Merchant.
- [ ] All four magical weapon names have hand-translated, gender-correct display entries in every language; a test checks every magical weapon type translates in every language.
- [ ] The magical weapon's name and damage appear on the combat stats screen (tested by building a combat directly and checking its stats display).
- [ ] Playthrough tests through the game's start method (scripted console input, ending with exit) cover: buy weapon (Thief: damage 6 → 10, 5 Gold left), leave, invalid input then valid, second South trip.
- [ ] Key-parity test passes.

## Comments
