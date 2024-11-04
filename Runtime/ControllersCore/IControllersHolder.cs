using System.Collections;
using System.Collections.Generic;
using Kuci.Controllers;
using UnityEngine;

namespace Kuci.Controllers
{
    public interface IControllersHolder
    {
        public T Get<T>() where T : ControllerBase;
    }
}
