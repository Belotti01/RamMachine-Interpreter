### BETA VERSION
#### Current issues and TODOs
- Code and Inputs fields are plain text areasL one line = one instruction/input
- The Memory view currently displays all registries from position 0 to the highest index used by the code in a single table. This can become slow and ruin the page navigation experience.
- The Memory view is also missing language-specific registries (e.g.: the Instruction Register)
- The output area is just plain ugly
- Missing descriptions for the Instructions Set
- Step-by-step execution and breakpoints will be implemented after the code area gets an upgrade
- TODO: Verbose mode, where each line outputs what it did as it is executed
- TODO: Registry exportation in various format
- TODO: Code importation and exportation
- Implement moar languages (Brainfuck maybe?)

# RamMachine Interpreter
Software used to write and execute RamMachine code, in an environment supporting a dynamic view of the memory state, code syntax checking and runtime error reporting, with all the information needed to identify errors.

The program is built as a framework that allows the implementation of various other languages just by defining the set of instructions and the parsing methodology, requiring minimal or no changes to the UI and the code execution system.

Original Idea: https://github.com/skni-kod/Ram-machine
