using ConsoleIOManager;
using RemindersLib;
using static System.Console;
namespace Program;

//-----------------------------V1.1-KeganF------------------------------//
// CLASS > Program                                                      //
//         Contains the Main method for the Reminders Console App       //
//         Allows users to manage their reminders via the console       //
//----------------------------------------------------------------------//
public class Program
{
    public static void Main(string[] args)
    {
        Add();
    }

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
                $"{date} has already past. Do you want to finish creating this reminder?"));

            if (!isAcceptedDate)
            {
                return;
            }
        }
        
        bool isAutoDel = Convert.ToBoolean(
            ConsoleManager.DisplayCheckBoxes(new string[]{"No", "Yes"}, 
            $"Do you want this reminder to be automatically deleted after {date} at {time}?"));

        Reminder r = new Reminder(title, desc, date, time, isAutoDel);
        r.DisplaySummary();
    }
}
