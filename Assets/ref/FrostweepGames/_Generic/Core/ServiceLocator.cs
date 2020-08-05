using System;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.Core
{
    public class ServiceLocator : IDisposable
    {
		private static ServiceLocator _Instance;
		public static ServiceLocator Instance
		{
			get
			{
				if(_Instance == null)
				{
					_Instance = new ServiceLocator();
				}

				return _Instance;
			}
		}

		private Dictionary<Type, IService> _services;

        internal ServiceLocator()
        {
            _services = new Dictionary<Type, IService>();      
        }

        public static void InitServices()
        {
            foreach (var service in Instance._services)
                service.Value.Init();
        }

        public void Update()
        {
            foreach (var service in _services)
                service.Value.Update();
        }

        public void Dispose()
        {
            foreach (var service in _services)
                service.Value.Dispose();
            _Instance = null;
        }

        public static T Get<T>()
        {
            if (Instance._services.ContainsKey(typeof(T)))
                return (T)Instance._services[typeof(T)];
            else
                throw new NotImplementedException(typeof(T) + " not implemented!");
        }

        public static void Register<T>(IService service)
        {
			Instance._services.Add(typeof(T), service);
        }
    }
}