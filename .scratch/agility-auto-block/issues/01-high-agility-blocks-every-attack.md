# 01 — Agility 21+ blocks every dragon attack

Status: needs-triage

Elves and Halflings get Agility d20 + d4 (up to 24). The Defense Roll succeeds on `d20 ≤ Agility` (`Dragon.cs`), so Agility ≥ 20 means the dragon can never damage the player — and Magical Armor is worthless to them. (The same applies to the dragon's Agility vs. the player's attacks.)

Possible fix: a natural 20 always fails the Defense Roll. Found while designing the merchant encounter (`.scratch/merchant-encounter/spec.md`); deliberately not worked around there.

## Comments
