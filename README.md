WindowsAzure.Messaging.iOS
==========================

Xamarin.iOS binding for the Microsoft WindowsAzure.Messaging SDK for iOS

This is a first version that tries to mimick the .NET version of the API, but with the Objective-C library underneath.

Use http://msdn.microsoft.com/en-us/library/windowsazure/jj927169.aspx as a starting point.

To get started with the sample:

- Setup the app with its own app bundle name and sign it with your own Apple certificate
- Setup your own Azure Service Bus Notification Hub and upload the APN .pfx in the Azure portal (see article)
- Use the correct hub address, secret, hub name and registration names in the code

You'll need to run the app on the device in order to test the notifications. This will not work in the Simulator.

For more info on Push Notifications in Xamarin.iOS, see http://roycornelissen.wordpress.com/2011/05/12/push-notifications-in-ios-with-monotouch/

No guarantees :-)
