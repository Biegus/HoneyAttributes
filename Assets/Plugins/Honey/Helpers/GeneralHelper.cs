#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using System.Collections;
namespace Honey.Helper
{
    public static class GeneralExtension
    {
        public static T As<T>(this object obj)
            where T:class
        {
            return (T) obj;
        }
    }
}