# BlazorDualMode
Blazor dual mode with best practices in mind!

In Directory.build.props, you can switch between blazor server and client by using either

```xml
<BlazorMode>Client</BlazorMode>
```

```xml
<BlazorMode>Server</BlazorMode>
```

Note that for Client mode, set BlazorDualMode.Api.csproj as startup project, but for Server mode set both BlazorDualMode.Api.csproj and BlazorDualMode.Web.csproj as startup projects.

In shared project, you can also detect code is running in blazor server or client/wasm modes by use any of followings:

```cs

if (BlazorDualModeDetector.IsRunningServerSideBlazor())
{
}

#if BlazorClient

#endif

```

It's recommended to clear your browser's cache while switching between server and client/wasm modes!