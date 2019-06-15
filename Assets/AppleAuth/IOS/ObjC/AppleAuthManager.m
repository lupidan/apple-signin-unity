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

#import <AuthenticationServices/AuthenticationServices.h>
#import "AppleAuthManager.h"
#import "NativeMessageHandler.h"

@interface AppleAuthManager () <ASAuthorizationControllerDelegate, ASAuthorizationControllerPresentationContextProviding>
@property (nonatomic, strong) ASAuthorizationAppleIDProvider *appleIdProvider;
@property (nonatomic, strong) ASAuthorizationPasswordProvider *passwordProvider;
@property (nonatomic, strong) NSMutableDictionary<NSValue *, NSNumber *> *authorizationsInProgress;
@end

@implementation AppleAuthManager

+ (instancetype) sharedManager
{
    static AppleAuthManager *_defaultManager = nil;
    static dispatch_once_t defaultManagerInitialization;
    
    dispatch_once(&defaultManagerInitialization, ^{
        _defaultManager = [[AppleAuthManager alloc] init];
    });
    
    return _defaultManager;
}

- (instancetype) init
{
    self = [super init];
    if (self)
    {
        _appleIdProvider = [[ASAuthorizationAppleIDProvider alloc] init];
        _passwordProvider = [[ASAuthorizationPasswordProvider alloc] init];
        _authorizationsInProgress = [NSMutableDictionary dictionary];
    }
    return self;
}

#pragma mark - Public methods

- (void) loginSilently:(uint)requestId
{
    ASAuthorizationAppleIDRequest *appleIDSilentRequest = [[self appleIdProvider] createRequest];
    ASAuthorizationPasswordRequest *passwordSilentRequest = [[self passwordProvider] createRequest];
    
    ASAuthorizationController *authorizationController = [[ASAuthorizationController alloc] initWithAuthorizationRequests:@[appleIDSilentRequest, passwordSilentRequest]];
    [self performAuthorizationRequestsForController:authorizationController withRequestId:requestId];
}

- (void) loginWithAppleId:(uint)requestId withOptions:(AppleAuthManagerLoginOptions)options
{
    ASAuthorizationAppleIDRequest *request = [[self appleIdProvider] createRequest];
    NSMutableArray *scopes = [NSMutableArray array];
    
    if (options & AppleAuthManagerIncludeName)
        [scopes addObject:ASAuthorizationScopeFullName];
        
    if (options & AppleAuthManagerIncludeEmail)
        [scopes addObject:ASAuthorizationScopeEmail];
        
    [request setRequestedScopes:[scopes copy]];
    
    ASAuthorizationController *authorizationController = [[ASAuthorizationController alloc] initWithAuthorizationRequests:@[request]];
    [self performAuthorizationRequestsForController:authorizationController withRequestId:requestId];
}


- (void) getCredentialStateForUser:(NSString *)userId withRequestId:(uint)requestId
{
    [[self appleIdProvider] getCredentialStateForUserID:userId completion:^(ASAuthorizationAppleIDProviderCredentialState credentialState, NSError * _Nullable error) {
        NSNumber *credentialStateNumber = nil;
        NSDictionary *errorDictionary = nil;
        
        if (error)
            errorDictionary = [AppleAuthManager dictionaryForNSError:error];
        else
            credentialStateNumber = @(credentialState);
        
        NSDictionary *responseDictionary = [AppleAuthManager credentialResponseDictionaryForCredentialState:credentialStateNumber
                                                                                            errorDictionary:errorDictionary];
        
        [self sendNativeMessage:responseDictionary withRequestId:requestId];
    }];
}

#pragma mark - Private methods

- (void) performAuthorizationRequestsForController:(ASAuthorizationController *)authorizationController withRequestId:(uint)requestId
{
    NSValue *authControllerAsKey = [NSValue valueWithNonretainedObject:authorizationController];
    [[self authorizationsInProgress] setObject:@(requestId) forKey:authControllerAsKey];
    
    [authorizationController setDelegate:self];
    [authorizationController setPresentationContextProvider:self];
    [authorizationController performRequests];
}

- (void) sendNativeMessage:(NSDictionary *)toSerialize withRequestId:(uint)requestId
{
    NSError *error = nil;
    NSData *payloadData = [NSJSONSerialization dataWithJSONObject:toSerialize options:0 error:&error];
    NSString *payloadString = error ? [NSString stringWithFormat:@"Serialization error %@", [error localizedDescription]] : [[NSString alloc] initWithData:payloadData encoding:NSUTF8StringEncoding];
    [[NativeMessageHandler defaultHandler] sendNativeMessage:payloadString forRequestWithId:requestId];
}

#pragma mark - ASAuthorizationControllerDelegate protocol implementation

