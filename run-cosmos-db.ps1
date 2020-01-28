$CosmosDBDataPath = $Env:CosmosDBDataPath
 & "C:\Program Files\Azure Cosmos DB Emulator\CosmosDB.Emulator.exe" `
	/DataPath=$CosmosDBDataPath `
	/PartitionCount=100