namespace RamMachineInterpreter.Data;

public class Memory {
	public int this[int registry] {
		get => ReadMemory(registry);
		set => SetMemory(registry, value);
	}

	public int Size => _memory.Count;

	public int Accumulator = 0;
	protected readonly List<int> _memory = new();

	public int ReadMemory(int registry)
	{
		if(registry >= _memory.Count)
		{
			// Registry has no assigned value - return default
			return 0;
		}
		return _memory[registry];
	}

	public void SetMemory(int registry, int value)
	{
		if(registry >= _memory.Count)
		{
			// Registry has no assigned value - return default
			Pan(registry + 1);
		}

		_memory[registry] = value;
	}

	public void Reset()
	{
		Accumulator = 0;
		_memory.Clear();
	}

	protected void Pan(int toIndex)
	{
		for(int i = _memory.Count; i < toIndex; i++)
		{
			_memory.Add(0);
		}
	}
}
