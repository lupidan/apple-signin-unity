//
//  MIT License
//
//  Copyright (c) 2019 Daniel Lupiañez Casares
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//

#import "AppleAuthSerializer.h"

@implementation AppleAuthSerializer

+ (NSDictionary *) dictionaryForNSError:(NSError *)error
{
    if (!error)
        return nil;
    
    NSMutableDictionary *result = [NSMutableDictionary dictionary];
    [result setValue:@([error code]) forKey:@"_code"];
    [result setValue:[error domain] forKey:@"_domain"];
    [result setValue:[error localizedDescription] forKey:@"_localizedDescription"];
    [result setValue:[error localizedRecoveryOptions] forKey:@"_localizedRecoveryOptions"];
    [result setValue:[error localizedRecoverySuggestion] forKey:@"_localizedRecoverySuggestion"];
    [result setValue:[error localizedFailureReason] forKey:@"_localizedFailureReason"];
    return [result copy];
}

+ (NSDictionary *) credentialResponseDictionaryForCredentialState:(NSNumber *)credentialStateNumber
                                                  errorDictionary:(NSDictionary *)errorDictionary
{
    NSMutableDictionary *result = [[NSMutableDictionary alloc] init];
    
    [result setValue:@(errorDictionary == nil) forKey:@"_success"];
    [result setValue:@(credentialStateNumber != nil) forKey:@"_hasCredentialState"];
    [result setValue:@(errorDictionary != nil) forKey:@"_hasError"];
    
    [result setValue:credentialStateNumber forKey:@"_credentialState"];
    [result setValue:errorDictionary forKey:@"_error"];
    
    return [result copy];
}

+ (NSDictionary *) loginResponseDictionaryForAppleIdCredentialDictionary:(NSDictionary *)appleIdCredentialDictionary
                                            passwordCredentialDictionary:(NSDictionary *)passwordCredentialDictionary
                                                         errorDictionary:(NSDictionary *)errorDictionary
{
    NSMutableDictionary *result = [[NSMutableDictionary alloc] init];
    
    [result setValue:@(errorDictionary == nil) forKey:@"_success"];
    [result setValue:@(appleIdCredentialDictionary != nil) forKey:@"_hasAppleIdCredential"];
    [result setValue:@(passwordCredentialDictionary != nil) forKey:@"_hasPasswordCredential"];
    [result setValue:@(errorDictionary != nil) forKey:@"_hasError"];
    
    [result setValue:appleIdCredentialDictionary forKey:@"_appleIdCredential"];
    [result setValue:passwordCredentialDictionary forKey:@"_passwordCredential"];
    [result setValue:errorDictionary forKey:@"_error"];
    
    return [result copy];
}

// IOS/TVOS 9.0 | MACOS 10.11 | VISIONOS 1.0
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 90000 || __TV_OS_VERSION_MAX_ALLOWED >= 90000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101100 || __VISION_OS_VERSION_MAX_ALLOWED >= 10000

+ (NSDictionary *) dictionaryForNSPersonNameComponents:(NSPersonNameComponents *)nameComponents
{
    if (!nameComponents)
        return nil;
    
    // Sometimes, when not requesting a name in the ASAuthorizationAppleIDRequest scopes,
    // Apple will just send an empty NSPersonNameComponents instance...
    // This should be treated as a nil person name components
    if ([nameComponents namePrefix] == nil &&
        [nameComponents givenName] == nil &&
        [nameComponents middleName] == nil &&
        [nameComponents familyName] == nil &&
        [nameComponents nameSuffix] == nil &&
        [nameComponents nickname] == nil &&
        [nameComponents phoneticRepresentation] == nil)
        return nil;
    
    NSMutableDictionary *result = [NSMutableDictionary dictionary];
    [result setValue:[nameComponents namePrefix] forKey:@"_namePrefix"];
    [result setValue:[nameComponents givenName] forKey:@"_givenName"];
    [result setValue:[nameComponents middleName] forKey:@"_middleName"];
    [result setValue:[nameComponents familyName] forKey:@"_familyName"];
    [result setValue:[nameComponents nameSuffix] forKey:@"_nameSuffix"];
    [result setValue:[nameComponents nickname] forKey:@"_nickname"];
    
    NSDictionary *phoneticRepresentationDictionary = [AppleAuthSerializer dictionaryForNSPersonNameComponents:[nameComponents phoneticRepresentation]];
    [result setValue:@(phoneticRepresentationDictionary != nil) forKey:@"_hasPhoneticRepresentation"];
    [result setValue:phoneticRepresentationDictionary forKey:@"_phoneticRepresentation"];
    
    return [result copy];
}

#endif

// IOS/TVOS 13.0 | MACOS 10.15 | VISIONOS 1.0
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500 || __VISION_OS_VERSION_MAX_ALLOWED >= 10000

+ (NSDictionary *) dictionaryForASAuthorizationAppleIDCredential:(ASAuthorizationAppleIDCredential *)appleIDCredential
{
    if (!appleIDCredential)
        return nil;
    
    NSMutableDictionary *result = [NSMutableDictionary dictionary];
    [result setValue:[[appleIDCredential identityToken] base64EncodedStringWithOptions:0] forKey:@"_base64IdentityToken"];
    [result setValue:[[appleIDCredential authorizationCode] base64EncodedStringWithOptions:0] forKey:@"_base64AuthorizationCode"];
    [result setValue:[appleIDCredential state] forKey:@"_state"];
    [result setValue:[appleIDCredential user] forKey:@"_user"];
    [result setValue:[appleIDCredential authorizedScopes] forKey:@"_authorizedScopes"];
    [result setValue:[appleIDCredential email] forKey:@"_email"];
    [result setValue:@([appleIDCredential realUserStatus]) forKey:@"_realUserStatus"];
    
    NSDictionary *fullNameDictionary = [AppleAuthSerializer dictionaryForNSPersonNameComponents:[appleIDCredential fullName]];
    [result setValue:@(fullNameDictionary != nil) forKey:@"_hasFullName"];
    [result setValue:fullNameDictionary forKey:@"_fullName"];
    
    return [result copy];
}

+ (NSDictionary *) dictionaryForASPasswordCredential:(ASPasswordCredential *)passwordCredential
{
    if (!passwordCredential)
        return nil;
    
    NSMutableDictionary *result = [NSMutableDictionary dictionary];
    [result setValue:[passwordCredential user] forKey:@"_user"];
    [result setValue:[passwordCredential password] forKey:@"_password"];
    return [result copy];
}

#endif

@end
