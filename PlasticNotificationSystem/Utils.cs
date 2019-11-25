using System;
using System.Collections.Generic;
using System.Text;

namespace PlasticNotificationSystem
{
    public static class Utils
    {
        public static string ChopString(this string inputString, int length, string append = " ...")
        {
            if(inputString.Length > length)
            {
                return inputString.Remove(length - append.Length - 1) + append;
            }
            return inputString;
        }

        public static string SubChopString(this string inputString, int maxLength, string append ="... ")
        {
            if(inputString.Length < maxLength)
            {
                return inputString;
            }
            if(append.Length >= maxLength)
            {
                append = "";
            }            
            return append + inputString.Substring(inputString.Length - maxLength + append.Length);
        }
    }
}
