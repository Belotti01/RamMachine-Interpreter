using System.Text.RegularExpressions;

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
		try
		{
			int commentStartIndex = codeLine.IndexOf('#');
			if(commentStartIndex >= 0)
			{
				codeLine = codeLine[..commentStartIndex].Trim();
			}

			string[] parts = Regex.Split(codeLine, @"\s+")
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.ToArray();
			
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
			Instruction = parts[0].ToUpper();

			if(!set.OperationAttributes.TryGetValue(Instruction, out var attribute))
				throw new CommandException($"Unknown operation: {Instruction}", this);

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
					throw new CommandException($"Operation {Instruction} does not take a parameter.", this);
				return;
			}

			if(parts.Length == 1)
				throw new CommandException($"Operation {Instruction} requires a parameter.", this);

			if(parts.Length > 2)
				throw new CommandException($"Too many arguments.", this);

			if(parts[1].StartsWith('*'))
			{
				if(!attribute.AllowPointer)
					throw new CommandException($"Operation {Instruction} does not allow pointer parameters.", this);
				IsPointerValue = true;
				parts[1] = parts[1][1..];
			}else if(parts[1].StartsWith('='))
			{
				if(!attribute.AllowDirect)
					throw new CommandException($"Operation {Instruction} does not allow direct value parameters.", this);
				IsDirectValue = true;
				parts[1] = parts[1][1..];
			}

			if(!long.TryParse(parts[1], out long arg))
				throw new CommandException("Non-numeric argument found.", this);
			Value = arg;
		}catch(Exception ex)
		{
			int n = 0;
		}
	}

	public override string ToString()
	{
		return $"[Line {OperationNumber}] {RawLine}";
	}
}