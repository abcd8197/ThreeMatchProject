using System;
using System.Collections.Generic;
using CodeJay.Module;

namespace CodeJay
{
    public class Main : IDisposable
    {
        private const string ManagerNotRegisteredError = "Manager of type \"{0}\" is not registered.";

        public static Main Instance => Lazy.Value;
        private static readonly Lazy<Main> Lazy = new(() => new Main());

        private Dictionary<Type, IManager> _managers = new();

        public void RegisterManager<T>(T manager) where T : IManager
        {
            if (manager == null)
                throw new ArgumentNullException(nameof(manager));

            var type = typeof(T);

            if (_managers.ContainsKey(type))
                throw new InvalidOperationException(string.Format(ManagerNotRegisteredError, type.Name));

            _managers[type] = manager;

            TryRegisterModuleToRegistrar(manager);
        }

        public void UnregisterManager<T>() where T : IManager
        {
            var type = typeof(T);

            if (!_managers.ContainsKey(type))
                throw new InvalidOperationException(string.Format(ManagerNotRegisteredError, type.Name));

            _managers[type].Dispose();
            _managers.Remove(type);
        }

        private void TryRegisterModuleToRegistrar(IManager manager)
        {
            if (manager is not IModule module)
                return;

            foreach(var registrarEntry in _managers)
            {
                var registrarType = registrarEntry.Key;
                var registrar = registrarEntry.Value;

                var interfaces = registrarType.GetInterfaces();
                foreach (var iface in interfaces)
                {
                    if (iface.IsGenericType && iface.GetGenericTypeDefinition() == typeof(IModuleRegistrar<>))
                    {
                        var moduleType = iface.GetGenericArguments()[0];
                        if (moduleType.IsAssignableFrom(manager.GetType()))
                        {
                            ((IModuleRegistrar<IModule>)registrar).Register(module);
                            return;
                        }
                    }
                }
            }
        }

        public T Get<T>() where T : IManager, new()
        {
            var type = typeof(T);

            if (!_managers.TryGetValue(type, out var manager))
                throw new InvalidOperationException(string.Format(ManagerNotRegisteredError, type.Name));

            return (T)manager;
        }

        public void Clear()
        {
            foreach (var manager in _managers.Values)
            {
                manager.Dispose();
            }

            _managers.Clear();
        }

        public void Dispose()
        {
            Clear();
        }
    }
}