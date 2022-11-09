namespace RamMachineInterpreter.Data;

public class RamMachineInstructionSet : InstructionSet<RamMachineInterpreter, RamMachineMemory, RamMachineInstructionSet, RamMachineOperation, RamMachineOperationAttribute, long> {

	public RamMachineInstructionSet(RamMachineInterpreter interpreter)
		: base(interpreter, false)
	{
	}

	protected long GetRegistryValue(RamMachineOperation operation)
	{
		if(operation.Value is null)
			throw new CommandException("Missing value for operation:", operation);
		long value = operation.Value.Value;

		if(operation.IsDirectValue)
			throw new CommandException("Direct value not allowed for operation:", operation);

		if(value < 0)
			throw new CommandException($"Attempted to retrieve negative registry {value}:", operation);

		if(operation.IsPointerValue)
		{
			value = Memory[(ulong)value];
			if(value < 0)
				throw new CommandException($"Attempted to retrieve negative registry {value}:", operation);
		}

		return value;
	}

	protected long GetValue(RamMachineOperation operation)
	{
		if(operation.Value is null)
			throw new CommandException("Missing value for operation:", operation);
		long value = operation.Value.Value;

		if(operation.IsDirectValue)
			return value;

		if(value < 0)
			throw new CommandException($"Attempted to access negative registry {value}:", operation);

		if(operation.IsPointerValue)
		{
			value = Memory[(ulong)value];
			if(value < 0)
				throw new CommandException($"Attempted to access negative registry {value}:", operation);
		}

		return Memory[(ulong)value];
	}

	[RamMachineOperation("LOAD", true, true, true)]
	public string? Load(RamMachineOperation operation)
	{
		Memory.Accumulator = GetValue(operation);
		return null;
	}

	[RamMachineOperation("STORE", true, false, true)]
	public string? Store(RamMachineOperation operation)
	{
		Memory[(ulong)GetRegistryValue(operation)] = Memory.Accumulator;
		return null;
	}

	[RamMachineOperation("ADD", true, true, true)]
	public string? Add(RamMachineOperation operation)
	{
		Memory.Accumulator += GetValue(operation);
		return null;
	}

	[RamMachineOperation("SUB", true, true, true)]
	public string? Sub(RamMachineOperation operation)
	{
		Memory.Accumulator -= GetValue(operation);
		return null;
	}

	[RamMachineOperation("MUL", true, true, true)]
	public string? Mul(RamMachineOperation operation)
	{
		Memory.Accumulator *= GetValue(operation);
		return null;
	}

	[RamMachineOperation("DIV", true, true, true)]
	public string? Div(RamMachineOperation operation)
	{
		Memory.Accumulator /= GetValue(operation);
		return null;
	}

	[RamMachineOperation("WRITE", true, true, true)]
	public string? Write(RamMachineOperation operation)
	{
		return GetValue(operation).ToString();
	}

	[RamMachineOperation("READ", true, false, true)]
	public string? Read(RamMachineOperation operation)
	{
		if(Interpreter.Inputs is null)
			throw new CommandException("No input available for operation:", operation);
		if(Interpreter.NextInputIndex >= Interpreter.Inputs.Length)
			throw new CommandException("No inputs left for operation:", operation);
		if(!long.TryParse(Interpreter.Inputs[Interpreter.NextInputIndex], out long value))
			throw new CommandException($"Non-numeric input detected for operation:", operation);

		ulong registry = (ulong)GetRegistryValue(operation);
		Memory[registry] = value;
		Interpreter.NextInputIndex++;
		return null;
	}

	[RamMachineOperation("JUMP", false, false, false, true)]
	public string? Jump(RamMachineOperation operation)
	{
		int nextOperation = Interpreter.Instructions.FindIndex(x => x.Label == operation.TargetLabel);
		if(nextOperation == -1)
			throw new CommandException($"Label \"{operation.TargetLabel}\" not found for operation:", operation);

		Memory.InstructionRegister = nextOperation - 1;
		return null;
	}

	[RamMachineOperation("JZERO", false, false, false, true)]
	public string? Jzero(RamMachineOperation operation)
	{
		if(Memory.Accumulator == 0)
			Jump(operation);
		return null;
	}

	[RamMachineOperation("JGTZ", false, false, false, true)]
	public string? Jgtz(RamMachineOperation operation)
	{
		if(Memory.Accumulator > 0)
			Jump(operation);
		return null;
	}

	[RamMachineOperation("HALT", false, false, false, false)]
	public string? Halt()
	{
		Memory.InstructionRegister = Interpreter.InstructionsCount;
		return null;
	}
}