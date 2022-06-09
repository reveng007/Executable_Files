using System;   // 
using System.Text;  // Namespace which contain Class named, "StringBuilder" which represents a mutable string of characters
using Microsoft.Win32;  // Namespace which contain Class named, "Registry" which Provides RegistryKey objects that represent the root keys in the Windows registry, and static methods to access key/value pairs.
using System.Collections.Generic;   // Namespace which contain Class named, "Dictionary<TKey,TValue>" which represents a collection of keys and values.

namespace RegistryTinker
{
    public class program
    {

        // links: 
        // https://www.c-sharpcorner.com/UploadFile/rohatash/difference-between-object-and-dynamic-keyword-in-C-Sharp/
        // https://www.dotnettricks.com/learn/csharp/differences-between-object-var-and-dynamic-type
        
        // Creating a dictionary to store all the registry bases or registry hives
        public static Dictionary<string, dynamic> registryHives = new Dictionary<string, object>();

        // Object list Under Registry:
        // https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.registry?view=net-6.0#remarks


        // ============================ All Command Menus ======================================

        public static void FirstCmdMenu()
        {
            Console.WriteLine(@"[*] Use: 
    1. readkey: To read any reg key
    2. createkey: To create any reg key
    3. deletekey: To delete any reg key
    4. setvalue: To set any value to a specific reg key
    5. deletevalue: To delete any previously set value of a specific reg key
    6. exit: To exit");
        }

        public static void SecondCmdMenu()
        {
            Console.WriteLine("Give a key in the following formats:");
            Console.WriteLine("====================================\n");
            Console.WriteLine("RegistryKeyHiveName\\KEY\\SUBKEY\\...\\SUBKEY: \n");
            Console.WriteLine("\n[*] SOME HINTS:\n");
            Console.WriteLine("[1] HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run \tAdd the reg keys, to automatically run software whenever user logs in. \n\t\t\t\t\t\t\t\t\tWhere, HKEY_CURRENT_USER: Its subkeys contain user profiles for all users that ever logged in locally to this machine.");
            Console.WriteLine("[2] HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run \tAdd the reg keys, to automatically run software whenever system reboots. \n\t\t\t\t\t\t\t\t\tWhere, HKEY_LOCAL_MACHINE: Contains machine wide configuration information including hardware and software settings");
            Console.WriteLine("[3] HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall \tTo uninstall added reg keys, to disable automatic running of it while system reboot.");
            Console.WriteLine("[4] HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce \tAdd the reg keys, to automatically run software whenever user logs in, the program run \n\t\t\t\t\t\t\t\t\t        one time, and then the key is deleted. \n\t\t\t\t\t\t\t\t\tWhere, HKEY_LOCAL_MACHINE: Configuration information including hardware and software settings.");
            Console.WriteLine("[5] HKEY_CURRENT_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce \tAdd the reg keys, to automatically run software whenever system reboots, the program \n\t\t\t\t\t\t\t\t\t       run one time, and then the key is deleted. \n\t\t\t\t\t\t\t\t\tWhere, HKEY_CURRENT_USER: profile of the currently logged-on user.");
        }

        // ============================== Getting reg key input from User =============================
        // ======================================= Console No. 2 ======================================

        public static string GetRegKeyInput(string cmd)
        {
            string key;

            Console.WriteLine("\n[*] Registry key to {0}?\n", cmd);
            Console.WriteLine(@"[+] [TIPS: 
        1. Press any key to move back to previous Console Prompt
        2. ""exit"": To exit]");
            
            Console.Write("\n[KeyName>] ");
            key = Console.ReadLine();

            return key;
        }

        // ============================ Convertion of String type (hivename) to RegistryKey type (hivename) ===================

        public static RegistryKey ConvertStringToRegistryKey(string hivename)
        {
            RegistryKey rkey;

            if(hivename == "HKEY_CURRENT_USER")
            {
                rkey = Registry.CurrentUser;
                return rkey;
            }
            else if(hivename == "HKEY_CLASSES_USER")
            {
                rkey = Registry.ClassesRoot;
                return rkey;
            }
            else if(hivename == "HKEY_CURRENT_CONFIG")
            {
                rkey = Registry.CurrentConfig;
                return rkey;
            }
            else if(hivename == "HKEY_LOCAL_MACHINE")
            {
                rkey = Registry.LocalMachine;
                return rkey;
            }
            else
            {
                rkey = Registry.Users;
                return rkey;
            }
        }


    // ===================================== Interacting with REGISTRY KEYS with various goals in mind ==================================

        // ====== Exit function ==============

        public static void EXIT(string cmd)
        {
            if (cmd.Equals("exit"))
            {
                // exiting
                System.Environment.Exit(1);
            }
        }

        // Seperate funtion for parsing hive/base name and Key name
        public static string[] ParsingRegKeyPath(string cmd)
        {
            // We would return 2 strings: hivename and keyname
            string[] ret = new string[2];

            // hivename: Storing the registry hive name from Reg key path inputed i.e. cmd
            ret[0] = cmd.Split('\\')[0];

            EXIT(cmd);

            if(!registryHives.ContainsKey(ret[0]))
            {
                Console.WriteLine("\n[-] Registry base/hive name is not found.");
                Console.WriteLine("[*] Moving to previous shell...\n");
                MainConsole();
            }

            // keyname: Storing the registry key name from the whole Reg key path inputed i.e. cmd
            ret[1] = cmd.Substring(ret[0].Length+1, cmd.Length-ret[0].Length-1);
            
            return ret;
        }

        // ============================== Getting reg key to read =============================

        public static void ReadRegKey(string cmd)
        {
            // Storing 2 strings:  hivename and keyname
            string[] hivename_keyname = ParsingRegKeyPath(cmd);

            // Printing reg hive name
            Console.WriteLine("\n[+] Registry base/hive name :\t {0}\n", hivename_keyname[0]);

            // As, converting of String type to RegistryKey type is not possible
            // => link: https://www.dotnetspider.com/forum/17404-Urgent-How-to-convert-e-string-to-RegistryKey
            // So, Creating custom function to do the Convertion
            // Calling function named, ConvertStringToRegistryKey()
            RegistryKey rkey = ConvertStringToRegistryKey(hivename_keyname[0]);

            // Retrieve all the subkeys for the specified key.
            string [] subkeynames = rkey.GetSubKeyNames();

            int count1 = 1;

            //Print the Subkeynames Under Current Registry Hive name
            Console.WriteLine(@"    [1] Registry SubKeys available Under ""{0}""",registryHives[hivename_keyname[0]]);
            Console.WriteLine("    ========================================================\n");

            // Print the contents of the array to the console, i.e, All subkeys under requested Registry Hive
            foreach (string s in subkeynames)
            {
                Console.WriteLine("\t [{0}] {1}",count1,s);
                count1++;
            }

            using(RegistryKey key = registryHives[hivename_keyname[0]].OpenSubKey(hivename_keyname[1]))    // Reading the keys
            {
                // Printing the registry key name
                Console.WriteLine("\n[+] Requested Registry Key/key name :\t {0}\n",hivename_keyname[1]);

                //Print the Value Names and Values
                int count2 = 1;
                Console.WriteLine("     [1] ValueName  :  Value");
                Console.WriteLine("     ========================\n");

                try
                {   
                    foreach(string valuename in key.GetValueNames())        // Printing the Values
                    {
                        Object obj = key.GetValue(valuename);
                        
                        if(obj == null)
                        {
                            Console.WriteLine("[-] Returned object was empty");
                        }

                        /*
                        // Have to do something so that System.Byte[] object type gets printed on the Console

                        // Getting Object types
                        Console.WriteLine("Type: {0}",obj.GetType());
                    
                        if (obj.GetType().Equals("System.Byte[]"))
                        {

                            Console.WriteLine(Encoding.Default.GetString(obj));
                            Console.WriteLine("\t [{0}] {1} :   {2}", count2,valuename,obj);
                            break;
                        }
                        */
                        Console.WriteLine("\t [{0}] {1} :   {2}", count2,valuename,obj);
                        count2++;
                    }
                }
                catch(System.NullReferenceException e)
                {
                    Console.WriteLine("[-] KeyName in the provided Registry Key path is absent: {0}\t Try Again!!", e.Message);
                }
            }
            // mimicking the Swtich Case scenario of Main() function to enable the smooth flow of [KeyName>] prompt
            string mimic_main_key = GetRegKeyInput("read");
            ReadRegKey(mimic_main_key);
        }


        // ============================== Creating registry Sub keys to add to registry hive =============================
        
        public static void CreateRegKey(string cmd)
        {
            // Storing 2 strings:  hivename and keyname
            string[] hivename_keyname = ParsingRegKeyPath(cmd);

            // Printing reg hive name
            Console.WriteLine("\n[+] Registry base/hive name :\t {0}", hivename_keyname[0]);

            // Printing the registry key name
            Console.WriteLine("[+] Requested Registry Key/key name :\t {0}\n",hivename_keyname[1]);

            using(RegistryKey KEY = registryHives[hivename_keyname[0]].OpenSubKey(hivename_keyname[1]))    // Reading the keys
            {
                try
                {
                    if (KEY == null)
                    {
                        Console.WriteLine("\n[+] Similar KeyName in the provided Registry Key path doesn't exist previously");
                        
                        // Creating registry Sub keys to add
                        RegistryKey key = registryHives[hivename_keyname[0]].CreateSubKey(hivename_keyname[1]);
                        Console.WriteLine("[+] Registry Sub Key Created!!\n");
                    }
                    else
                    {
                        Console.WriteLine("\n[-] Similar KeyName in the provided Registry Key path already exists:\t Try Again!!");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[-] Unable to Create Registry Sub Key: {0}", e.Message);
                }
            }
            // mimicking the Swtich Case scenario of Main() function to enable the smooth flow of [KeyName>] prompt
            string mimic_main_key = GetRegKeyInput("create");
            CreateRegKey(mimic_main_key);
        }


        // ============================== Deleting registry Sub keys from registry hive =============================

        public static void DeleteRegKey(string cmd)
        {
            // Storing 2 strings:  hivename and keyname
            string[] hivename_keyname = ParsingRegKeyPath(cmd);

            // Printing reg hive name
            Console.WriteLine("\n[+] Registry base/hive name :\t {0}", hivename_keyname[0]);

            // Printing the registry key name
            Console.WriteLine("[+] Requested Registry Key/key name :\t {0}\n",hivename_keyname[1]);

            using(RegistryKey KEY = registryHives[hivename_keyname[0]].OpenSubKey(hivename_keyname[1]))    // Reading the keys
            {
                try
                {
                    if (KEY == null)
                    {
                        Console.WriteLine("\n[-] KeyName in the provided Registry Key path doesn't exist:\t Try Again!!");
                    }
                    else
                    {
                        Console.WriteLine("\n[+] KeyName in the provided Registry Key path exists\n");

                        // Deleting registry Sub key
                        registryHives[hivename_keyname[0]].DeleteSubKey(hivename_keyname[1]);
                        registryHives[hivename_keyname[0]].Close();

                        Console.WriteLine("[+] Registry Sub Key Deleted!!\n");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[-] Unable to Delete Registry Sub Key: {0}", e.Message);
                }
            }
            // mimicking the Swtich Case scenario of Main() function to enable the smooth flow of [KeyName>] prompt
            string mimic_main_key = GetRegKeyInput("delete");
            DeleteRegKey(mimic_main_key);
        }


        // ============================== Setting value to a registry Subkey =========================
        public static void SetValue(string cmd)
        {
            // Storing 2 strings:  hivename and keyname
            string[] hivename_keyname = ParsingRegKeyPath(cmd);

            // Printing reg hive name
            Console.WriteLine("\n[+] Registry base/hive name :\t {0}", hivename_keyname[0]);

            // Printing the registry key name
            Console.WriteLine("[+] Requested Registry Key/key name :\t {0}\n",hivename_keyname[1]);

            using(RegistryKey KEY = registryHives[hivename_keyname[0]].OpenSubKey(hivename_keyname[1],true))    // Reading the keys and true makes it writable
            {
                try
                {
                    if (KEY == null)
                    {
                        Console.WriteLine("\n[-] KeyName in the provided Registry Key path doesn't exist:\t Try Again!!");
                    }
                    else
                    {
                        Console.WriteLine("\n[+] KeyName in the provided Registry Key path exists\n");

                        //Entering value name
                        Console.WriteLine("[*] Registry Value Name?");
                        Console.Write("[valuename>] ");
                        string valuename = Console.ReadLine();

                        EXIT(valuename);

                        //Entering value
                        Console.WriteLine("\n[*] Registry Value?");
                        Console.Write("[value>] ");
                        string Value = Console.ReadLine();

                        EXIT(Value);

                        //Adding valuename and value, to registry Sub key
                        /*
                         * We could have also used this:
                         * 
                         * link: https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.registry.setvalue?view=net-6.0
                         * Syntax: 
                         * public static void SetValue (string keyName, 
                         *                               string? valueName, 
                         *                               object value, 
                         *                               Microsoft.Win32.RegistryValueKind valueKind);

                         *  Sets the specified name/value pair on the specified registry key. If the specified key does not exist, it is created.
                         *  As, Already the not existing registry key is discarded using try catch previously, we can use this... This will not create accidental Registry Key.
                         
                         * Registry.SetValue(cmd,valuename,Value,RegistryValueKind.String);
                         */
                        KEY.SetValue(valuename,Value);

                        Console.WriteLine("\n[+] Registry Value added!!\n");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[-] Unable to Add Registry Value: {0}", e.Message);
                }
            }
            // mimicking the Swtich Case scenario of Main() function to enable the smooth flow of [KeyName>] prompt
            string mimic_main_key = GetRegKeyInput("set value");
            SetValue(mimic_main_key);
        }

        // ============================== Deleting value from a registry Subkey =========================
        public static void DeleteValue(string cmd)
        {
            // Storing 2 strings:  hivename and keyname
            string[] hivename_keyname = ParsingRegKeyPath(cmd);

            // Printing reg hive name
            Console.WriteLine("\n[+] Registry base/hive name :\t {0}", hivename_keyname[0]);

            // Printing the registry key name
            Console.WriteLine("[+] Requested Registry Key/key name :\t {0}\n",hivename_keyname[1]);

            using(RegistryKey KEY = registryHives[hivename_keyname[0]].OpenSubKey(hivename_keyname[1],true))    // Reading the keys and true makes it writable
            {
                try
                {
                    if (KEY == null)
                    {
                        Console.WriteLine("\n[-] KeyName in the provided Registry Key path doesn't exist:\t Try Again!!");
                    }
                    else
                    {
                        Console.WriteLine("\n[+] KeyName in the provided Registry Key path exists\n");

                        //Entering value name
                        Console.WriteLine("[*] Registry Value Name?");
                        Console.Write("[valuename>] ");
                        string valuename = Console.ReadLine();
                        
                        EXIT(valuename);

                        KEY.DeleteValue(valuename);

                        Console.WriteLine("\n[+] Registry Value deleted!!\n");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("[-] Unable to Delete Registry Value: {0}", e.Message);
                }
            }
            // mimicking the Swtich Case scenario of Main() function to enable the smooth flow of [KeyName>] prompt
            string mimic_main_key = GetRegKeyInput("delete value");
            DeleteValue(mimic_main_key);
        }

        // ===================================== Main Console ========================================

        public static void MainConsole()
        {
            while(true)
            {
                FirstCmdMenu();
                // Starting Menu:
                Console.Write("\n[>] ");
                string userinput = Console.ReadLine();
                if (userinput == null  || userinput.Equals(""))
                {
                    Console.WriteLine("[-] User input is out of Command Menu Syllabus\n");
                    continue;
                }
                
                EXIT(userinput);

                string key;

                switch(userinput)
                {
                    case "readkey":

                        SecondCmdMenu();

                        // Get reg key from the user
                        key = GetRegKeyInput("read");
                        
                        // Calling read reg key
                        ReadRegKey(key);
                        break;
                    
                    case "createkey":
                        
                        SecondCmdMenu();

                        // Get reg key from the user
                        key = GetRegKeyInput("create");
                        
                        // Calling read reg key
                        CreateRegKey(key);
                        break;
                    
                    case "deletekey":

                        SecondCmdMenu();

                        // Get reg key from the user
                        key = GetRegKeyInput("delete");
                        
                        // Calling read reg key
                        DeleteRegKey(key);
                        break;

                    case "setvalue":

                        SecondCmdMenu();

                        // Get reg key from the user
                        key = GetRegKeyInput("set value");
                        
                        // Calling read reg key
                        SetValue(key);
                        break;

                    case "deletevalue":

                        SecondCmdMenu();

                        // Get reg key from the user
                        key = GetRegKeyInput("delete value");
                        
                        // Calling read reg key
                        DeleteValue(key);
                        break;

                    default:
                        break;
                }
            }
        }

        // ====================================== Entry Funtion ============================

        public static void Main()
        {
            // Building a dictionary for all registry hive names
            // so that we can parse the request and ask 
            // them seperately.
            
            registryHives.Add("HKEY_CURRENT_USER", Registry.CurrentUser);
            registryHives.Add("HKEY_CLASSES_USER", Registry.ClassesRoot);
            registryHives.Add("HKEY_CURRENT_CONFIG", Registry.CurrentConfig);
            registryHives.Add("HKEY_LOCAL_MACHINE", Registry.LocalMachine);
            registryHives.Add("HKEY_USERS", Registry.Users);

            MainConsole();
        }
    }
}
