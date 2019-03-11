To start new C# development project in MS Visual Studio to produce app that will use the Scia OpenAPI:
- start MS Visual Studio
- create empty C# console app project .NET 4.6.1
- Add reference to SCIA.OpenAPI.dll in Scia Engineer install folder
- Create reference to EsaENV80.dll and edit property of reference to switch "Embed Interop Types" to "False"
- Create new configuration for x86 / x64 as needed
- Write your application

To use this example application:
- clone GIT repo
- open project in MS Visual Studio
- fix paths to referenced DLLs: SCIA.OpenAPI.dll and EsaENV80.dll
- fix paths in code poinitng to esa.exe location, temp and template project
- select appropriate configuration (x86/x64) and build
