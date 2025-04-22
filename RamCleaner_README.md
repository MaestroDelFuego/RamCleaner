
# RamCleaner

## Overview

**RamCleaner** is a simple, open-source C# application designed to optimize memory usage by clearing unused RAM from running processes. It periodically checks memory usage and frees up system RAM every 2 minutes.

## Features
- Measures and displays RAM usage before and after cleaning.
- Clears unused memory from running processes using the `psapi.dll` library.
- Periodically checks and cleans memory every 2 minutes.

## Requirements
- .NET Framework 4.7.2 or later (for compiling and running the application).
- Windows operating system (as it uses `psapi.dll` and other Windows-specific libraries).

## Installation
1. Download the `RamCleaner.cs` file.
2. Compile it using a C# compiler or in Visual Studio.
3. Run the program in your terminal or IDE.

## Usage
1. Run the app to see the initial memory usage.
2. The app will clean memory every 2 minutes and display how much RAM is freed up.
3. You can stop the app by closing the terminal window.

## How it Works
- The app uses the Windows `psapi.dll` library to retrieve system memory stats and clean up unused memory by clearing the working set of processes.
- The program checks the memory usage, cleans it up, and then waits for 2 minutes before performing the next clean-up.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author
By **maestrodelfuego**

