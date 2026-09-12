//
//  MIT License
//
//  Copyright (c) 2019-2020 Daniel Lupiañez Casares
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

#import "AppleAuthManager.h"
#import "AppleAuthSerializer.h"

#pragma mark - AppleAuthManager Implementation

// IOS/TVOS 13.0 | MACOS 10.15 | VISIONOS 1.0
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500 || __VISION_OS_VERSION_MAX_ALLOWED >= 10000
#define AUTHENTICATION_SERVICES_AVAILABLE true
#import <AuthenticationServices/AuthenticationServices.h>
#endif

@interface AppleAuthManager ()
@property (nonatomic, assign) NativeMessageHandlerDelegate mainCallback;
@property (nonatomic, weak) NSOperationQueue *callingOperationQueue;

- (void) sendNativeMessageForDictionary:(NSDictionary *)payloadDictionary forRequestId:(uint)requestId;
- (void) sendNativeMessageForString:(NSString *)payloadString forRequestId:(uint)requestId;
- (NSError *)internalErrorWithCode:(NSInteger)code andMessage:(NSString *)message;
- (void) sendsCredentialStatusInternalErrorWithCode:(NSInteger)code andMessage:(NSString *)message forRequestWithId:(uint)requestId;
- (void) sendsLoginResponseInternalErrorWithCode:(NSInteger)code andMessage:(NSString *)message forRequestWithId:(uint)requestId;
@end

#if AUTHENTICATION_SERVICES_AVAILABLE
API_AVAILABLE(ios(13.0), macos(10.15), tvos(13.0), watchos(6.0), visionos(1.0))
@interface AppleAuthManager () <ASAuthorizationControllerDelegate, ASAuthorizationControllerPresentationContextProviding>
@property (nonatomic, strong) ASAuthorizationAppleIDProvider *appleIdProvider;
@property (nonatomic, strong) ASAuthorizationPasswordProvider *passwordProvider;
@property (nonatomic, strong) NSObject *credentialsRevokedObserver;
@property (nonatomic, strong) NSMutableDictionary<NSValue *, NSNumber *> *authorizationsInProgress;
@end
#endif

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
#if AUTHENTICATION_SERVICES_AVAILABLE
        if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, visionOS 1.0, *))
        {
            _appleIdProvider = [[ASAuthorizationAppleIDProvider alloc] init];
            _passwordProvider = [[ASAuthorizationPasswordProvider alloc] init];
            _authorizationsInProgress = [NSMutableDictionary dictionary];
        }
#endif
    }
    return self;
}

#pragma mark Public methods

- (void) quickLogin:(uint)requestId withNonce:(NSString *)nonce andState:(NSString *)state
{
#if AUTHENTICATION_SERVICES_AVAILABLE
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, visionOS 1.0, *))
    {
        ASAuthorizationAppleIDRequest *appleIDRequest = [[self appleIdProvider] createRequest];
        [appleIDRequest setNonce:nonce];
        [appleIDRequest setState:state];

        ASAuthorizationPasswordRequest *keychainRequest = [[self passwordProvider] createRequest];

        ASAuthorizationController *authorizationController = [[ASAuthorizationController alloc] initWithAuthorizationRequests:@[appleIDRequest, keychainRequest]];
        [self performAuthorizationRequestsForController:authorizationController withRequestId:requestId];
    }
    else
    {
        [self sendsLoginResponseInternalErrorWithCode:-100
                                           andMessage:@"Native AppleAuth is only available from iOS 13.0"
                                     forRequestWithId:requestId];
    }
#else
    [self sendsLoginResponseInternalErrorWithCode:-100
                                       andMessage:@"Native AppleAuth is only available from iOS 13.0"
                                 forRequestWithId:requestId];
#endif
}

