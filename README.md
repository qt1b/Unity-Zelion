# Unity-Zelion
idk what i am doing for now

had been following [this guy](https://www.youtube.com/@MisterTaftCreates/playlists)'s' tutorials at the begining of the project,
now doing most things without following a series


### working :
- basic movements
- sword
- arrows
- poison bomb ( but can't damage at a fixed time )

### wip
- poison bomb
- new sprites
- AI
- UI : healthbar

### todo:
### marcus :
- have (sword) swing, bow aiming and bomb aiming animations
### louis
- AI
- network
### adrien
- UI : menus
- healthbar merged
### quentin
- time-related capacities
- fix sword & poison bomb

### network
to implement it, it think we should make 1 to 3 inactive players, that become active when someone joins the game.

for syncing, we should probably have a main and a sub instance, the main handling all game events and the sub instance applying everything received from main.
that way we could easily have 1,2 or 3 sub instances.

### mabye
- interpolation if some calculations are slowing the game too much
