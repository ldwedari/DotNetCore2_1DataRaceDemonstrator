# DotNetCore2_1DataRaceDemonstrator
This shows that .net Core 2.1 may have Data Race conditions on HTTPContext.
It consists of two solutions:
* DotNetCore2_1HttpContextDataRace: is a server were requests http requests are handled creating an additional thread in the Get action of the Values controller.
```csharp
await Task.WhenAny(service.SomethingSlowAsync(), Task.Delay(1000));
```
this causes the request thread to end before the `SomethingSlowAsync()` call that takes 1500 ms finishes.
Causing the HttpContext sometimes to be replaced by any of a concurrent request.
Place a breakpoint in line 54 of file Startup.cs to detect the DataRace.

* MakeThousandsOfConcurrentRequests: this is a client using `HttpClient` to perform many simultaneus request to the server.
You may have to run the code multiple many times as this is still hard to reproduce. A proof is demostrated in this screen capture:

![Screen capture with data race](https://raw.githubusercontent.com/ldwedari/DotNetCore2_1DataRaceDemonstrator/master/ProofOfSwappedContext.png)


This code is based on what @davidfowl posted in issue https://github.com/dotnet/aspnetcore/issues/14975. His code is for .Net Core 3.1, but it also happens in 2.1.

(MIT License)
