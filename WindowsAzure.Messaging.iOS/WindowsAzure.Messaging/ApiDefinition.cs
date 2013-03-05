using System;
using System.Drawing;

using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace WindowsAzure.Messaging
{
	delegate void ReturnToken(string token);
	delegate void GenerateTokenCallback (SBNotificationHubRequestInfo info, ReturnToken generateTokenCallBack);
	delegate void ErrorCallback(NSError error);
	delegate void RegistrationCallback(SBRegistration registration, NSError error);
	delegate void RegistrationsCallback(NSArray registrations, NSError error);
	delegate void BooleanResultCallback(bool result, NSError error);

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
		[Static, Export("version")]
		string Version ();
	
		[Export("initWithConnectionString:notificationHubPath:")]
		IntPtr Constructor(string connectionString, string notificationHubPath);

		[Export("initWithEndpoint:notificationHubPath:generateTokenCallback:")]
		IntPtr Constructor(NSUrl endPoint, string notificationHubPath, GenerateTokenCallback generateTokenCallback);
		//SBNotificationHub InitWithEndpointAsync(NSUrl endpoint, string notificationHubPath, GenerateTokenCallback callback);
	
		[Export("refreshRegistrationsWithDeviceToken:completion:")]
		void RefreshRegistrationsWithDeviceTokenAsync(NSData deviceToken, ErrorCallback callback);

		[Export("createDefaultRegistrationWithTags:completion:")]
		void CreateDefaultRegistrationAsync(NSSet tags, ErrorCallback callback);
	
		[Export("createDefaultRegistrationWithTags:tags:completion:")]
		void CreateDefaultRegistrationAsync(string name, NSSet tags, ErrorCallback callback);

		[Export("createTemplateRegistrationWithName:jsonBodyTemplate:expiryTemplate:tags:completion:")]
		void CreateTemplateRegistrationAsync(string name, string jsonBodyTemplate, [NullAllowed] string expiryTemplate, [NullAllowed] NSSet tags, ErrorCallback callback);

		[Export("retrieveDefaultRegistrationWithCompletion")]
		void RetrieveDefaultRegistrationAsync(RegistrationCallback callback);
			
		[Export("retrieveRegistrationWithName:completion:")]
		void RetrieveRegistrationAsync(string names, RegistrationCallback callback);
			
		[Export("retrieveAllRegistrationsWithCompletion")]
		void RetrieveAllRegistrationsAsync(RegistrationsCallback callback);

		[Export("deleteDefaultRegistrationWithCompletion")]
		void DeleteDefaultRegistrationAsync(ErrorCallback callback);

		[Export("deleteRegistrationWithName:completion:")]
		void DeleteRegistrationAsync(string name, ErrorCallback callback);

		[Export("deleteAllRegistrationsWithCompletion")]
		void DeleteAllRegistrationsAsync(ErrorCallback callback);

		[Export("updateRegistrationWithRegistration:completion:")]
		void UpdateRegistrationAsync(SBRegistration registration, ErrorCallback callback);

		[Export("defaultRegistrationExistsWithCompletion:")]
		void DefaultRegistrationExistsAsync(BooleanResultCallback callback);

		[Export("registrationExistsWithName:completion:")]
		void RegistrationExistsAsync(string name, BooleanResultCallback callback);

		[Export("refreshRegistrationsWithDeviceToken:error:")]
		bool RefreshRegistration(NSData deviceToken, out NSError error);

		[Export("createDefaultRegistrationWithTags:error:")]
		bool CreateDefaultRegistration(NSSet tags, out NSError error);

		[Export("createRegistrationWithName:tags:error:")]
		bool CreateRegistration(string name, NSSet tags, out NSError error);

		[Export("createTemplateRegistrationWithName:jsonBodyTemplate:expiryTemplate:tags:error:")]
		bool CreateTemplateRegistration(string name, string jsonBodyTemplate, [NullAllowed] string expiryTemplate, [NullAllowed] NSSet tags, out NSError error);

		[Export("retrieveDefaultRegistrationWithError:")]
		SBRegistration RetrieveDefaultRegistration(out NSError error);

		[Export("retrieveRegistrationWithName:error:")]
		SBRegistration RetrieveRegistration(string name, out NSError error);

		[Export("retrieveAllRegistrationsWithError:")]
		NSArray RetrieveAllRegistrations(out NSError error);

		[Export("deleteDefaultRegistrationWithError")]
		bool DeleteDefaultRegistration(out NSError error);

		[Export("deleteRegistrationWithName:error:")]
		bool DeleteRegistration(string name, out NSError error);

		[Export("deleteAllRegistrationsWithError:")]
		bool DeleteAllRegistrations(out NSError error);

		[Export("updateRegistrationWithRegistration:error:")]
		bool UpdateRegistration(SBRegistration registration, out NSError error);
	
		[Export("defaultRegistrationExistsWithError:")]
		bool DefaultRegistrationExists(out NSError error);
	
		[Export("registrationExistsWithName:error:")]
		bool RegistrationExists(string name, out NSError error);
	}	

	[BaseType(typeof(NSObject))]
	interface SBNotificationHubRequestInfo
	{
		[Export("requestUri")]
		string RequestUri { get; set; }

		[Export("tags")]
		NSArray Tags { get; set; }

		[Export("operationType")]
		string OperationType { get; set; }

		[Export("notificationHubPath")]
		string NotificationHubPath { get; set; }

		[Export("initWithRequest")]
		SBNotificationHubRequestInfo Init(NSUrlRequest request);

		[Export("jsonString")]
		string JsonString();
	}

	[BaseType(typeof(NSObject))]
	interface SBRegistration
	{
		[Export("expiresAt")]
		NSDate ExpiresAt { get; set; }

		[Export("tags")]
		NSSet Tags { get; set; }

		[Export("deviceToken")]
		string DeviceToken { get; set; }

		[Export("name")]
		string Name { get; set; }

		[Static, Export("DefaultName")]
		string DefaultName();

		[Static, Export("NativeXmlPayloadWithName:deviceToken:")]
		string NativeXmlPayload(string name, string deviceToken);

		[Static, Export("TemplateXmlPayloadWithName:deviceToken:bodyTemplate:expiryTemplate:")]
		string TemplateXmlPayload(string name, string deviceToken, string bodyTemplate, string expiryTemplate);
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

