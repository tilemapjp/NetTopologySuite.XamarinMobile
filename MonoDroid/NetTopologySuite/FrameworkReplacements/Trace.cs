﻿
#if SILVERLIGHT || __ANDROID__ || __IOS__
namespace System.Diagnostics
{
    public static class Trace
    {
        public static void WriteLine(string message)
        {
            Debug.WriteLine(message);
        }

        internal static void Write(string message)
        {
            WriteLine(message);
        }
    }
}
#endif