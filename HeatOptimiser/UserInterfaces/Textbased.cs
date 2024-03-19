namespace HeatOptimiser
{
    public class TextBasedUI
    {
        private readonly SourceDataManager sourceDataManager;
        private readonly Optimiser optimiser;
        private readonly SourceData sourceData;
        public AssetManager assetManager;


         public TextBasedUI()
        {
            sourceDataManager = new SourceDataManager();
            sourceData = new SourceData();
            assetManager = new AssetManager();
            optimiser = new Optimiser(sourceDataManager, assetManager);
        }
        public void Interface()
        {

            while (true)
            {
                Console.WriteLine("\nHello, User");
                Console.WriteLine("Type Number To Select:");
                Console.WriteLine("1. Add Unit Option");
                Console.WriteLine("2. Edit Unit");
                Console.WriteLine("3. Delete Unit");
                Console.WriteLine("4. Save Units");
                Console.WriteLine("5. Optimise Schedule");
                Console.WriteLine("6. Exit");

                string? userInput = Console.ReadLine();
     
                switch (userInput)
                {

                    case "1":
                        Console.WriteLine("Selected: Add Unit");

                        Console.WriteLine("Enter unit name:");
                        string? name = Console.ReadLine();

                        Console.WriteLine("Enter unit image path:");
                        string? image = Console.ReadLine();

                         if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(image))
                    {
                        Console.WriteLine("Enter unit heat:");
                        double heat;
                        while (!double.TryParse(Console.ReadLine(), out heat))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number for heat:");
                        }
                        Console.WriteLine("Enter unit electricity:");
                        double electricity;
                        while (!double.TryParse(Console.ReadLine(), out electricity))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number for electricity:");
                        }
                        Console.WriteLine("Enter unit energy:");
                        double energy;
                        while (!double.TryParse(Console.ReadLine(), out energy))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number for energy:");
                        }
                        Console.WriteLine("Enter unit cost:");
                        double cost;
                        while (!double.TryParse(Console.ReadLine(), out cost))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number for cost:");
                        }
                        Console.WriteLine("Enter unit carbon dioxide:");
                        double carbonDioxide;
                        while (!double.TryParse(Console.ReadLine(), out carbonDioxide))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid number for carbon dioxide:");
                        }
                        assetManager.AddUnit(name, image, heat, electricity, energy, cost, carbonDioxide);
                    }
                    else
                    {
                        Console.WriteLine("Name and Image cannot be empty or null. Unit not added.");
                    }
                        break;

                    case "2":
                        Console.WriteLine("Selected: Edit Unit");

                         Console.WriteLine("Enter Unit ID:");
                         Guid id;
                        while (!Guid.TryParse(Console.ReadLine(), out id))
                        {
                            Console.WriteLine("Invalid GUID. Enter a valid Unit ID");
                        }
                        Console.WriteLine("Enter property index to edit (0: Name, 1: Image, 2: Heat, 3: Electricity, 4: Energy, 5: Cost, 6: CarbonDioxide):");
                        int index;
                        while(!int.TryParse(Console.ReadLine(), out index) || index < 0 || index > 6)
                        {
                            Console.WriteLine("Invalid input. Please enter a number from 0 - 6");
                        }
                
                        Console.WriteLine("Enter new value:");
                        string? value = Console.ReadLine();
                        try
                        {
                            if(index >= 0 && index <= 1)
                            {
                                assetManager.EditUnit(id , index , value!);
                            }
                            else if (index >=2 && index <= 6)
                            {
                                double doubleValue;
                                while(!double.TryParse(value, out doubleValue))
                                {
                                     Console.WriteLine("Invalid input. Please enter a valid number:");
                                     value = Console.ReadLine();
                                }
                                 assetManager.EditUnit(id, index, doubleValue);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                        break;
                    case "3":
                        Console.WriteLine("Selected: Delete Unit");
                        Console.WriteLine("Enter Unit ID");
                        Guid deleteId;
                        while(!Guid.TryParse(Console.ReadLine(), out deleteId))
                        {
                            Console.WriteLine("Invalid GUID. Enter a valid Unit ID:");
                        }
                        try
                        {
                            assetManager.DeleteUnit(deleteId);
                             Console.WriteLine("Unit deleted successfully.");
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                        break;

                    case "4": 
                    Console.WriteLine("Selected: Save Units");
                    Console.WriteLine("Enter file name to save units:");
                    string? fileName = Console.ReadLine();
                    try
                    {
                        assetManager.SaveUnits(assetManager.GetAllUnits(), fileName!);
                        Console.WriteLine("Units saved successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                    break;
                    case "5":
                        Console.WriteLine("Selected: Optimise Schedule");
                        Console.WriteLine("Enter start date (dd/MM/yyyy):");
                        DateTime startDate;
                        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out startDate))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid date (dd/MM/yyyy):");
                        }
                        Console.WriteLine("Enter end date (dd/MM/yyyy):");
                        DateTime endDate;
                        while (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out endDate))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid date (dd/MM/yyyy):");
                        }
                        Schedule schedule = optimiser.Optimise(startDate, endDate);
                        DisplaySchedule(schedule);
                        break;

                    case "6":
                        Console.WriteLine("Exiting...");
                        return; 
                    default:
                        Console.WriteLine("Invalid input. Please select a valid option.");
                        break;
                }
            }
        }
    
                private void DisplaySchedule(Schedule schedule)
                 {
            Console.WriteLine("Optimised Schedule:");
            foreach (var hour in schedule.schedule)
            {
                Console.WriteLine($"Hour: {hour.Hour}, Assets: {string.Join(",", hour.Assets)}, Demands: {string.Join(",", hour.Demands)}");
            }
                }
}
}