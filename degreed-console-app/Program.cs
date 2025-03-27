using degreed_core.Clients;

Console.WriteLine("Hello, World!");
var client = new CanHazApiClient();
var resultRandom = await client.Random();
Console.WriteLine(resultRandom?.joke ?? "There was an error retrieving the joke...");
var resultSearch = await client.Search("cat","dogs","space");
Console.WriteLine(resultRandom!=null ? string.Join("\r\n",resultSearch.results.Select(x => x.joke)) : "There was an error retrieving the joke...");
Console.WriteLine("Done...");