using degreed.Clients;
using degreed.Services;
using degreed.Services.Interfaces;
using degreed.Services.Models;
using degreed.Utils;

// Setup dependencies
ICanHazApiClient apiClient = new CanHazApiClient();
IJokeService jokeService = new JokeService(apiClient);

// Main menu loop
bool exit = false;
while (!exit)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("==================================");
    Console.WriteLine("||      DAD JOKES CONSOLE      ||");
    Console.WriteLine("==================================");
    Console.ResetColor();
    Console.WriteLine();
    Console.WriteLine("1. Get a random joke");
    Console.WriteLine("2. Search for jokes");
    Console.WriteLine("0. Exit");
    Console.WriteLine();
    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine();
    Console.WriteLine();

    switch (choice)
    {
        case "0":
            exit = true;
            break;
        case "1":
            await GetRandomJoke(jokeService);
            break;
        case "2":
            await SearchJokes(jokeService);
            break;
        default:
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid choice. Please try again.");
            Console.ResetColor();
            WaitForKeyPress();
            break;
    }
}

// Helper methods
async Task GetRandomJoke(IJokeService service)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("RANDOM DAD JOKE");
    Console.WriteLine("==============");
    Console.ResetColor();
    
    Console.WriteLine("Fetching a random joke...");
    var jokeResult = await service.GetRandomJoke();
    
    Console.WriteLine();
    if (jokeResult != null)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(jokeResult.joke);
        Console.ResetColor();
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("There was an error retrieving the joke.");
        Console.ResetColor();
    }
    
    Console.WriteLine();
    WaitForKeyPress();
}

async Task SearchJokes(IJokeService service)
{
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("SEARCH FOR DAD JOKES");
    Console.WriteLine("===================");
    Console.ResetColor();
    
    Console.Write("Enter search term(s): ");
    string? searchTerm = Console.ReadLine();
    
    if (string.IsNullOrWhiteSpace(searchTerm))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Search term cannot be empty.");
        Console.ResetColor();
        WaitForKeyPress();
        return;
    }
    
    Console.WriteLine($"\nSearching for jokes containing '{searchTerm}'...");
    var viewModel = await service.SearchJokes(searchTerm);
    
    if (viewModel == null || viewModel.TotalJokes == 0)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("No jokes found matching your search term.");
        Console.ResetColor();
        WaitForKeyPress();
        return;
    }
    
    Console.Clear();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"Found {viewModel.TotalJokes} jokes! (Showing page {viewModel.CurrentPage} of {viewModel.TotalPages})");
    Console.ResetColor();
    Console.WriteLine();
    
    // Display joke categories
    DisplayJokeCategory("SHORT JOKES (< 10 words)", viewModel.ShortJokes);
    DisplayJokeCategory("MEDIUM JOKES (10-19 words)", viewModel.MediumJokes);
    DisplayJokeCategory("LONG JOKES (20+ words)", viewModel.LongJokes);
    
    // Navigation options
    bool browsing = true;
    int currentPage = viewModel.CurrentPage;
    int totalPages = viewModel.TotalPages;
    
    while (browsing && totalPages > 1)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Page {currentPage} of {totalPages}");
        Console.ResetColor();
        Console.WriteLine("N - Next page | P - Previous page | Q - Return to menu");
        Console.Write("Your choice: ");
        
        string? nav = Console.ReadLine()?.ToUpper();
        
        switch (nav)
        {
            case "N":
                if (currentPage < totalPages)
                {
                    currentPage++;
                    viewModel = await service.SearchJokes(searchTerm, currentPage);
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Page {currentPage} of {totalPages} - Found {viewModel?.TotalJokes ?? 0} jokes total");
                    Console.ResetColor();
                    Console.WriteLine();
                    DisplayJokeCategory("SHORT JOKES (< 10 words)", viewModel?.ShortJokes);
                    DisplayJokeCategory("MEDIUM JOKES (10-19 words)", viewModel?.MediumJokes);
                    DisplayJokeCategory("LONG JOKES (20+ words)", viewModel?.LongJokes);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You are already on the last page.");
                    Console.ResetColor();
                }
                break;
            case "P":
                if (currentPage > 1)
                {
                    currentPage--;
                    viewModel = await service.SearchJokes(searchTerm, currentPage);
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Page {currentPage} of {totalPages} - Found {viewModel?.TotalJokes ?? 0} jokes total");
                    Console.ResetColor();
                    Console.WriteLine();
                    DisplayJokeCategory("SHORT JOKES (< 10 words)", viewModel?.ShortJokes);
                    DisplayJokeCategory("MEDIUM JOKES (10-19 words)", viewModel?.MediumJokes);
                    DisplayJokeCategory("LONG JOKES (20+ words)", viewModel?.LongJokes);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You are already on the first page.");
                    Console.ResetColor();
                }
                break;
            case "Q":
                browsing = false;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid choice. Please try again.");
                Console.ResetColor();
                break;
        }
    }
    
    if (totalPages <= 1)
    {
        WaitForKeyPress();
    }
}

void DisplayJokeCategory(string categoryName, List<HighlightedJoke>? jokes)
{
    if (jokes == null || !jokes.Any())
    {
        return;
    }
    
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(categoryName);
    Console.WriteLine(new string('-', categoryName.Length));
    Console.ResetColor();
    
    int index = 1;
    foreach (var joke in jokes)
    {
        // Create a plain text version of the joke with asterisks for highlighting
        string plainText = joke.OriginalJoke;
        if (!string.IsNullOrEmpty(joke.HighlightedText))
        {
            // Replace HTML highlighting with console-friendly version
            string highlighted = joke.HighlightedText
                .Replace("<strong>", "*")
                .Replace("</strong>", "*");
            
            // Display with proper word wrapping
            Console.Write($"{index}. ");
            WriteHighlightedText(highlighted);
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"{index}. {plainText}");
        }
        
        index++;
    }
    
    Console.WriteLine();
}

void WriteHighlightedText(string highlightedText)
{
    // Split the text by the highlight markers (*)
    string[] parts = highlightedText.Split('*');
    
    // Write each part with appropriate coloring
    bool isHighlighted = false;
    foreach (string part in parts)
    {
        if (string.IsNullOrEmpty(part))
        {
            // Toggle highlight state for empty parts (resulting from consecutive markers)
            isHighlighted = !isHighlighted;
            continue;
        }
        
        if (isHighlighted)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(part);
            Console.ResetColor();
        }
        else
        {
            Console.Write(part);
        }
        
        // Toggle highlight state for the next part
        isHighlighted = !isHighlighted;
    }
}

void WaitForKeyPress()
{
    Console.WriteLine("\nPress any key to continue...");
    Console.ReadKey(true);
}