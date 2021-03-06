﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DodoOpenId.OpenId {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class OpenIdStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal OpenIdStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DodoOpenId.OpenIdStrings", typeof(OpenIdStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An absolute URI is required for this value..
        /// </summary>
        internal static string AbsoluteUriRequired {
            get {
                return ResourceManager.GetString("AbsoluteUriRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This is already a PPID Identifier..
        /// </summary>
        internal static string ArgumentIsPpidIdentifier {
            get {
                return ResourceManager.GetString("ArgumentIsPpidIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The requested association type &apos;{0}&apos; with session type &apos;{1}&apos; is unrecognized or not supported by this Provider due to security requirements..
        /// </summary>
        internal static string AssociationOrSessionTypeUnrecognizedOrNotSupported {
            get {
                return ResourceManager.GetString("AssociationOrSessionTypeUnrecognizedOrNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The length of the shared secret ({0}) does not match the length required by the association type (&apos;{1}&apos;)..
        /// </summary>
        internal static string AssociationSecretAndTypeLengthMismatch {
            get {
                return ResourceManager.GetString("AssociationSecretAndTypeLengthMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The length of the encrypted shared secret ({0}) does not match the length of the hashing algorithm ({1})..
        /// </summary>
        internal static string AssociationSecretHashLengthMismatch {
            get {
                return ResourceManager.GetString("AssociationSecretHashLengthMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No association store has been given but is required for the current configuration..
        /// </summary>
        internal static string AssociationStoreRequired {
            get {
                return ResourceManager.GetString("AssociationStoreRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If an association store is given, a nonce store must also be provided..
        /// </summary>
        internal static string AssociationStoreRequiresNonceStore {
            get {
                return ResourceManager.GetString("AssociationStoreRequiresNonceStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An attribute with type URI &apos;{0}&apos; has already been added..
        /// </summary>
        internal static string AttributeAlreadyAdded {
            get {
                return ResourceManager.GetString("AttributeAlreadyAdded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only {0} values for attribute &apos;{1}&apos; were requested, but {2} were supplied..
        /// </summary>
        internal static string AttributeTooManyValues {
            get {
                return ResourceManager.GetString("AttributeTooManyValues", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The private data supplied does not meet the requirements of any known Association type.  Its length may be too short, or it may have been corrupted..
        /// </summary>
        internal static string BadAssociationPrivateData {
            get {
                return ResourceManager.GetString("BadAssociationPrivateData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} extension failed to deserialize and will be skipped.  {1}.
        /// </summary>
        internal static string BadExtension {
            get {
                return ResourceManager.GetString("BadExtension", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Callback arguments are only supported when a {0} is provided to the {1}..
        /// </summary>
        internal static string CallbackArgumentsRequireSecretStore {
            get {
                return ResourceManager.GetString("CallbackArgumentsRequireSecretStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A Simple Registration request can only generate a response on the receiving end..
        /// </summary>
        internal static string CallDeserializeBeforeCreateResponse {
            get {
                return ResourceManager.GetString("CallDeserializeBeforeCreateResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The openid.claimed_id and openid.identity parameters must both be present or both be absent..
        /// </summary>
        internal static string ClaimedIdAndLocalIdMustBothPresentOrAbsent {
            get {
                return ResourceManager.GetString("ClaimedIdAndLocalIdMustBothPresentOrAbsent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The ClaimedIdentifier property cannot be set when IsDelegatedIdentifier is true to avoid breaking OpenID URL delegation..
        /// </summary>
        internal static string ClaimedIdentifierCannotBeSetOnDelegatedAuthentication {
            get {
                return ResourceManager.GetString("ClaimedIdentifierCannotBeSetOnDelegatedAuthentication", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The ClaimedIdentifier property must be set first..
        /// </summary>
        internal static string ClaimedIdentifierMustBeSetFirst {
            get {
                return ResourceManager.GetString("ClaimedIdentifierMustBeSetFirst", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An extension with this property name (&apos;{0}&apos;) has already been registered..
        /// </summary>
        internal static string ClientScriptExtensionPropertyNameCollision {
            get {
                return ResourceManager.GetString("ClientScriptExtensionPropertyNameCollision", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The extension &apos;{0}&apos; has already been registered..
        /// </summary>
        internal static string ClientScriptExtensionTypeCollision {
            get {
                return ResourceManager.GetString("ClientScriptExtensionTypeCollision", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An authentication request has already been created using CreateRequest()..
        /// </summary>
        internal static string CreateRequestAlreadyCalled {
            get {
                return ResourceManager.GetString("CreateRequestAlreadyCalled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Only OpenIDs issued directly by their OpenID Provider are allowed here..
        /// </summary>
        internal static string DelegatingIdentifiersNotAllowed {
            get {
                return ResourceManager.GetString("DelegatingIdentifiersNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The following properties must be set before the Diffie-Hellman algorithm can generate a public key: {0}.
        /// </summary>
        internal static string DiffieHellmanRequiredPropertiesNotSet {
            get {
                return ResourceManager.GetString("DiffieHellmanRequiredPropertiesNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to URI is not SSL yet requireSslDiscovery is set to true..
        /// </summary>
        internal static string ExplicitHttpUriSuppliedWithSslRequirement {
            get {
                return ResourceManager.GetString("ExplicitHttpUriSuppliedWithSslRequirement", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An extension sharing namespace &apos;{0}&apos; has already been added.  Only one extension per namespace is allowed in a given request..
        /// </summary>
        internal static string ExtensionAlreadyAddedWithSameTypeURI {
            get {
                return ResourceManager.GetString("ExtensionAlreadyAddedWithSameTypeURI", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot lookup extension support on a rehydrated ServiceEndpoint..
        /// </summary>
        internal static string ExtensionLookupSupportUnavailable {
            get {
                return ResourceManager.GetString("ExtensionLookupSupportUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fragment segments do not apply to XRI identifiers..
        /// </summary>
        internal static string FragmentNotAllowedOnXRIs {
            get {
                return ResourceManager.GetString("FragmentNotAllowedOnXRIs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The HTML head tag must include runat=&quot;server&quot;..
        /// </summary>
        internal static string HeadTagMustIncludeRunatServer {
            get {
                return ResourceManager.GetString("HeadTagMustIncludeRunatServer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ClaimedIdentifier and LocalIdentifier must be the same when IsIdentifierSelect is true..
        /// </summary>
        internal static string IdentifierSelectRequiresMatchingIdentifiers {
            get {
                return ResourceManager.GetString("IdentifierSelectRequiresMatchingIdentifiers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The openid.identity and openid.claimed_id parameters must either be both present or both absent from the message..
        /// </summary>
        internal static string IdentityAndClaimedIdentifierMustBeBothPresentOrAbsent {
            get {
                return ResourceManager.GetString("IdentityAndClaimedIdentifierMustBeBothPresentOrAbsent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Provider requested association type &apos;{0}&apos; and session type &apos;{1}&apos;, which are not compatible with each other..
        /// </summary>
        internal static string IncompatibleAssociationAndSessionTypes {
            get {
                return ResourceManager.GetString("IncompatibleAssociationAndSessionTypes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} (Contact: {1}, Reference: {2}).
        /// </summary>
        internal static string IndirectErrorFormattedMessage {
            get {
                return ResourceManager.GetString("IndirectErrorFormattedMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot encode &apos;{0}&apos; because it contains an illegal character for Key-Value Form encoding.  (line {1}: &apos;{2}&apos;).
        /// </summary>
        internal static string InvalidCharacterInKeyValueFormInput {
            get {
                return ResourceManager.GetString("InvalidCharacterInKeyValueFormInput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot decode Key-Value Form because a line was found without a &apos;{0}&apos; character.  (line {1}: &apos;{2}&apos;).
        /// </summary>
        internal static string InvalidKeyValueFormCharacterMissing {
            get {
                return ResourceManager.GetString("InvalidKeyValueFormCharacterMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The scheme must be http or https but was &apos;{0}&apos;..
        /// </summary>
        internal static string InvalidScheme {
            get {
                return ResourceManager.GetString("InvalidScheme", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value &apos;{0}&apos; is not a valid URI..
        /// </summary>
        internal static string InvalidUri {
            get {
                return ResourceManager.GetString("InvalidUri", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Not a recognized XRI format..
        /// </summary>
        internal static string InvalidXri {
            get {
                return ResourceManager.GetString("InvalidXri", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The OpenID Provider issued an assertion for an Identifier whose discovery information did not match.  
        ///Assertion endpoint info: 
        ///{0}
        ///Discovered endpoint info:
        ///{1}.
        /// </summary>
        internal static string IssuedAssertionFailsIdentifierDiscovery {
            get {
                return ResourceManager.GetString("IssuedAssertionFailsIdentifierDiscovery", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The list of keys do not match the provided dictionary..
        /// </summary>
        internal static string KeysListAndDictionaryDoNotMatch {
            get {
                return ResourceManager.GetString("KeysListAndDictionaryDoNotMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &apos;{0}&apos; and &apos;{1}&apos; parameters must both be or not be &apos;{2}&apos;..
        /// </summary>
        internal static string MatchingArgumentsExpected {
            get {
                return ResourceManager.GetString("MatchingArgumentsExpected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The maximum time allowed to complete authentication has been exceeded.  Please try again..
        /// </summary>
        internal static string MaximumAuthenticationTimeExpired {
            get {
                return ResourceManager.GetString("MaximumAuthenticationTimeExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No recognized association type matches the requested length of {0}..
        /// </summary>
        internal static string NoAssociationTypeFoundByLength {
            get {
                return ResourceManager.GetString("NoAssociationTypeFoundByLength", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No recognized association type matches the requested name of &apos;{0}&apos;..
        /// </summary>
        internal static string NoAssociationTypeFoundByName {
            get {
                return ResourceManager.GetString("NoAssociationTypeFoundByName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unless using transport layer encryption, &quot;no-encryption&quot; MUST NOT be used..
        /// </summary>
        internal static string NoEncryptionSessionRequiresHttps {
            get {
                return ResourceManager.GetString("NoEncryptionSessionRequiresHttps", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No identifier has been set..
        /// </summary>
        internal static string NoIdentifierSet {
            get {
                return ResourceManager.GetString("NoIdentifierSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No XRDS document containing OpenID relying party endpoint information could be found at {0}..
        /// </summary>
        internal static string NoRelyingPartyEndpointDiscovered {
            get {
                return ResourceManager.GetString("NoRelyingPartyEndpointDiscovered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Diffie-Hellman session type &apos;{0}&apos; not found for OpenID {1}..
        /// </summary>
        internal static string NoSessionTypeFound {
            get {
                return ResourceManager.GetString("NoSessionTypeFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation is not supported by serialized authentication responses.  Try this operation from the LoggedIn event handler..
        /// </summary>
        internal static string NotSupportedByAuthenticationSnapshot {
            get {
                return ResourceManager.GetString("NotSupportedByAuthenticationSnapshot", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No OpenID endpoint found..
        /// </summary>
        internal static string OpenIdEndpointNotFound {
            get {
                return ResourceManager.GetString("OpenIdEndpointNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No OpenID url is provided..
        /// </summary>
        internal static string OpenIdTextBoxEmpty {
            get {
                return ResourceManager.GetString("OpenIdTextBoxEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This operation is only allowed when IAuthenticationResponse.State == AuthenticationStatus.SetupRequired..
        /// </summary>
        internal static string OperationOnlyValidForSetupRequiredState {
            get {
                return ResourceManager.GetString("OperationOnlyValidForSetupRequiredState", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An positive OpenID assertion was received from OP endpoint {0} that is not on this relying party&apos;s whitelist..
        /// </summary>
        internal static string PositiveAssertionFromNonWhitelistedProvider {
            get {
                return ResourceManager.GetString("PositiveAssertionFromNonWhitelistedProvider", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find the signing secret by the handle &apos;{0}&apos;..
        /// </summary>
        internal static string PrivateRPSecretNotFound {
            get {
                return ResourceManager.GetString("PrivateRPSecretNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} property must be set first..
        /// </summary>
        internal static string PropertyNotSet {
            get {
                return ResourceManager.GetString("PropertyNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This property value is not supported by this control..
        /// </summary>
        internal static string PropertyValueNotSupported {
            get {
                return ResourceManager.GetString("PropertyValueNotSupported", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to determine the version of the OpenID protocol implemented by the Provider at endpoint &apos;{0}&apos;..
        /// </summary>
        internal static string ProviderVersionUnrecognized {
            get {
                return ResourceManager.GetString("ProviderVersionUnrecognized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An HTTP request to the realm URL ({0}) resulted in a redirect, which is not allowed during relying party discovery..
        /// </summary>
        internal static string RealmCausedRedirectUponDiscovery {
            get {
                return ResourceManager.GetString("RealmCausedRedirectUponDiscovery", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sorry.  This site only accepts OpenIDs that are HTTPS-secured, but {0} is not a secure Identifier..
        /// </summary>
        internal static string RequireSslNotSatisfiedByAssertedClaimedId {
            get {
                return ResourceManager.GetString("RequireSslNotSatisfiedByAssertedClaimedId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The response is not ready.  Use IsResponseReady to check whether a response is ready first..
        /// </summary>
        internal static string ResponseNotReady {
            get {
                return ResourceManager.GetString("ResponseNotReady", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to return_to &apos;{0}&apos; not under realm &apos;{1}&apos;..
        /// </summary>
        internal static string ReturnToNotUnderRealm {
            get {
                return ResourceManager.GetString("ReturnToNotUnderRealm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The {0} parameter ({1}) does not match the actual URL ({2}) the request was made with..
        /// </summary>
        internal static string ReturnToParamDoesNotMatchRequestUrl {
            get {
                return ResourceManager.GetString("ReturnToParamDoesNotMatchRequestUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The ReturnTo property must not be null to support this operation..
        /// </summary>
        internal static string ReturnToRequiredForOperation {
            get {
                return ResourceManager.GetString("ReturnToRequiredForOperation", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The openid.return_to parameter is required in the request message in order to construct a response, but that parameter was missing..
        /// </summary>
        internal static string ReturnToRequiredForResponse {
            get {
                return ResourceManager.GetString("ReturnToRequiredForResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The following parameter(s) are not included in the signature but must be: {0}.
        /// </summary>
        internal static string SignatureDoesNotIncludeMandatoryParts {
            get {
                return ResourceManager.GetString("SignatureDoesNotIncludeMandatoryParts", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid birthdate value.  Must be in the form yyyy-MM-dd..
        /// </summary>
        internal static string SregInvalidBirthdate {
            get {
                return ResourceManager.GetString("SregInvalidBirthdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The type must implement {0}..
        /// </summary>
        internal static string TypeMustImplementX {
            get {
                return ResourceManager.GetString("TypeMustImplementX", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The property {0} had unexpected value {1}..
        /// </summary>
        internal static string UnexpectedEnumPropertyValue {
            get {
                return ResourceManager.GetString("UnexpectedEnumPropertyValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unexpected HTTP status code {0} {1} received in direct response..
        /// </summary>
        internal static string UnexpectedHttpStatusCode {
            get {
                return ResourceManager.GetString("UnexpectedHttpStatusCode", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An unsolicited assertion cannot be sent for the claimed identifier {0} because this is not an authorized Provider for that identifier..
        /// </summary>
        internal static string UnsolicitedAssertionForUnrelatedClaimedIdentifier {
            get {
                return ResourceManager.GetString("UnsolicitedAssertionForUnrelatedClaimedIdentifier", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Rejecting unsolicited assertions requires a nonce store and an association store..
        /// </summary>
        internal static string UnsolicitedAssertionRejectionRequiresNonceStore {
            get {
                return ResourceManager.GetString("UnsolicitedAssertionRejectionRequiresNonceStore", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsolicited assertions are not allowed at this relying party..
        /// </summary>
        internal static string UnsolicitedAssertionsNotAllowed {
            get {
                return ResourceManager.GetString("UnsolicitedAssertionsNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unsolicited assertions are not allowed from 1.0 OpenID Providers..
        /// </summary>
        internal static string UnsolicitedAssertionsNotAllowedFrom1xOPs {
            get {
                return ResourceManager.GetString("UnsolicitedAssertionsNotAllowedFrom1xOPs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Providing a DateTime whose Kind is Unspecified is not allowed..
        /// </summary>
        internal static string UnspecifiedDateTimeKindNotAllowed {
            get {
                return ResourceManager.GetString("UnspecifiedDateTimeKindNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This feature is unavailable due to an unrecognized channel configuration..
        /// </summary>
        internal static string UnsupportedChannelConfiguration {
            get {
                return ResourceManager.GetString("UnsupportedChannelConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The openid.user_setup_url parameter is required when sending negative assertion messages in response to immediate mode requests..
        /// </summary>
        internal static string UserSetupUrlRequiredInImmediateNegativeResponse {
            get {
                return ResourceManager.GetString("UserSetupUrlRequiredInImmediateNegativeResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The X.509 certificate used to sign this document is not trusted..
        /// </summary>
        internal static string X509CertificateNotTrusted {
            get {
                return ResourceManager.GetString("X509CertificateNotTrusted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XRI support has been disabled at this site..
        /// </summary>
        internal static string XriResolutionDisabled {
            get {
                return ResourceManager.GetString("XriResolutionDisabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to XRI resolution failed..
        /// </summary>
        internal static string XriResolutionFailed {
            get {
                return ResourceManager.GetString("XriResolutionFailed", resourceCulture);
            }
        }
    }
}
