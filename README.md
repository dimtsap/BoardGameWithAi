# 2048 Game with AI enabled hints
This game is a custom implementation of the 2048 game. The goal of the game is to reach the a tile with the number 2048 by sliding numbered tiles on a grid to combine them. 
Both UI and game engine are implemented in C# and similar to the original game the user can move tiles by using the keyboard arrows. 
This implementation includes an AI feature that provides hints to the player, suggesting optimal moves based on the current state of the game.

## Gameplay

The board is assumed to be a 4x4 grid, and the player can move the tiles in four directions: up, down, left, and right. When two tiles with the same number collide while moving, they merge into a tile with the total value of the two tiles that collided.
When the the board is initialized a random number of '2's are placed on the grid. 

After each successful move, a new tile with a value of '2' or '4' is added to an empty spot on the board. 
The player can continue to make moves until there are no valid moves left or the player reaches the 2048 tile.


An hint button is added, which prompts an LLM to analyze the current game state and suggest the best possible move. The AI uses a heuristic approach to evaluate the board and determine the best next move for the player. 

## Installation instructions
The game requires .NET 10.0 or later to run. 

In addition, the LLM of choice is a local Ollama with llama2:latest model, which can be downloaded from:

https://ollama.com/download/windows

Upon installation the Ollama should be served at the default port 11434.

The llamma2:latest model can be downloaded and installed by running the following command in the terminal:

```
ollama run llama2
```

Any other LLM can be used, but the code may need to be modified to accommodate the specific API of the chosen model.