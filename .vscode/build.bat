echo off

REM remove unnecessary assemblies
DEL .\*\Assemblies\*.*

REM build dll
dotnet build Source/1.5/