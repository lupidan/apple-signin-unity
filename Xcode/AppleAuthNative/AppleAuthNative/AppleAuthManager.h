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

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

typedef NS_OPTIONS(int, AppleAuthManagerLoginOptions) {
    AppleAuthManagerIncludeName = 1 << 0,
    AppleAuthManagerIncludeEmail = 1 << 1,
};

typedef void (*NativeMessageHandlerDelegate)(uint requestId,  const char* payload);

@interface AppleAuthManager : NSObject

+ (instancetype) sharedManager;

- (void) quickLogin:(uint)requestId withNonce:(NSString *)nonce andState:(NSString *)state;
- (void) loginWithAppleId:(uint)requestId withOptions:(AppleAuthManagerLoginOptions)options nonce:(NSString *)nonce andState:(NSString *)state;
- (void) getCredentialStateForUser:(NSString *)userId withRequestId:(uint)requestId;
- (void) registerCredentialsRevokedCallbackForRequestId:(uint)requestId;

@end

bool AppleAuth_IsCurrentPlatformSupported(void);
void AppleAuth_SetupNativeMessageHandlerCallback(NativeMessageHandlerDelegate callback);
void AppleAuth_GetCredentialState(uint requestId, const char* userId);
void AppleAuth_LoginWithAppleId(uint requestId, int options, const char* _Nullable nonceCStr, const char* _Nullable stateCStr);
void AppleAuth_QuickLogin(uint requestId, const char* _Nullable nonceCStr, const char* _Nullable stateCStr);
void AppleAuth_RegisterCredentialsRevokedCallbackId(uint requestId);
void AppleAuth_LogMessage(const char* _Nullable messageCStr);

NS_ASSUME_NONNULL_END
