#if UNITY_EDITOR
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Honey.Helper;
using static System.String;

namespace Honey.Editor
{
    public class FolderPath : IComparable<FolderPath>
    {
     
      

        private readonly string path;
        private readonly string[] elements;
        public FolderPath(string path)
        {
            this.path = path;
            elements = path.Split('/').Where(item => item != Empty).ToArray();
        }

        public IReadOnlyList<string> Elements => elements;
        public ReadOnlySpan<char> GetFather()
        {
            if (Elements.Count <=1)
            {
                return string.Empty;
            }
            int indx = path.LastIndexOf('/');
            return this.path[..indx];

        }
        public static bool operator ==(FolderPath? a, FolderPath? b)
        {
            bool aNull = object.ReferenceEquals(a, null);
            bool bNull = object.ReferenceEquals(b, null);
            if (aNull|| bNull) return !(aNull ^ bNull);

            return a!.path == b!.path;
        }

        public static bool operator !=(FolderPath? a, FolderPath? b)
        {
            return !(a == b);
        }
        
        public int CompareTo(FolderPath y)
        {
            FolderPath x = this;
            if (x == y) 
                return 0;
            if (x.path == y.GetFather()) 
                return -1;
            if (x.GetFather() == y.path)
                return 1;
            int firstIncorrectIndex = EnumerableHelper.GetFirstIncorrectIndex(x.elements, y.elements);
            if (firstIncorrectIndex == Math.Min(x.elements.Length, y.elements.Length))
            {
                return x.Elements.Count - y.Elements.Count;
            }

            return 0;

        }
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj is FolderPath other)
            {
                return path == other.path && elements.Equals(other.elements);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(path, elements);
        }

        public override string ToString()
        {
            return this.path;
        }
    }
}
#endif