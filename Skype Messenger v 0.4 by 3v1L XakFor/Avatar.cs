using System.Drawing;
using System.IO;
using System.Threading;
using SKYPE4COMLib;

namespace Skype_Messenger_v_0._4_by_3v1L_XakFor
{
    internal class Avatar
    {
        private readonly Skype s;

        public Avatar(Skype S)
        {
            s = S;
        }

        public Bitmap grabAvatarAPI(string handle)
            //Saves users image from Skype using the API, then returns it as bitmap
        {
            var c = new Command();
            File.Delete(Path.GetTempPath() + handle + ".png");
            c.Command = string.Format("GET USER {0} AVATAR 1 {1}", handle, Path.GetTempPath() + handle + ".png");
            s.SendCommand(c);
            Thread.Sleep(100); //THIS IS MANDATORY
            return new Bitmap(Path.GetTempPath() + handle + ".png");
        }

        public string grabAvatarOnline(string handle)
            //Downloads it off Skypes website. You get the profile picture as if you aren't a contact. returns string
        {
            return "http://api.skype.com/users/" + handle + "/profile/avatar";
        }
    }
}