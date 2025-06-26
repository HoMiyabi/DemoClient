using System;

namespace KiraraTerminal
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ProgramAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public ProgramAttribute(string name, string description = null)
        {
            Name = name;
            Description = description;
        }
    }
}