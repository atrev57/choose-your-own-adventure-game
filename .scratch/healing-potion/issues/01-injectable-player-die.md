# 01: Player dice that tests can control

**What to build:** A prefactor with no gameplay change. The player can be given a Die, the same way the dragon is, so tests can script the player's attack and damage rolls with the existing scripted-die helper. A player created without a Die uses a normal one, so production behavior and character creation are unchanged.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

Spec: `../spec.md`.

- [ ] A player can be created with a supplied Die; a player created without one rolls as before.
- [ ] A test with a scripted Die drives a player attack through hit, dragon defense and damage, and asserts on the attack output (e.g. a scripted miss prints the miss message; a scripted hit with damage 7 lowers the dragon's Health Points by 7).
- [ ] All existing tests still pass unchanged.

## Comments
