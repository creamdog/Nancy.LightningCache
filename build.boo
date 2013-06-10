solution_file = "Nancy.LightningCache.sln"
configuration = "release"
nextVersion = "0.1.1"

target default, (updateVersion, compile, nuget):
	pass

target updateVersion:  
  exec("tools\\UpdateVersion.exe", "-p ${nextVersion} -i .\\Nancy.LightningCache\\Properties\\AssemblyInfo.cs -o .\\Nancy.LightningCache\\Properties\\AssemblyInfo.cs")

target compile:
	msbuild(file: solution_file, configuration: configuration)

target nuget:
	exec("Tools\\NuGet.exe", "pack .\\Nancy.LightningCache\\Nancy.LightningCache.csproj.nuspec -Version ${nextVersion} ");