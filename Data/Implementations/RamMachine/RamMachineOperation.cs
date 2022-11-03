namespace RamMachineInterpreter.Data;

public class RamMachineOperation : OperationBase<long> {
	public string? Label { get; protected set; }
	public bool IsDirectValue { get; protected set; }
	public bool IsPointerValue { get; protected set; }
	public long? Value { get; protected set; }
	public string? TargetLabel { get; protected set; }
	
	public RamMachineOperation(string codeLine, RamMachineInstructionSet set, int lineNumber = -1) 
		: base(codeLine, lineNumber)
	{
		int commentStartIndex = codeLine.IndexOf('#');
		if(commentStartIndex >= 0)
		{
			codeLine = codeLine[..commentStartIndex].Trim();
		}
		
		string[] parts = codeLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		if(parts[0].EndsWith('='))
		{
			Operation = parts[0][..^1].ToUpper();
			IsDirectValue = true;
		} else
		{
			if(!set.Operations.ContainsKey(parts[0].ToUpper()))
			{
				if(parts.Length == 1)
					throw new CommandException($"Invalid operation: {parts[0]}", this);
				Label = parts[0];
				parts = parts[1..];
				if(parts[0].EndsWith('='))
				{
					parts[0] = parts[0][..^1];
					IsDirectValue = true;
				}
			}
			Operation = parts[0].ToUpper();
		}

		if(!set.OperationAttributes.TryGetValue(Operation, out var attribute))
			throw new CommandException($"Unknown operation: {Operation}", this);

		if(attribute.RequiresLabelArgument)
		{
			if(parts.Length == 1)
				throw new CommandException("Missing required Label argument.", this);
			if(parts.Length > 2)
				throw new CommandException($"Too many arguments.", this);
			TargetLabel = parts[1];
			return;
		}
		
		if(!attribute.RequiresNumericArgument)
		{
			if(parts.Length > 1)
				throw new CommandException($"Operation {Operation} does not take a parameter.", this);
			return;
		}

		if(parts.Length == 1)
			throw new CommandException($"Operation {Operation} requires a parameter.", this);

		if(parts.Length > 2)
			throw new CommandException($"Too many arguments.", this);

		if(parts[1].StartsWith('*'))
		{
			if(!attribute.AllowPointer)
				throw new CommandException($"Operation {Operation} does not allow pointer parameters.", this);
			if(IsDirectValue)
				throw new CommandException($"Parameter \"{parts[1]}\" cannot be both direct and pointer.", this);
			IsPointerValue = true;
			parts[1] = parts[1][1..];
		}

		if(!long.TryParse(parts[1], out long arg))
			throw new CommandException("Non-numeric argument found.", this);
		Value = arg;
	}

	public override string ToString()
	{
		return $"{OperationNumber}: {RawLine}";
	}
}
