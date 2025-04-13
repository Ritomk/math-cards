# Math Cards

**Math Cards** is a turn-based card game developed as part of an engineering thesis at Rzeszów University of Technology. The game is designed to teach and reinforce the understanding of Reverse Polish Notation (RPN) through engaging gameplay mechanics.

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [License](#license)
- [Acknowledgements](#acknowledgements)

## Overview

The game integrates educational content with interactive gameplay, allowing players to construct mathematical expressions using cards that represent numbers and operators. The primary objective is to build valid RPN expressions to outscore the AI opponent.

## Features

- Card-Based Gameplay: Players draw and play cards to form RPN expressions.
- AI Opponent: Compete against an AI that utilizes behavior trees for decision-making.
- Educational Focus: Reinforces understanding of RPN through practical application.
- Turn-Based Mechanics: Strategic play with a focus on planning and expression building.

## Technologies Used

- Unity Engine: Core game development platform.
- C#: Primary programming language for game logic.
- ShaderLab & HLSL: Used for custom shader development.
- NodeCanvas: Employed for AI behavior trees (Note: NodeCanvas is a paid Unity asset and is not included in this repository).

## Installation

1. Clone the Repository:
   git clone https://github.com/Ritomk/math-cards-shippuden.git

2. Open in Unity:
   - Use Unity Hub to open the project.
   - Ensure you have the appropriate Unity version installed.

3. Import NodeCanvas:
   - Purchase and import NodeCanvas from the Unity Asset Store.
   - This is required for the AI components to function correctly.

## Usage

After setting up the project in Unity and importing all necessary assets:

1. Build the Project:
   - Navigate to File > Build Settings in Unity.
   - Configure your build settings as desired.

2. Run the Game:
   - Press the Play button in Unity to start the game.
   - Engage in matches against the AI to practice RPN.

## Project Structure

math-cards-shippuden/
├── Assets/                  # Contains game assets including scripts, prefabs, and shaders
├── Packages/                # Unity package configurations
├── ProjectSettings/         # Project settings and configurations
├── UIElementsSchema/        # UI schema definitions
├── .gitignore               # Specifies files to ignore in version control
├── .vsconfig                # Visual Studio configuration
└── README.md                # Project documentation

## License

This project is intended for educational purposes and is not licensed for commercial use. All rights reserved by the author.

## Acknowledgements

- NodeCanvas: https://assetstore.unity.com/packages/tools/visual-scripting/nodecanvas-14914
- Rzeszów University of Technology for academic support.
