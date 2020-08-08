using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PMeM
{
    class Mp3
    {
        public static uint SND_ASYNC = 0x0001;
        public static uint SND_FILENAME = 0x00020000;
        [DllImport("winmm.dll")]
        public static extern uint mciSendString(string lpstrCommand, string lpstrReturnString, uint uReturnLength, uint hWndCallback);

        public static void Play(string path)
        {
            mciSendString(@"close temp_alias", null, 0, 0);
            mciSendString(string.Format(@"open ""{0}"" alias temp_alias", path), null, 0, 0);
            mciSendString("play temp_alias repeat", null, 0, 0);


            

        }
    }
}
　　


