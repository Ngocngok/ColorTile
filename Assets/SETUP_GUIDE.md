# COLOR TILES - Unity Setup Guide

## Game Overview
COLOR TILES is an arcade territory control game where you compete against AI bots to claim the most tiles on a grid within 60 seconds.

## Scripts Created
All core game scripts have been successfully created:

1. **Tile.cs** - Individual tile behavior with state management
2. **GridManager.cs** - Generates and manages the 20x20 tile grid
3. **PlayerController.cs** - WASD/Arrow key player movement
4. **BotController.cs** - AI opponents with intelligent pathfinding
5. **GameManager.cs** - Game flow, timer, scoring system
6. **UIManager.cs** - HUD and end screen management

## Unity Setup Instructions

### Step 1: Create Game Objects and Prefabs

#### A. Create Tile Prefab
1. In Unity Hierarchy: Right-click → 3D Object → Cube
2. Rename it to "Tile"
3. Scale: X=0.9, Y=0.1, Z=0.9 (flat tile appearance)
4. Add the **Tile.cs** script component
5. Create a new Material, name it "TileMaterial"
6. Assign the material to the Tile
7. Drag the Tile to the Project window to create a prefab
8. Delete the Tile from the Hierarchy

#### B. Create Player Prefab
1. Create: 3D Object → Cube
2. Rename to "Player"
3. Scale: X=0.7, Y=0.7, Z=0.7
4. Position Y: 0.5
5. Create a Material with Blue color
6. Add the **PlayerController.cs** script
7. Drag to Project to create prefab
8. Delete from Hierarchy

#### C. Create Bot Prefab
1. Create: 3D Object → Cube
2. Rename to "Bot"
3. Scale: X=0.7, Y=0.7, Z=0.7
4. Position Y: 0.5
5. Create a Material (color will be set by script)
6. Add the **BotController.cs** script
7. Drag to Project to create prefab
8. Delete from Hierarchy

### Step 2: Setup Scene GameObjects

#### A. Create Grid Manager
1. Create Empty GameObject, rename to "GridManager"
2. Add **GridManager.cs** script
3. In Inspector:
   - Grid Width: 20
   - Grid Height: 20
   - Tile Size: 1
   - Tile Prefab: Drag the Tile prefab here

#### B. Create Game Manager
1. Create Empty GameObject, rename to "GameManager"
2. Add **GameManager.cs** script
3. In Inspector:
   - Game Duration: 60
   - Number Of Bots: 2 (or 3)
   - Player Prefab: Drag Player prefab
   - Bot Prefab: Drag Bot prefab

#### C. Setup Camera
1. Select Main Camera
2. Set Position: X=9.5, Y=25, Z=9.5
3. Set Rotation: X=90, Y=0, Z=0
4. Set Projection: Orthographic
5. Orthographic Size: 12 (adjust to see full grid)

### Step 3: Create UI

#### A. Create Canvas
1. Right-click Hierarchy → UI → Canvas
2. Canvas Scaler: Scale With Screen Size
3. Reference Resolution: 1920x1080

#### B. Create HUD Elements
Inside Canvas, create:

1. **Timer Text** (Top Center)
   - UI → Text - TextMeshPro
   - Name: "TimerText"
   - Position: Top center
   - Font Size: 48
   - Text: "01:00"

2. **Player Score Text** (Top Left)
   - Name: "PlayerScoreText"
   - Text: "Tiles: 0"
   - Font Size: 36

3. **Player Percentage Text** (Top Left, below score)
   - Name: "PlayerPercentageText"
   - Text: "0.0%"
   - Font Size: 32

#### C. Create End Screen Panel
1. Create UI → Panel, name it "EndScreenPanel"
2. Set it to fill the screen (Anchor: Stretch both)
3. Background color: Semi-transparent black (0,0,0,200)

Inside EndScreenPanel, create:

1. **Result Title** (Top center)
   - TextMeshPro, name: "ResultTitleText"
   - Text: "VICTORY!"
   - Font Size: 72

2. **Player Final Score** 
   - TextMeshPro, name: "PlayerFinalScoreText"
   - Text: "Player: 0 tiles (0%)"
   - Font Size: 36

3. **Bot Scores** (3 texts below player score)
   - "Bot1ScoreText"
   - "Bot2ScoreText"
   - "Bot3ScoreText"
   - Font Size: 32

4. **High Score Text**
   - Name: "HighScoreText"
   - Text: "Best Score: 0 tiles"
   - Font Size: 36

5. **Retry Button**
   - UI → Button - TextMeshPro
   - Name: "RetryButton"
   - Text: "RETRY"

6. **Menu Button**
   - UI → Button - TextMeshPro
   - Name: "MenuButton"
   - Text: "MAIN MENU"

#### D. Setup UI Manager
1. Create Empty GameObject: "UIManager"
2. Add **UIManager.cs** script
3. Drag all UI elements to their respective slots in Inspector:
   - Timer Text
   - Player Score Text
   - Player Percentage Text
   - End Screen Panel
   - Result Title Text
   - All score texts
   - Buttons

### Step 4: Final Setup
1. Make sure all prefabs are assigned in GameManager
2. Make sure all UI elements are assigned in UIManager
3. Set EndScreenPanel to inactive by default
4. Save the scene

### Step 5: Play!
Press Play in Unity. The game will:
- Generate a 20x20 grid
- Spawn player (blue) and bots (red, yellow, green)
- Start 60-second timer
- Player uses WASD or Arrow keys to move
- Claim tiles by walking on them
- After 60 seconds, see results screen

## Game Controls
- **WASD** or **Arrow Keys**: Move player
- Goal: Claim more tiles than the bots in 60 seconds!

## Features Implemented
✅ 20x20 tile grid system
✅ Player movement with WASD/Arrow keys
✅ 2-3 AI bots with intelligent pathfinding
✅ Tile claiming and locking mechanism
✅ 60-second game timer
✅ Score tracking and percentage calculation
✅ High score system (saved between sessions)
✅ Victory/defeat detection
✅ End game screen with full rankings
✅ Retry and menu options
✅ Color-coded tiles for each player/bot

## Optional Enhancements
You can further customize:
- Adjust grid size in GridManager (gridWidth, gridHeight)
- Change number of bots (1-3)
- Modify game duration
- Adjust bot decision speed (decisionDelay)
- Change colors in Tile.cs
- Add sound effects
- Add particle effects when claiming tiles
- Add different difficulty levels

## Troubleshooting
- **If tiles don't appear**: Check if Tile Prefab is assigned in GridManager
- **If player/bots don't spawn**: Check prefab assignments in GameManager
- **If UI doesn't show**: Verify all UI elements are assigned in UIManager
- **If TextMeshPro prompts appear**: Import TMP Essentials when prompted

Enjoy your COLOR TILES game!

