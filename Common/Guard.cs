using System;

namespace Common
{
    public static class Guard
    {
        public static void ArgumentNotNull<T>(T argument, string argumentName)
        {
            if (argument == null)
                throw new ArgumentNullException(argumentName);
        }
    }
}
