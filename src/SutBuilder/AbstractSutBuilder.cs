using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace SutBuilder
{
    public abstract class AbstractSutBuilder<T>  where T : class
    {
        private Dictionary<Type, object> _stubs;
        
        protected abstract TStub CreateStub<TStub>() where TStub: class;

        protected AbstractSutBuilder()
        {
            _stubs = CreateStubs();
        }

        public AbstractSutBuilder<T> Reset() 
        {
            _stubs = CreateStubs();
            
            return this;
        }
        
        public AbstractSutBuilder<T> Inject<TStub>(TStub dependency) where TStub : class
        {
            _stubs[typeof(TStub)] = dependency;
            
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

        private Dictionary<Type, object> CreateStubs()
        {
            var createStubMethodInfo = GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(mi => mi.Name == nameof(CreateStub));
            
            return typeof(T)
                .GetConstructors().SelectMany(c => c.GetParameters())
                .GroupBy(p => p.ParameterType)
                .ToDictionary(
                    g => g.Key,
                    g => createStubMethodInfo
                        .MakeGenericMethod(g.Key)
                        .Invoke(this, new object[] {}));
        }        
    }
}