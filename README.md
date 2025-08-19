# 2.5D Side Scroller
By Madison Kinney

A technically-crafted and artistically driven side-scrolling game project, featuring custom artwork, environments, and visual effects. Using a hybrid approach to movement systems for different game elements.

## Project Overview

### Technical Focus
- Character Controller for precise player movement
- Physics-based systems for dynamic enemy behaviors *(Planned)*
- Clean Architecture in Unity development
- Test-Driven Development (TDD) practices
- SOLID principles implementation

### Current Features
- Basic character movement system
  - Stamina with exhaustion state timeout
  - Sprint
  - Jump
  - Gravity handling
- Game event system v1
- Basic Companion system with VFX

### Planned Features
#### Core Systems
- [ ] Game State
- [ ] Player Health
- [ ] Scene Navigation
- [ ] Main Menu and Settings

#### Gameplay
- [ ] Physics-based enemy systems
- [ ] Enemy AI and behavior patterns
- [ ] Combat mechanics
- [ ] Environmental hazards
- [ ] Advanced jumping mechanics
  - [ ] Coyote Time
  - [ ] Expandable double jump
  - [ ] Wall jumping
  - [ ] Exhaustion effects on jumping
- [ ] Advanced VFX and particles

## In Development

### Core Systems
- Game State
- Player Character (Controller-based movement)
- Enemy Systems (Physics-based)

### Art and Visual Design
- Custom 3D models and animations
- Environment design and world-building
- VFX and particle systems
- URP-based dynamic lighting
- Post-processing and custom shaders

## Technical Architecture

### Design Principles
- Pure C# logic separated from MonoBehaviours
- SOLID-focused component design
- Testable code structure
- Data-driven ScriptableObjects
- Event Driven Logic

### Testing Strategy
- Unit tests for core logic
- Behavior-driven test cases
- Physics system testing
- Game event integration tests

## Development Setup

### Requirements
- Unity Editor 6000.1.11f1
- Universal Render Pipeline (URP)
- .NET Framework 4.7.1
- C# 9.0

### Getting Started
1. Clone repository
2. Open in Unity Editor 6000.1.11f1
3. Run test suite
4. Load LevelOne scene

## Contributing
While this is a personal learning project, feedback is welcome through issues and discussions.

