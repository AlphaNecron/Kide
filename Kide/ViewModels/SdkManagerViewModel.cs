using System.Collections.Generic;
using Kide.Models;

namespace Kide.ViewModels
{
    public class SdkManagerViewModel : ViewModelBase
    {
        public Dictionary<string, PlatformSdk> DownloadLinks => new()
        {
            /*{
                "OSX", new PlatformSdk
                { }
            },*/
            {
                "WinNT", new PlatformSdk
                {
                    X64 = "https://mirror.freemirror.org/pub/fpc/dist/3.2.2/i386-win32/fpc-3.2.2.i386-win32.cross.x86_64-win64.exe",
                    X86 = "https://mirror.freemirror.org/pub/fpc/dist/3.2.2/i386-win32/fpc-3.2.2.i386-win32.exe"
                }
            },
            {
                "Linux", new PlatformSdk
                {
                    X64 = "https://mirror.freemirror.org/pub/fpc/dist/3.2.2/x86_64-linux/fpc-3.2.2.x86_64-linux.tar",
                    X86 = "https://mirror.freemirror.org/pub/fpc/dist/3.2.2/i386-linux/fpc-3.2.2.i386-linux.cross.x86_64-linux.tar"
                }
            }
        };
    }
}