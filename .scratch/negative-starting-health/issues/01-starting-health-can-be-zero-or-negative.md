# 01 — Starting Health Points can be 0 or negative

Status: needs-triage

Health Points are rolled as d20 plus the Strength modifier. A Halfling's modifier is subtracted (d4), so a Halfling who rolls 1 then 4 starts with **-3 Health Points**. Characters can also start at exactly 0. Either way the character is already defeated before the game begins.

Found while designing the Healing Potion (`.scratch/healing-potion/`). Healing is half of Maximum Health Points, so a value at or below 0 makes the heal amount meaningless. The potion feature works around this with a minimum heal of 1 and does not fix the underlying roll.

Possible fixes to weigh: give starting Health Points a floor of 1, or don't let the Halfling modifier reduce Health Points.

## Comments
