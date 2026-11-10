# Alderaan Adventure

A text-based choose-your-own-adventure game: the player creates a character, picks a path at a mountain fork, and faces what lies down it.

## Language

### Economy

**Gold**:
The player's currency. Every character starts with the same fixed amount — enough for exactly one Merchant item.
_Avoid_: Money, coins, currency

**Merchant**:
A trader met once per character on the South path, who sells magical items for Gold.
_Avoid_: Shop, vendor, trader, store

**Magical Weapon**:
The player's own weapon after the Merchant enchants it: same kind of weapon, higher maximum damage.
_Avoid_: New weapon, upgraded weapon, enchanted weapon

### Defense

**Defense Roll**:
The d20 the player rolls when the dragon's attack hits, to see how much of it they block.
_Avoid_: Dodge roll, save

**Full Block**:
A Defense Roll at or below the player's Agility; the attack does no damage.
_Avoid_: Dodge, evade

**Partial Block**:
A Defense Roll above Agility but within Agility plus the Armor Bonus; the attack does half damage, rounded down.
_Avoid_: Glancing blow, damage reduction

**Magical Armor**:
A Merchant item that grants an Armor Bonus, creating the Partial Block range.
_Avoid_: Shield, protection item

**Armor Bonus**:
How far Magical Armor extends the Defense Roll beyond Agility into the Partial Block range.
_Avoid_: Protection, armor class, defense bonus
