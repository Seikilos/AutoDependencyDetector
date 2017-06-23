powershell -Command "Invoke-WebRequest http://www.dependencywalker.com/depends22_x64.zip -OutFile depends.zip"
powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::ExtractToDirectory('depends.zip', 'DependencyWalker'); }"

del depends.zip
