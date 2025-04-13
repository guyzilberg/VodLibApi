using System;
using System.Collections.Generic;
using System.Text;

namespace VodLibCore.Utils
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns a new string with asci letters or digits only
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveSpecialChars(this string str)
        {
            if(string.IsNullOrEmpty(str)) 
                return str;

            string result  = String.Empty;
            foreach(char c in str)
            {
                if (c >= 'A' && c <= 'z' || c >= '0' && c <= '9')
                    result += c;
            }
            return result;
        }
    }
}
