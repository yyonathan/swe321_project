# UPMAXXING
An infinite vertical platformer. Climb as high as you can before the camera leaves you behind.

## Download
See the realeases page for the latest builds

## Gameplay
The camera scrolls upward and accelerates over time. Jump between platforms, don't fall off the bottom. The longer you survive, the harder it gets.

## Controls
**Mobile:** On-screen left/right and jump buttons
**PC:** Arrow keys / A and D keys + Space

## Features
- Chunk-based procedural level generation with weighted difficulty phases
- Global leaderboard backed by Firebase. one entry per player, top 50 all-time displayed
- Moving, disappearing, and static platform types
- Polished movement through horizontal acceleration/deceleration, coyote time, jump buffer, jump cut, apex boost, fall gravity multiplier
- Custom platform shaders

## Leaderboard
Uses Firebase Realtime Database via REST API. Scores are submitted on death if it's a personal best. Leaderboard is accessible from the main menu.
