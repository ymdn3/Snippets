﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KOILib.Common
{
    public class Tasks
    {
        public static IEnumerable<Task> From(IEnumerable<Action> actions)
        {
            return actions.Select(x => Task.Run(x));
        }
        
        public static IEnumerable<Task> From(params Action[] actions)
        {
            return From((IEnumerable<Action>)actions);
        }

        public static IEnumerable<Task<T>> From<T>(IEnumerable<Func<T>> functions)
        {
            return functions.Select(x => Task.Run(x));
        }
        
        public static IEnumerable<Task<T>> From<T>(params Func<T>[] functions)
        {
            return From((IEnumerable<Func<T>>)functions);
        }

    }
}
