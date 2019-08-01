
using System.Collections.Generic;
using System.Text;

namespace Loju.Build
{
    public sealed class BuildCompilationDefines
    {

        private HashSet<string> _defines;

        public BuildCompilationDefines()
        {
            _defines = new HashSet<string>();
        }

        public BuildCompilationDefines(string defines) : this()
        {
            AddAll(defines);
        }

        public BuildCompilationDefines Clone()
        {
            BuildCompilationDefines clone = new BuildCompilationDefines();
            foreach (string value in _defines)
            {
                clone._defines.Add(value);
            }

            return clone;
        }

        public void Clear()
        {
            _defines.Clear();
        }

        public void Add(string value)
        {
            if (!_defines.Contains(value)) _defines.Add(value);
        }

        public void AddAll(string defines)
        {
            string[] parts = defines.Split(';');
            int i = 0, l = parts.Length;
            for (; i < l; ++i)
            {
                Add(parts[i]);
            }
        }

        public void AddAll(BuildCompilationDefines defines)
        {
            foreach (string value in defines._defines)
            {
                Add(value);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string value in _defines)
            {
                sb.AppendFormat("{0};", value);
            }

            return sb.ToString();
        }

    }
}