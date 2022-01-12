using System.Collections.Generic;
using RightToAskClient.Models;

public class MPsInChamber : List<MP>
{
    public string Title { get; set; }
    public string ShortName { get; set; } //will be used for jump lists
    public string Subtitle { get; set; }
    private MPsInChamber(string title, string shortName)
    {
        Title = title;
        ShortName = shortName;
    }

    public static IList<MPsInChamber> All { private set; get; }
}