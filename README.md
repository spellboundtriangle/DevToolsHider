# DevToolsHider
Hides Dev Tools or other objects in BONELAB

## Features
- Hiding Dev Tools in the spectator view
  - Overridable toggle for FlatPlayer
- Hiding other player-selected individual objects
- Applying item hiding to custom spawnable cameras

## Effects on gameplay
It should be noted that this mod makes modifications to certain GameObjects, which in rare cases may have unintended effects.
- The GameObject layer of Renderers found under the Spawn Gun and Nimbus Gun hierarchies (including those of mod equivalents) will be changed to Layer 11 (an unused layer in BONELAB)
- The GameObject layer of player-selected individual objects' Renderers will be toggled between Layers 11 and 10 (the Dynamic layer)
- The culling mask of cameras affected by the camera mask toggling function will disable or enable Layer 11 in the mask
- Layer 11 is modified to collide with layers 6 and 11, to reduce possibility of issues caused by Colliders being placed on Renderer GameObjects

## Requirements
- MelonLoader (v0.6.5+ recommended)
- BoneLib (v3.2.1+ recommended)
- BONELAB Patch 6 (earlier or later versions may not be compatible)

## Credits
- [TrevTV](https://github.com/TrevTV) for the [MelonLoader VS Wizard template](https://github.com/TrevTV/MelonLoader.VSWizard)
- [BONELAB modding community](https://discord.gg/mjmpUR8) for assistance during development
