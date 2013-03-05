using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using WindowsAzure.Messaging;

namespace SampleApp
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		SampleAppViewController viewController;

		string launchWithCustomKeyValue;

		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			//This tells our app to go ahead and ask the user for permission to use Push Notifications
			// You have to specify which types you want to ask permission for
			// Most apps just ask for them all and if they don't use one type, who cares
			UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Alert
			                                                                   | UIRemoteNotificationType.Badge
			                                                                   | UIRemoteNotificationType.Sound);
			
			//The NSDictionary options variable would contain our notification data if the user clicked the 'view' button on the notification
			// to launch the application.  So you could process it here.  I find it nice to have one method to process these options from the
			// FinishedLaunching, as well as the ReceivedRemoteNotification methods.
			processNotification(options, true);
			
			//See if the custom key value variable was set by our notification processing method
			if (!string.IsNullOrEmpty(launchWithCustomKeyValue))
			{
				//Bypass the normal view that shows when launched and go right to something else since the user
				// launched with some custom value (eg: from a remote notification's 'View' button being pressed, or from a url handler)
				
				//TODO: Insert your own logic here to open a view with a direct link to the item the user wants to "View"
			}
			
			window = new UIWindow (UIScreen.MainScreen.Bounds);
			
			viewController = new SampleAppViewController ();
			window.RootViewController = viewController;
			window.MakeKeyAndVisible ();
			
			return true;
		}

		private SBNotificationHub _hub;

		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			//The deviceToken is of interest here, this is what your push notification server needs to send out a notification
			// to the device.  So, most times you'd want to send the device Token to your servers when it has changed
			
			//First, get the last device token we know of
			string lastDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("deviceToken");
			
			//There's probably a better way to do this
			NSString strFormat = new NSString("%@");
			NSString newDeviceToken = new NSString(MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(new MonoTouch.ObjCRuntime.Class("NSString").Handle, new MonoTouch.ObjCRuntime.Selector("stringWithFormat:").Handle, strFormat.Handle, deviceToken.Handle));
			
			//We only want to send the device token to the server if it hasn't changed since last time
			// no need to incur extra bandwidth by sending the device token every time
			//if (!newDeviceToken.Equals(lastDeviceToken))
			{
				var connectionString = SBConnectionString.CreateUsingSharedAccessSecretWithListenAccess(
					new NSUrl(@"sb://xxxxxx.servicebus.windows.net"), "xxxxxxxxx");

				this._hub = new SBNotificationHub(connectionString, "xxxxxx");

				_hub.RefreshRegistrationsWithDeviceTokenAsync(NSData.FromString(newDeviceToken), (error) => {
					if (error == null)
					{
//						NSError err;
//						_hub.DeleteAllRegistrations(out err);
						_hub.RegistrationExistsAsync("myToastRegistration", (exists, error2) => {
							if (error2 == null)	{
								if (!exists){
									var tags = new NSSet("tag");
									_hub.CreateTemplateRegistrationAsync("myToastRegistration", @"{""aps"": {""alert"": ""$(msg)""}}", null, tags, (error3) => {
										if (error3 != null) {
											Console.WriteLine("Error creating template registration: {0}", error3);
										}
									});
								}
							} else { Console.WriteLine("Error checking existence of template registration: {0}", error2); }
						});
					} else { Console.WriteLine("Error refreshing registrations: {0}", error); }
				});
				
				//Save the new device token for next application launch
				NSUserDefaults.StandardUserDefaults.SetString(newDeviceToken, "deviceToken");
			}
		}
		
		public override void FailedToRegisterForRemoteNotifications (UIApplication application, NSError error)
		{
			//Registering for remote notifications failed for some reason
			//This is usually due to your provisioning profiles not being properly setup in your project options
			// or not having the right mobileprovision included on your device
			// or you may not have setup your app's product id to match the mobileprovision you made
			
			Console.WriteLine("Failed to Register for Remote Notifications: {0}", error.LocalizedDescription);
		}

		public override void ReceivedRemoteNotification (UIApplication application, NSDictionary userInfo)
		{
			//This method gets called whenever the app is already running and receives a push notification
			// YOU MUST HANDLE the notifications in this case.  Apple assumes if the app is running, it takes care of everything
			// this includes setting the badge, playing a sound, etc.
			processNotification(userInfo, false);
		}
		
		void processNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			//Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey (new NSString ("aps"))) {
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey (new NSString ("aps")) as NSDictionary;
				
				string alert = string.Empty;
				string sound = string.Empty;
				int badge = -1;
				
				//Extract the alert text
				//NOTE: If you're using the simple alert by just specifying "  aps:{alert:"alert msg here"}  "
				//      this will work fine.  But if you're using a complex alert with Localization keys, etc., your "alert" object from the aps dictionary
				//      will be another NSDictionary... Basically the json gets dumped right into a NSDictionary, so keep that in mind
				if (aps.ContainsKey (new NSString ("alert")))
					alert = (aps [new NSString ("alert")] as NSString).ToString ();
				
				//Extract the sound string
				if (aps.ContainsKey (new NSString ("sound")))
					sound = (aps [new NSString ("sound")] as NSString).ToString ();
				
				//Extract the badge
				if (aps.ContainsKey (new NSString ("badge"))) {
					string badgeStr = (aps [new NSString ("badge")] as NSObject).ToString ();
					int.TryParse (badgeStr, out badge);
				}
				
				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching) {
					//Manually set the badge in case this came from a remote notification sent while the app was open
					if (badge >= 0)
						UIApplication.SharedApplication.ApplicationIconBadgeNumber = badge;
					
					//Manually play the sound
					if (!string.IsNullOrEmpty (sound)) {
						//This assumes that in your json payload you sent the sound filename (like sound.caf)
						// and that you've included it in your project directory as a Content Build type.
						var soundObj = MonoTouch.AudioToolbox.SystemSound.FromFile (sound);
						soundObj.PlaySystemSound ();
					}
					
					//Manually show an alert
					if (!string.IsNullOrEmpty (alert)) {
						UIAlertView avAlert = new UIAlertView ("Notification", alert, null, "OK", null);
						avAlert.Show ();
					}
				}
			
				//You can also get the custom key/value pairs you may have sent in your aps (outside of the aps payload in the json)
				// This could be something like the ID of a new message that a user has seen, so you'd find the ID here and then skip displaying
				// the usual screen that shows up when the app is started, and go right to viewing the message, or something like that.
				if (options.ContainsKey (new NSString ("customKeyHere"))) {
					launchWithCustomKeyValue = (options [new NSString ("customKeyHere")] as NSString).ToString ();
				
					//You could do something with your customData that was passed in here
				}
			}
		}

	}
}

