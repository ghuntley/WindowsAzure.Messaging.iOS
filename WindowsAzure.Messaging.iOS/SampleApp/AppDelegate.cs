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
		private string hubName = "[your hub name]";
		private string hubNamespace = "[your servicebus namespace]";
		private string key = "[your shared listen secret]";

		// TODO: make sure to set the app identifier in the Project Settings > iOS Application!

		// class-level declarations
		UIWindow window;
		SampleAppViewController viewController;

		// This method is invoked when the application has loaded its UI and its ready to run
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			//This tells our app to go ahead and ask the user for permission to use Push Notifications
			// You have to specify which types you want to ask permission for
			// Most apps just ask for them all and if they don't use one type, who cares
			UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Alert);
			
			//The NSDictionary options variable would contain our notification data if the user clicked the 'view' button on the notification
			// to launch the application.  So you could process it here.  I find it nice to have one method to process these options from the
			// FinishedLaunching, as well as the ReceivedRemoteNotification methods.
			processNotification(options, true);

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

			//There's probably a better way to do this
			NSString strFormat = new NSString("%@");
			NSString newDeviceToken = new NSString(MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(new MonoTouch.ObjCRuntime.Class("NSString").Handle, new MonoTouch.ObjCRuntime.Selector("stringWithFormat:").Handle, strFormat.Handle, deviceToken.Handle));
			
			//We only want to send the device token to the server if it hasn't changed since last time
			// no need to incur extra bandwidth by sending the device token every time
			var connectionString = SBConnectionString.CreateUsingSharedAccessSecretWithListenAccess(
				new NSUrl(@"sb://" + hubNamespace + ".servicebus.windows.net"), key);

			this._hub = new SBNotificationHub(connectionString, hubName);

			_hub.RefreshRegistrationsAsync(deviceToken, (error) => {
				if (error == null)
				{
					_hub.TemplateRegistrationExistsAsync("App2000iOSRegistration", (exists, error2) => {
						if (error2 == null)	{
							if (!exists){
								NSSet tags = null; //new NSSet();
								_hub.CreateTemplateRegistrationAsync("App2000iOSRegistration", @"{""aps"": {""alert"": ""$(msg)""}}", @"$(expiryProperty)", tags, (error3) => {
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
			processNotification(userInfo, false);
		}
		
		void processNotification(NSDictionary options, bool fromFinishedLaunching)
		{
			//Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
			if (null != options && options.ContainsKey (new NSString ("aps"))) {
				//Get the aps dictionary
				NSDictionary aps = options.ObjectForKey (new NSString ("aps")) as NSDictionary;
				
				string alert = string.Empty;

				//Extract the alert text
				//NOTE: If you're using the simple alert by just specifying "  aps:{alert:"alert msg here"}  "
				//      this will work fine.  But if you're using a complex alert with Localization keys, etc., your "alert" object from the aps dictionary
				//      will be another NSDictionary... Basically the json gets dumped right into a NSDictionary, so keep that in mind
				if (aps.ContainsKey (new NSString ("alert")))
					alert = (aps [new NSString ("alert")] as NSString).ToString ();

				//If this came from the ReceivedRemoteNotification while the app was running,
				// we of course need to manually process things like the sound, badge, and alert.
				if (!fromFinishedLaunching) {
					//Manually show an alert
					if (!string.IsNullOrEmpty (alert)) {
						UIAlertView avAlert = new UIAlertView ("Notification", alert, null, "OK", null);
						avAlert.Show ();
					}
				}			
			}
		}

	}
}

