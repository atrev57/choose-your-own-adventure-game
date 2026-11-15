# Alderaan Adventure

A text-based choose-your-own-adventure game: the player creates a character, picks a path at a mountain fork, and faces what lies down it.

## Language

### Economy

**Gold**:
The player's currency. Every character starts with the same fixed amount — enough for exactly one Magical Item.
_Avoid_: Money, coins, currency

**Merchant**:
A trader met once per character on the South path, who sells Magical Items and a limited stock of Healing Potions for Gold.
_Avoid_: Shop, vendor, trader, store

**Magical Item**:
Magical Armor or a Magical Weapon. A player can afford only one.
_Avoid_: Merchant item, upgrade

**Healing Potion**:
A single-use drink that restores half of the player's Maximum Health Points (rounded up), never beyond that maximum. Drinking one in combat takes the place of the player's attack that round. Every character finds one on their first South trip, and the Merchant sells two more.
_Avoid_: Potion (alone), health item, consumable, medkit

**Sold Out**:
The Merchant has no Healing Potions left for this character.
_Avoid_: Out of stock, empty

**Magical Weapon**:
The player's own weapon after the Merchant enchants it: same kind of weapon, higher maximum damage.
_Avoid_: New weapon, upgraded weapon, enchanted weapon

### Health

**Health Points**:
How much more damage the player can take; at 0 they are defeated.
_Avoid_: HP (in prose), life, hit points

**Maximum Health Points**:
The Health Points rolled at character creation; healing never raises Health Points above it.
_Avoid_: Max HP, full health, starting health

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
A Magical Item that grants an Armor Bonus, creating the Partial Block range.
_Avoid_: Shield, protection item

**Armor Bonus**:
How far Magical Armor extends the Defense Roll beyond Agility into the Partial Block range.
_Avoid_: Protection, armor class, defense bonus

### Inventory

**Inventory**:
Everything the player is Wielding, Wearing and Carrying.
_Avoid_: Equipment, gear, items, bag

**Wielding**:
The weapon the player attacks with — including a Magician's lightning bolt spell, and a Magical Weapon once enchanted.
_Avoid_: Holding, equipped weapon

**Wearing**:
The Magical Armor the player has on, if any.
_Avoid_: Equipped armor

**Carrying**:
The Healing Potions and Gold the player has with them.
_Avoid_: Backpack, holding
