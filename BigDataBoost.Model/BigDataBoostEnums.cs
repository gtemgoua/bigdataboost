using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BigDataBoost.Model
{
    public enum TagStatus
    {
        Good = 0,
        Offline = 1,
        Uninitialized = 2,
        Error = 3,
        Questionable = 4
    }
}
