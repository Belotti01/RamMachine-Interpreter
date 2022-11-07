namespace RamMachineInterpreter.Data;

[AttributeUsage(AttributeTargets.Property)]
public class NamedRegistryAttribute : Attribute {
	public string Name { get; protected set; }
	public string? Description { get; protected set; }

	public NamedRegistryAttribute(string name)
	{
		Name = name;
	}
	public NamedRegistryAttribute(string name, string description)
	{
		Name = name;
		Description = description;
	}
}
