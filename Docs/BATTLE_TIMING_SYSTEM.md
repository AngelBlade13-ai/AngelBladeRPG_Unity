# Battle Timing System

`Petals in the Dusk` uses independent, speed-based action gauges. Combat does
not ask the player to queue the entire party before resolving a round. When a
combatant's gauge reaches `100`, that combatant receives one turn and the
chosen action resolves immediately.

## Timing Modes

### Wait Mode

Wait Mode is the default and recommended mode.

- All living combatants fill their gauges while no command menu is open.
- Every gauge pauses while the player chooses a command or target.
- Enemies cannot gain extra turns because the player stopped to read a menu.
- Time resumes after the selected action resolves.

### Active Mode

Active Mode is an optional setting for players who prefer time pressure.

- Gauges continue filling while command and target menus are open.
- Ready enemies act automatically even while the player is choosing.
- The currently ready party member keeps their pending turn until the player
  selects an action or that character is incapacitated.

Both modes use the same gauge math, action resolver, abilities, targeting,
tutorial rules, and rewards. Only menu-pausing behavior changes.

## Gauge Rules

- Gauge range: `0-100`.
- Fill rate: `Speed x 2` gauge points per second for the current prototype.
- A combatant with `0` Speed uses a minimum effective Speed of `1`, ensuring
  every living combatant eventually receives a turn.
- When multiple gauges become ready during the same update, the combatant that
  mathematically crossed the threshold first acts first.
- Exact ties use stable formation order: party formation first, then enemy
  formation.
- A resolved action resets only the acting combatant's gauge.
- A short presentation pause separates consecutive ready actions so combat
  feedback is readable and one action cannot instantly overwrite another.
- Tutorial wave transitions reset all gauges so newly authored battle beats
  begin from a clean timing state.

## Command Rules

- A ready party member chooses Attack, their current core Ability, Defend, or
  Escape where the encounter allows it.
- The command resolves immediately; other allies do not need to choose first.
- Enemies select and resolve their command automatically when ready.
- Defend reduces applicable physical damage until that character's next turn
  begins. Choosing Defend again renews the stance.
- Taunt is an active Reaver ability. It redirects enemy attacks to the Reaver
  until the Reaver's next turn begins; it is never applied automatically just
  because the character entered battle.
- If a selected target becomes invalid before resolution, the action retargets
  to the first valid target.

## Current Prototype Presentation

The existing battle status text displays each living combatant's action gauge
as `AT 0%-100%`. This is temporary verification UI. A later UI pass will replace
the percentage with stable visual bars, focus states, controller navigation,
and a settings control for choosing Wait or Active Mode.

The implementation deliberately uses original terminology and presentation
rather than reproducing another game's named system or gauge layout.