- (void) loginWithAppleId:(uint)requestId withOptions:(AppleAuthManagerLoginOptions)options nonce:(NSString *)nonce andState:(NSString *)state
{
#if AUTHENTICATION_SERVICES_AVAILABLE
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, visionOS 1.0, *))
    {
        ASAuthorizationAppleIDRequest *request = [[self appleIdProvider] createRequest];
        NSMutableArray *scopes = [NSMutableArray array];

        if (options & AppleAuthManagerIncludeName)
            [scopes addObject:ASAuthorizationScopeFullName];

        if (options & AppleAuthManagerIncludeEmail)
            [scopes addObject:ASAuthorizationScopeEmail];

        [request setRequestedScopes:[scopes copy]];
        [request setNonce:nonce];
        [request setState:state];

        ASAuthorizationController *authorizationController = [[ASAuthorizationController alloc] initWithAuthorizationRequests:@[request]];
        [self performAuthorizationRequestsForController:authorizationController withRequestId:requestId];
    }
    else
    {
        [self sendsLoginResponseInternalErrorWithCode:-100
                                           andMessage:@"Native AppleAuth is only available from iOS 13.0"
                                     forRequestWithId:requestId];
    }
#else
    [self sendsLoginResponseInternalErrorWithCode:-100
                                       andMessage:@"Native AppleAuth is only available from iOS 13.0"
                                 forRequestWithId:requestId];
#endif
}


- (void) getCredentialStateForUser:(NSString *)userId withRequestId:(uint)requestId
{
#if AUTHENTICATION_SERVICES_AVAILABLE
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, visionOS 1.0, *))
    {
        [[self appleIdProvider] getCredentialStateForUserID:userId completion:^(ASAuthorizationAppleIDProviderCredentialState credentialState, NSError * _Nullable error) {
            NSNumber *credentialStateNumber = nil;
            NSDictionary *errorDictionary = nil;

            if (error)
                errorDictionary = [AppleAuthSerializer dictionaryForNSError:error];
            else
                credentialStateNumber = @(credentialState);

            NSDictionary *responseDictionary = [AppleAuthSerializer credentialResponseDictionaryForCredentialState:credentialStateNumber
                                                                                               errorDictionary:errorDictionary];

            [self sendNativeMessageForDictionary:responseDictionary forRequestId:requestId];
        }];
    }
    else
    {
        [self sendsCredentialStatusInternalErrorWithCode:-100
                                              andMessage:@"Native AppleAuth is only available from iOS 13.0"
                                        forRequestWithId:requestId];
    }
#else
    [self sendsCredentialStatusInternalErrorWithCode:-100
                                          andMessage:@"Native AppleAuth is only available from iOS 13.0"
                                    forRequestWithId:requestId];
#endif
}

- (void) registerCredentialsRevokedCallbackForRequestId:(uint)requestId
{
#if AUTHENTICATION_SERVICES_AVAILABLE
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, visionOS 1.0, *))
    {
        if ([self credentialsRevokedObserver])
        {
            [[NSNotificationCenter defaultCenter] removeObserver:[self credentialsRevokedObserver]];
            [self setCredentialsRevokedObserver:nil];
        }

        if (requestId != 0)
        {
            NSObject *observer = [[NSNotificationCenter defaultCenter] addObserverForName:ASAuthorizationAppleIDProviderCredentialRevokedNotification
                                                                               object:nil
                                                                                queue:nil
                                                                           usingBlock:^(NSNotification * _Nonnull note) {
                                                                               [self sendNativeMessageForString:@"Credentials Revoked" forRequestId:requestId];
                                                                           }];
            [self setCredentialsRevokedObserver:observer];
        }
    }
#endif
}

#pragma mark Private methods

- (void) sendNativeMessageForDictionary:(NSDictionary *)payloadDictionary forRequestId:(uint)requestId
{
    NSError *error = nil;
    NSData *payloadData = [NSJSONSerialization dataWithJSONObject:payloadDictionary options:0 error:&error];
    NSString *payloadString = error ? [NSString stringWithFormat:@"Serialization error %@", [error localizedDescription]] : [[NSString alloc] initWithData:payloadData encoding:NSUTF8StringEncoding];
    [self sendNativeMessageForString:payloadString forRequestId:requestId];
}

