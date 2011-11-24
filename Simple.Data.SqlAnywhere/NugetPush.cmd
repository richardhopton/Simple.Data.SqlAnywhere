set package=%1
if "%package%"=="" set /p package="Enter package number: "
nuget push Simple.Data.SqlAnywhere.%package%.nupkg
