using System;

namespace Honey.Core
{
    [Flags]
    public enum HoneyValueParseFlags
    {
        None=0,
        IntegerMode=1<<0,
        StringMode=1<<1,
        ReferenceFlag=1<<2,
      
    }
}