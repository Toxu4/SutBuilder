using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;

namespace SutBuilder
{
    public abstract class AbstractSutBuilder<T>  where T : class
    {
        private Dictionary<Type, object> _stubs;
        
        private readonly Action<AbstractSutBuilder<T>> _defautConfig;
        
        private MethodInfo _createStubMethodInfo;
        private MethodInfo CreateStubMethodInfo => 
            _createStubMethodInfo ?? 
            (
                _createStubMethodInfo = GetType()
                   .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                       .First(mi => mi.Name == nameof(CreateStub)));
        
        protected abstract TStub CreateStub<TStub>() where TStub: class;

        protected AbstractSutBuilder(Action<AbstractSutBuilder<T>> defaultConfig)
        {
            _defautConfig = defaultConfig;
            
            CreateStubs();
        }
                        
        public AbstractSutBuilder<T> Inject(params object[] dependencies)
        {
            if (dependencies?.Length > 0)
            {
                foreach (var dependency in dependencies)
                {
                    var dependencyType = dependency.GetType();

                    var stubKey = dependencyType
                        .GetInterfaces()
                        .Union(new[] {dependencyType})
                        .First(i => _stubs.ContainsKey(i)); 
                    
                    _stubs[stubKey] = dependency;
                }                
            }
            
            return this;
        }

        public AbstractSutBuilder<T> Configure<TStub>(Action<TStub> configurationAction) where TStub : class 
        {
            configurationAction((TStub)_stubs[typeof(TStub)]);
            
            return this;
        }

        public TStub Get<TStub>() where TStub : class
        {
            return (TStub) _stubs[typeof(TStub)];
        }
        
        public T Build()
        {
            var serviceProvider = CreateServiceProvider();
            
            return serviceProvider.GetService<T>();
        }

        public void Reset() 
        {
            CreateStubs();
        }               

        private IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();
            
            services.AddSingleton<T>();
                      
            foreach (var stub in _stubs)
            {              
                services.AddSingleton(stub.Key, stub.Value);
            }
            
            return services.BuildServiceProvider();
        }

        private void CreateStubs()
        {
            _stubs = typeof(T)
                .GetConstructors().SelectMany(c => c.GetParameters())
                .GroupBy(p => p.ParameterType)
                .Select(g => 
                    new KeyValuePair<Type, object>(
                        g.Key, 
                        CreateStubSafety(g.Key)))
                .ToDictionary(
                    kv => kv.Key,
                    kv => kv.Value);
            
            _defautConfig?.Invoke(this);
            
            object CreateStubSafety(Type dependencyType)
            {
                try
                {
                    return CreateStubMethodInfo
                        .MakeGenericMethod(dependencyType)
                        .Invoke(this, new object[] {});
                }
                catch
                {
                    return dependencyType.IsValueType 
                        ? Activator.CreateInstance(dependencyType) 
                        : null;
                }
            }           
        }        
    }
}