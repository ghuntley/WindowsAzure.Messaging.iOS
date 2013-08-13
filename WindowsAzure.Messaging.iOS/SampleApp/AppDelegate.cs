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
		private string hubName = "[your hub name here]";
		private string hubNamespace = "[your namespace here]";
		private string key = "[your shared listen access key here]";

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
			var connectionString = SBConnectionString.CreateUsingSharedAccessSecretWithListenAccess(
				new NSUrl(@"sb://" + hubNamespace + ".servicebus.windows.net"), key);

			this._hub = new SBNotificationHub(connectionString, hubName);

			_hub.UnregisterAllAsync (deviceToken, (error) => {
				if (error != null) {
					Console.WriteLine("Error calling Unregister: {0}", error.ToString());
				} else {
					NSSet tags = null; // create tags if you want
					_hub.RegisterTemplateAsync(deviceToken, "App2000iOSRegistration", @"{""aps"": {""alert"": ""$(msg)""}}", @"$(expiryProperty)", tags, (registrationError) => {
						if (registrationError != null) {
							Console.WriteLine("Error calling RegisterTemplate: {0}", registrationError.ToString());
						}
					});
				}
			});
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

