Get-ChildItem C:\Repo\WorkerWeb -Recurse -Filter *.csproj | Select-String -Pattern '<ProjectReference'
