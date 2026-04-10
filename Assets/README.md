# Bad Dice - Developer Architecture Guide

This project uses a **layered architecture** so gameplay systems stay modular, testable, and easy to extend.

The main goal is simple:
- keep **data** separate from **behavior**,
- keep **game rules** independent from Unity details,
- and connect systems through **interfaces, use cases, commands, and events**.

---

## Project Structure

All custom project code lives under `Assets/_Project/`.

### `Assets/_Project/Domain/`
**Pure game data and configuration.**

Use this layer for:
- entities and session/state data
- value-like structures that represent game rules data
- `ScriptableObject` assets used as configuration or definitions

**Put code here when:**
- it represents a rule, state, or data model
- it must not depend on Unity scene objects or infrastructure services
- it should be reusable by multiple systems

**Do not put here:**
- MonoBehaviours
- scene references
- input, loading, saving, UI, or Unity API logic

**Important:**
Domain should not contain core rules or business logic.

### Where To Put New Files

#### If you are adding game data
Put it in:
- `Assets/_Project/Domain/Features/<FeatureName>/`
- or `Assets/_Project/Domain/ScriptableObjects/`

Examples:
- dice definitions
- combat state data
- run state data
- game configuration assets

---

### `Assets/_Project/Application/`
**Use cases, contracts, commands, and orchestration logic.**

Use this layer for:
- service interfaces
- use case interfaces and application flows
- commands (`ICommand` implementations)
- state machine contracts and runtime events

**Put code here when:**
- it defines what the game should do
- it coordinates a flow between systems
- it needs to stay independent from Unity-specific implementation details

**Examples:**
- start or end a run
- resolve combat steps
- request actions

### Where To Put New Files
#### If you are adding a new rule or flow
Put it in:
- `Assets/_Project/Application/UseCases/`
- `Assets/_Project/Application/Commands/`
- `Assets/_Project/Application/Interfaces/`

Examples:
- new encounter flow
- dice merge operation
- combat resolution step
- state transition

---

### `Assets/_Project/Infrastructure/`
**Concrete implementations and Unity integration.**

Use this layer for:
- services that implement application interfaces
- adapters for Unity APIs
- persistence, scene loading, and other engine-facing code
- Zenject installers and dependency bindings

**Put code here when:**
- the implementation needs `UnityEngine`
- the code talks to the scene, physics, camera, input, save files, or asset loading
- you are binding dependencies through Zenject

**Examples:**
- scene loading service
- run persistence service
- combat health service
- input adapter

### Where To Put New Files
#### If you are integrating Unity or external systems
Put it in:
- `Assets/_Project/Infrastructure/Features/`
- `Assets/_Project/Infrastructure/Shared/`
- `Assets/_Project/Infrastructure/DependencyInjection/`

Examples:
- save/load implementation
- scene loading
- camera/input adapters
- Zenject bindings

---

### `Assets/_Project/Presentation/`
**Views, controllers, visuals, and scene-facing behaviour.**

Use this layer for:
- UI Toolkit views
- controllers that connect input to use cases/commands
- MonoBehaviour state handlers
- visual prefabs, sprites, materials, animations, and scenes

**Put code here when:**
- it displays information to the player
- it reacts to gameplay state and triggers UI/visual feedback
- it bridges player interaction to application logic

**Important:**
- Presentation should not contain core rules or business logic.
- Do not place scripts into the `Presentation/Visuals/` folder. This folder is only for assets like prefabs, sprites, materials, and animations.

### Where To Put New Files
#### If you are creating UI or scene-facing features
Put it in:
- `Assets/_Project/Presentation/Scripts/Features/<FeatureName>/`
- `Assets/_Project/Presentation/Visuals/`

Examples:
- HUD controller
- dice panel view
- menu interactions
- visual feedback animations

---

## Feature Creation Checklist

When adding a new feature, follow this order:

