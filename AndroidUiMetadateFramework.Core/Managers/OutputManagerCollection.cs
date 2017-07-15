namespace AndroidUiMetadateFramework.Core.Managers
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Reflection;
	using AndroidUiMetadateFramework.Core.Attributes;

	public class OutputManagerCollection
	{
		private readonly ConcurrentDictionary<string, Func<IOutputManager>> managers =
			new ConcurrentDictionary<string, Func<IOutputManager>>();

		public IOutputManager GetManager(string entityType)
		{
			Func<IOutputManager> factory;
			if (this.managers.TryGetValue(entityType, out factory))
			{
				return factory.Invoke();
			}

			throw new ApplicationException($"Output manager '{entityType}' is not registered.");
		}

		public void RegisterAssembly(Assembly assembly)
		{
			var assemblyManagers = assembly.ExportedTypes
				.Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
				.Where(t => t.GetInterfaces().Any(i => i == typeof(IOutputManager)))
				.ToList();

			foreach (var manager in assemblyManagers)
			{
				var attribute = manager.GetCustomAttribute<OutputAttribute>();
				this.managers.TryAdd(attribute.Type, () => (IOutputManager)Activator.CreateInstance(manager));
			}
		}
	}
}