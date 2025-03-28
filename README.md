# Degreed Dad Jokes Application

A C# .NET application for searching and displaying dad jokes using the [icanhazdadjoke.com](https://icanhazdadjoke.com/) API.

## Project Structure

The solution consists of three projects:

- **degreed-core**: Contains the core logic, API clients, and services.
- **degreed-take-home-ui**: A Blazor web application that provides a user-friendly interface for searching and viewing jokes.
- **degreed-console-app**: A simple console application that demonstrates using the core API.

## Features

- **Random Joke**: Get a random dad joke.
- **Search Jokes**: Search for dad jokes containing specific terms.
- **Highlighting**: Search terms are highlighted in the joke results.
- **Categorization**: Jokes are categorized by length (short, medium, and long).
- **Pagination**: Navigate through search results with a paginated interface.

## Technical Implementation

- **API Client**: The `CanHazApiClient` connects to the icanhazdadjoke.com API.
- **Service Layer**: The `JokeService` provides business logic for joke retrieval and processing.
- **UI Layer**: Blazor components for displaying jokes with a responsive UI.
- **Extension Methods**: Utilities for grouping jokes by length and highlighting search terms.

## Recent Updates

- Optimized joke highlighting to work only once per joke
- Added namespace refactoring for better organization
- Improved search term highlighting to work with word boundaries and multiple words
- Fixed null handling in service methods

## Getting Started

1. Clone the repository
2. Open the solution in Visual Studio
3. Build and run the degreed-take-home-ui project to see the web interface
4. Alternatively, run the degreed-console-app for a simple console demo

## Technologies Used

- .NET 8
- Blazor Server
- Bootstrap for UI styling
- HttpClient for API communication