# COMP 4770

- [Overview](#overview "Overview")
- [Assignment 1](#assignment-1 "Assignment 1")
  - [Getting Started](#getting-started "Getting Started")
  - [Requirements](#requirements "Requirements")
- [Details](#details "Details")

# Overview

- If you would prefer to read this file on GitHub for markdown formatting, email me your GitHub account and I will add you to the repository [here](https://github.com/StevenRice99/COMP-4770 "COMP 4770 Repository") which otherwise I am keeping private so others in the class do not have access to my solutions.
- This project uses my own AI library, [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI"), so there will likely be some differences between my solutions and those using Dr. Goodwin's library although [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI") is based upon Dr. Goodwin's library. I will explain any of these differences when they occur.
  - I developed [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI") directly in this project and its fully commented source code can be found under "Packages > Easy AI".
  - Samples for [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI") are also included under "Assets > Samples" although they are not directly related to assignment solutions.
  - Script templates under "Assets > ScriptTemplates" are also for [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI") which allow for creating specific component scripts by right clicking in the project explorer and going to "Create > Easy AI".

# Assignment 1

## Getting Started

- Under "Assets", go to "Scenes" and open "Assignment 1". The level will generate itself and spawn the cleaner agent when you click play.
- All scripts for assignment one are located under "Assets > Scripts > A1" and the sub folders within.
- The prefab for the cleaner agent is located at "Assets > Prefabs > Cleaner Agent".
- The general flow of the scene is as follow:
  1. A floor is generated of multiple tiles each with a "Floor" component attached to them where a percentage of floor tiles are twice as likely to get dirty as others being distinguished by their whiter and shinier material.
  2. At a set interval, tiles are randomly chosen to increase in dirt level which changes their materials.
  3. If the agent is on a dirty floor tile, it cleans it which takes a set amount of time. Otherwise, the agent moves and looks towards the nearest dirty floor tile if there are any. If all tiles are clean, the agent calculates and moves towards the weighted midpoint of the level being the optimal place to wait while factoring in the tiles that are more likely to get dirty than others.
- The general controls for the scene are as follows:
  - Click the "Details" button to see:
    - The agent, its performance, position, rotation, and all messages.
    - Further clicking buttons in this GUI will allow you to view messages for specific sensors and actuators.
  - Click the "Controls" button to see:
    - Buttons to increase or decrease the size of the floor.
    - Buttons to pause, resume, or step through the scene.
    - Buttons to switch between cameras.

## Requirements

1. **Review the A1 project files (get A1-Agent branch from GitLab then add to Unity).**

- Went through the core of the library to help build the foundation of [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI").

2. **Consider the design of the Agent Architecture. It is a first cut at providing a somewhat general framework that could be part of a Game AI library.**

- Same as above, went through the core of the library to help build the foundation of [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI").

3. **You may revise and/or extend it as you see fit. Bear in mind that it should be something you can build on and handle a variety of agent types.**

- Did exactly this and remade my own library [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI") to ensure I had a full understanding of the concepts for a general agent while also making some design changes that fit better with my general workflow with Unity projects.

4. **Search the source files for “// TODO for A1” comments. This will direct you to points in the code where changes are need.**

- These comments were related to filling in the gaps for getting the agent to work for assignment one which involved allowing the mind the think, agent movement and turning along with optional acceleration, and the sensors and actuators which needed to be completed.
- These issues are all taken care of in my solution, even though the logic is in some areas in different places, as my cleaner agent thinks based upon two sensors for the floor, moves and turns with acceleration rates, and cleans through an actuator which takes a set amount of time to finish cleaning.

5. **Create a test environment for a vacuum agent. This should have at least five locations with the same Y and Z (or X and Y) coordinates. There should be a way to visually indicate, sense, and change the clean/dirty status of a location. One approach might be to have each location be a separate plane and use its colour to indicate its status. You may use the provided environment or create your own fanciful world.**

- The environment is procedurally generated by setting the "Floor Size" on the "Cleaner Agent Manager" which can create any environment size from a 1x1 space to in theory as large as you want. By default I have it generating a 5x5 floor area so 25 floor tiles.
  - This can be adjusted at runtime as well by clicking the "Controls" button and then the increase and decrease X and Y buttons. For simplicity these runtime controls allow for a max size of 5x5 so the cameras fit the whole scene.
- Floor tiles can be either clean, dirty, very dirty, or extremely dirty which will change their material depending upon their state so they are visually identifiable.
  - Clean floor tiles are a grey color. If a tile is clean and it is also "likely to get dirty", meaning it has double the chance of normal floor tiles to increase in dirty level every time dirt is added to the floor, its clean material is instead a shiny white. More on this later in requirement thirteen.
  - Dirty floor tiles are a tan color.
  - Very dirty floor tiles are a light brown color.
  - Extremely dirty floor tiles are a dark brown color.
- At least one floor tile is increased in dirt level at a set time interval.
  - This is controlled by the field "Time Between Dirt Generation" field on the "Cleaner Agent Manager" which I have set to five seconds.
- The chance a floor tile gains a dirt level during these updates is given by "Chance Dirty" on the "Cleaner Agent Manager" which I have set to five percent. A random number between zero and one is generated three times for each floor tile and if the value is less than or equal to this percentage, the dirt level increases, meaning there is a chance for a floor tile to gain multiple dirt levels during a single generation.
- "Likely To Get Dirty Chance" on the "Cleaner Agent Manager" is used during initial floor generation and determines the odds a floor tile is twice as likely to get dirty compared to others giving it the shiny white material when clean over the standard gray material. More on this later in requirement thirteen.

6. **Add a SimpleReflexMind for the vacuum agent using SeekerMind for inspiration.**

- The mind is called "CleanerMind" instead of "SimpleReflexMind" and it is located in "Assets > Scripts > A1 > Minds".
- The mind is attached to the cleaner agent.
- The mind operates by first seeing if its sensors detected if it is standing on a dirty floor tile in which case it stops to clean the current floor tile. Otherwise, it moves towards the nearest dirty floor tile.

7. **Add a sensor to detect the location and status of the agent’s current position.**

- The cleaner agent has two sensors both of which the scripts for can be found in "Assets > Scripts > A1 > Sensors" and both are attached to the cleaner agent.
- "FloorsSensor" reads all floors in the level and returns their position and if they are dirty to the agent in a "FloorsPercept".
- "DirtySensor" detects if the floor tile the agent is currently on is dirty or not and returns this to the agent in a "DirtyPercept".

8. **Add a suction actuator to change the dirty/clean status of the location of the agent.**

- The actuator is named "CleanActuator" and is located in "Assets > Scripts > A1 > Actuators" and is attached to the cleaner agent.
- If it receives a "CleanAction" from the agent, it will clean the floor tile the agent is currently on.
- It's "Time To Clean" field indicates the time in seconds it takes to clean a floor tile by one dirt level which I have this set to 0.25 seconds. This means that if the agent wishes to clean an extremely dirty floor tile, it will take them 0.75 seconds to make the tile fully clean.

9. **Add an appropriate performance measure.**

- The performance measure is named "CleanerPerformance" and is located in "Assets > Scripts > A1 > PerformanceMeasures" and is attached to the cleaner agent.
- The performance measure is a value from zero to a hundred representing a percentage as to how clean the entirety of the floor is. If the entire floor is clean, it is a hundred percent. If the entire floor is extremely dirty, it is zero percent.
- The current value of the performance measure can be seen by clicking the "Details" button when the scene is running.
- The agent in this scenario has no concept of energy level thus it never gets tired or experience any similar effects and its sole objective is to keep the floors clean, thus, I did not add any factor to the performance measure involving how much the agent moves since there realistically is no punishment for this. If this was a more advanced multi-agent scenario where perhaps after an agent got tired and needed to rest it would swap roles with another agent, then adding in a measure for movement would be valuable, but in this limited scenario, it serves little benefit thus I did not incorporate one.

10. **Add anything else you need for functionality, elegant framework, creativity, esthetics.**

- Developed the [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI") framework along with its custom GUI.
- Multiple cameras which can all be switched to by hitting the "Controls" button when the scene is running.
- Resizeable floor by hitting the "Controls" button when the scene is running.
- Particle effects for the vacuum cleaner model when it is cleaning.

11. **Review the lectures where additional details will be provided including a walk through of the library.**

- As stated earlier, went through the core of the library to help build the foundation of [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI") and attended all lectures.

12. **See the video of my 80% solution.**

- Able to do the required 5x1 floor size as seen in the video and any other floor size due to its procedural generation.
- Floor tiles change color based on their state as seen in the video with mine having additional dirty levels.
- Multiple camera perspectives which can be controlled from the GUI as seen in the video.
- Messages displayed to a console window as seen in the video which can be seen by clicking the "Details" button.

13. **Challenge: What if the locations had different probabilities of becoming dirty and the agent knew this? Could its performance be enhanced? Example: Think of strategies for positioning the read/write head of a disk or the idle position of an elevator based on anticipated requests.**

- Floor tiles which are white and shiny are "likely to get dirty" meaning they are twice as likely to get dirty as other tiles when dirt is added to the floors.
- For instance, if floor tiles have a five percent chance to get dirty, these "likely to get dirty" floor tiles have a ten percent chance of getting dirty.
- If the entire floor is clean, instead of returning to the exact center of the floor, the agent calculates the weighted midpoint of all floor tiles. You can see the agent's current position at any point in time by clicking the "Details" button.
  - First, it sums the positions of all floor tiles once.
  - Then, it adds the positions of all floor tiles that are likely to get dirty again which will shift this midpoint as now these tiles have been added to the sum twice given they are twice as likely to get dirty.
  - Then, the agent moves to this location. Given the nature of this being a square floor, this will still be close to the center of the floor, and depending upon how the floor was randomly generated, may be the exact center, however, it is often slightly off center to be at the weighted center of the floor tiles.

# Details

- Created using Unity 2020.3.27f1.
- Created using [Easy AI](https://github.com/StevenRice99/Easy-AI "Easy AI").
- Project setup using the [Universal Render Pipeline](https://unity.com/srp/universal-render-pipeline "Universal Render Pipeline").
- Project setup using Unity's [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.1/manual/index.html "Input System").