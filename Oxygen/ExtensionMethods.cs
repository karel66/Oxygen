﻿using System.Text;

namespace Oxygen
{
    public static class ExtensionMethods
    {
        public static StringBuilder AppendWithComma(this StringBuilder sb, string text)
        {
            if (sb.Length > 0)
            {
                sb.Append(", ");
            }
            return sb.Append(text);
        }
    }
}