- (void) sendNativeMessageForString:(NSString *)payloadString forRequestId:(uint)requestId
{
    if ([self mainCallback] == NULL)
        return;

    if ([self callingOperationQueue])
    {
        [[self callingOperationQueue] addOperationWithBlock:^{
            [self mainCallback](requestId, [payloadString UTF8String]);
        }];
    }
    else
    {
        [self mainCallback](requestId, [payloadString UTF8String]);
    }
}

- (NSError *)internalErrorWithCode:(NSInteger)code andMessage:(NSString *)message
{
    return [NSError errorWithDomain:@"com.unity.AppleAuth"
                               code:code
                           userInfo:@{NSLocalizedDescriptionKey : message}];
}

- (void) sendsCredentialStatusInternalErrorWithCode:(NSInteger)code andMessage:(NSString *)message forRequestWithId:(uint)requestId
{
    NSError *customError = [self internalErrorWithCode:code andMessage:message];
    NSDictionary *customErrorDictionary = [AppleAuthSerializer dictionaryForNSError:customError];
    NSDictionary *responseDictionary = [AppleAuthSerializer credentialResponseDictionaryForCredentialState:nil
                                                                                           errorDictionary:customErrorDictionary];

    [self sendNativeMessageForDictionary:responseDictionary forRequestId:requestId];
}

- (void) sendsLoginResponseInternalErrorWithCode:(NSInteger)code andMessage:(NSString *)message forRequestWithId:(uint)requestId
{
    NSError *customError = [self internalErrorWithCode:code andMessage:message];
    NSDictionary *customErrorDictionary = [AppleAuthSerializer dictionaryForNSError:customError];
    NSDictionary *responseDictionary = [AppleAuthSerializer loginResponseDictionaryForAppleIdCredentialDictionary:nil
                                                                                     passwordCredentialDictionary:nil
                                                                                                  errorDictionary:customErrorDictionary];

    [self sendNativeMessageForDictionary:responseDictionary forRequestId:requestId];
}

#if AUTHENTICATION_SERVICES_AVAILABLE

- (void) performAuthorizationRequestsForController:(ASAuthorizationController *)authorizationController withRequestId:(uint)requestId
API_AVAILABLE(ios(13.0), macos(10.15), tvos(13.0), watchos(6.0), visionos(1.0))
{
    NSValue *authControllerAsKey = [NSValue valueWithNonretainedObject:authorizationController];
    [[self authorizationsInProgress] setObject:@(requestId) forKey:authControllerAsKey];

    [authorizationController setDelegate:self];
    [authorizationController setPresentationContextProvider:self];
    [authorizationController performRequests];
}

#pragma mark ASAuthorizationControllerDelegate protocol implementation

- (void) authorizationController:(ASAuthorizationController *)controller didCompleteWithAuthorization:(ASAuthorization *)authorization
API_AVAILABLE(ios(13.0), macos(10.15), tvos(13.0), watchos(6.0), visionos(1.0))
{
    NSValue *authControllerAsKey = [NSValue valueWithNonretainedObject:controller];
    NSNumber *requestIdNumber = [[self authorizationsInProgress] objectForKey:authControllerAsKey];
    if (requestIdNumber)
    {
        NSDictionary *appleIdCredentialDictionary = nil;
        NSDictionary *passwordCredentialDictionary = nil;
        if ([[authorization credential] isKindOfClass:[ASAuthorizationAppleIDCredential class]])
        {
            appleIdCredentialDictionary = [AppleAuthSerializer dictionaryForASAuthorizationAppleIDCredential:(ASAuthorizationAppleIDCredential *)[authorization credential]];
        }
        else if ([[authorization credential] isKindOfClass:[ASPasswordCredential class]])
        {
            passwordCredentialDictionary = [AppleAuthSerializer dictionaryForASPasswordCredential:(ASPasswordCredential *)[authorization credential]];
        }

        NSDictionary *responseDictionary = [AppleAuthSerializer loginResponseDictionaryForAppleIdCredentialDictionary:appleIdCredentialDictionary
                                                                                      passwordCredentialDictionary:passwordCredentialDictionary
                                                                                                   errorDictionary:nil];

        [self sendNativeMessageForDictionary:responseDictionary forRequestId:[requestIdNumber unsignedIntValue]];

        [[self authorizationsInProgress] removeObjectForKey:authControllerAsKey];
    }
}

