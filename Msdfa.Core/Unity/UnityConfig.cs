using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Msdfa.Core.Unity
{
    public static class UnityConfig
    {
        public static IUnityContainer Container;
        //public static void RegisterComponents()
        //{
        //    //container.RegisterType<IAccessLogBLL, AccessLogBLL>();
        //    //DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        //}

        static UnityConfig()
        {
            Container = new UnityContainer();
        }
    }
}
