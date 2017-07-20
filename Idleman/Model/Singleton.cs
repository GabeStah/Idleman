using System.Collections.Generic;
using Utility;

namespace Idleman.Model
{
    public sealed class Singleton
    {
        private static readonly Singleton _instance = new Singleton();
        private List<int> Values { get; set; } = new List<int>();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Singleton()
        {
        }

        private Singleton()
        {
        }

        public static Singleton Instance
        {
            get
            {
                return _instance;
            }
        }

        public void AddValue(int value)
        {
            Values.Add(value);
        }

        public void RemoveValue(int value)
        {
            Values.Remove(value);
        }

        public (int index, int value) PopValue()
        {
            if (!Values.IsAny()) return (-1, -1);
            var index = Values.Count - 1;
            var value = Values[index];
            Values.RemoveAt(index);
            return (index, value);
        }

        public void ListValues()
        {
            Logging.Log(Values);
        }
    }
}