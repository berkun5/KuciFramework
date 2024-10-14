using System;
using System.Collections.Generic;
using System.Linq;

namespace Kuci.Core.Extensions
{
    public static class EnumExtensions
    {
        public static HashSet<T> ToHashSet<T>() where T : Enum
        {
            return new HashSet<T>((T[])Enum.GetValues(typeof(T)));
        }
        
        public static T Max<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Max();
        }
        
        public static T Next<T>(this T src, bool loop = false) where T : Enum
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            }

            var arr = (T[])Enum.GetValues(typeof(T));
            var list = arr.ToList();

            // removing negative indices
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr[i].ToInt() < 0)
                {
                    list.RemoveAt(i);
                }
            }
            
            var index = list.IndexOf(src) + 1;
            if (index >= list.Count)
            {
                return loop ? list.FirstOrDefault(e => !e.Equals(list[0])) : list[^1];
            }
            
            return list[index];
        }

        // todo fix previous so it works like next
        public static T Previous<T>(this T src, bool loop = false) where T : Enum
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            }

            var arr = (T[])Enum.GetValues(src.GetType());

            if (arr == null)
            {
                throw new ArgumentNullException(nameof(arr));
            }

            var j = Array.IndexOf(arr, src) - 1;

            if (loop)
            {
                return (j == -1) ? arr[^1] : arr[j];
            }
            else
            {
                return (j == -1) ? arr[0] : arr[j];
            }
        }

        public static bool IsFirstIndex<T>(this T src) where T : Enum
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            }
            
            var arr = (T[])Enum.GetValues(src.GetType());
            var i = Array.IndexOf(arr, src);
            return i == 0;
        }
        public static bool IsLastIndex<T>(this T src) where T : Enum
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException($"Argument {typeof(T).FullName} is not an Enum");
            }
            
            var arr = (T[])Enum.GetValues(src.GetType());
            var i = Array.IndexOf<T>(arr, src) + 1;
            return arr.Length == i;
        }
        
        public static int ToInt<T>(this T src) where T : Enum
        {
            return Convert.ToInt32(src);
        }

        public static T FromInt<T>(int value) where T : Enum
        {
            if (!Enum.IsDefined(typeof(T), value))
            {
                throw new ArgumentException($"No enum with value {value} found in {typeof(T).FullName}");
            }

            return (T)Enum.ToObject(typeof(T), value);
        }
    }
}
