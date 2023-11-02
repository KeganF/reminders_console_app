using static System.Console;
using static System.IO.Directory;
using static System.IO.Path;
using static System.Environment;
using ConsoleIOManager;
using RemindersLib;
using Newtonsoft.Json;
namespace Program;

//-----------------------------V1.4-KeganF------------------------------//
// CLASS > Program                                                      //
//         Contains the Main method for the Reminders Console App       //
//         Allows users to manage their reminders via the console       //
//----------------------------------------------------------------------//
public class Program
{
    //------------------------------v-Fields-v------------------------------//
    private static readonly string dirPath = Combine(GetFolderPath(SpecialFolder.Personal), "Reminders");
    private static readonly string filePath = Combine(dirPath, "reminders.json");
    private static List<Reminder> reminders = new ();

    public static void Main(string[] args)
    {
        DeserializeJson();
        DeleteExpiredReminders();
        
        // Set operating mode based on command line args
        string mode = "";
        if (args.Length > 0)
        {
            mode = args[0].ToLower();
        }

        switch (mode)
        {
            // TODO - Add a case to display a full menu of options
            // (Don't want to display a full menu by default, 
            // could be annoying if you have to close a menu everytime you open a CLI)
            // case "m":
            // case "menu":
            //     DisplayMenu();
            case "n":
            case "new":
                Add();
                break;
            case "d":
            case "delete":
                Delete();
                break;
            case "v":
            case "verbose":
                DisplayReminders();
                break;
            default:
                DisplayShortReminders();
                break;
        }

        SerializeJson();
    }

    //-----------------------------v-Methods-v------------------------------//
    //----------------------------------------------------------------------//
    // METHOD > Add                                                         //          
    //          Creates a new Reminder object constructed with values given //
    //          by the user                                                 //             
    //                                                                      //
    // PARAMS > none                                                        //
    //                                                                      //
    // RETURN > void                                                        //
    //----------------------------------------------------------------------//
    public static void Add()
    {
        bool isDone = false;
        do {
            // Set title and description
            Write("Set a title for your reminder > ");
            string title = ReadLine() ?? "";
            Write("Set a description for your reminder > ");
            string desc = ReadLine() ?? "";

            // Set date and time
            DateOnly date = DateOnly.FromDateTime(
                ConsoleManager.GetConvertedInput<DateTime>("Set a date for this reminder "));
            TimeOnly time = TimeOnly.FromDateTime(
                ConsoleManager.GetConvertedInput<DateTime>("Set a time for this reminder "));

            // Check for dates in the past
            if (DateOnly.FromDateTime(DateTime.Now) > date)
            {
                bool isAcceptedDate = ConsoleManager.DisplayYesNo(
                    $"{date} has already passed. Do you want to finish creating this reminder?");

                if (!isAcceptedDate)
                {
                    ConsoleManager.WriteLineColored("This reminder has been cancelled!", ConsoleColor.Red);
                    return;
                }
            }
            
            bool isAutoDel = ConsoleManager.DisplayYesNo(
                $"Do you want this reminder to be automatically deleted after {date} at {time}?");

            Reminder r = new (title, desc, date, time, isAutoDel);
            reminders.Add(r);

            // Prompt user if they would like to continue adding records
            isDone = !ConsoleManager.DisplayYesNo("Do you want to create another reminder?");

        } while (!isDone);
    }

    //----------------------------------------------------------------------//
    // METHOD > Delete                                                      //          
    //          Allows the user to manually select and delete reminders     //
    //                                                                      //
    // PARAMS > none                                                        //
    //                                                                      //
    // RETURN > void                                                        //
    //----------------------------------------------------------------------//
    public static void Delete()
    {
        bool isDone = false;
        do
        {
            // Create array of reminder titles
            string[] remindersArray = new string[reminders.Count];
            for (int i = 0; i < reminders.Count; i++)
            {
                remindersArray[i] = reminders[i].Title;
            }

            // Display array of reminder titles for the user to choose from
            int selectedIndex = ConsoleManager.DisplayCheckBoxes(remindersArray,
                "Select a reminder to delete:");
            // Remove the reminder at the selected index from the list
            reminders.RemoveAt(selectedIndex);

            // Prompt user if they would like to continue deleting records
            isDone = !ConsoleManager.DisplayYesNo("Do you want to delete more reminders?");
                
        } while (!isDone && reminders.Count > 0);
    }

