using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using UnityEngine;
using UnityExplorer;
using Color = UnityEngine.Color;

namespace ForestNanosuit;

public class MaterialSync
{
    public static Thread _receiverThread;
    
    public static Material _targetMaterial;

    public static Queue<string> _LineQueue = new();

    public static void Start()
    {
        ExplorerCore.Log("Starting MaterialSync...");
        
        _receiverThread = new Thread(StartReceiver);
        _receiverThread.IsBackground = true;
        _receiverThread.Start();
    }

    private static void StartReceiver()
    {
        while (true)
        {
            using var server = new NamedPipeServerStream("MaterialSyncPipe", PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
            //LogOnMain("Waiting for connection...", Color.Aquamarine);
            server.WaitForConnection();

            using (var reader = new StreamReader(server))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    lock (_LineQueue)
                    {
                        _LineQueue.Enqueue(line);
                    }
                }
            }
        }
    }

    public static void Tick()
    {
        lock (_LineQueue)
        {
            while (_LineQueue.Count > 0)
            {
                ParseAndApplyProperty(_LineQueue.Dequeue());
            }
        }
    }

    private static void ParseAndApplyProperty(string propertyLine)
    {
        if (InspectorManager.ActiveInspector.Target is GameObject gameObject)
        {
            var renderer = gameObject.GetComponent<Renderer>();
            if (renderer)
            {
                _targetMaterial = renderer.sharedMaterial;
            }
        }
        
        if (!_targetMaterial)
        {
            ExplorerCore.Log("Target material is not set!");
            return;
        }
        
        //ExplorerCore.Log($"Parsing for {_targetMaterial.name}...");
        var matName = _targetMaterial.name;

        string[] parts = propertyLine.Split(',');
        string propertyName = parts[0];
        string propertyType = parts[1];
        string propertyValue = parts[2];

        switch (propertyType)
        {
            case "Color":
                var color = ColorFromString(propertyValue);
                _targetMaterial.SetColor(propertyName, color);
                ExplorerCore.Log($"Color property {propertyName} set to {color} ({matName})");
                break;
            case "Vector":
                string[] vectorParts = propertyValue.Split(':');
                Vector4 vector = new Vector4(
                    float.Parse(vectorParts[0], CultureInfo.InvariantCulture),
                    float.Parse(vectorParts[1], CultureInfo.InvariantCulture),
                    float.Parse(vectorParts[2], CultureInfo.InvariantCulture),
                    float.Parse(vectorParts[3], CultureInfo.InvariantCulture)
                );
                _targetMaterial.SetVector(propertyName, vector);
                ExplorerCore.Log($"Vector property {propertyName} set to {vector} ({matName})");
                break;
            case "Float":
            case "Range":
                _targetMaterial.SetFloat(propertyName, float.Parse(propertyValue, CultureInfo.InvariantCulture));
                ExplorerCore.Log($"Float property {propertyName} set to {propertyValue} ({matName})");
                break;
            case "Texture":
                // Texture texture = Resources.Load<Texture>(propertyValue);
                // _targetMaterial.SetTexture(propertyName, texture);
                ExplorerCore.Log("Texture properties are not supported yet!");
                break;
        }
    }

    private static Color ColorFromString(string color)
    {
        var colorParts = color.Split(':');
        var r = float.Parse(colorParts[0], CultureInfo.InvariantCulture);
        var g = float.Parse(colorParts[1], CultureInfo.InvariantCulture);
        var b = float.Parse(colorParts[2], CultureInfo.InvariantCulture);
        var a = 1f;
        if (colorParts.Length > 3)
        {
            a = float.Parse(colorParts[3], CultureInfo.InvariantCulture);
        }
        
        return new Color(r, g, b, a);
    }
    
    // private static void LogOnMain(string message, Color color)
    // {
    //     RunOnMain(() =>
    //     {
    //         ExplorerCore.Log(message);
    //     });
    // }

    // private static void RunOnMain(Action action)
    // {
    //     UnityMainThreadDispatcher.Instance().Enqueue(action);
    // }
}