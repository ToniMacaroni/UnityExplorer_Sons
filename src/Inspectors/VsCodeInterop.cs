using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using RedLoader;
using UnityEngine;
using UnityExplorer;
using UnityExplorer.CSConsole;
using Color = UnityEngine.Color;

namespace UnityExplorer;

public class VsCodeInterop
{
    public static Thread _connectionThread;
    
    public static string _textBuffer = "";
    
    public static object _readLock = new object();
    
    private static StreamWriter _writer;
    private static StreamReader _reader;

    public static void Start()
    {
        ExplorerCore.Log("Starting VsCodeInterop...");
        
        _connectionThread = new Thread(StartReceiver);
        _connectionThread.IsBackground = true;
        _connectionThread.Start();
    }

    private static void StartReceiver()
    {
        using var client = new NamedPipeClientStream(".", "uegen", PipeDirection.InOut, PipeOptions.Asynchronous);
        client.Connect();

        RLog.Msg("Connected!");
        _reader = new StreamReader(client);
        _writer = new StreamWriter(client);
        
        while (true)
        {
            string temp;
            while ((temp = _reader.ReadLine()) != null)
            {
                lock (_readLock)
                {
                    _textBuffer = temp.Replace("~~", "\n");
                }
            }
        }
    }

    public static void Tick()
    {
        lock (_readLock)
        {
            if (!string.IsNullOrEmpty(_textBuffer))
            {
                OnNewText(_textBuffer);
                _textBuffer = String.Empty;
            }
        }
    }

    public static void Send(string text)
    {
        if (_writer == null)
            return;

        try
        {
            _writer.WriteLine(text);
            _writer.Flush();
            Console.WriteLine("Sent data!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void OnNewText(string text)
    {
        ConsoleController.Input.Text = text;
        ConsoleController.Evaluate();
    }
}