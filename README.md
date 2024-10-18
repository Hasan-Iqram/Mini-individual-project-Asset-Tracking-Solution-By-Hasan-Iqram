This C# console App is an Asset Tracking Solution designed to help users manage assets efficiently by adding, viewing, updating, and deleting assets and generating data as demand. As a simple solution for internal use  Here's how it works and the key design choices:
1. Main Workflow:
Dashboard: When the user runs the application, they are presented with a dashboard showing different options (Add Asset, View Assets, Update Asset, Delete Asset, or Exit).
2. Main Features:
a) Adding Assets:
The user can add an asset by selecting option 1 from the dashboard.
They are prompted to enter the article name, model, quantity, price, and country code.
The application validates the input (e.g., ensuring the quantity is a non-negative integer and the country code is valid).
Each asset is automatically assigned a unique Article Number (e.g., ATS0001), and the asset is added to the assets list.
After adding the asset, the user can choose where to save it:
Default file: Saves in the default assets.json file.
Existing file: Saves to an existing file from the list of JSON files in the Assets folder.
New file: Allows the user to create a new file with a custom name.
b) Viewing Assets:
When viewing assets (option 2), the user first selects a file from the available asset files.
The application loads the assets from the selected file and displays them in a table format using DisplayAssetsTable(). It includes columns for Country, Article Name, Model, Quantity, Price, and Article Number.
c) Updating Assets:
To update an asset, the user selects a file, views the available assets, and then inputs the Article Number of the asset they want to update.
The application prompts for new values for each field and validates the input, then saves the updated asset back to the same file.
d) Deleting Assets:
The delete process works similarly to updating, where the user selects the file, views the assets, and provides the Article Number to delete.
The application asks for confirmation before deleting the asset from the list and saving the changes.
3. Data Persistence:
Assets are saved and loaded from JSON files. The SaveAssets() method serializes the assets list into JSON format, while LoadAssets() deserializes the JSON data back into the assets list.
Multiple asset files can be managed, allowing for flexibility in storing and retrieving assets from different files.
4. Error Handling and Validation:
The application includes robust error handling to deal with potential issues during file reading and writing (e.g., file not found, JSON format errors). Errors are displayed using ShowErrorMessage().
Input validation ensures that the user provides correct values, such as valid integers for quantities, valid decimals for prices, and proper three-letter uppercase country codes.
5. User Experience:
The user interface is built to be simple and interactive, with instructions and error messages clearly communicated.
After each operation, the user is prompted to press any key to return to the dashboard, creating a smooth user flow.
6. File Management:
The application automatically creates a directory called Assets to store JSON files if it doesn’t already exist.
It also lists available asset files and provides the option to save assets to existing or new files, adding flexibility in managing asset data.
Key Highlights of the Design:
Scalability: The application allows for managing multiple asset files and dynamically handles loading/saving based on user input.
Error-Resilience: Exception handling ensures the program continues running even if errors occur during file I/O operations.
Modular Design: The program is structured into small, manageable methods, improving readability, maintainability, and ease of debugging.
This project demonstrates the ability to handle file-based data persistence, input validation, user-friendly console applications, and basic CRUD operations.


UML Diagram 

+--------------------------------+
|           Asset                |
+--------------------------------+
| - ArticleNumber: string        |
| - ArticleName: string          |
| - Model: string                |
| - Quantity: int                |
| - UnitPrice: decimal           |
| - TotalPrice: decimal {get}    |
| - Country: string              |
+--------------------------------+
| + Asset(...)                   |
| + ToString(): string           |
+--------------------------------+

+--------------------------------+
|           Program              |
+--------------------------------+
| - assets: List<Asset>          |
| - nextArticleNumber: int       |
| - defaultDirectory: string     |
+--------------------------------+
| + Main(): void                 |
| + ShowDashboard(): void        |
| + AddAsset(): void             |
| + ViewAssets(): void           |
| + UpdateAsset(): void          |
| + DeleteAsset(): void          |
| + SaveAssets(filePath: string) |
| + LoadAssets(filePath: string) |
| + ConfirmExit(): void          |
| + GetCountryCode(): string     |
| + GetValidInteger(...): int    |
| + GetValidDecimal(...):
