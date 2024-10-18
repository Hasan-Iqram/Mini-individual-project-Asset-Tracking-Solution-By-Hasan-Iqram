using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class Asset
{
    public string ArticleNumber { get; } // Auto-generate Article number
    public string ArticleName { get; set; }
    public string Model { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Unit Price of the asset
    public decimal TotalPrice => Quantity * UnitPrice; // Automatically calculated as Quantity * UnitPrice
    public string Country { get; set; }

    public Asset(string articleNumber, string articleName, string model, int quantity, decimal unitPrice, string country) // Constructor for creating assets.
    {
        ArticleNumber = articleNumber ?? throw new ArgumentNullException(nameof(articleNumber));
        ArticleName = articleName ?? throw new ArgumentNullException(nameof(articleName));
        Model = model ?? throw new ArgumentNullException(nameof(model));
        Quantity = quantity;
        UnitPrice = unitPrice; // Assign the UnitPrice
        Country = country ?? throw new ArgumentNullException(nameof(country)); // 3 uppercase letters, e.g., 'SWE'
    }

    public override string ToString()
    {
        return $"{ArticleNumber} | {ArticleName} | {Model} | {Quantity} | {UnitPrice:C} | {TotalPrice:C} | {Country}";
    }
}

class Program
{
    static List<Asset> assets = new List<Asset>();
    static int nextArticleNumber = 1;
    const string defaultDirectory = "Assets";

    static void Main(string[] args)
    {
        Directory.CreateDirectory(defaultDirectory);
        LoadAssets($"{defaultDirectory}/assets.json");

        bool running = true;
        while (running)
        {
            Console.Clear();
            ShowDashboard();
            Console.Write("Choose an option: ");
            string? option = Console.ReadLine();

            switch (option)
            {
                case "1": AddAsset(); break;
                case "2": ViewAssets(); break;
                case "3": UpdateAsset(); break;
                case "4": DeleteAsset(); break;
                case "5": running = false; break;
                default: Console.WriteLine("Invalid option. Please try again."); break;
            }
        }

        SaveAssets($"{defaultDirectory}/assets.json");
    }

    static void LoadAssets(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                assets = JsonSerializer.Deserialize<List<Asset>>(json) ?? new List<Asset>();
                nextArticleNumber = assets.Count > 0 ? int.Parse(assets[^1].ArticleNumber[3..]) + 1 : 1;
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error loading assets: {ex.Message}");
        }
    }

    static void SaveAssets(string filePath)
    {
        try
        {
            string json = JsonSerializer.Serialize(assets, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error saving assets: {ex.Message}");
        }
    }

    static void CenteredPrint(string text)
    {
        int consoleWidth = Console.WindowWidth;
        int padding = (consoleWidth - text.Length) / 2;
        Console.WriteLine(new string(' ', padding) + text);
    }

    static void ShowDashboard()
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        CenteredPrint("ASSETS TRACKING SOLUTION BY WALY");
        Console.WriteLine(new string('-', Console.WindowWidth));
        CenteredPrint("YOUR OPTIONS ARE BELOW");
        Console.WriteLine(new string('-', Console.WindowWidth));
        CenteredPrint("[1] Add Asset  |  [2] View Assets  |  [3] Update Asset  |  [4] Delete Asset  |  [5] Exit");
        Console.WriteLine(new string('-', Console.WindowWidth));
        Console.ResetColor();
    }

    static void AddAsset()
    {
        try
        {
            string country = GetCountryCode(); // Use the new method
            string articleName = GetNonEmptyInput("Enter article name: ");
            string model = GetNonEmptyInput("Enter model: ");
            int quantity = GetValidInteger("Enter quantity: ", "Invalid input. Please enter a valid quantity (non-negative integer).");
            decimal unitPrice = GetValidDecimal("Enter unit price: ", "Invalid input. Please enter a valid unit price (non-negative decimal).");

            string formattedArticleNumber = $"ATS{nextArticleNumber:D4}";
            assets.Add(new Asset(formattedArticleNumber, articleName, model, quantity, unitPrice, country));
            nextArticleNumber++;
            Console.WriteLine("Asset added successfully.");

            Console.WriteLine("How would you like to save the assets?");
            Console.WriteLine("[1] Save to Default File");
            Console.WriteLine("[2] Save to Existing File");
            Console.WriteLine("[3] Save to New File");

            string choice = Console.ReadLine();
            string filePath;

            switch (choice)
            {
                case "1":
                    SaveAssets($"{defaultDirectory}/assets.json");
                    Console.WriteLine("Assets saved successfully to assets.json.");
                    break;
                case "2":
                    SaveToExistingFile();
                    break;
                case "3":
                    Console.Write("Enter the name of the new file (without extension): ");
                    string newFileName = GetNonEmptyInput("Enter file name: ") + ".json";
                    filePath = Path.Combine(defaultDirectory, newFileName);
                    SaveAssets(filePath);
                    break;
                default:
                    Console.WriteLine("Invalid choice, saving to default file.");
                    SaveAssets($"{defaultDirectory}/assets.json");
                    break;
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error adding asset: {ex.Message}");
        }
        PressAnyKeyToContinue();
    }

    static void SaveToExistingFile()
    {
        string[] jsonFiles = Directory.GetFiles(defaultDirectory, "*.json");
        if (jsonFiles.Length == 0)
        {
            ShowErrorMessage("No asset files found to save to.");
            return;
        }

        Console.WriteLine("Available asset files:");
        for (int i = 0; i < jsonFiles.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {Path.GetFileName(jsonFiles[i])}");
        }

        int fileChoice = GetValidInteger("Choose a file by number: ", "Invalid input. Please enter a valid file number.");

        if (fileChoice < 1 || fileChoice > jsonFiles.Length)
        {
            ShowErrorMessage("Invalid choice. Please choose a valid file number.");
            return;
        }

        SaveAssets(jsonFiles[fileChoice - 1]);
        Console.WriteLine("Assets saved successfully.");
    }

    static void ViewAssets()
    {
        Console.Clear();
        ShowDashboard();

        string[] jsonFiles = Directory.GetFiles(defaultDirectory, "*.json");
        if (jsonFiles.Length == 0)
        {
            ShowErrorMessage("No asset files found. Please add an asset file first.");
            PressAnyKeyToContinue();
            return;
        }

        Console.WriteLine("Available asset files:");
        for (int i = 0; i < jsonFiles.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {Path.GetFileName(jsonFiles[i])}");
        }

        int fileChoice = GetValidInteger("Choose a file by number: ", "Invalid input. Please enter a valid file number.");

        if (fileChoice < 1 || fileChoice > jsonFiles.Length)
        {
            ShowErrorMessage("Invalid choice. Please choose a valid file number.");
            PressAnyKeyToContinue();
            return;
        }

        LoadAssets(jsonFiles[fileChoice - 1]);

        if (assets.Count == 0)
        {
            ShowErrorMessage("No assets found in the selected file.");
            PressAnyKeyToContinue();
            return;
        }

        DisplayAssetsTable();
        PressAnyKeyToContinue();
    }

    static void DisplayAssetsTable()
    {
        // Set column widths
        const int CountryWidth = 10;
        const int ArticleNameWidth = 20;
        const int ModelWidth = 10;
        const int QuantityWidth = 8;
        const int UnitPriceWidth = 15;
        const int TotalPriceWidth = 15;
        const int ArticleNumberWidth = 15;

        // Print table header
        Console.WriteLine(new string('-', CountryWidth + ArticleNameWidth + ModelWidth + QuantityWidth + UnitPriceWidth + TotalPriceWidth + ArticleNumberWidth + 20));
        Console.WriteLine($"| {"Country",-CountryWidth} | {"Article Name",-ArticleNameWidth} | {"Model",-ModelWidth} | {"Quantity",-QuantityWidth} | {"Unit Price",-UnitPriceWidth} | {"Total Price",-TotalPriceWidth} | {"Article Number",-ArticleNumberWidth} |");
        Console.WriteLine(new string('-', CountryWidth + ArticleNameWidth + ModelWidth + QuantityWidth + UnitPriceWidth + TotalPriceWidth + ArticleNumberWidth + 20));

        // Print each asset row
        foreach (var asset in assets)
        {
            Console.WriteLine($"| {asset.Country,-CountryWidth} | {asset.ArticleName,-ArticleNameWidth} | {asset.Model,-ModelWidth} | {asset.Quantity,-QuantityWidth} | {asset.UnitPrice,-UnitPriceWidth:C} | {asset.TotalPrice,-TotalPriceWidth:C} | {asset.ArticleNumber,-ArticleNumberWidth} |");
        }

        // Print table footer
        Console.WriteLine(new string('-', CountryWidth + ArticleNameWidth + ModelWidth + QuantityWidth + UnitPriceWidth + TotalPriceWidth + ArticleNumberWidth + 20));
    }

    static void UpdateAsset()
    {
        Console.Clear();
        ShowDashboard();

        string[] jsonFiles = Directory.GetFiles(defaultDirectory, "*.json");
        if (jsonFiles.Length == 0)
        {
            ShowErrorMessage("No asset files found. Please add an asset file first.");
            PressAnyKeyToContinue();
            return;
        }

        // Ask user to choose which file to work with
        Console.WriteLine("Available asset files:");
        for (int i = 0; i < jsonFiles.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {Path.GetFileName(jsonFiles[i])}");
        }

        int fileChoice = GetValidInteger("Choose a file by number: ", "Invalid input. Please enter a valid file number.");
        if (fileChoice < 1 || fileChoice > jsonFiles.Length)
        {
            ShowErrorMessage("Invalid choice. Please choose a valid file number.");
            PressAnyKeyToContinue();
            return;
        }

        // Load the selected file
        LoadAssets(jsonFiles[fileChoice - 1]);

        if (assets.Count == 0)
        {
            ShowErrorMessage("No assets found in the selected file.");
            PressAnyKeyToContinue();
            return;
        }

        // Display assets table before updating
        DisplayAssetsTable();

        string articleNumber = GetNonEmptyInput("Enter article number to update (e.g., ATS0001): ");
        var asset = assets.Find(a => string.Equals(a.ArticleNumber, articleNumber, StringComparison.OrdinalIgnoreCase));

        if (asset == null)
        {
            ShowErrorMessage("Invalid article number. Please choose a valid asset.");
            PressAnyKeyToContinue();
            return;
        }

        try
        {
            asset.Country = GetCountryCode();
            asset.ArticleName = GetNonEmptyInput("Enter new article name: ");
            asset.Model = GetNonEmptyInput("Enter new model: ");
            asset.Quantity = GetValidInteger("Enter new quantity: ", "Invalid input. Please enter a valid quantity (non-negative integer).");
            asset.UnitPrice = GetValidDecimal("Enter new unit price: ", "Invalid input. Please enter a valid price (non-negative decimal).");

            Console.WriteLine("Asset updated successfully.");
            SaveAssets(jsonFiles[fileChoice - 1]);  // Save changes to the selected file
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error updating asset: {ex.Message}");
        }

        PressAnyKeyToContinue();
    }


    static void DeleteAsset()
    {
        Console.Clear();
        ShowDashboard();

        string[] jsonFiles = Directory.GetFiles(defaultDirectory, "*.json");
        if (jsonFiles.Length == 0)
        {
            ShowErrorMessage("No asset files found. Please add an asset file first.");
            PressAnyKeyToContinue();
            return;
        }

        // Ask user to choose which file to work with
        Console.WriteLine("Available asset files:");
        for (int i = 0; i < jsonFiles.Length; i++)
        {
            Console.WriteLine($"[{i + 1}] {Path.GetFileName(jsonFiles[i])}");
        }

        int fileChoice = GetValidInteger("Choose a file by number: ", "Invalid input. Please enter a valid file number.");
        if (fileChoice < 1 || fileChoice > jsonFiles.Length)
        {
            ShowErrorMessage("Invalid choice. Please choose a valid file number.");
            PressAnyKeyToContinue();
            return;
        }

        // Load the selected file
        LoadAssets(jsonFiles[fileChoice - 1]);

        if (assets.Count == 0)
        {
            ShowErrorMessage("No assets found in the selected file.");
            PressAnyKeyToContinue();
            return;
        }

        // Display assets table before deleting
        DisplayAssetsTable();

        string articleNumber = GetNonEmptyInput("Enter article number to delete (e.g., ATS0001): ");
        var asset = assets.Find(a => string.Equals(a.ArticleNumber, articleNumber, StringComparison.OrdinalIgnoreCase));

        if (asset == null)
        {
            ShowErrorMessage("Invalid article number. Please choose a valid asset.");
            PressAnyKeyToContinue();
            return;
        }

        DisplayAssetDetails(asset);

        Console.Write("Are you sure you want to delete this asset? (y/n): ");
        string? confirmation = Console.ReadLine()?.Trim().ToLower();

        if (confirmation == "y")
        {
            assets.Remove(asset);
            Console.WriteLine("Asset deleted successfully.");
            SaveAssets(jsonFiles[fileChoice - 1]);  // Save changes to the selected file
        }
        else
        {
            Console.WriteLine("Deletion canceled.");
        }

        PressAnyKeyToContinue();
    }


    private static object DisplayAssetDetails(Asset asset)
    {
        throw new NotImplementedException();
    }

    static string GetCountryCode()
    {
        while (true)
        {
            string country = GetNonEmptyInput("Enter country code (3 uppercase letters, e.g., SWE): ").ToUpper();
            if (country.Length == 3 && country.All(char.IsLetter))
            {
                return country;
            }
            Console.WriteLine("Invalid country code. Please enter a valid 3-letter code.");
        }
    }

    static int GetValidInteger(string prompt, string errorMessage)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value >= 0)
            {
                return value;
            }
            Console.WriteLine(errorMessage);
        }
    }

    static decimal GetValidDecimal(string prompt, string errorMessage)
    {
        while (true)
        {
            Console.Write(prompt);
            if (decimal.TryParse(Console.ReadLine(), out decimal value) && value >= 0)
            {
                return value;
            }
            Console.WriteLine(errorMessage);
        }
    }

    static string GetNonEmptyInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            Console.WriteLine("Input cannot be empty. Please try again.");
        }
    }

    static void ShowErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    static void PressAnyKeyToContinue()
    {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}