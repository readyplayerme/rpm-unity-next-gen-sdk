using System.IO;
using UnityEngine;

public class EndpointGenerator
{
    public string Generate(string name)
    {
        var text = Resources.Load<TextAsset>("endpoint_template").text;
        
        text = text.Replace("{name}", name);
        
        var path = Application.dataPath + "/Phoenix SDK/Runtime/API/" + name + "Endpoint.cs";

        StreamWriter outputFile = new StreamWriter(path);
        outputFile.Write(text);
        outputFile.Close();
        
        return path;
    }
}
