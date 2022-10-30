using System.Text.RegularExpressions;

namespace RamMachineInterpreter.Data;

public class Interpreter {
	public readonly Dictionary<string, Func<Command, string?>> _operations;

	protected int _currentCommandIndex = -1;
	public List<Command> Commands { get; } = new();
	public Memory Memory { get; } = new();
	public int Accumulator { get; protected set; }
	protected List<string>? _inputs;
	protected int _nextInputIndex = 0;

	public Interpreter() {
		_operations = new(StringComparer.OrdinalIgnoreCase)
		{
			{ "ADD", Add },
			{ "SUB", Sub },
			{ "DIV", Div },
			{ "MULT", Mult },
			{ "LOAD", Load },
			{ "STORE", Store },
			{ "HALT", Halt },
			{ "JUMP", Jump },
			{ "JZERO", Jzero },
			{ "JGTZ", Jgtz },
			{ "WRITE", Write },
			{ "READ", Read }
		};
	}


	public void ParseCode(string code)
	{
		string[] lines = code.Split("\n");
		ParseCode(lines);
	}

	public void ParseCode(string[] lines)
	{
		string line;
		Commands.Clear();

		for(int i = 1; i <= lines.Length; i++)
		{
			line = Regex.Replace(lines[i - 1], @"#.*", "").Trim();
			if(line.Length == 0)
				continue;

			Command command = new(line);
			Commands.Add(command);
		}
	}

	public string[] Execute(IEnumerable<string>? inputs = null)
	{
		_inputs = inputs?.ToList();
		_nextInputIndex = 0;
		string? newOutput;
		List<string> output = new();

		for(_currentCommandIndex = 0; _currentCommandIndex < Commands.Count; _currentCommandIndex++) {
			newOutput = Execute(Commands[_currentCommandIndex]);
			if(newOutput is not null)
			{
				output.Add(newOutput);
			}
		}

		_currentCommandIndex = -1;
		return output.ToArray();
	}

	protected string? Execute(Command command)
	{
		string? output = null;
		if(command.Operation is null)
			return output;

		if(command.Operation is not null)
		{
			if(!_operations.TryGetValue(command.Operation, out var op))
				throw new CommandException("Unknown operation.", command);
			output = op.Invoke(command);
		}
		return output;
	}

	protected int GetRawValue(Command command)
	{
		if(command.Value is null)
			throw new CommandException("Expected value, registry or pointer is missing.", command);

		if(command.IsValuePointer)
		{
			// Convert from pointer to value
			return Memory[Memory[command.Value.Value]];
		} else if(command.IsValueRegistry)
		{
			// Load the value from the registry
			return Memory[command.Value.Value];
		}
		return command.Value.Value;
	}

	protected int GetRegistryValue(Command command)
	{
		if(command.Value is null)
			throw new CommandException("Registry or pointer value has not been specified.", command);

		if(command.IsValuePointer)
		{
			// Get registry at the pointer's position
			return Memory[command.Value.Value];
		} else if(command.IsValueRegistry)
		{
			// Already a registry
			return command.Value.Value;
		}
		throw new CommandException("Registry or pointer value was expected.", command);
	}


	protected string? Add(Command cmd)
	{
		Accumulator += GetRawValue(cmd);
		return null;
	}
	protected string? Sub(Command cmd)
	{
		Accumulator -= GetRawValue(cmd);
		return null;
	}
	protected string? Mult(Command cmd)
	{
		Accumulator *= GetRawValue(cmd);
		return null;
	}
	protected string? Div(Command cmd)
	{
		Accumulator /= GetRawValue(cmd);
		return null;
	}
	protected string? Load(Command cmd)
	{
		Accumulator = GetRawValue(cmd);
		return null;
	}
	protected string? Store(Command cmd)
	{
		Memory.SetMemory(GetRegistryValue(cmd), Accumulator);
		return null;
	}

	protected string? Jump(Command cmd)
	{
		if(cmd.TargetLabel is null)
			throw new CommandException("JUMP operation target has not been specified.", cmd);
		int newIndex = Commands
			.FindIndex(x => x.Label is not null 
				&& x.Label.Equals(cmd.TargetLabel));

		if(newIndex < 0)
			throw new CommandException($"Target label \"{cmd.TargetLabel}\" could not found.", cmd);
		
		_currentCommandIndex = newIndex;
		return null;
	}

	protected string? Jzero(Command cmd)
	{
		return Accumulator == 0
			? Jump(cmd)
			: null;
	}

	protected string? Jgtz(Command cmd)
	{
		return Accumulator > 0
			? Jump(cmd)
			: null;
	}

	protected string? Halt(Command cmd)
	{
		_currentCommandIndex = Commands.Count + 1;
		return null;
	}

	protected string? Write(Command cmd) 
		=> GetRawValue(cmd).ToString();

	protected string? Read(Command cmd)
	{
		if(_inputs is null)
			throw new CommandException("No inputs were provided for READ operation.", cmd);
		if(!int.TryParse(_inputs[_nextInputIndex++], out int value))
		{
			throw new CommandException("Non-numeric input provided for READ operation.", cmd);
		}
		Memory[GetRegistryValue(cmd)] = value;
		return null;
	}
}
