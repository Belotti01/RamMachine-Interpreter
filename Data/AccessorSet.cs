namespace RamMachineInterpreter.Data;

public class AccessorSet<T> {
	protected Func<T> Getter { get; init; }
	protected Action<T> Setter { get; init; }

	public AccessorSet(Func<T> getter, Action<T> setter)
	{
		Getter = getter;
		Setter = setter;
	}

	public T Get()
		=> Getter();
	
	public void Set(T value)
		=> Setter(value);
}
