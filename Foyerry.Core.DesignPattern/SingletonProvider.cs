using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Foyerry.Core.DesignPattern
{
    public class SingletonProvider<T> where T : new()
    {
        private SingletonProvider()
        {
        }
        public static T Instance
        {
            get { return SingletonCreator.instance; }
        }

        private class SingletonCreator
        {
            static SingletonCreator() { }
            internal static readonly T instance = new T();
        }
    }
}
