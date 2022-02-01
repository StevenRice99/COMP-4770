# Easy AI

**Easily create basic artificial intelligence in Unity.**

- [Overview](#overview 'Overview')
- [Features](#features 'Features')
- [Installation](#installation 'Installation')
- [Getting Started](#getting-started 'Getting Started')
- [Dependencies](#dependencies 'Dependencies')

# Overview

Easy AI was created to allow for easily creating intelligent agents in Unity without a need for a deep understanding of Unity itself. Easy AI handles all boilerplate such as agent movement, cameras, and a GUI rendering automatically so you can focus on developing the behaviour of your agents and let Easy AI take care of the rest. This library is mainly intended for teaching and learning purposes that emphasizes a distinct separation between agents and their sensors, actuators, and minds, as well as the percepts and actions which allow these components to communicate. In a production environment, you would instead most likely want to create specifically optimized agents that directly have access to their required information abd thus Easy AI is not recommended for use directly in a production environment but its core logic can still be used as a starting point for specialized agents.

# Features

- Three types of agents allowing for movement either by directly moving the transform, using a character controller, or using a rigidbody.
- Agents will automatically collect all sensors, actuators, and minds attached to them meaning you do not need to worry about forgetting to assign any references in the inspector.
- An agent manager handles all agents which allows for limiting how many agents are updated during a single frame meaning you can use Easy AI even if you have a lot of agents in the scene or your computer is older.
- A built in messaging system which allows for displaying messages on the GUI for ease of seeing exactly what your agents are performing at any given moment.
- The built in GUI allows for selecting every agent, sensor, and actuator in the scene so you can see their exact details and messages.
- Several included cameras.
  - A camera type to look at the currently selected agent from a fixed position.
  - A camera type to follow the currently selected agent from behind and zoom in and out.
  - A camera type to track the currently selected agent from above and zoom in and out.
  - All cameras are selectable from the built in GUI which handles ensuring they are all focused on the currently selected agent.
- Easily implement methods for drawing lines directly to the screen so you can further visualize what your agents are doing.
- The built in GUI includes controls to easily play, pause, and step through a single time step.
- Fully documented source code under the MIT licence.

# Installation

TODO

# Getting Started

TODO

# Dependencies

- Although there are no known version-specific requirements, this library has been developed on Unity 2020.3 and thus is it recommended to use this version to avoid any potential issues.
- Easy AI requires Unity's [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.1/manual/index.html "Input System") for zooming in and out with the included cameras.