# ZoomJWT
C# Library (.NET Framework) for creating Java Web Tokens (JWTs) for use by Zoom Apps to authenticate to the various Zoom APIs/SDKs.

Generating these tokens can be a bit tricky.  At the time this library was written, the information on how to generate these tokens was not available in any one place, and a reliable implementation in C# could not be found.  Thus, I created this library for my own use and figured I'd share it with others.

Eseentially, this project is an amalgamation of the following:
* https://marketplace.zoom.us/docs/guides/auth/jwt
* https://github.com/zoom/zoom-sdk-windows/blob/master/CHANGELOG.md#new-sdk-initialization-method-using-jwt-token
* https://devforum.zoom.us/t/zoom-sdk-initialize-failed-when-running-the-sample-project/26511
* https://devforum.zoom.us/t/how-to-create-jwt-token-using-rest-api-in-c/6620

This library was written primarily for use with the [Zoom Client SDK C# Wrapper](https://marketplace.zoom.us/docs/sdk/native-sdks/windows/c-sharp-wrapper) as part of my [In-Meeting Zoom Bot SDK](https://github.com/apresence/ZoomMeetingBotSDK) project.  It can be used to generate valid JWTs for any Zoom APIs, and those generated JWTs can be used from any programming language.  The CreateToken() function should even work with other applications besides Zoom, but this has not been tested.

To use, compile the DLL and add a reference to it in your C# project.  Then, use the library to generate and use a token (A key and secret are required, please consult the appropriate Zoom [Client SDK](https://marketplace.zoom.us/docs/sdk/native-sdks/introduction) or [API](https://marketplace.zoom.us/docs/guides/auth/jwt#key-secret) documentation for details).

Here is some example code for use with the Zoom Client SDK C# Wrapper:
```C#
using static ZoomJWT.CZoomJWT;

...

    sdkErr = CZoomSDKeDotNetWrap.Instance.GetAuthServiceWrap().SDKAuth(new AuthContext()
    {
        jwt_token = CreateClientSDKToken(YOUR_SDK_KEY, YOUR_SDK_SECRET),
    });
```

If you need to use the Zoom API, it requires a different type of token.  Example:
```C#
using static ZoomJWT.CZoomJWT;

...

    Console.WriteLine("Your API token is: " + CreateAPIToken(YOUR_API_KEY, YOUR_API_SECRET));
```

Contributions by the community are welcome!
