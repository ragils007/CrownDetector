using System;
using System.Collections.Generic;
using System.Text;
using Unity;

namespace Msdfa.Core.Unity
{
    public interface IConfigureUnity
    {
        public void Configure(UnityContainer container);
    }
}
