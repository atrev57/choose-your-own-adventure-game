# 01: Prefactor — dice that tests can control

**What to build:** Make the dice overridable so tests can script exact rolls, and pin down today's dragon-attack Defense Roll behavior with tests before ticket 03 changes it. The game plays exactly as before.

**Blocked by:** None (can start immediately)

**Status:** ready-for-agent

Spec: `../spec.md`

- [x] The Die's roll can be overridden in tests; production rolls are unchanged.
- [x] A scripted-die test helper returns a queued sequence of rolls (and fails clearly if it runs out).
- [x] The dragon's attack, driven by a scripted die, is tested for: a miss; a hit that the player Full Blocks (Defense Roll = Agility) with no damage; a hit with Defense Roll = Agility + 1 that does full damage.
- [x] All existing tests still pass.

## Comments
