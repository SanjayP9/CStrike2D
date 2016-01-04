using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStrike2D
{
    static class Console
    {

    }

    struct ConVar<T>
    {
        public T Value { get; private set; }
        public string Command { get; private set; }

        public ConVar(T value, string command)
        {
            Value = value;
            Command = command;
        } 
    }
}
