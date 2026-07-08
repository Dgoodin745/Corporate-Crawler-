# Printer Room Panic: Local Single-Player Validation

Build and tune this loop before adding Unity Netcode, Mirror, Fish-Networking, or any other multiplayer transport.

## Playable loop

1. Spawn in the printer room with the cursor locked and the player camera active.
2. Move with `WASD`, sprint with `Left Shift`, aim with right mouse, fire the stapler with left mouse, and reload with `R`.
3. Enemies idle with a yellow readability color, turn red when the player enters their attention range, rotate toward the player, and path toward the player.
4. Each enemy takes stapler hits until defeated.
5. The room clear banner appears after the last enemy is defeated and the pacing delay expires.

## Validation checklist

- **Movement:** walking and sprinting should feel responsive without sliding after key release.
- **Camera feel:** mouse look should be stable, pitch-limited, and comfortable at the default sensitivity.
- **Aiming:** right mouse should smoothly tighten FOV; hits should register on enemies under the crosshair ray.
- **Enemy readability:** idle, aggro, and defeated states should be understandable before any networking complexity is introduced.
- **Room-clear pacing:** the clear banner should appear after a short beat, not instantly, so the final defeat has time to read.

## Networking gate

Only start a 2-player networking pass after the local loop above is playable in-editor. At that point, wrap the already-working systems with replicated input, authority, and state synchronization instead of designing combat and pacing inside the networking layer.
