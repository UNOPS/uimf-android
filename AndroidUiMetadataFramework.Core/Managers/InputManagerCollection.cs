namespace AndroidUiMetadataFramework.Core.Managers
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Reflection;
	using AndroidUiMetadataFramework.Core.Attributes;

	public class InputManagerCollection
	{
		private readonly ConcurrentDictionary<string, Func<IInputManager>> managers =
			new ConcurrentDictionary<string, Func<IInputManager>>();

		public IInputManager GetManager(string entityType)
		{
			Func<IInputManager> factory;
			if (this.managers.TryGetValue(entityType, out factory))
			{
				return factory.Invoke();
			}

			throw new ApplicationException($"Input manager '{entityType}' is not registered.");
		}

		public void RegisterAssembly(Assembly assembly)
		{
			var assemblyManagers = assembly.ExportedTypes
				.Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
				.Where(t => t.GetInterfaces().Any(i => i == typeof(IInputManager)))
				.ToList();

			foreach (var manager in assemblyManagers)
			{
				var attribute = manager.GetCustomAttribute<InputAttribute>();
				this.managers.TryAdd(attribute.Type, () => (IInputManager)Activator.CreateInstance(manager));
			}
		}
	}
}