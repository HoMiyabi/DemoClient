using System.Text.RegularExpressions;

namespace KiraraDirectBinder.Editor
{
    public static class VarName
    {
        private static readonly Regex _varNameRegex = new("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

        public static bool IsValid(string name)
        {
            return _varNameRegex.IsMatch(name);
        }

        private static readonly Regex validVarNameRegex = new("[ ()]", RegexOptions.Compiled);

        public static string ReplaceToValid(string text)
        {
            return validVarNameRegex.Replace(text, "");
        }
    }
}