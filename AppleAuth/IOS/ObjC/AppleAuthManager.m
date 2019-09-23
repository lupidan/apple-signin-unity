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

#import "AppleAuthManager.h"
#import "NativeMessageHandler.h"
#import "AppleAuthSerializer.h"

#pragma mark - AppleAuthManager Implementation

// IOS/TVOS 13.0 | MACOS 10.15
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500
#import <AuthenticationServices/AuthenticationServices.h>

@interface AppleAuthManager () <ASAuthorizationControllerDelegate, ASAuthorizationControllerPresentationContextProviding>
@property (nonatomic, strong) ASAuthorizationAppleIDProvider *appleIdProvider;
@property (nonatomic, strong) ASAuthorizationPasswordProvider *passwordProvider;
@property (nonatomic, strong) NSObject *credentialsRevokedObserver;
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

#pragma mark Public methods

- (void) quickLogin:(uint)requestId
{
    ASAuthorizationAppleIDRequest *appleIDRequest = [[self appleIdProvider] createRequest];
    ASAuthorizationPasswordRequest *keychainRequest = [[self passwordProvider] createRequest];
    
    ASAuthorizationController *authorizationController = [[ASAuthorizationController alloc] initWithAuthorizationRequests:@[appleIDRequest, keychainRequest]];
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
            errorDictionary = [AppleAuthSerializer dictionaryForNSError:error];
        else
            credentialStateNumber = @(credentialState);
        
        NSDictionary *responseDictionary = [AppleAuthSerializer credentialResponseDictionaryForCredentialState:credentialStateNumber
                                                                                               errorDictionary:errorDictionary];
        
        [[NativeMessageHandler defaultHandler] sendNativeMessageForDictionary:responseDictionary
                                                                 forRequestId:requestId];
    }];
}

- (void) registerCredentialsRevokedCallbackForRequestId:(uint)requestId
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
                                                                               [[NativeMessageHandler defaultHandler] sendNativeMessageForString:@"Credentials Revoked"
                                                                                                                                    forRequestId:requestId];
                                                                           }];
        [self setCredentialsRevokedObserver:observer];
    }
}

#pragma mark Private methods

- (void) performAuthorizationRequestsForController:(ASAuthorizationController *)authorizationController withRequestId:(uint)requestId
{
    NSValue *authControllerAsKey = [NSValue valueWithNonretainedObject:authorizationController];
    [[self authorizationsInProgress] setObject:@(requestId) forKey:authControllerAsKey];
    
    [authorizationController setDelegate:self];
    [authorizationController setPresentationContextProvider:self];
    [authorizationController performRequests];
}

#pragma mark ASAuthorizationControllerDelegate protocol implementation

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
            appleIdCredentialDictionary = [AppleAuthSerializer dictionaryForASAuthorizationAppleIDCredential:(ASAuthorizationAppleIDCredential *)[authorization credential]];
        }
        else if ([[authorization credential] isKindOfClass:[ASPasswordCredential class]])
        {
            passwordCredentialDictionary = [AppleAuthSerializer dictionaryForASPasswordCredential:(ASPasswordCredential *)[authorization credential]];
        }

        NSDictionary *responseDictionary = [AppleAuthSerializer loginResponseDictionaryForAppleIdCredentialDictionary:appleIdCredentialDictionary
                                                                                      passwordCredentialDictionary:passwordCredentialDictionary
                                                                                                   errorDictionary:nil];
        
        [[NativeMessageHandler defaultHandler] sendNativeMessageForDictionary:responseDictionary
                                                                 forRequestId:[requestIdNumber unsignedIntValue]];
        
        [[self authorizationsInProgress] removeObjectForKey:authControllerAsKey];
    }
}

- (void) authorizationController:(ASAuthorizationController *)controller didCompleteWithError:(NSError *)error
{
    NSValue *authControllerAsKey = [NSValue valueWithNonretainedObject:controller];
    NSNumber *requestIdNumber = [[self authorizationsInProgress] objectForKey:authControllerAsKey];
    if (requestIdNumber)
    {
        NSDictionary *errorDictionary = [AppleAuthSerializer dictionaryForNSError:error];
        NSDictionary *responseDictionary = [AppleAuthSerializer loginResponseDictionaryForAppleIdCredentialDictionary:nil
                                                                                         passwordCredentialDictionary:nil
                                                                                                      errorDictionary:errorDictionary];
        
        [[NativeMessageHandler defaultHandler] sendNativeMessageForDictionary:responseDictionary
                                                                 forRequestId:[requestIdNumber unsignedIntValue]];
        
        [[self authorizationsInProgress] removeObjectForKey:authControllerAsKey];
    }
}

