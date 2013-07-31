using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using MonoTouch;
using Foundation;
using UIKit;

namespace instruments
{
	public class Application {
	    static void Main (string[] args)
		{
 			UIApplication.Main (args, null, "AppDelegate");
	    }
	}
}
