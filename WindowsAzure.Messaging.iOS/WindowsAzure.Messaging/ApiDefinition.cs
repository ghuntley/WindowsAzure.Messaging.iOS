using System;
using System.Drawing;

using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace WindowsAzure.Messaging
{
	delegate void ReturnToken(string token);
	delegate void ErrorCallback(NSError error);
	delegate void RegistrationCallback(SBRegistration registration, NSError error);
	delegate void RegistrationsCallback(NSArray registrations, NSError error);
	delegate void BooleanResultCallback(bool result, NSError error);
	delegate void TemplateRegistrationCallback(SBTemplateRegistration registration, NSError error);

	[BaseType (typeof(NSObject))]
	interface SBConnectionString {
		[Static, Export("stringWithEndpoint:issuer:issuerSecret:")]
		string CreateUsingSharedSecret (NSUrl endPoint, string issuer, string issuerSecret);

		[Static, Export("stringWithEndpoint:fullAccessSecret:")]
		string CreateUsingSharedAccessSecretWithFullAccess (NSUrl endPoint, string fullAccessSecret);

		[Static, Export("stringWithEndpoint:listenAccessSecret:")]
		string CreateUsingSharedAccessSecretWithListenAccess (NSUrl endPoint, string listenAccessSecret);

		[Static, Export("stringWithEndpoint:sharedAccessKeyName:accessSecret:")]
		string CreateUsingSharedAccessSecret (NSUrl endPoint, string keyName, string accessSecret);
	}

	[BaseType (typeof(NSObject))]
	interface SBNotificationHub
	{
		/// <summary>
		/// Returns the API version of this library.
		/// </summary>
		[Static, Export("version")]
		string Version ();

		[Export("initWithConnectionString:notificationHubPath:")]
		IntPtr Constructor(string connectionString, string notificationHubPath);

		[Export("createTemplateRegistrationWithName:jsonBodyTemplate:expiryTemplate:tags:completion:")]
		void CreateTemplateRegistrationAsync(string templateName, string jsonBodyTemplate, string expiryTemplate, [NullAllowed] NSSet tags, ErrorCallback callback);

		[Export("refreshRegistrationsWithDeviceToken:completion:")]
		void RefreshRegistrationsAsync(NSData deviceToken, ErrorCallback completion);

		/// <summary>
		/// Creates a native registration with tags.
		/// </summary>
		/// <param name="tags">Tags.</param>
		/// <param name="callback">Callback.</param>
		[Export("createNativeRegistrationWithTags:completion:")]
		void CreateNativeRegistrationAsync([NullAllowed] NSSet tags, ErrorCallback callback);

		[Export("retrieveNativeRegistrationWithCompletion:")]
		void RetrieveNativeRegistrationAsync(RegistrationCallback callback);

		[Export("retrieveTemplateRegistrationWithName:completion:")]
		void RetrieveTemplateRegistrationAsync(string templateName, TemplateRegistrationCallback callback);

		[Export("retrieveAllRegistrationsWithCompletion:")]
		void RetrieveAllRegistrationsAsync(RegistrationsCallback callback);

		[Export("deleteNativeRegistrationWithCompletion:")]
		void DeleteNativeRegistrationAsync(ErrorCallback callback);

		[Export("deleteTemplateRegistrationWithName:completion:")]
		void DeleteTemplateRegistrationAsync(string templateName, ErrorCallback callback);

		[Export("deleteAllRegistrationsWithCompletion:")]
		void DeleteAllRegistrationsAsync(ErrorCallback callback);

		[Export("updateRegistrationWithRegistration:completion:")]
		void UpdateRegistrationAsync(SBRegistration registration, ErrorCallback callback);

		[Export("nativeRegistrationExistsWithCompletion:")]
		void NativeRegistrationExistsAsync(BooleanResultCallback callback);

		[Export("templateRegistrationExistsWithName:completion:")]
		void TemplateRegistrationExistsAsync(string templateName, BooleanResultCallback callback);

		// sync operations

		[Export("refreshRegistrationsWithDeviceToken:error:")]
		bool RefreshRegistrations(NSData deviceToken, out NSError error);

		[Export("createNativeRegistrationWithTags:error:")]
		bool CreateNativeRegistration([NullAllowed] NSSet tags, out NSError error);

		[Export("createTemplateRegistrationWithName:jsonBodyTemplate:expiryTemplate:tags:error:")]
		bool CreateTemplateRegistration(string templateName, string jsonBodyTemplate, string expiryTemplate, [NullAllowed] NSSet tags, out NSError error);

		[Export("retrieveNativeRegistrationWithError:")]
		SBRegistration RetrieveNativeRegistration(out NSError error);

		[Export("retrieveTemplateRegistrationWithName:error:")]
		SBTemplateRegistration RetrieveTemplateRegistration(string templateName, out NSError error);

		[Export("retrieveAllRegistrationsWithError:")]
		NSArray RetrieveAllRegistrations(out NSError error);

		[Export("deleteNativeRegistrationWithError:")]
		bool DeleteNativeRegistration(out NSError error);

		[Export("deleteTemplateRegistrationWithName:error:")]
		bool DeleteTemplateRegistration(string templateName, out NSError error);

		[Export("deleteAllRegistrationsWithError:")]
		bool DeleteAllRegistrations(out NSError error);

		[Export("updateRegistrationWithRegistration:error:")]
		bool UpdateRegistration(SBRegistration registration, out NSError errror);

		[Export("nativeRegistrationExistsWithError:")]
		bool NativeRegistrationExists(out NSError error);

		[Export("templateRegistrationExistsWithName:error:")]
		bool TemplateRegistrationExists(string templateName, out NSError error);

	}	
	
	[BaseType(typeof(NSObject))]
	interface SBRegistration
	{
		[Export("ETag")]
		string ETag { get; set; }

		[Export("expiresAt")]
		NSDate ExpiresAt { get; set; }

		[Export("tags")]
		NSSet Tags { get; set; }

		[Export("registrationId")]
		string RegistrationId { get; set; }

		[Export("deviceToken")]
		string DeviceToken { get; set; }

		[Static, Export("Name")]
		string Name();

		[Static, Export("PayloadWithDeviceToken:tags:")]
		string Payload(string deviceToken, [NullAllowed] NSSet tags);
	}

	[BaseType(typeof(SBRegistration))]
	interface SBTemplateRegistration
	{
		[Export("bodyTemplate")]
		string BodyTemplate { get; set; }

		[Export("expiry")]
		string Expiry { get; set; }
	}
}

