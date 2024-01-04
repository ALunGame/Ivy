@echo off 
for %%d in (%~dp0..) do set ParentDirectory=%%~fd

pushd %1 & for %%i in (.) do set curr=%%~ni

set filename=%date:~0,4%%date:~5,2%%date:~8,2%%time:~0,2%%time:~3,2%%time:~6,2%
set "filename=%filename: =0%"

set src=%cd%
echo 原始工程目录 %src%
set dir=%ParentDirectory%\%curr%_%filename%
echo 要创建的目录 %dir%

set assets="\Assets"
set packages="\Packages"
set library="\Library"
set projectsettings="\ProjectSettings"
set luaActivity="\LuaActivity"
set luaCode="\LuaCode"
set userSettings="\UserSettings"
set xppackages="\IA-Packages"

if not exist %dir% ( md %dir%)

mklink/J %dir%%assets% %src%%assets%
mklink/J %dir%%packages% %src%%packages%
mklink/J %dir%%library% %src%%library%
mklink/J %dir%%projectsettings% %src%%projectsettings%
mklink/J %dir%%luaActivity% %src%%luaActivity%
mklink/J %dir%%luaCode% %src%%luaCode%
mklink/J %dir%%userSettings% %src%%userSettings%
mklink/J %dir%%xppackages% %src%%xppackages%

