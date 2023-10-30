using static System.Console;
using static System.IO.Directory;
using static System.IO.Path;
using static System.Environment;
using ConsoleIOManager;
using RemindersLib;
using Newtonsoft.Json;
namespace Program;

//-----------------------------V1.2-KeganF------------------------------//
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
        Add();
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
            bool isAcceptedDate = Convert.ToBoolean(
                ConsoleManager.DisplayCheckBoxes(new string[]{"No", "Yes"},
                $"{date} has already passed. Do you want to finish creating this reminder?"));

            if (!isAcceptedDate)
            {
                WriteLine("This reminder has been cancelled!");
                return;
            }
        }
        
        bool isAutoDel = Convert.ToBoolean(
            ConsoleManager.DisplayCheckBoxes(new string[]{"No", "Yes"}, 
            $"Do you want this reminder to be automatically deleted after {date} at {time}?"));

        Reminder r = new Reminder(title, desc, date, time, isAutoDel);
        r.DisplaySummary();
        reminders.Add(r);
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
