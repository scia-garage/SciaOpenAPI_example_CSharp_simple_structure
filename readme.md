# To start new C# development project in MS Visual Studio to produce app that will use the Scia OpenAPI:
- install SCIA Engineer (32/64bit)
- start MS Visual Studio https://visualstudio.microsoft.com/cs/vs/
- create empty C# console app project .NET 4.6.1
- Add reference to SCIA.OpenAPI.dll located in Scia Engineer install folder, edit properties of reference and set Copy Local = False
- Create new / use configuration for x86 / x64 as needed according to SCIA Engineer Architecture
- Write your application that use the SCIA.OpenAPI functions
- copy following DLLs to your application output directory: SCIA.OpenAPI.dll



# To use this example application:
- install SCIA Engineer (32/64bit)
- clone this GIT repo
  - start command line
  - go to desired directory
  - write command "git clone <url_to_this_exmaple_app_repo_that_can_be_found_on_github_project_page>"
- open project in MS Visual Studio
- fix paths to referenced DLL located in your Scia Engineer install folder: SCIA.OpenAPI.dll
- fix paths in code poinitng to Scia Engineer install directory and template file
- select appropriate configuration (x86/x64) as needed according to SCIA Engineer Architecture
- build
- copy following DLLs to your application output directory: SCIA.OpenAPI.dll
- run

# Troubleshooting:
* if you get following exception, just register esa libraries
	* run cmd AS ADMINISTRATOR
	* navigate to Scia Engineer directory
	* run "EP_regsvr32 esa.exe"
```
Unhandled Exception: System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. ---> System.AccessViolationException: Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
```
