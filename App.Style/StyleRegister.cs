namespace App.Style
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    public class StyleRegister
    {
        private readonly ConcurrentDictionary<string, Func<IStyle>> managers =
            new ConcurrentDictionary<string, Func<IStyle>>();

        public void AddStyle(IStyle style)
        {
            var attribute = style.GetType().GetCustomAttribute<StyleAttribute>();
            this.managers.TryAdd(attribute.Name, () => style);
        }

        public void ApplyStyle(string name, object element)
        {
            var styles = name.Split(' ');
            foreach (var style in styles)
            {
                var manager = this.GetManager(style);
                manager?.ApplyStyle(element);
            }
        }

        public IStyle GetManager(string entityName)
        {
            Func<IStyle> factory;
            if (this.managers.TryGetValue(entityName, out factory))
            {
                return factory.Invoke();
            }

            return null;
        }

        public void RegisterAssembly(Assembly assembly)
        {
            var assemblyManagers = assembly.ExportedTypes
                .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .Where(t => t.GetInterfaces().Any(i => i == typeof(IStyle)))
                .ToList();

            foreach (var manager in assemblyManagers)
            {
                var attribute = manager.GetCustomAttribute<StyleAttribute>();
                this.managers.TryAdd(attribute.Name, () => (IStyle)Activator.CreateInstance(manager));
            }
        }
    }
}