using System;
using UnityEngine;

namespace Honey
{
    
    public class HoneyRun : PropertyAttribute
    {
       
        public HoneyRun()
        {
            order = Int32.MinValue;
        }
    }
}