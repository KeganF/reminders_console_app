using static System.Console;
using ConsoleIOManager;
using Newtonsoft.Json;
namespace RemindersLib;
//-----------------------------Constructors-----------------------------//
//-----------------------------V1.1-KeganF------------------------------//
// CLASS > Reminder                                                     //
//         Provides members for defining Reminder objects               //
//----------------------------------------------------------------------//
public class Reminder
{
    //---------------------------v-Constructors-v---------------------------//
    public Reminder(string title, string desc, 
        DateOnly date, TimeOnly time, bool isAutoDel)
    {
        Title     = title;
        Desc      = desc;
        Date      = date;
        Time      = time;
        IsAutoDel = isAutoDel;
    }
    
    //-----------------------------v-Methods-v------------------------------//
    //----------------------------------------------------------------------//
    // METHOD > DisplaySummary                                              //          
    //          Formats and displays object values to quickly summarize     //
    //          a Reminder                                                  //
    //                                                                      //
    // PARAMS > none                                                        //
    //                                                                      //
    // RETURN > void                                                        //
    //----------------------------------------------------------------------//
    public void DisplaySummary()
    {
        WriteLine($"Title: {Title}");
        WriteLine($"Desc: {Desc}");
        WriteLine($"Date: {Date}");
        WriteLine($"Time: {Time}");
        if (ExpiredDays > 0)
        {
            ConsoleManager.WriteColoredLine(
                $"This reminder expired {ExpiredDays} ago!", ConsoleColor.Red);
        }
    }
    
    //----------------------------v-Properties-v----------------------------//
    public string Title   { get; set; }
    public string Desc    { get; set; }
    public DateOnly Date  { get; set; }
    public TimeOnly Time  { get; set; }
    public bool IsAutoDel { get; set; }
    [JsonIgnore]
    public int ExpiredDays 
    { 
        get 
        { 
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            return today.DayNumber - Date.DayNumber;
        }
    }

}
