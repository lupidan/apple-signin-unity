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

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

// IOS/TVOS 13.0 | MACOS 10.15
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500

typedef NS_OPTIONS(int, AppleAuthManagerLoginOptions) {
    AppleAuthManagerIncludeName = 1 << 0,
    AppleAuthManagerIncludeEmail = 1 << 1,
} API_AVAILABLE(ios(13.0), macos(10.15), tvos(13.0), watchos(6.0));

API_AVAILABLE(ios(13.0), macos(10.15), tvos(13.0), watchos(6.0))
@interface AppleAuthManager : NSObject

+ (instancetype) sharedManager;

- (void) loginSilently:(uint)requestId;
- (void) loginWithAppleId:(uint)requestId withOptions:(AppleAuthManagerLoginOptions)options;
- (void) getCredentialStateForUser:(NSString *)userId withRequestId:(uint)requestId;

@end

#endif

bool AppleAuth_IOS_IsCurrentPlatformSupported();
void AppleAuth_IOS_GetCredentialState(uint requestId, const char* userId);
void AppleAuth_IOS_LoginWithAppleId(uint requestId, int options);
void AppleAuth_IOS_LoginSilently(uint requestId);
void AppleAuth_IOS_SendUnsupportedPlatformCredentialStatusResponse(uint requestId);
void AppleAuth_IOS_SendUnsupportedPlatformLoginResponse(uint requestId);

NS_ASSUME_NONNULL_END
