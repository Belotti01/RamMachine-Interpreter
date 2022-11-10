using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;

namespace RamMachineInterpreter.Data;

public class Memory<T> : IMemory<T>
	where T : struct {

	/// <summary>
	/// Get or Set the value of a registry.
	/// </summary>
	/// <param name="registry">The index of the registry to access.</param>
	/// <returns>The value of the registry at the specified index, or <see langword="default"/> if
	/// no value was assigned to it.</returns>
	public T this[ulong registry]
	{
		get => GetRegistryValue(registry);
		set => SetRegistryValue(registry, value);
	}

	protected readonly Dictionary<ulong, T> _memory = new();

	public T Accumulator { get; set; }
	public ulong Size { get; protected set; } = 0;  // Set manually - more efficient than checking the _memory.Keys everytime

	public int InstructionRegister { get; set; }
	public ReadOnlyDictionary<string, AccessorSet<T>> NamedRegistriesAccessors { get; set; }

	/// <summary>
	/// Stores the name-value pairs of the registries that are not tied to a property.
	/// Useful to keep the inheriting classes cleaner, avoiding property definitions for registries with
	/// the default <c>{ get; set; }</c> implementation.
	/// </summary>
	/// <remarks>Values are mapped into <see cref="NamedRegistriesAccessors"/>, so this is
	/// kept hidden in inheriting classes for simplicity.</remarks>
	private Dictionary<string, T> _UndefinedNamedRegistries { get; init; }


	public Memory(params string[] undefinedNamedRegistriesNames)
	{
		var namedRegistries = _GetDefinedNamedRegistries();
		bool added;
		_UndefinedNamedRegistries = new();
		foreach (var name in undefinedNamedRegistriesNames) 
		{
			_UndefinedNamedRegistries.Add(name, default);
			added = namedRegistries.TryAdd(name, new(
				() => _UndefinedNamedRegistries[name],
				value => _UndefinedNamedRegistries[name] = value
			));
			Debug.Assert(added, $"Duplicate registry definition for name \"{name}\".");
		}
		
		NamedRegistriesAccessors = new(namedRegistries);
		Debug.WriteLine($"Created {GetType().Name} with {namedRegistries.Count} named registries.");
	}

	
	public virtual void Clear()
	{
		Accumulator = default;
		_memory.Clear();
		InstructionRegister = 0;
		Size = 0;
	}

	public bool HasValue(ulong registry)
	{
		return _memory.ContainsKey(registry);
	}

	public bool HasRegistry(string registryName)
	{
		return NamedRegistriesAccessors.ContainsKey(registryName);
	}

	public virtual T GetRegistryValue(ulong registry)
	{
		if(registry < Size && _memory.TryGetValue(registry, out T value))
			return value;
		// Registry has no assigned value - return default
		return default;
	}

	public virtual void SetRegistryValue(ulong registry, T value)
	{
		if(_memory.TryAdd(registry, value))
		{
			Size = Math.Max(Size, registry + 1);
		} else
		{
			// Registry already exists in _memory
			_memory[registry] = value;
		}
	}

	public virtual bool TryGetRegistryValue(string registryName, out T value)
	{
		if(!NamedRegistriesAccessors.TryGetValue(registryName, out AccessorSet<T>? accessor))
		{
			value = default;
			return false;
		}
		
		value = accessor.Get();
		return true;
	}

	public virtual bool TrySetRegistryValue(string registryName, T value)
	{
		if(!NamedRegistriesAccessors.TryGetValue(registryName, out AccessorSet<T>? accessor))
			return false;
		
		accessor.Set(value);
		return true;
	}

	/// <summary>
	/// Map the properties in this Type with the <see cref="RegistryAttribute"/> into a Dictionary, which can be
	/// assigned to <see cref="NamedRegistriesAccessors"/>.
	/// </summary>
	private Dictionary<string, AccessorSet<T>> _GetDefinedNamedRegistries()
	{
		Dictionary<string, AccessorSet<T>> namedRegistries = new(StringComparer.OrdinalIgnoreCase);
		bool added;
		
		foreach(PropertyInfo property in GetType().GetProperties())
		{
			if(property.GetCustomAttribute(typeof(NamedRegistryAttribute)) is NamedRegistryAttribute attribute)
			{
				Debug.Assert(property.PropertyType == typeof(T), $"Invalid Property type \"{property.PropertyType.Name}\" of \"{property.Name}\". Generic type T was expected.");
				added = namedRegistries.TryAdd(attribute.Name, new(
					() => (T)property.GetValue(this)!,
					value => property.SetValue(this, value)
				));
				Debug.Assert(added, $"Duplicate registry definition for name \"{attribute.Name}\".");
			}
		}

		Debug.Print($"Loaded {namedRegistries.Count} property-based named registries for type {GetType().Name}.");
		return namedRegistries;
	}
}