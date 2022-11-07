# RamMachine Interpreter
Software used to write and execute RamMachine code, in an environment supporting a dynamic view of the memory state, code syntax checking and runtime error reporting, with all the information needed to identify errors.

The program is built as a framework that allows the implementation of various other languages just by defining the set of instructions and the parsing methodology, requiring minimal or no changes to the UI and the code execution system.

Original Idea: https://github.com/skni-kod/Ram-machine

## BETA VERSION
### Current issues and TODOs
- Code and Inputs fields are plain text areas: one line = one instruction/input
- ~~The Memory view currently displays all registries from position 0 to the highest index used by the code in a single table. This can become slow and ruin the page navigation experience.~~
- The Memory view is also missing language-specific registries (e.g.: the Instruction Register)
- The output area is just plain ugly
- Missing descriptions for the Instructions Set
- Step-by-step execution and breakpoints will be implemented after the code area gets an upgrade
- TODO: Verbose mode, where each line outputs what it did as it is executed
- TODO: Registry exportation in various format
- TODO: Code importation and exportation
- Implement moar languages (Brainfuck maybe?)

## Running the Program
### Requirements 
- The [.NET6 Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

### How to Run:
1. Download the [latest release](https://github.com/Belotti01/RamMachine-Interpreter/tags) and extract the contents of the .zip
2. Start the **RamMachineInterpreter.exe** file
3. Open the page https://localhost:5001

NOTE: if the port is already in use, it may be different from 5001. If so, just check the console to identify which port the software is using.
