#region Imports
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

using AddressBook;
using Foundation;
using CoreMedia;
using CoreText;
using UIKit;
using CoreImage;
using CoreGraphics;
using ObjCRuntime;
using EventKit;
using StoreKit;
using SystemConfiguration;
using CoreFoundation;
using CoreAnimation;
using MediaPlayer;
using AVFoundation;
using Social;
using AudioUnit;
using MonoTouch.Dialog;
using MessageUI;
using ExternalAccessory;
using JavaScriptCore;
using MapKit;
using SpriteKit;
using OpenTK;

using NUnit.Framework;
#endregion

[Register ("AppDelegate")]
public partial class AppDelegate : UIApplicationDelegate
{
	UIWindow window;
	DialogViewController dvc;
	GlassButton misc;
	GlassButton run;

	[DllImport ("libc")]
	public extern static void exit (int status);

	/*async */void TickOnce ()
	{
		var types = new List<Type> ();
		foreach (var type in GetType ().Assembly.GetTypes ()) {
			if (type.IsSubclassOf (typeof (NSObject)))
				types.Add (type);
		}

		const int n = 10;
		var threads = new Thread [n];
		var cntr = new int [n];
		Exception ex = null;
		var startPistol = new ManualResetEvent (false);
		var stopLine = new CountdownEvent (n);
		for (int i = 0; i < n; i++) {
			var idx = i;
			threads [i] = new Thread (() => {
				startPistol.WaitOne ();
				try {
					foreach (var type in types) {
						var c = Class.GetHandle (type.Name);
						if (c != IntPtr.Zero) {
							try {
								Class.Lookup (new Class (c));
							} catch (Exception e) {
								ex = e;
								return;
							}
							cntr [idx]++;
						}
					}
				} finally {
					stopLine.Signal ();
				}
			});
			threads [i].IsBackground = true;
			threads [i].Start ();
		}
		startPistol.Set ();
		stopLine.Wait ();

		dvc.TableView.BackgroundColor = ex == null ? UIColor.Green : UIColor.Red;
	}

	void MiscTapped ()
	{
	}
	
	public override bool FinishedLaunching (UIApplication app, NSDictionary options)
	{
		window = new UIWindow (UIScreen.MainScreen.Bounds);
//		var rect = new CGRect (0, 0, window.Bounds.Width, 100);

		NSTimer.CreateScheduledTimer (0.1, (v) => TickOnce ());

		dvc = new DialogViewController (
			new RootElement ("Root")
			{
//				new Section ("Main") {
//					(misc = new GlassButton (rect)),
//					(run = new GlassButton (rect)),
//				},
			}
		);
		dvc.Autorotate = true;
//
//		if (misc != null) {
//			misc.SetTitle ("Click MEEE!", UIControlState.Normal);
//			misc.Tapped += (obj) => 
//			{
//				try {
//					MiscTapped ();
//				} catch (Exception e) {
//					Console.WriteLine (e);
//				}
//			};
//		}
//		if (run != null) {
//			run.SetTitle ("Run tests", UIControlState.Normal);
//			run.Tapped += (obj) => {
//				var builder = new NUnit.Framework.Internal.NUnitLiteTestAssemblyBuilder ();
//				var suite = builder.Build (typeof (AppDelegate).Assembly, new Dictionary<string, object> ());
//				var wi = suite.CreateWorkItem (NUnit.Framework.Internal.TestFilter.Empty);
//				wi.Execute (NUnit.Framework.Internal.TestExecutionContext.CurrentContext);
//			};
//		}
		window.RootViewController = dvc;
		window.MakeKeyAndVisible ();

		return true;
	}
}
