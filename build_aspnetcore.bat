echo off

rem update project.json for version number

call dotnet restore NLog.Web.AspNetCore 
call dotnet pack NLog.Web.AspNetCore --configuration release 