# Astral-Adventure
A platformer through outer space featuring the dragonborn armorer Houston.

## The Game

### How to Play
The game is avliable to play in the browser at https://dragonbrn.itch.io/astral-adventure.
The game can also be downloaded. In order to run the game in this fashion, extract the downloaded ZIP to anywhere on your computer, and run the executable.

### Gameplay
The game is a platformer with a few elements to make the gameplay interesting. Some of the following are.
- Gravity Fields, down isn't always down.
- Thunder Gauntlets, pound the ground with the force of gravity.
- Singularity Breath, the legacy of ameythist dragons allows Houston to launch into the air.

### Houston
You play as Houston, a dragonborn with scales that shimmer the colors of the cosmos. He is the designer of his spacesuit/power armor.

### Abilities
As Houston you have a number of abilities at your disposal
- Singuarity Breath: A breath weapon that can launch you into the air or repel enemies.
- Gravity Slam: If you are in the air you can slam into the ground using your armor's unique gauntlets.

## Development
This game is both a peronal project and a way to utelize many of the verious skills and techniques leanred trhough education.

### Skills
This game untelizes many object oriented principals in order to create gameplay architecture capable of supporting more additions to the game without required major rewrites to the gameplay architecture. For example, in order to simulate the necissary gravity, there is a **gravity object** class which handles the current "down" direction of the object and the acceleration of that object. From there, the **creature** class inherits the **gravity object** becuase all the creatures in the game are effected by the various gravity fields. **Creature** describes anthing in the game that has hitpoints, and can take damage. This strategy allows easy additon of new **creatures** or **gravity objects** into the game.

### Difficulties
This project has come with another of challenges to be overcome. This section what those challenges are, how they effected development and how they were overcome.

#### Interactions between Physics based gameplay and Arcade Gameplay
Many of the objects within the game utelize Unity's physics system to create a more varied and interesting gameplay experience. This includes the player character, but the player character's controller includes arcade gameplay; allowing the player character to have a "tight" controlling experience. In order to mesh these two systems, I applied forces in such a way and at specific times that emulate the necissary effects needed in arcade gameplay, while still maintaining physics based gameplay.
