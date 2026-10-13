# UNO Card Game

A desktop implementation of the UNO card game developed in C# using WPF and .NET 8.

## Overview

This project implements a graphical version of UNO, combining game logic, player management and a desktop user interface.

The application was developed to put object-oriented programming concepts into practice while exploring graphical application development, data persistence and game statistics.

## Features

- **Graphical User Interface:** A desktop interface built with Windows Presentation Foundation (WPF).
- **Game Logic:** Classes to manage the game, cards, deck and players.
- **Player and Bot Selection:** Interface components for configuring players and selecting bots.
- **Data Persistence:** XML-based storage for saving and loading application data.
- **Game Statistics:** A dedicated window for displaying game statistics.

## Technologies

- **Language:** C#
- **Framework:** .NET 8
- **Desktop UI:** Windows Presentation Foundation (WPF)
- **Data Persistence:** XML
- **Development Environment:** Visual Studio

## Getting Started

### Prerequisites

- Windows.
- .NET 8 SDK.
- Visual Studio with the .NET desktop development workload, or a compatible .NET development environment.

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/Goncaloctomas/Jogo-UNO-WPF.git
   cd Jogo-UNO-WPF
   ```

2. Open `Jogo.slnx` or `Jogo.csproj` in Visual Studio.

3. Restore the project dependencies if required.

4. Build and run the application.

Alternatively, run the project from a terminal on Windows:

```bash
dotnet run --project Jogo.csproj
```

## Project Structure

The project separates several responsibilities into different classes and windows, including:

- **Game Logic:** Core game management and gameplay operations.
- **Cards and Deck:** Representation and management of cards.
- **Players:** Player-related data and behaviour.
- **XML Management:** Persistence through XML files.
- **User Interface:** WPF windows for gameplay, configuration and statistics.

## Learning Objectives

This project provided practical experience with:

- Object-oriented programming in C#.
- Designing desktop applications with WPF and XAML.
- Separating game logic from user interface components.
- Persisting and retrieving data using XML.
- Organizing a multi-file application.
- Managing application state and game statistics.

## Author

**Gonçalo Tomás**

[GitHub Profile](https://github.com/Goncaloctomas)
