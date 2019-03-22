# To start new C# development project in MS Visual Studio to produce app that will use the Scia OpenAPI:
- install SCIA Engineer
- start MS Visual Studio
- create empty C# console app project .NET 4.6.1
- Add reference to SCIA.OpenAPI.dll located in Scia Engineer install folder
- Add reference to EnvESA80.dll located in Scia Engineer install folder and 
- Edit EnvESA80 reference property and switch "Embed Interop Types" to "False" 
- Create new / use configuration for x86 / x64 as needed according to SCIA Engineer Architecture
- Write your application that use the SCIA.OpenAPI functions

# To use this example application:
- install SCIA Engineer
- clone this GIT repo
  - start command line
  - go to desired directory
  - write command "git clone <url_to_this_exmaple_app_repo_that_can_be_found_on_github_project_page>"
- open project in MS Visual Studio
- fix paths to referenced DLLs located in Scia Engineer install folder: SCIA.OpenAPI.dll and EnvESA80.dll
- fix paths in code poinitng to esa.exe location, temp and template project
- select appropriate configuration (x86/x64) as needed according to SCIA Engineer Architecture
- build

# Troubleshooting:
* if you get following exception, just register esa libraries
	* run cmd AS ADMINISTRATOR
	* navigate to Scia Engineer directory
	* run "EP_regsvr32 esa.exe"
```
Unhandled Exception: System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. ---> System.AccessViolationException: Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
```
