using Castle.DynamicProxy;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element.Core
{
    internal class ElementComponentActivator : IComponentActivator
    {
        private readonly ViewModelCache modelCache;
        private readonly ComponentCache componentCache;

        public ElementComponentActivator(ViewModelCache modelCache, ComponentCache componentCache)
        {
            this.modelCache = modelCache;
            this.componentCache = componentCache;
        }
        public IComponent CreateInstance(Type componentType)
        {
            //componentCache.TryGetValue(componentType, out var comopnent);
            //if (comopnent != null)
            //{
            //    return (IComponent)comopnent;
            //}
            ProxyGenerator generator = new ProxyGenerator();

            var intercept = new ComponentInterceptor(modelCache);
            var comopnent = generator.CreateClassProxy(componentType, intercept);
            //componentCache.TryAdd(componentType, comopnent);
            return (IComponent)comopnent;
        }
    }
}