#pragma mark ASAuthorizationControllerPresentationContextProviding protocol implementation

- (ASPresentationAnchor) presentationAnchorForAuthorizationController:(ASAuthorizationController *)controller
{
    return [[[UIApplication sharedApplication] delegate] window];
}

@end

#endif

#pragma mark - Native C Calls

bool AppleAuth_IOS_IsCurrentPlatformSupported()
{
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, *))
    {
        return true;
    }
    else
    {
        return false;
    }
}

void AppleAuth_IOS_GetCredentialState(uint requestId, const char* userId)
{
    // IOS/TVOS 13.0 | MACOS 10.15
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, *))
        [[AppleAuthManager sharedManager] getCredentialStateForUser:[NSString stringWithUTF8String:userId] withRequestId:requestId];
    else
        AppleAuth_IOS_SendUnsupportedPlatformCredentialStatusResponse(requestId);
#else
    AppleAuth_IOS_SendUnsupportedPlatformCredentialStatusResponse(requestId);
#endif
}

void AppleAuth_IOS_LoginWithAppleId(uint requestId, int options)
{
    // IOS/TVOS 13.0 | MACOS 10.15
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, *))
        [[AppleAuthManager sharedManager] loginWithAppleId:requestId withOptions:options];
    else
        AppleAuth_IOS_SendUnsupportedPlatformLoginResponse(requestId);
#else
    AppleAuth_IOS_SendUnsupportedPlatformLoginResponse(requestId);
#endif
}

void AppleAuth_IOS_QuickLogin(uint requestId)
{
    // IOS/TVOS 13.0 | MACOS 10.15
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, *))
        [[AppleAuthManager sharedManager] quickLogin:requestId];
    else
        AppleAuth_IOS_SendUnsupportedPlatformLoginResponse(requestId);
#else
    AppleAuth_IOS_SendUnsupportedPlatformLoginResponse(requestId);
#endif
}

void AppleAuth_IOS_RegisterCredentialsRevokedCallbackId(uint requestId)
{
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500
    if (@available(iOS 13.0, tvOS 13.0, macOS 10.15, *))
        [[AppleAuthManager sharedManager] registerCredentialsRevokedCallbackForRequestId:requestId];
#endif
}

void AppleAuth_IOS_SendUnsupportedPlatformCredentialStatusResponse(uint requestId)
{
    NSError *customError = [NSError errorWithDomain:@"com.unity.AppleAuth"
                                               code:-100
                                           userInfo:@{NSLocalizedDescriptionKey : @"Native AppleAuth is only available from iOS 13.0"}];
    
    NSDictionary *customErrorDictionary = [AppleAuthSerializer dictionaryForNSError:customError];
    NSDictionary *responseDictionary = [AppleAuthSerializer credentialResponseDictionaryForCredentialState:nil
                                                                                           errorDictionary:customErrorDictionary];
    
    [[NativeMessageHandler defaultHandler] sendNativeMessageForDictionary:responseDictionary forRequestId:requestId];
}

void AppleAuth_IOS_SendUnsupportedPlatformLoginResponse(uint requestId)
{
    NSError *customError = [NSError errorWithDomain:@"com.unity.AppleAuth"
                                               code:-100
                                           userInfo:@{NSLocalizedDescriptionKey : @"Native AppleAuth is only available from iOS 13.0"}];
    
    NSDictionary *customErrorDictionary = [AppleAuthSerializer dictionaryForNSError:customError];
    NSDictionary *responseDictionary = [AppleAuthSerializer loginResponseDictionaryForAppleIdCredentialDictionary:nil
                                                                                     passwordCredentialDictionary:nil
                                                                                                  errorDictionary:customErrorDictionary];
    
    [[NativeMessageHandler defaultHandler] sendNativeMessageForDictionary:responseDictionary forRequestId:requestId];
}
