using System.Text.RegularExpressions;

namespace RamMachineInterpreter.Data;

public class Interpreter {
	public readonly Dictionary<string, Func<Command, string?>> _operations;

	bool _interrupt = false;
	protected int _currentCommandIndex = -1;
	public List<Command> Commands { get; } = new();
	public Memory Memory { get; } = new();
	protected List<string>? _inputs;
	protected int _nextInputIndex = 0;

	public bool IsExecuting { get; protected set; } = false;

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

	public void Interrupt()
	{
		_interrupt = true;
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
		_interrupt = false;
		_inputs = inputs?.ToList();
		_nextInputIndex = 0;
		Memory.Reset();
		string? newOutput;
		List<string> output = new();

		IsExecuting = true;
		try
		{
			for(_currentCommandIndex = 0; _currentCommandIndex < Commands.Count; _currentCommandIndex++)
			{
				if(_interrupt)
					break;

				newOutput = Execute(Commands[_currentCommandIndex]);
				if(newOutput is not null)
				{
					output.Add(newOutput);
				}
			}
		} catch(CommandException)
		{
			IsExecuting = false;
			throw;
		}
		IsExecuting = false;

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
		int registry;
		if(command.IsValuePointer)
		{
			// Convert from pointer to registry
			if(command.Value.Value < 0)
				throw new CommandException($"Attempted to access negative registry {command.Value.Value}.", command);
			registry = Memory[(uint)command.Value.Value];
		} else if(command.IsValueRegistry)
		{
			// Load the value from the registry
			registry = command.Value.Value;
		}else
		{
			return command.Value.Value;
		}
		
		return registry < 0
			? throw new CommandException($"Attempted to access negative registry {registry}.", command)
			: Memory[(uint)registry];
	}

	protected uint GetRegistryValue(Command command)
	{
		if(command.Value is null)
			throw new CommandException("Registry or pointer value has not been specified.", command);

		int registry;
		if(command.IsValuePointer)
		{
			if(command.Value.Value < 0)
				throw new CommandException($"Attempted to access negative registry {command.Value.Value}.", command);
			// Get registry at the pointer's position
			registry = Memory[(uint)command.Value.Value];
		} else if(command.IsValueRegistry)
		{
			// Already a registry
			registry = command.Value.Value;
		} else
		{
			throw new CommandException("Registry or pointer value was expected.", command);
		}

		return registry < 0 
			? throw new CommandException($"Attempted to access negative registry {registry}.", command) 
			: (uint)registry;
	}


	protected string? Add(Command cmd)
	{
		Memory.Accumulator += GetRawValue(cmd);
		return null;
	}
	protected string? Sub(Command cmd)
	{
		Memory.Accumulator -= GetRawValue(cmd);
		return null;
	}
	protected string? Mult(Command cmd)
	{
		Memory.Accumulator *= GetRawValue(cmd);
		return null;
	}
	protected string? Div(Command cmd)
	{
		Memory.Accumulator /= GetRawValue(cmd);
		return null;
	}
	protected string? Load(Command cmd)
	{
		Memory.Accumulator = GetRawValue(cmd);
		return null;
	}
	protected string? Store(Command cmd)
	{
		Memory.SetMemory(GetRegistryValue(cmd), Memory.Accumulator);
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
		return Memory.Accumulator == 0
			? Jump(cmd)
			: null;
	}

	protected string? Jgtz(Command cmd)
	{
		return Memory.Accumulator > 0
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
		if(_inputs.Count <= _nextInputIndex)
			throw new CommandException("Not enough inputs were provided for READ operations.", cmd);
		if(!int.TryParse(_inputs[_nextInputIndex++], out int value))
		{
			throw new CommandException("Non-numeric input provided for READ operation.", cmd);
		}
		Memory[GetRegistryValue(cmd)] = value;
		return null;
	}
}
