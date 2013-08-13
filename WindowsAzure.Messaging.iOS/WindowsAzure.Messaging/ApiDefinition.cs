using System;
using System.Drawing;

using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace WindowsAzure.Messaging
{
	delegate void ErrorCallback(NSError error);

	[BaseType (typeof(NSObject))]
	interface SBConnectionString {

		// + (NSString*) stringWithEndpoint:(NSURL*)endpoint issuer:(NSString*) issuer issuerSecret:(NSString*)secret;
		[Static, Export("stringWithEndpoint:issuer:issuerSecret:")]
		string CreateUsingSharedSecret (NSUrl endPoint, string issuer, string issuerSecret);

		// + (NSString*) stringWithEndpoint:(NSURL*)endpoint fullAccessSecret:(NSString*)fullAccessSecret;
		[Static, Export("stringWithEndpoint:fullAccessSecret:")]
		string CreateUsingSharedAccessSecretWithFullAccess (NSUrl endPoint, string fullAccessSecret);

		// + (NSString*) stringWithEndpoint:(NSURL*)endpoint listenAccessSecret:(NSString*)listenAccessSecret;
		[Static, Export("stringWithEndpoint:listenAccessSecret:")]
		string CreateUsingSharedAccessSecretWithListenAccess (NSUrl endPoint, string listenAccessSecret);

		// + (NSString*) stringWithEndpoint:(NSURL*)endpoint sharedAccessKeyName:(NSString*)keyName accessSecret:(NSString*)secret;
		[Static, Export("stringWithEndpoint:sharedAccessKeyName:accessSecret:")]
		string CreateUsingSharedAccessSecret (NSUrl endPoint, string keyName, string accessSecret);
	}

	[BaseType (typeof(NSObject))]
	interface SBNotificationHub
	{
		// + (NSString*) version;
		/// <summary>
		/// Returns the API version of this library.
		/// </summary>
		[Static, Export("version")]
		string Version ();

		// - (SBNotificationHub*) initWithConnectionString:(NSString*) connectionString notificationHubPath:(NSString*)notificationHubPath;
		[Export("initWithConnectionString:notificationHubPath:")]
		IntPtr Constructor(string connectionString, string notificationHubPath);

		// Async operations

		//- (void) registerNativeWithDeviceToken:(NSData*)deviceToken tags:(NSSet*)tags completion:(void (^)(NSError* error))completion;
		[Export("registerNativeWithDeviceToken:tags:completion:")]
		void RegisterNativeAsync(NSData deviceToken, [NullAllowed] NSSet tags, ErrorCallback callback);

		//- (void) registerTemplateWithDeviceToken:(NSData*)deviceToken name:(NSString*)name jsonBodyTemplate:(NSString*)bodyTemplate expiryTemplate:(NSString*)expiryTemplate tags:(NSSet*)tags completion:(void (^)(NSError* error))completion;
		[Export("registerTemplateWithDeviceToken:name:jsonBodyTemplate:expiryTemplate:tags:completion:")]
		void RegisterTemplateAsync(NSData deviceToken, string name, string jsonBodyTemplate, string expiryTemplate, [NullAllowed] NSSet tags, ErrorCallback callback);

		//- (void) unregisterNativeWithCompletion:(void (^)(NSError* error))completion;
		[Export("unregisterNativeWithCompletion:")]
		void UnregisterNativeAsync(ErrorCallback callback);

		//- (void) unregisterTemplateWithName:(NSString*)name completion:(void (^)(NSError* error))completion;
		[Export("unregisterTemplateWithName:completion:")]
		void UnregisterTemplateAsync(string name, ErrorCallback callback);

		//- (void) unregisterAllWithDeviceToken:(NSData*)deviceToken completion:(void (^)(NSError* error))completion;
		[Export("unregisterAllWithDeviceToken:completion:")]
		void UnregisterAllAsync(NSData deviceToken, ErrorCallback callback);

		// sync operations
		
		//- (BOOL) registerNativeWithDeviceToken:(NSData*)deviceToken tags:(NSSet*)tags error:(NSError**)error;
		[Export("registerNativeWithDeviceToken:tags:error:")]
		bool RegisterNative(NSData deviceToken, [NullAllowed] NSSet tags, out NSError error);
		
		//- (BOOL) registerTemplateWithDeviceToken:(NSData*)deviceToken name:(NSString*)templateName jsonBodyTemplate:(NSString*)bodyTemplate expiryTemplate:(NSString*)expiryTemplate tags:(NSSet*)tags error:(NSError**)error;
		[Export("registerTemplateWithDeviceToken:name:jsonBodyTemplate:expiryTemplate:tags:error:")]
		bool RegisterTemplate(NSData deviceToken, string name, string jsonBodyTemplate, string expiryTemplate, [NullAllowed] NSSet tags, out NSError error);
		
		//- (BOOL) unregisterNativeWithError:(NSError**)error;
		[Export("unregisterNativeWithError:")]
		bool UnregisterNative(out NSError error);
		
		//- (BOOL) unregisterTemplateWithName:(NSString*)name error:(NSError**)error;
		[Export("unregisterTemplateWithName:error:")]
		bool UnregisterTemplate(string name, out NSError error);
		
		//- (BOOL) unregisterAllWithDeviceToken:(NSData*)deviceToken error:(NSError**)error;
		[Export("unregisterAllWithDeviceToken:error:")]
		bool UnregisterAll(NSData deviceToken, out NSError error);
	}	
}