- (void) authorizationController:(ASAuthorizationController *)controller didCompleteWithError:(NSError *)error
API_AVAILABLE(ios(13.0), macos(10.15), tvos(13.0), watchos(6.0), visionos(1.0))
{
    NSValue *authControllerAsKey = [NSValue valueWithNonretainedObject:controller];
    NSNumber *requestIdNumber = [[self authorizationsInProgress] objectForKey:authControllerAsKey];
    if (requestIdNumber)
    {
        NSDictionary *errorDictionary = [AppleAuthSerializer dictionaryForNSError:error];
        NSDictionary *responseDictionary = [AppleAuthSerializer loginResponseDictionaryForAppleIdCredentialDictionary:nil
                                                                                         passwordCredentialDictionary:nil
                                                                                                      errorDictionary:errorDictionary];

        [self sendNativeMessageForDictionary:responseDictionary forRequestId:[requestIdNumber unsignedIntValue]];

        [[self authorizationsInProgress] removeObjectForKey:authControllerAsKey];
    }
}

#pragma mark ASAuthorizationControllerPresentationContextProviding protocol implementation

- (ASPresentationAnchor) presentationAnchorForAuthorizationController:(ASAuthorizationController *)controller
API_AVAILABLE(ios(13.0), macos(10.15), tvos(13.0), watchos(6.0), visionos(1.0))
{
    
    #if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __VISION_OS_VERSION_MAX_ALLOWED >= 10000
        return [[[UIApplication sharedApplication] delegate] window];
    #elif __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500
        return [[NSApplication sharedApplication] mainWindow];
    #else
        return nil;
    #endif
}

#endif

@end

#pragma mark - Native C Calls

bool AppleAuth_IsCurrentPlatformSupported(void)
{
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, visionOS 1.0, *))
    {
        return true;
    }
    else
    {
        return false;
    }
}

void AppleAuth_SetupNativeMessageHandlerCallback(NativeMessageHandlerDelegate callback)
{
    [[AppleAuthManager sharedManager] setMainCallback:callback];
    [[AppleAuthManager sharedManager] setCallingOperationQueue: [NSOperationQueue currentQueue]];
}

void AppleAuth_GetCredentialState(uint requestId, const char* userId)
{
    [[AppleAuthManager sharedManager] getCredentialStateForUser:[NSString stringWithUTF8String:userId] withRequestId:requestId];
}

void AppleAuth_LoginWithAppleId(uint requestId, int options, const char* _Nullable nonceCStr, const char* _Nullable stateCStr)
{
    NSString *nonce = nonceCStr != NULL ? [NSString stringWithUTF8String:nonceCStr] : nil;
    NSString *state = stateCStr != NULL ? [NSString stringWithUTF8String:stateCStr] : nil;
    [[AppleAuthManager sharedManager] loginWithAppleId:requestId withOptions:options nonce:nonce andState:state];
}

void AppleAuth_QuickLogin(uint requestId, const char* _Nullable nonceCStr, const char* _Nullable stateCStr)
{
    NSString *nonce = nonceCStr != NULL ? [NSString stringWithUTF8String:nonceCStr] : nil;
    NSString *state = stateCStr != NULL ? [NSString stringWithUTF8String:stateCStr] : nil;
    [[AppleAuthManager sharedManager] quickLogin:requestId withNonce:nonce andState:state];
}

void AppleAuth_RegisterCredentialsRevokedCallbackId(uint requestId)
{
    [[AppleAuthManager sharedManager] registerCredentialsRevokedCallbackForRequestId:requestId];
}

void AppleAuth_LogMessage(const char* _Nullable messageCStr)
{
    NSString *message = messageCStr != NULL ? [NSString stringWithUTF8String:messageCStr] : nil;
    NSLog(@"%@", message);
}
