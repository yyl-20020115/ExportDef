namespace ExportDef;
/*
Microsoft (R) COFF/PE Dumper Version 14.36.32532.0
Copyright (C) Microsoft Corporation.  All rights reserved.


Dump of file libiotgm.dll

File Type: DLL

  Section contains the following exports for libiotgm.dll

    00000000 characteristics
    647803B4 time date stamp Thu Jun  1 10:34:28 2023
        0.00 version
           1 ordinal base
          60 number of functions
          60 number of names

    ordinal hint RVA      name

          1    0 000026FE iotgm_assign_ca
          2    1 00002868 iotgm_assign_chip_opt

  Summary

        1000 .CRT
        5000 .bss
        A000 .data
        1000 .edata
       51000 .eh_fram
        2000 .idata
       AD000 .rdata
       17000 .reloc
      21C000 .text
        1000 .tls

 */

internal class Program
{
    enum Phase
    {
        None = 0,
        Started = 1,
        GotName = 2,
        GotFunctions = 3,
        GotSummary = 4
    }
    static int Main(string[] args)
    {
        if (args.Length < 2 || !File.Exists(args[0]))
        {
            Console.WriteLine("ExportDef.exe [Input.Info] [Output.Def]");
            return -1;
        }
        using var reader = new StreamReader(args[0]);
        using var writer = new StreamWriter(args[1]);   

        var phase =  Phase.None;

        var name = "default";
        string? line;
        while(null!=(line = reader.ReadLine()))
        {
            line = line.Trim();
            if(line.Length==0)
                continue;
            else
            if(line.StartsWith("Microsoft (R) COFF/PE Dumper Version "))
            {
                phase= Phase.Started;
                continue;
            }else if(line.StartsWith("Dump of file "))
            {
                phase= Phase.GotName;
                name = line["Dump of file ".Length..].Trim();
                writer.WriteLine("LIBRARY " + Path.GetFileNameWithoutExtension(name));
                writer.WriteLine("EXPORTS");
                continue;
            }
            else if(line.StartsWith("ordinal hint RVA"))
            {
                phase= Phase.GotFunctions;
                continue;
            }
            else if (line.StartsWith("Summary"))
            {
                phase = Phase.GotSummary;
                continue;
            }
            else if(phase == Phase.GotFunctions)
            {
                //1    0 000026FE iotgm_assign_ca
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if(parts.Length == 4)
                {
                    writer.WriteLine("\t" + parts[3]);
                }
            }
        }

        return 0;
    }
}