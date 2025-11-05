# Smart Travel Planner

A Windows Forms application for planning optimal travel routes between cities using Dijkstra's shortest path algorithm. The application allows users to create travelers, load city maps, calculate routes, and save/load travel data.

## Features

- **Traveler Management**: Create and manage traveler profiles with names, starting locations, and routes
- **Map Loading**: Load city connection data from text files with distance information
- **Route Planning**: Calculate shortest paths between cities using Dijkstra's algorithm
- **Data Persistence**: Save and load traveler information in JSON format
- **Interactive UI**: User-friendly Windows Forms interface with validation and error handling
- **Route Visualization**: Display planned routes with total distance calculations

## Architecture

The project is structured as follows:

```
SmartTravelPlanner/
├── CityGraphLib/          # Core business logic library
│   ├── CityGraph.cs      # Graph implementation with Dijkstra's algorithm
│   └── Traveler.cs       # Traveler data model and route management
├── UI/                   # Windows Forms user interface
│   ├── Forms/
│   │   ├── MainForm.cs   # Main application window
│   │   └── CalculateForm.cs # Route calculation dialog
│   ├── Helpers/
│   │   ├── FileHelper.cs # File operations and validation
│   │   └── DialogHelper.cs # Standardized message dialogs
│   ├── Program.cs        # Application entry point
│   ├── traveler.json     # Sample traveler data
│   └── map.txt          # Sample city map data
└── MissionControl.sln   # Visual Studio solution file
```

### Key Components

#### CityGraphLib
- **CityGraph**: Implements graph data structure for cities and connections
  - `LoadFromFile(string path)`: Loads city connections from text file
  - `FindShortestPath(string start, string end)`: Calculates shortest path using Dijkstra's
  - `GetPathDistance(List<string> path)`: Calculates total distance of a route
  - `GetCities()`: Returns list of all cities in the graph

- **Traveler**: Data model for travelers with route management
  - Properties: `name`, `currentLocation`, `route`
  - Methods: `SaveToFile()`, `LoadFromFile()`, `PlanRouteTo()`, `AddCity()`, `RemoveCity()`

#### UI Layer
- **MainForm**: Primary interface for traveler creation, map selection, and route display
- **CalculateForm**: Modal dialog for route planning with city selection and path calculation
- **Helpers**: Utility classes for file operations and user dialogs

## Prerequisites

- .NET 9.0 SDK
- Windows operating system
- Visual Studio 2022 or compatible IDE

## Installation

1. Clone or download the project
2. Open `MissionControl.sln` in Visual Studio
3. Build the solution (F6 or Build > Build Solution)
4. Run the application (F5 or Debug > Start Debugging)

## Usage

### Getting Started

1. **Launch the Application**: Run SmartTravelPlanner.exe
2. **Create a Traveler**:
   - Enter traveler name (required)
   - Optionally enter starting city
   - Click "Create Traveler"

3. **Load a Map**:
   - Click "Browse..." next to "Map file (.txt)"
   - Select a text file containing city connections

4. **Plan Routes**:
   - Click "Calculate Route" to open the route planning dialog
   - Select destination city from the list or enter manually
   - Click "Plan Route" to calculate shortest path

### Map File Format

Map files should be plain text files with one connection per line:
```
CityA-CityB,distance
```

Example:
```
Kyiv-Lviv,540
Lviv-Warsaw,390
Kyiv-Odessa,480
```

### Traveler Data Format

Traveler data is stored in JSON format:
```json
{
  "name": "Alice",
  "currentLocation": "Kyiv",
  "route": ["Lviv", "Warsaw", "Kharkiv"]
}
```

## API Reference

### CityGraph Class

```csharp
public class CityGraph
{
    // Load city connections from file
    public static CityGraph LoadFromFile(string filePath)
    
    // Find shortest path between cities
    public List<string> FindShortestPath(string startCity, string endCity)
    
    // Calculate total distance of a path
    public int GetPathDistance(List<string> path)
    
    // Get all cities in the graph
    public List<string> GetCities()
}
```

### Traveler Class

```csharp
public class Traveler
{
    public string name { get; set; }
    public string currentLocation { get; set; }
    public List<string> route { get; set; }
    
    // Constructor
    public Traveler(string name)
    
    // Save traveler data to JSON file
    public void SaveToFile(string filePath)
    
    // Load traveler data from JSON file
    public static Traveler LoadFromFile(string filePath)
    
    // Plan route to destination using CityGraph
    public bool PlanRouteTo(string destination, CityGraph map)
    
    // Route management
    public void AddCity(string city)
    public bool RemoveCity(string city)
    public void ClearRoute()
    
    // Route information
    public string GetRoute()  // Returns "City1 -> City2 -> City3"
    public int GetStopCount()
}
```

## Examples

### Sample Map File (map.txt)
```
Kyiv-Lviv,540
Lviv-Warsaw,390
Kyiv-Odessa,480
Kyiv-Dnipro,300
Dnipro-Lviv,600
```

### Sample Traveler File (traveler.json)
```json
{
  "name": "Alice",
  "currentLocation": "Kyiv",
  "route": ["Lviv", "Warsaw"]
}
```

### Basic Usage in Code

```csharp
// Load a city map
CityGraph map = CityGraph.LoadFromFile("map.txt");

// Create a traveler
Traveler traveler = new Traveler("John");
traveler.SetLocation("Kyiv");

// Plan route to destination
traveler.PlanRouteTo("Warsaw", map);

// Save traveler data
traveler.SaveToFile("traveler.json");
```

## Error Handling

The application includes comprehensive error handling for:
- Invalid map file formats
- Missing or corrupted traveler data files
- Network/graph connectivity issues
- File I/O operations
- User input validation

## Contributing

This is a demonstration project. For modifications:
1. Ensure all changes maintain backward compatibility with existing map and traveler file formats
2. Add appropriate error handling for new features
3. Update this README with any new functionality

## License

This project is provided as-is for educational and demonstration purposes.

## Troubleshooting

### Common Issues

1. **"Map file not found"**: Ensure the selected file exists and is accessible
2. **"Invalid JSON format"**: Check traveler JSON file syntax
3. **"No route found"**: Verify cities exist in map and are connected
4. **Build errors**: Ensure .NET 9.0 SDK is installed and project references are correct

### File Format Validation

- Map files: Each line must be "CityA-CityB,distance" with positive integer distance
- JSON files: Must contain valid traveler object with name and optional route array</content>
<parameter name="filePath">c:\Users\AlexS\innCamous\mission_control\README.md
