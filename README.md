Cave-In

A simple 3D arcade survival game made in Unity.

Pitch: Dodge falling rocks in a collapsing gold mine for 60 seconds, but every gold bar you grab makes the rocks fall faster.

 Gameplay video: https://drive.google.com/file/d/1xAbMGOjs1v5CW9ULjbdV0GysCctuvMC_/view?usp=sharing

How to play
Rocks fall from above. A red circle on the floor shows where each rock will land, and it grows as the rock gets closer.
Gold bars often fall right next to rocks (yellow circle). Grab them for points.
Greed mechanic: every gold bar you collect makes the rocks fall faster and appear more often. The Danger value on screen shows the current speed.
Win: survive 60 seconds.
Lose: get hit 3 times and you are buried, losing your gold.
Feedback
The player flashes yellow when collecting gold, and the Danger value grows and turns yellow.
The player flashes red when hit, then blinks for 1 second while it can't be hit again.
Gold, Lives, Time and Danger are always shown at the top-left.
Controls
Key	Action
W A S D / Arrow keys	Move
Space	Start / play again
How to run
Clone or download this repository.
Open the folder in Unity Hub (Unity 6 or 2022.3+, Universal 3D project).
Open Assets/Scenes/SampleScene and press Play.
Click inside the Game view and press Space to start.

The game creates its own scene by code when Play is pressed, so no setup is needed.

Scripts (Assets/CaveIn)
Script	What it does
GameManager.cs	Builds the scene, keeps score, lives and time, greed speed-up, win/lose, restart, on-screen text
Player.cs	Moves the player, detects touching rocks and gold, colour flashes, 1-second safety after a hit
Spawner.cs	Drops rocks at random spots and gold next to them; spawns faster as danger rises
Falling.cs	Makes rocks and gold fall faster as danger rises; shows the growing landing circle
Credits

All shapes are Unity's built-in primitives; no third-party assets were used. Code was developed with the assistance of Claude (Anthropic).

Team: Mohamed Ahmed Mohamed Kilany (ID 242500825), marwan elmaghrabi (ID 232400517 )