    //----------------------------------------------------------------------//
    // METHOD > DisplayShortReminders                                       //          
    //          Displays a list of all current reminders in a brief/compact //
    //          format                                                      //
    //                                                                      //
    // PARAMS > none                                                        //
    //                                                                      //
    // RETURN > void                                                        //
    //----------------------------------------------------------------------//
    public static void DisplayShortReminders()
    {
        foreach(Reminder r in reminders)
        {
            r.DisplayShort();
        }
    }

    //----------------------------------------------------------------------//
    // METHOD > DisplayReminders                                            //          
    //          Displays a list of all current reminders in a detailed      //
    //          format                                                      //
    //                                                                      //
    // PARAMS > none                                                        //
    //                                                                      //
    // RETURN > void                                                        //
    //----------------------------------------------------------------------//
    public static void DisplayReminders()
    {
        foreach (Reminder r in reminders)
        {
            r.Display();
        }
    }

    //----------------------------------------------------------------------//
    // METHOD > DeleteExpiredReminders                                      //          
    //          Removes Reminder objects from the reminders List if the     //
    //          Reminder has expired and has IsAutoDel set to true          //
    //                                                                      //
    // PARAMS > none                                                        //
    //                                                                      //
    // RETURN > void                                                        //
    //----------------------------------------------------------------------//
    public static void DeleteExpiredReminders()
    {
        for (int i = 0; i < reminders.Count; i++)
        {
            if (reminders[i].ExpiredDays > 0 && reminders[i].IsAutoDel)
            {
                reminders.RemoveAt(i);
            }
        }
    }

    //----------------------------------------------------------------------//
    // METHOD > DeserializeJson                                             //          
    //          Reads JSON data from a file and deserializes the JSON       //
    //          string to a List<Reminder> object                           //
    //                                                                      //
    // PARAMS > none                                                        //
    //                                                                      //
    // RETURN > void                                                        //
    //----------------------------------------------------------------------//
    public static void DeserializeJson()
    {
        // Create directory if it doesn't exist
        if (!Directory.Exists(dirPath))
        {
            CreateDirectory(dirPath);
        }

        // Return if file doesn't exist
        if (!File.Exists(filePath))
        {
            WriteLine("\nYou have no reminders.");
            return;
        }

        // Deserialze JSON -> List<Reminders>
        try
        {
            StreamReader reader  = File.OpenText(filePath);
            string remindersJson = reader.ReadToEnd();
            reader.Close();

            reminders = JsonConvert.DeserializeObject<List<Reminder>>(remindersJson) 
                        ?? new List<Reminder>();  
        }
        catch (Exception ex)
        {
            WriteLine($"Exception occured reading from:\n{filePath}");
            WriteLine($"Exception message: {ex.Message}");
        }
    }

    //----------------------------------------------------------------------//
    // METHOD > SerializeJson                                               //          
    //          Serializes a List<Reminder> object to JSON and writes the   //
    //          data to a file                                              //             
    //                                                                      //
    // PARAMS > none                                                        //
    //                                                                      //
    // RETURN > void                                                        //
    //----------------------------------------------------------------------//
    public static void SerializeJson()
    {
        // Create directory if it doesn't exist
        if (!Directory.Exists(dirPath))
        {
            CreateDirectory(dirPath);
        }

        // Serialize List<Reminder> -> JSON
        string remindersJson = JsonConvert.SerializeObject(reminders);

        // Write data to file
        try
        {
            StreamWriter writer = File.CreateText(filePath);
            writer.Write(remindersJson);
            writer.Close();

            WriteLine($"\nYour reminders have been saved!");
        }
        catch (Exception ex)
        {
            WriteLine($"Exception occured writing to:\n{filePath}");
            WriteLine($"Exception message: {ex.Message}"); 
        }
    }
}
