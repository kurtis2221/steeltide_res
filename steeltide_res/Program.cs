using System;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace steeltide_res
{
    class Program
    {
        static MemoryEdit.Memory mem;
        static uint baseaddr;

        static void Main(string[] args)
        {
            try
            {
                int width;
                int height;
                using (StreamReader sr = new StreamReader("steeltide_res.ini"))
                {
                    width = Convert.ToInt32(sr.ReadLine());
                    height = Convert.ToInt32(sr.ReadLine());
                }
                Process proc = Process.Start("SteelTide.exe");
                mem = new MemoryEdit.Memory();
                mem.Attach(proc, 0x001F0FFF);
                baseaddr = (uint)proc.MainModule.BaseAddress.ToInt64();
                Console.WriteLine("Waiting for resolution values to load...");
                while (ReadData(0x0C898C) == 0 || ReadData(0x291D58) == 0) Thread.Sleep(1000);
                //Height
                byte[] buffer = BitConverter.GetBytes(height);
                //960
                WriteData(0x2BAF20, buffer);
                WriteData(0x291DA8, buffer);
                WriteData(0x291100, buffer);
                //480
                WriteData(0x0C898C, buffer);
                WriteData(0x2910B0, buffer);
                WriteData(0x291D58, buffer);
                //Width
                buffer = BitConverter.GetBytes(width);
                //1280
                WriteData(0x291DA4, buffer);
                WriteData(0x2910FC, buffer);
                WriteData(0x290B40, buffer);
                //640
                WriteData(0x0C8988, buffer);
                WriteData(0x2910AC, buffer);
                WriteData(0x291D54, buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed: " + ex.Message);
                Console.Read();
            }
        }

        static int ReadData(uint addr)
        {
            return mem.Read(baseaddr + addr);
        }

        static void WriteData(uint addr, byte[] buffer)
        {
            mem.WriteByte(baseaddr + addr, buffer, buffer.Length);
        }
    }
}