using System.Reflection;

namespace KiraraTerminal
{
    public struct ProgramInfo
    {
        public readonly string name;
        public readonly string description;
        public readonly MethodInfo method;

        public ProgramInfo(ProgramAttribute attribute, MethodInfo method)
        {
            name = attribute.Name;
            description = attribute.Description;
            this.method = method;
        }
    }
}