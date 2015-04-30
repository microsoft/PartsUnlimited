using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using Microsoft.Practices.Unity;

namespace PartsUnlimited.Utils
{
    public class SignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IUnityContainer inner = Global.UnityContainer.CreateChildContainer();

        public override object GetService(Type serviceType)
        {
            try
            {
                return inner.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return base.GetService(serviceType);
            }
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return inner.ResolveAll(serviceType).Concat(base.GetServices(serviceType));
        }
    }
}