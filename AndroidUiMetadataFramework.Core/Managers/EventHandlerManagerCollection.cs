namespace AndroidUiMetadataFramework.Core.Managers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;
    using AndroidUiMetadataFramework.Core.Attributes;

    public class EventHandlerManagerCollection
    {
        private readonly ConcurrentDictionary<string, Func<IEventHandlerManager>> managers =
            new ConcurrentDictionary<string, Func<IEventHandlerManager>>();

        public IEventHandlerManager GetManager(string entityType)
        {
            Func<IEventHandlerManager> factory;
            if (this.managers.TryGetValue(entityType, out factory))
            {
                return factory.Invoke();
            }

            throw new ApplicationException($"Event manager '{entityType}' is not registered.");
        }

        public void RegisterAssembly(Assembly assembly)
        {
            var assemblyManagers = assembly.ExportedTypes
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .Where(t => t.GetInterfaces().Any(i => i == typeof(IEventHandlerManager)))
                .ToList();

            foreach (var manager in assemblyManagers)
            {
                var attribute = manager.GetCustomAttribute<EventHandlerAttribute>();
                this.managers.TryAdd(attribute.Type, () => (IEventHandlerManager)Activator.CreateInstance(manager));
            }
        }
    }
}