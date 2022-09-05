using UnityEditor;
using UnityEngine;

namespace Population
{
    public class Decorator : Task
    {
        protected Task Child { get; private set; }

        protected Decorator(Task child)
        {
            this.Child = child;
        }
    }
}