- (void) authorizationController:(ASAuthorizationController *)controller didCompleteWithAuthorization:(ASAuthorization *)authorization
{
    NSValue *authControllerAsKey = [NSValue valueWithNonretainedObject:controller];
    NSNumber *requestIdNumber = [[self authorizationsInProgress] objectForKey:authControllerAsKey];
    if (requestIdNumber)
    {
        NSDictionary *appleIdCredentialDictionary = nil;
        NSDictionary *passwordCredentialDictionary = nil;
        if ([[authorization credential] isKindOfClass:[ASAuthorizationAppleIDCredential class]])
        {
            appleIdCredentialDictionary = [AppleAuthManager dictionaryForASAuthorizationAppleIDCredential:(ASAuthorizationAppleIDCredential *)[authorization credential]];
        }
        else if ([[authorization credential] isKindOfClass:[ASPasswordCredential class]])
        {
            passwordCredentialDictionary = [AppleAuthManager dictionaryForASPasswordCredential:(ASPasswordCredential *)[authorization credential]];
        }

        NSDictionary *responseDictionary = [AppleAuthManager loginResponseDictionaryForAppleIdCredentialDictionary:appleIdCredentialDictionary
                                                                                      passwordCredentialDictionary:passwordCredentialDictionary
                                                                                                   errorDictionary:nil];
        
        [self sendNativeMessage:responseDictionary withRequestId:[requestIdNumber unsignedIntValue]];
        [[self authorizationsInProgress] removeObjectForKey:authControllerAsKey];
    }
}

- (void) authorizationController:(ASAuthorizationController *)controller didCompleteWithError:(NSError *)error
{
    NSValue *authControllerAsKey = [NSValue valueWithNonretainedObject:controller];
    NSNumber *requestIdNumber = [[self authorizationsInProgress] objectForKey:authControllerAsKey];
    if (requestIdNumber)
    {
        NSDictionary *errorDictionary = [AppleAuthManager dictionaryForNSError:error];
        NSDictionary *responseDictionary = [AppleAuthManager loginResponseDictionaryForAppleIdCredentialDictionary:nil
                                                                                      passwordCredentialDictionary:nil
                                                                                                   errorDictionary:errorDictionary];
        
        [self sendNativeMessage:responseDictionary withRequestId:[requestIdNumber unsignedIntValue]];
        [[self authorizationsInProgress] removeObjectForKey:authControllerAsKey];
    }
}

#pragma mark - ASAuthorizationControllerPresentationContextProviding protocol implementation

- (ASPresentationAnchor) presentationAnchorForAuthorizationController:(ASAuthorizationController *)controller
{
    return [[[UIApplication sharedApplication] delegate] window];
}

#pragma mark - Dictionary Generation Static Methods

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

+ (NSDictionary *) dictionaryForASAuthorizationAppleIDCredential:(ASAuthorizationAppleIDCredential *)appleIDCredential
{
    if (!appleIDCredential)
        return nil;
    
    NSMutableDictionary *result = [NSMutableDictionary dictionary];
    [result setValue:[[appleIDCredential identityToken] base64EncodedStringWithOptions:0] forKey:@"_identityToken"];
    [result setValue:[[appleIDCredential authorizationCode] base64EncodedStringWithOptions:0] forKey:@"_authorizationCode"];
    [result setValue:[appleIDCredential state] forKey:@"_state"];
    [result setValue:[appleIDCredential user] forKey:@"_user"];
    [result setValue:[appleIDCredential authorizedScopes] forKey:@"_authorizedScopes"];
    [result setValue:[appleIDCredential email] forKey:@"_email"];
    [result setValue:@([appleIDCredential realUserStatus]) forKey:@"_realUserStatus"];
    
    NSDictionary *fullNameDictionary = [AppleAuthManager dictionaryForNSPersonNameComponents:[appleIDCredential fullName]];
    [result setValue:@(fullNameDictionary != nil) forKey:@"_hasFullName"];
    [result setValue:fullNameDictionary forKey:@"_fullName"];
    
    return [result copy];
}

+ (NSDictionary *) dictionaryForNSPersonNameComponents:(NSPersonNameComponents *)nameComponents
{
    if (!nameComponents)
        return nil;
    
    NSMutableDictionary *result = [NSMutableDictionary dictionary];
    [result setValue:[nameComponents namePrefix] forKey:@"_namePrefix"];
    [result setValue:[nameComponents givenName] forKey:@"_givenName"];
    [result setValue:[nameComponents middleName] forKey:@"_middleName"];
    [result setValue:[nameComponents familyName] forKey:@"_familyName"];
    [result setValue:[nameComponents nameSuffix] forKey:@"_nameSuffix"];
    [result setValue:[nameComponents nickname] forKey:@"_nickname"];
    
    NSDictionary *phoneticRepresentationDictionary = [AppleAuthManager dictionaryForNSPersonNameComponents:[nameComponents phoneticRepresentation]];
    [result setValue:@(phoneticRepresentationDictionary != nil) forKey:@"_hasPhoneticRepresentation"];
    [result setValue:phoneticRepresentationDictionary forKey:@"_phoneticRepresentation"];
    
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

@end

void AppleAuth_IOS_GetCredentialState(uint requestId, const char* userId)
{
    [[AppleAuthManager sharedManager] getCredentialStateForUser:[NSString stringWithUTF8String:userId] withRequestId:requestId];
}

void AppleAuth_IOS_LoginWithAppleId(uint requestId, int options)
{
    [[AppleAuthManager sharedManager] loginWithAppleId:requestId withOptions:options];
}

void AppleAuth_IOS_LoginSilently(uint requestId)
{
    [[AppleAuthManager sharedManager] loginSilently:requestId];
}
