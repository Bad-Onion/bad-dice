# Bad Dice - Project Startup Guide

This guide explains how to open and run the project, plus the key Unity configuration and scene setup to verify before implementing features.

## 1) Prerequisites

- Unity Editor version: `6000.3.10f1`
  - Source: `ProjectSettings/ProjectVersion.txt`
- Packages used by startup flow:
  - Zenject/Extenject: `com.svermeulen.extenject`
  - Input System: `com.unity.inputsystem`
  - URP: `com.unity.render-pipelines.universal`
  - Source: `Packages/manifest.json`

## 2) Open and run

1. Open Unity Hub and add/open project folder: `bad-dice`.
2. Let Unity finish import and script compilation.
3. Open `Assets/_Project/Presentation/Visuals/Scenes/MainScene.unity`.
4. Press Play.
5. From main menu, start game to load `Level_01`.

## 3) Editor configuration to verify

These are the important baseline settings used by this project.

- **Input backend**: Input System package
  - `ProjectSettings/ProjectSettings.asset` -> `activeInputHandler: 1`
- **Render pipeline**: URP asset assigned
  - `ProjectSettings/GraphicsSettings.asset` -> `m_CustomRenderPipeline` is set
- **Build scene list** (enabled):
  1. `Assets/_Project/Presentation/Visuals/Scenes/MainScene.unity`
  2. `Assets/_Project/Presentation/Visuals/Scenes/Level_01.unity`
  - Source: `ProjectSettings/EditorBuildSettings.asset`

## 4) Startup architecture at runtime

## 5) Quick troubleshooting

- If the game does not start correctly:
  1. Confirm Unity version (`6000.3.10f1`).
  2. Confirm `ProjectContext` prefab exists and `ProjectInstaller` references are assigned.
  3. Confirm `MainScene` and `Level_01` are enabled in Build Settings.
  4. Confirm no missing scripts on the key GameObjects in the scenes or prefabs.
  5. Confirm both `UIDocument` components point to the expected UXML assets.

