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

const char* AppleAuth_CopyCString(const char* string)
{
    if (string == NULL)
        return NULL;
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    
    return res;
}

const char* AppleAuth_GetPersonNameUsingFormatter(const char *payload, int style, bool usePhoneticRepresentation)
{
    if (payload == NULL)
        return NULL;
    
    NSError *error = nil;
    NSData *payloadData = [NSData dataWithBytes:payload length:strlen(payload)];
    NSDictionary * nameComponentsDictionary = [NSJSONSerialization JSONObjectWithData:payloadData options:0 error:&error];
    if (error)
        return NULL;
    
    if (@available(iOS 9.0, tvOS 9.0, macOS 10.11, visionOS 1.0, *)) {
        NSPersonNameComponents *nameData = [[NSPersonNameComponents alloc] init];
        [nameData setNamePrefix:[nameComponentsDictionary objectForKey:@"_namePrefix"]];
        [nameData setGivenName:[nameComponentsDictionary objectForKey:@"_givenName"]];
        [nameData setMiddleName:[nameComponentsDictionary objectForKey:@"_middleName"]];
        [nameData setFamilyName:[nameComponentsDictionary objectForKey:@"_familyName"]];
        [nameData setNameSuffix:[nameComponentsDictionary objectForKey:@"_nameSuffix"]];
        [nameData setNickname:[nameComponentsDictionary objectForKey:@"_nickname"]];
        
        NSDictionary *phoneticRepresentationDictionary = [nameComponentsDictionary objectForKey:@"_phoneticRepresentation"];
        if (phoneticRepresentationDictionary)
        {
            NSPersonNameComponents *phoneticRepresentation = [[NSPersonNameComponents alloc] init];
            [phoneticRepresentation setNamePrefix:[phoneticRepresentationDictionary objectForKey:@"_namePrefix"]];
            [phoneticRepresentation setGivenName:[phoneticRepresentationDictionary objectForKey:@"_givenName"]];
            [phoneticRepresentation setMiddleName:[phoneticRepresentationDictionary objectForKey:@"_middleName"]];
            [phoneticRepresentation setFamilyName:[phoneticRepresentationDictionary objectForKey:@"_familyName"]];
            [phoneticRepresentation setNameSuffix:[phoneticRepresentationDictionary objectForKey:@"_nameSuffix"]];
            [phoneticRepresentation setNickname:[phoneticRepresentationDictionary objectForKey:@"_nickname"]];
            [nameData setPhoneticRepresentation:phoneticRepresentation];
        }
        
        NSPersonNameComponentsFormatterOptions options = usePhoneticRepresentation ? NSPersonNameComponentsFormatterPhonetic : 0;
        NSString *localizedName = [NSPersonNameComponentsFormatter localizedStringFromPersonNameComponents:nameData style:style options:options];
        
        return AppleAuth_CopyCString([localizedName UTF8String]);
    } else {
        return NULL;
    }
}
