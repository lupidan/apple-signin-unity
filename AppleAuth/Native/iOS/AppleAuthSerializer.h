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

// IOS/TVOS 13.0 | MACOS 10.15
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500
#import <AuthenticationServices/AuthenticationServices.h>
#endif

@interface AppleAuthSerializer : NSObject

+ (NSDictionary * _Nullable) dictionaryForNSError:(NSError * _Nullable)error;

+ (NSDictionary * _Nullable) credentialResponseDictionaryForCredentialState:(NSNumber * _Nullable)credentialStateNumber
                                                            errorDictionary:(NSDictionary * _Nullable)errorDictionary;

+ (NSDictionary * _Nullable) loginResponseDictionaryForAppleIdCredentialDictionary:(NSDictionary * _Nullable)appleIdCredentialDictionary
                                                      passwordCredentialDictionary:(NSDictionary * _Nullable)passwordCredentialDictionary
                                                                   errorDictionary:(NSDictionary * _Nullable)errorDictionary;

// IOS/TVOS 9.0 | MACOS 10.11
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 90000 || __TV_OS_VERSION_MAX_ALLOWED >= 90000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101100

+ (NSDictionary * _Nullable) dictionaryForNSPersonNameComponents:(NSPersonNameComponents * _Nullable)nameComponents
API_AVAILABLE(ios(9.0), macos(10.11), tvos(9.0), watchos(2.0));

#endif

// IOS/TVOS 13.0 | MACOS 10.15
#if __IPHONE_OS_VERSION_MAX_ALLOWED >= 130000 || __TV_OS_VERSION_MAX_ALLOWED >= 130000 || __MAC_OS_X_VERSION_MAX_ALLOWED >= 101500

+ (NSDictionary * _Nullable) dictionaryForASAuthorizationAppleIDCredential:(ASAuthorizationAppleIDCredential * _Nullable)appleIDCredential
API_AVAILABLE(ios(13.0), macos(10.15), tvos(13.0), watchos(6.0));

+ (NSDictionary * _Nullable) dictionaryForASPasswordCredential:(ASPasswordCredential * _Nullable)passwordCredential
API_AVAILABLE(ios(13.0), macos(10.15), tvos(13.0), watchos(6.0));

#endif

@end
