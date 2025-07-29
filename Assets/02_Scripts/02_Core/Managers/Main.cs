using System;
using System.Collections.Generic;

namespace CodeJay
{
    public class Main : IDisposable
    {
        private const string ManagerNotRegisteredError = "Manager of type \"{0}\" is not registered.";

        public Main Instance => Lazy.Value;
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
        }

        public void UnregisterManager<T>() where T : IManager
        {
            var type = typeof(T);

            if (!_managers.ContainsKey(type))
                throw new InvalidOperationException(string.Format(ManagerNotRegisteredError, type.Name));

            _managers[type].Dispose();
            _managers.Remove(type);
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