1. **Define the data** in `Domain`.
2. **Define the contract** in `Application`.
3. **Implement the logic** in `Infrastructure`.
4. **Expose the interaction** in `Presentation`.
5. **Bind dependencies** in `Infrastructure/DependencyInjection/`.
6. **Use event channels** only for feedback that should be visible to designers.

---

## Practical Rule Of Thumb

Ask these questions before choosing a folder or pattern:

- **Is this just data?** → `Domain`
- **Is this a game rule or orchestration step?** → `Application`
- **Does this talk to Unity or an external system?** → `Infrastructure`
- **Does this update the UI or the player-facing scene?** → `Presentation`
- **Should designers configure it in the Inspector?** → `ScriptableObject`
- **Should it be injected?** → Zenject
- **Should it notify other systems globally?** → Event Bus
- **Should it drive visual/audio feedback?** → Event Channel
- **Is it a single action with validation/execution?** → Command
- **Is it a multi-step feature flow?** → Use Case

---

## Main Patterns Used In This Project

### 1. ScriptableObject Architecture
Use `ScriptableObject` for:
- game configuration
- dice definitions
- event channels
- reusable designer-editable data

**Use it when:**
- designers need to tune values in the Inspector
- data should be shared across scenes or systems
- the content is static configuration, not runtime state

**Avoid it when:**
- the object is runtime-only state that changes frequently
- the logic belongs in a service or use case

---

### 2. Dependency Injection with Zenject
Dependencies are bound in installers under:
- `Assets/_Project/Infrastructure/DependencyInjection/`

**Use DI when:**
- a class needs a service, repository, adapter, or configuration object
- you want to keep classes decoupled and test-friendly
- multiple implementations may exist later

**Rules:**
- inject dependencies through constructors or Zenject bindings
- do not create hard references with `new` unless the object is a simple data object or command factory output
- do not use singletons for project services

---

### 3. Command Pattern
Commands live in `Application/Commands/`.

**Use commands when:**
- you need a discrete action with a clear input and output
- you want validation, logging, or other middleware support
- the action should be executable through a shared pipeline

**Good examples:**
- deal damage
- request a dice roll
- start or stop an encounter

**Do not use commands for:**
- long-lived orchestration flows
- pure data models
- UI-only state changes

---

### 4. Use Cases
Use cases live in `Application/UseCases/`.

**Use a use case when:**
- a feature spans multiple steps or services
- you need a readable application-level entry point
- the feature represents a user goal or game flow

**Good examples:**
- initialize a run
- start an encounter
- progress combat
- control the main game flow

**Difference from commands:**
- **command** = one discrete action
- **use case** = orchestration of a flow or feature

---

### 5. Event Bus
Use the event bus for **global cross-system communication** between decoupled services.

**Use it when:**
- one service must react to something another service did
- the communication crosses domain boundaries
- you want to avoid direct service-to-service coupling

**Good examples:**
- run state changed
- encounter started
- combat ended
- player died

**Avoid it when:**
- the communication is local to one prefab or one view/controller pair
- a direct call through a use case or command is clearer

---

### 6. ScriptableObject Event Channels
Use event channels as the bridge between **logic** and **designer-facing feedback**.

**Use it when:**
- gameplay logic needs to notify UI, audio, VFX, or animation systems
- the recipient should remain decoupled from the sender
- designers need to hook feedback in the Inspector

**Typical uses:**
- show combat feedback
- play audio on state changes
- trigger UI animations

**Avoid it when:**
- the communication is purely code-to-code and belongs in the application layer

---

### 7. UI Toolkit
Use UI Toolkit for all UI work in this project.

**Use it when:**
- creating menus, HUD, run information, tooltips, and combat UI
- building flexible UI layouts and reusable views

**Place UI assets in:**
- `Presentation/Visuals/UI/`

**Avoid UGUI** unless there is a very specific legacy reason.

---

## Final Note

Keep new code small, focused, and placed in the correct layer. If a class starts mixing data, logic, Unity calls, and UI updates, split it into separate responsibilities and move each part to the appropriate folder.

