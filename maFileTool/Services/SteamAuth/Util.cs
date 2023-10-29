using System;
using System.Threading;

namespace maFileTool.Services.SteamAuth
{
    public class Util
    {
        public static long GetSystemUnixTime()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }

        public static void UsePreCompiling(bool param)
        {
            if (param)
                Thread.Sleep(7000);
        }
    }
}
