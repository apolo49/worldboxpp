using System.Reflection;
using System.IO;
using System.Diagnostics;

public class Extractor
{
    public static void extractAssemblies(string currentdir = null)
    {
        if (currentdir == null) currentdir = "./";
        Assembly assembly = Assembly.LoadFrom(currentdir + "NCMS.dll");
        string assembliesPath = currentdir + "Assemblies";
        Directory.CreateDirectory(assembliesPath);
        string[] allResources = getAllResources(assembly);
        string[] array = allResources;
        foreach (string text in array)
        {
            if (text.StartsWith("NCMS.Assemblies."))
            {
                string text2 = text.Replace("NCMS.Assemblies.", "");
                if (!File.Exists(assembliesPath + "/" + text2))
                {
                    Debug.WriteLine(assembliesPath + "/" + text2);
                    File.WriteAllBytes(assembliesPath + "/" + text2, getResource(text, assembly));
                }
            }
        }
    }

    static string[] getAllResources(Assembly assembly)
    {
        return assembly.GetManifestResourceNames();
    }

    static byte[] getResource(string name, Assembly assembly)
    {
        Stream manifestResourceStream = assembly.GetManifestResourceStream(name);
        return ReadFully(manifestResourceStream);
    }

    static byte[] ReadFully(Stream input)
    {
        byte[] array = new byte[16384];
        using MemoryStream memoryStream = new MemoryStream();
        int count;
        while ((count = input.Read(array, 0, array.Length)) > 0)
        {
            memoryStream.Write(array, 0, count);
        }
        return memoryStream.ToArray();
    }
}
