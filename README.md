# 3D Tetris
A 3D Tetris-style game built in Unity with C#. Place blocks on a three-dimensional board, complete horizontal layers, and keep the spawn area clear as the game speeds up.

[Play in your browser](https://dou186f.github.io/3d-tetris-unity/)

## Features
- **Grid-based movement** — Piece movement, rotation, hard drop, and ghost preview
- **Layer clearing** — Full layers clear with score tracking
- **Dynamic difficulty** — Fall speed increases as your score grows
- **Audio feedback** — Sound effects for piece placement and clearing

## Screenshots
<img width="180" height="320" alt="11" src="https://github.com/user-attachments/assets/9f2b85d9-f736-4ef5-9403-6dee43c7c031" />
<img width="180" height="320" alt="12" src="https://github.com/user-attachments/assets/cfd03982-6dca-48a2-ab0a-6f2d9a27db0d" />
<img width="180" height="320" alt="13" src="https://github.com/user-attachments/assets/5fecd691-04c1-45cc-b8a2-fa9c2d02742a" />
<img width="180" height="320" alt="14" src="https://github.com/user-attachments/assets/94bb138a-0780-4651-933d-4a7ba193c01d" />


## Controls
| Action | Key |
|---|---|
| Move on the board | W / A / S / D |
| Rotate around the vertical axis | Q / E |
| Hard Drop | Space |
| Start / Resume | Enter |
| Restart | R |
| Pause during play | Esc |
| Quit from the start/pause screen or after game over | Esc |

## Tech Stack
- **Engine:** Unity 2022.3.62f3
- **Language:** C#

## Getting Started
1. Install Unity Hub and Unity Editor **2022.3.62f3**.
2. Clone or download this repository and extract it if needed.
3. Add the repository root (the folder containing `Assets`, `Packages`, and `ProjectSettings`) to Unity Hub and open it with the matching Editor version.
4. Let Unity import the assets and restore the packages. An internet connection may be needed on the first open.
5. On a fresh open with an unmodified untitled scene, the Editor automatically opens `Assets/Scenes/SampleScene.unity`. Existing saved scenes and unsaved edits are left in place. If needed, open the scene manually, enter Play mode, click the Game view, and press **Enter** to start.

Unity regenerates `Library`, `Temp`, and IDE project files locally; they are not required in the repository. Movement advances one cell per key press.

## Gameplay
- The included scene uses a **6 x 12 x 6** board (width x height x depth).
- Six piece variants are selected randomly: a single cube, a two-block line, an L shape, a square, and two- or three-block vertical pieces.
- A full horizontal layer is cleared and the layers above move down.
- Each placed piece earns **10 points**, plus **100 points per cleared layer**.
- With the included scene settings, the fall interval starts at **0.8 seconds** and decreases by **0.04 seconds every 200 points**, down to **0.2 seconds**. These values can be adjusted on the GameManager in the Inspector.
- The game ends when the next piece cannot be placed at the spawn position.

## Building
1. Open **File > Build Settings** in Unity 2022.3.
2. Make sure `Assets/Scenes/SampleScene.unity` is enabled in **Scenes In Build**; it is already included in this project.
3. Select **PC, Mac & Linux Standalone**, choose your target platform, and switch platforms if necessary. Install the matching build support module through Unity Hub if required.
4. Select **Build** and save the output in a folder such as `Builds/Windows`.

To share a playable Windows version, ZIP the **entire build output folder**, including the executable, its data folder, and all accompanying files. Publish the ZIP as a GitHub Release asset alongside the source repository. Players can extract and run that build without installing Unity. Build output folders are ignored by this repository's `.gitignore`.

In the Unity Editor, **Esc** does not close the Editor; use the Play button to stop Play mode.
