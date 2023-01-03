using System;
using System.Collections.Generic;
using System.Text;

namespace RQ.Base.Config
{
    public interface IRQConfig
    {
        string GetUniqueId();
        string Name { get; }
    }
}
