using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KiraraTerminal
{
    public class Shell
    {
        private static readonly char[] sep = {' ', '\t'};

        private readonly List<string> inputs = new() {""};
        private int idx;

        private static bool initialized;
        public static readonly Dictionary<string, ProgramInfo> programs = new();

        public Shell()
        {
            Init();
        }

        public static void Init()
        {
            if (initialized) return;
            initialized = true;
            var assembly = typeof(BuiltinProgram).Assembly;
            foreach (var type in assembly.GetTypes())
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                              BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute<ProgramAttribute>();
                    if (attribute == null) continue;

                    if (!programs.TryAdd(attribute.Name, new ProgramInfo(attribute, method)))
                    {
                        Debug.LogError($"Shell程序 {attribute.Name} 重复");
                    }
                }
            }
        }

        public string Current
        {
            get => inputs[idx];
            set
            {
                if (inputs[idx] != value)
                {
                    if (idx < inputs.Count - 1)
                    {
                        idx = inputs.Count - 1;
                    }
                    inputs[idx] = value;
                }
            }
        }

        public void MoveNext()
        {
            if (idx < inputs.Count - 1)
            {
                idx++;
            }
        }

        public void MovePrev()
        {
            if (idx > 0)
            {
                idx--;
            }
        }

        public void Execute()
        {
            if (string.IsNullOrEmpty(Current)) return;
            if (string.IsNullOrWhiteSpace(Current)) return;

            string[] tokens = Current.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "\"\"")
                {
                    tokens[i] = "";
                }
            }
            if (tokens.Length >= 1)
            {
                if (programs.TryGetValue(tokens[0], out var program))
                {
                    if (tokens.Length == 2 && (tokens[1] == "-h" || tokens[1] == "--help"))
                    {
                        string s = $"名字: {program.name}, 描述: {program.description}, 参数：";
                        var parameters = program.method.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            s += $"{parameters[i].Name}: {parameters[i].ParameterType.Name} ";
                            if (i < parameters.Length - 1)
                            {
                                s += ", ";
                            }
                        }
                        Debug.Log(s);
                    }
                    else
                    {
                        program.method.Invoke(null, tokens.Skip(1).Select(it => (object)it).ToArray());
                    }
                }
                else
                {
                    Debug.LogError($"Shell命令 {tokens[0]} 不存在");
                }
            }
            inputs.Add("");
            idx = inputs.Count - 1;
        }
    }
}