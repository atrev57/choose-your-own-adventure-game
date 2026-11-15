# 03: Drinking a Healing Potion in combat

**What to build:**
- In the dragon fight, the prompt after each exchange offers `a`/`attack`, `h`/`heal` (showing how many Healing Potions are left) and `r`/`retreat`. The first round still opens with an automatic attack.
- Drinking restores half of Maximum Health Points, rounded up, with a minimum of 1. It never goes above Maximum Health Points, and the message reports the amount actually restored.
- Drinking takes the place of the player's attack that round, and the dragon still attacks.
- Healing with no Healing Potions, or at full health, is refused with a message and re-prompts. No round is played and nothing is used up.
- Attack-only fights play out exactly as before.

**Blocked by:** 01 (player dice that tests can control), 02 (Maximum Health Points and the found Healing Potion)

**Status:** ready-for-agent

Spec: `../spec.md`. Localization: ADR 0001.

- [ ] Combat (scripted Die + console input): heal amount for Maximum Health Points 17 → 9, 16 → 8, and 1 → 1.
- [ ] At 15/17, drinking reports 2 restored and health becomes 17.
- [ ] At full health, `h` is refused, the count is unchanged, and the dragon does not attack.
- [ ] With no Healing Potions, `h` is refused, and the dragon does not attack.
- [ ] In a heal round the player does not attack but the dragon does, and the count goes down by one.
- [ ] The dragon's attack after drinking can still defeat the player.
- [ ] The combat prompt shows the remaining count.
- [ ] An attack-only fight produces the same sequence as before; existing combat and game tests still pass.
- [ ] The drink message, both refusal messages and the updated combat prompt exist in all four languages without accents; key-parity passes.

## Comments
