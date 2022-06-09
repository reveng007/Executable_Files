using System;
using System.Diagnostics;

namespace Myprocesses
{
    class GetProcesses
    {
        static void CmdMenu()
        {
            Console.Write("\n");
            Console.Write(@"[*] Use: 
1 : To list all processes and corresponding PIDs.
2 : To get current process name and PID.
3 : Dump all injectable processes.
4 : To exit.");
        }

        static void ListAllProcesses()  
        {  
            Process[] processCollection = Process.GetProcesses();  
            int index = 1;
            foreach (Process p in processCollection)  
            {  
                Console.WriteLine("{0}. PID: {1} => ProcessName: {2}", index, p.Id, p.ProcessName);
                index ++;
            } 
        }

        static void CurrentProcess()
        {
            Process current = Process.GetCurrentProcess();
            Console.WriteLine("PID: {0} => ProcessName: {1}", current.Id, current.ProcessName);
        }

         // ====== Exit function ==============

        public static void EXIT(string cmd)
        {
            if (cmd.Equals("exit"))
            {
                // exiting
                System.Environment.Exit(1);
            }
        }

        // ============= Main Console =============

        public static void MainConsole()
        {
            while(true)
            {
                CmdMenu();
                // Starting Menu:
                Console.Write("\n[>] ");
                string userinput = Console.ReadLine();
                if (userinput == null  || userinput.Equals(""))
                {
                    Console.WriteLine("[-] User input is out of Command Menu Syllabus\n");
                    continue;
                }
                
                EXIT(userinput);

                switch(userinput)
                {
                    case "1":

                        ListAllProcesses();
                        break;
                    
                    case "2":
                        
                        CurrentProcess();
                        break;
                    
                    case "3":

                        Console.WriteLine("[+] Dumping injectable processes...");
                        break;

                    default:
                        break;
                }
            }
        }

        public static void Main()
        {
            MainConsole();
            //return 0;
        }
    }
    
}
