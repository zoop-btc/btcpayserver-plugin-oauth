dotnet publish -c Release -o bin/publish/BTCPayServer.Plugins.OAuth
dotnet run --project ../../BTCPayServer.PluginPacker $(pwd)/bin/publish/BTCPayServer.Plugins.OAuth BTCPayServer.Plugins.OAuth $(pwd)/bin/packed
