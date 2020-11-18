using Castle.DynamicProxy;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Element.Core
{
    internal class ViewModelInterceptor : IAsyncInterceptor
    {
        private object proxy;
        private Task delayTask;
        private CancellationTokenSource source;

        public ViewModelInterceptor(object proxy)
        {
            this.proxy = proxy;
        }

        private bool IsAccessSet(IInvocation invocation)
        {
            return invocation.Method.Name.StartsWith("set_");
        }
        public void InterceptAsynchronous(IInvocation invocation)
        {
            if (!IsAccessSet(invocation))
            {
                return;
            }
            throw new NotImplementedException();
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            if (!IsAccessSet(invocation))
            {
                return;
            }
            throw new NotImplementedException();
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            invocation.Proceed();
            if (!IsAccessSet(invocation))
            {
                return;
            }
            var componentBase = proxy as BComponentBase;
            if (delayTask != null)
            {
                if (delayTask.Status == TaskStatus.Running || delayTask.Status == TaskStatus.WaitingForActivation
                    || delayTask.Status == TaskStatus.Created || delayTask.Status == TaskStatus.WaitingForChildrenToComplete
                    || delayTask.Status == TaskStatus.WaitingToRun)
                {
                    source.Cancel();
                }
            }
            source = new CancellationTokenSource();
            delayTask = Task.Delay(500, source.Token).ContinueWith(task =>
            {
                if (task.IsCanceled || source.Token.IsCancellationRequested)
                {
                    return;
                }
                componentBase.allowRefresh = true;
                componentBase.Refresh();
            });
        }
    }
}
