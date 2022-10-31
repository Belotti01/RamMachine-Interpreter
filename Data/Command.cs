using System.Text;
using System.Text.RegularExpressions;

namespace RamMachineInterpreter.Data;

public struct Command {
	public string? Label { get; } = null;
	public string? Operation { get; } = null;
	public int? Value { get; set; } = null;
	public bool IsValuePointer { get; } = false;
	public bool IsValueRegistry { get; } = false;
	public string? TargetLabel { get; } = null;

	public Command(string input)
	{
		int opIndex = 0;
		string[] parts = Regex.Split(input, @"\s+")
			.Where(x => x.Length != 0)
			.ToArray();

		if(parts.Length > 2)
		{
			opIndex += 1;
			Label = parts[0];
		}

		if(parts.Length == 1)
		{
			if(parts[0].EndsWith(":"))
			{
				Label = parts[0][..^1];
			} else
			{
				Operation = parts[0];
			}
			return;
		}

		// parts.Length == 2, or 3 if a label preceeds it
		Operation = parts[0 + opIndex].ToUpper();
		string rawValue = parts[1 + opIndex];
		if(rawValue.StartsWith("*"))
		{
			IsValuePointer = true;
			rawValue = rawValue[1..];
		} else if(rawValue.StartsWith("="))
		{
			rawValue = rawValue[1..];
		} else
		{
			IsValueRegistry = true;
		}

		if(Operation.StartsWith("J")) {
			TargetLabel = rawValue;
		} else if(int.TryParse(rawValue, out int parsedValue)) {
			Value = parsedValue;
		} else
		{
			throw new CommandException("Invalid operation value.", this);
		}

		if(IsValueRegistry && Value < 0)
		{
			throw new CommandException($"Negative registry index detected: {Value}", this);
		}
	}

	public override string ToString()
	{
		string rawValue = Value is null ? "<None>" : IsValuePointer
			? $"*{Value.Value}"
			: IsValueRegistry
				? Value.Value.ToString()
				: $"={Value.Value}";
		
		StringBuilder str = new();
		str.Append($"- Operation: {Operation ?? "<Unspecified>"}")
			.Append($"\n- Value: {rawValue}")
			.Append($"\n- Label: {Label ?? "<None>"}");
		if(Operation is not null && Operation.StartsWith("J")) {
			str.Append($"\n- Target Label: {TargetLabel ?? "<Unspecified>"}");
		}

		return str.ToString();
	}
}
