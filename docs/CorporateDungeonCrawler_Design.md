# Corporate Dungeon Crawler Design

This document is the single source of truth for the Corporate Dungeon Crawler prototype. It condenses the concept into the MVP decisions needed to build, test, and iterate without carrying duplicate full-design blocks elsewhere.

## MVP Scope

- Build a short co-op dungeon run set inside a hostile corporate office tower.
- Support one playable floor with procedural or hand-authored room sequencing.
- Include three playable employee archetypes, three standard enemy types, one elite encounter, and one boss.
- Provide a small set of weapons, skills, pickups, and room rewards that demonstrate replayability.
- Prioritize readable combat, fast restart, and clear team roles over broad content volume.
- Ship with placeholder art/audio where needed, but keep naming and layouts production-friendly.

Out of scope for the first MVP:

- Full campaign progression.
- Large equipment crafting trees.
- Ranked matchmaking.
- Persistent economy beyond simple unlock flags or test save data.
- Multiple corporate biomes or floors.

## Core Loop

1. Players select a class and enter the office dungeon lobby.
2. The team clears rooms filled with corporate-themed enemies and hazards.
3. Defeated enemies drop temporary resources such as health, energy, currency, or skill charges.
4. Room completion grants a reward choice, upgrade, or route decision.
5. Players push deeper toward the executive suite while managing cooldowns and survivability.
6. The run culminates in a boss fight against senior management.
7. On victory or defeat, players return to the lobby, review run results, and start another run with any unlocked prototype options.

## Player Classes

### Intern

- Fast, fragile skirmisher focused on mobility and burst damage.
- Uses quick melee attacks, thrown office supplies, and evasive movement.
- MVP ability: dash through danger and briefly increase attack speed after dodging.

### Analyst

- Mid-range support/control class focused on debuffs and resource efficiency.
- Uses spreadsheets, charts, or projectiles to slow and mark enemies.
- MVP ability: apply a vulnerability mark that increases team damage against a target.

### Manager

- Durable frontline class focused on disruption and team protection.
- Uses broad melee swings, motivational shouts, and area denial.
- MVP ability: taunt nearby enemies and grant nearby allies a short defensive buff.

## Enemies

### Paperwork Swarm

- Low-health melee units that pressure players in groups.
- Teaches positioning, cleave attacks, and room control.

### Meeting Drone

- Ranged enemy that fires predictable projectiles after a telegraphed wind-up.
- Teaches line-of-sight breaks and target prioritization.

### Compliance Enforcer

- Heavier enemy with a shield or armor phase.
- Teaches flanking, debuffs, and coordinated burst windows.

### Elite: Performance Auditor

- Mini-boss variant that combines ranged pressure with area denial zones.
- Appears before the boss to validate that players understand movement and team focus.

## Boss

### The Chief Executive Overlord

The MVP boss is the final encounter of the prototype floor. The fight should be readable, theatrical, and mechanically simple enough to tune quickly.

Phases:

1. **Quarterly Review:** summons Paperwork Swarms while using slow frontal attacks.
2. **Mandatory Meeting:** creates hazard zones that force players to rotate around the arena.
3. **Hostile Takeover:** enrages at low health, increasing attack tempo and spawning one Compliance Enforcer.

Win condition: reduce the boss to zero health and survive the final enrage sequence.

Failure condition: all connected players are downed at the same time.

## Networking Assumptions

- Target co-op first; local single-player should still work for testing.
- Use an authoritative host or server model for enemy AI, damage, pickups, and room progression.
- Clients may predict basic movement and animation, but combat resolution should be validated by the host/server.
- Rooms should be deterministic from a shared seed where possible to simplify synchronization.
- MVP matchmaking can be manual, invite-only, or direct-connect; robust matchmaking is not required.
- Design combat around modest latency: clear telegraphs, forgiving hit windows, and limited frame-perfect mechanics.
- Disconnect handling can return remaining players to the lobby or continue with reduced party size, whichever is fastest to implement.

## Prototype Milestones

### Milestone 1: Playable Combat Room

- One test room.
- One player class with movement, attack, health, and death states.
- Paperwork Swarm enemy with basic pathing and damage.
- Restart flow after defeat.

### Milestone 2: Class and Enemy Variety

- Add Analyst and Manager classes.
- Add Meeting Drone and Compliance Enforcer enemies.
- Add basic pickups and room reward selection.
- Establish placeholder UI for health, cooldowns, and current objective.

### Milestone 3: Run Structure

- Connect multiple rooms into a short floor.
- Add route or reward decisions between encounters.
- Track run outcome and return players to a lobby or start screen.
- Add temporary upgrades that reset between runs.

### Milestone 4: Boss Encounter

- Implement the Chief Executive Overlord arena.
- Add the three boss phases and victory/defeat states.
- Tune enemy waves, boss health, and player damage for a short MVP run.

### Milestone 5: Co-op Prototype

- Synchronize player movement, enemy state, damage, pickups, room completion, and boss state.
- Validate host/server authority assumptions.
- Add simple session creation and joining.
- Run end-to-end co-op tests for victory, defeat, disconnect, and restart flows.
