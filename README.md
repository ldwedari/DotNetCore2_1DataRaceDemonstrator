# DotNetCore2_1DataRaceDemonstrator
This shows that .net Core 2.1 may have Data Race conditions on HTTPContext.

## Prerequisites
Install 64bit asp.net Core SDK 2.1.19. If you don't have it installed the Visual Studio will try to use the next version of asp.net core, ie. 2.2. An you won't be able to reproduce the issue.

## How to run
Load the `DotNetCore2_1DataRaceDemonstrator.sln` file in Visual Studio 2019.
Go to the solution properties dialog and select Multiple startup projects. Select Start action in all the projects of the solution.
Click F5 to start debugging and wait until the application breaks.

## Description
The application has two projects:
* DotNetCore2_1HttpContextDataRace: it is a server were requests http requests are handled creating an additional thread in the Get action of the Values controller.
```csharp
await Task.WhenAny(service.SomethingSlowAsync(), Task.Delay(1000));
```
this causes the request thread to end before the `SomethingSlowAsync()` call that takes 1500 ms finishes.
Causing the HttpContext sometimes to be replaced by any of a concurrent request.

* MakeThousandsOfConcurrentRequests: this is a client using `HttpClient` to perform many simultaneus request to the server.

This image shows how the content provided but HttpContext has been replaced after awaiting 1500 ms (code corresponds to version 1).
![Screen capture with data race](https://raw.githubusercontent.com/ldwedari/DotNetCore2_1DataRaceDemonstrator/master/ProofOfSwappedContext.png)


This code is based on what @davidfowl posted in issue https://github.com/dotnet/aspnetcore/issues/14975. His code is for .net Core 3.1, but it also happens in .net Core 2.1 up to version 2.1.19 (the latest I tried).

## License
MIT License
