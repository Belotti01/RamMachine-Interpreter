namespace RamMachineInterpreter.Data;

public class Memory {
	public int this[uint registry] {
		get => ReadMemory(registry);
		set => SetMemory(registry, value);
	}

	public uint MaxAssignedAddress { get; protected set; }

	public int Accumulator = 0;
	protected readonly Dictionary<uint, int> _memory = new();

	public int ReadMemory(uint registry)
	{
		if(_memory.TryGetValue(registry, out int value))
		{
			return value;
		}
		// Registry has no assigned value - return default
		return 0;
	}

	public void SetMemory(uint registry, int value)
	{
		if(_memory.TryAdd(registry, value))
		{
			MaxAssignedAddress = Math.Max(MaxAssignedAddress, registry);
		}else {
			// Registry already exists in _memory
			_memory[registry] = value;
		}
	}

	public void Reset()
	{
		Accumulator = 0;
		_memory.Clear();
	}
}
