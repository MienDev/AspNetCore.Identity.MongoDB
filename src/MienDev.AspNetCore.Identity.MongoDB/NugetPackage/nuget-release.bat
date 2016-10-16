
# move history pack

mv *.nupkg .\history

nuget pack MienDev.AspNetCore.Identity.MongoDB.nuspec

# setApiKey for 'https://www.myget.org/F/miendev/api/v2/package' first
nuget.exe push *.nupkg -source https://www.myget.org/F/miendev/api/v2/package