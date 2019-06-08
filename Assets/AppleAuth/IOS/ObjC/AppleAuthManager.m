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

static AppleAuthManager *_defaultManager = nil;
static dispatch_once_t defaultManagerInitialization;

@interface AppleAuthManager ()
+ (NSDictionary *) dictionaryForNSError:(NSError *)error;
@end

@implementation AppleAuthManager

+ (id) sharedManager
{
    dispatch_once(&defaultManagerInitialization, ^{
        _defaultManager = [[AppleAuthManager alloc] init];
    });
    
    return _defaultManager;
}

+ (NSDictionary *) dictionaryForNSError:(NSError *)error
{
    if (!error)
        return nil;
    
    return @{
        @"_code" : @([error code]),
        @"_domain" : [error domain],
        @"_localizedDescription" : [error localizedDescription],
        @"_localizedRecoveryOptions" : [error localizedRecoveryOptions],
        @"_localizedRecoverySuggestion" : [error localizedRecoverySuggestion],
        @"_localizedFailureReason" : [error localizedFailureReason],
        @"_userInfo" : [error userInfo],
    };
}

- (void) getCredentialState:(NSString *)userId withRequestId:(uint)requestId
{
    ASAuthorizationAppleIDProvider *provider = [[ASAuthorizationAppleIDProvider alloc] init];
    [provider getCredentialStateForUserID:userId completion:^(ASAuthorizationAppleIDProviderCredentialState credentialState, NSError * _Nullable error) {
        NSDictionary *result = [[NSDictionary alloc] init];
        if (error)
        {
            [result setValue:@NO forKey:@"_success"];
            [result setValue:@(-1) forKey:@"_credentialState"];
            [result setValue:[AppleAuthManager dictionaryForNSError:error] forKey:@"_error"];
        }
        else
        {
            [result setValue:@YES forKey:@"_success"];
            [result setValue:@(credentialState) forKey:@"_credentialState"];
        }
        
        [self sendNativeMessage:result withRequestId:requestId];
    }];
}

- (void) sendNativeMessage:(NSDictionary *)toSerialize withRequestId:(uint)requestId
{
    NSError *error = nil;
    NSData *payloadData = [NSJSONSerialization dataWithJSONObject:toSerialize options:0 error:&error];
    NSString *payloadString = error ? [NSString stringWithFormat:@"Serialization error %@", [error localizedDescription]] : [[NSString alloc] initWithData:payloadData encoding:NSUTF8StringEncoding];
    [[NativeMessageHandler defaultHandler] sendNativeMessage:payloadString forRequestWithId:requestId];
}

@end
