using Castle.DynamicProxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Element.Core
{
    internal class ComponentInterceptor : IAsyncInterceptor
    {
        private ViewModelCache modelCache;

        public ComponentInterceptor(ViewModelCache cache)
        {
            this.modelCache = cache;
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
        }
        private async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            invocation.Proceed();
            var task = (Task)invocation.ReturnValue;
            await task;
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            throw new NotImplementedException();
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            if (!new[] { "StateHasChanged", "Refresh", "ShouldRender", "get_ViewModel" }.Contains(invocation.Method.Name))
            {
                invocation.Proceed();
                return;
            }
            if (typeof(BComponentBase).IsAssignableFrom(invocation.Method.DeclaringType))
            {
                if (invocation.InvocationTarget is BComponentBase componentBase)
                {
                    if (componentBase.allowRefresh)
                    {
                        componentBase.allowRefresh = false;
                        invocation.Proceed();
                        return;
                    }
                }
                if (invocation.Method.Name == "ShouldRender")
                {
                    invocation.Proceed();
                    return;
                }
                if (invocation.Method.Name is "StateHasChanged" or "Refresh")
                {
                    return;
                }
            }
            invocation.Proceed();
            if (invocation.Method.Name == "get_ViewModel")
            {
                //modelCache.TryGetValue(invocation.TargetType, out var model);
                //if (model != null)
                //{
                //    invocation.ReturnValue = model;
                //    return;
                //}
                ProxyGenerator generator = new ProxyGenerator();

                var intercept = new ViewModelInterceptor(invocation.Proxy);
                invocation.ReturnValue = generator.CreateClassProxy(invocation.Method.ReturnType, intercept);
                //modelCache.TryAdd(invocation.TargetType, invocation.ReturnValue);
                return;
            }
        }
    }
}
