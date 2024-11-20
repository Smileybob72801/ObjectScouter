# ObjectScouter

**ObjectScouter** is a console application designed to interact with a RESTful API at [restful-api.dev](https://restful-api.dev/). The app provides an intuitive interface for performing CRUD operations and advanced search functionality on dynamic objects with custom properties.

## Features

- 🌐 **API Integration:** Add, delete, list, and search objects via the REST API.
- ⚡ **Asynchronous Operations:** Ensures the UI thread remains unblocked by simulating background work using built-in delays.
- 🛠️ **Dynamic Object Creation:** Supports runtime creation of objects with custom properties using a `Dictionary`.
- 📚 **Property Management:** Automatically extracts and stores all properties from API items into a `HashSet` for quick and easy searches.

## How It Works

1. **Interactive Console:** The app provides a menu-driven interface for users to interact with the API.
2. **Dynamic Properties:** Users can define custom properties for objects, enabling flexible and extensible data modeling.
3. **Asynchronous Processing:** Background tasks like API calls demonstrate smooth, responsive operation even during heavy workloads.
4. **Efficient Search:** Collects all object properties into a `HashSet`, enabling fast lookups and searches.

## Instructions

### Running the App

Clone the repository:
```bash
git clone https://github.com/YourUsername/ObjectScouter.git
```

Navigate to the project directory:
```bash
cd ObjectScouter
```

Build and run the project:
```bash
dotnet build && dotnet run
```

### Usage

1. **Start the application** and follow the menu prompts.
2. Choose options to:
   - Add a new object with custom properties.
   - List all existing objects in the API.
   - Delete an object by its unique identifier.
   - Search for objects based on property values.
3. View dynamic properties added at runtime for flexible data handling.

## Technologies Used

- **C#** with **.NET Core** for asynchronous programming and dynamic object management.
- **REST API** for data storage and retrieval.

## Future Enhancements

- 🔍 **Advanced Search:** Add support for complex search queries (e.g., AND/OR conditions).
- 📊 **Visualization:** Provide a graphical representation of dynamic object relationships.
- 🌍 **Cross-Platform Support:** Enhance compatibility with non-Windows operating systems.
