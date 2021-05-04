using System.IO;
using UnityEngine;

public class ModBuildLogger
{
    private static string filepath = "E:/DaggerModsLogs.txt";
    public static void Log(string log)
    {
        Debug.Log(log);
        if (File.Exists(filepath))
        {
            File.WriteAllText(filepath, File.ReadAllText(filepath) + "\n" + log);
        }
        else
        {
            File.WriteAllText(filepath, log);
        }
    }
}
