# Unity-Zelion
idk what i am doing for now

following [this guy](https://www.youtube.com/@MisterTaftCreates/playlists)'s' tutorials

done : 
- ep 1 to 4 of tloz in unity series
- basic animations
- quick & dirty dash implementation
- tilemaps
- begining of the UI (which type of text should be used ?)

issues : 
- able to get inside colisions, and to get stuck (due to player going too fast and having a small collisionbox ? )
- camera smoothness: the player and camera going in the same direction make a jittery/stuttery effect
- cloning the repo does not clone all unity settings, etc. need to find a way to make these settings reproducible
- after dashing, the player cannot move for half a sec
- too much delay in the transition between walking and idle animations

todo : 
- interfaces (player, ennemies, items)
- ui, inventory, main menu 
- custom sprites
- lighting aka ray tracing
- network :skull:


# main differences in conception with zelda
### ability to always move
even when attacking, you can always move (even if it may reduce your speed in specific cases).

that detail make the game more reactive, and the player happy (being in control of things is a good thing)

this may make the game feel more like a hack-n-slash, but i think it's the way to go (seriously, who stops in 2024 ? cringe)

### lighting
not implemented yet

### network
to implement it, it think we should make 1 to 3 inactive players, that become active when someone joins the game.
