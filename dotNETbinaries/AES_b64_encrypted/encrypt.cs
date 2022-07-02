/*
Compile: csc.exe /target:exe /out:encrypt.exe .\encrypt.cs
*/

using System;
using System.IO;			// StreamReader, MemoryStream
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Diagnostics;


class Program
{
	// Encryption keys:
	public static byte[] xor_key = Encoding.UTF8.GetBytes("mysecretkeee");			// Xor key 		// change
	//public static byte[] aes_key = Encoding.UTF8.GetBytes("ENCRYPTIONISGOOD");  	// 16 bytes key // change
  	//public static byte[] aes_iv = Encoding.UTF8.GetBytes("NOITISNTTHATGOOD");		// 16 bytes iv 	// change

    public static byte[] passwordBytes = Encoding.UTF8.GetBytes("pass");



	 public static bool IsControlChar(int ch)
    {
        return (ch > (char)0 && ch < (char)8) // (char)0 = Null char and (char)8 = Back Space
            || (ch > (char)13 && ch < (char)26); // (char)13 = Carriage Return and (char)26 = Substitute
    }

	public static bool IsBinary(string path)
    {
        long length = new FileInfo(path).Length;
        if (length == 0)
        {
			return false;
		}

        using (StreamReader stream = new StreamReader(path))
        {
            int ch;
            while ((ch = stream.Read()) != -1)
            {
            	// link: https://stackoverflow.com/questions/910873/how-can-i-determine-if-a-file-is-binary-or-text-in-c
                if (IsControlChar(ch))
                {
                    return true;
                }
            }
        }
        return false;
    }

    // =============================== Encrypting Algos ===========================

    // XOR Encryption: 
    public static byte[] XOR_Encrypt(byte[] cipher)
    {
    	byte[] xored = new byte[cipher.Length];

        for(int i = 0; i < cipher.Length; i++)
        {
            xored[i] = (byte)(cipher[i] ^ xor_key[i % xor_key.Length]);
        }

        Console.WriteLine("\nXOR Encrypted: ");
        PasteShellcode(xored);

        return xored;
    }

    // AES Encryption:
    public static byte[] AES_Encrypt(byte[] cipher)
    {
        /*
		SymmetricAlgorithm algorithm = Aes.Create();
		ICryptoTransform transform = algorithm.CreateEncryptor(passwordBytes, saltBytes);
        
		byte[] outputBuffer = transform.TransformFinalBlock(cipher, 0, cipher.Length);    // byte -> byte

        return outputBuffer;
        */
        passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

        byte[] encryptedBytes = null;

        byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        using (MemoryStream ms = new MemoryStream())
        {
            using (RijndaelManaged AES = new RijndaelManaged())
            {
                AES.KeySize = 256;
                AES.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                AES.Key = key.GetBytes(AES.KeySize / 8);
                AES.IV = key.GetBytes(AES.BlockSize / 8);

                AES.Mode = CipherMode.CBC;

                using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipher, 0, cipher.Length);
                    cs.Close();
                }
                encryptedBytes = ms.ToArray();
            }
        }

        Console.WriteLine("\nAES Encrypted: ");
        PasteShellcode(encryptedBytes);

        return encryptedBytes;
    }

    // Encryption: AES -> XOR
    // Convertion: input (unencrypted) byte -aes-> aes_byte -xor-> aes_xor_byte
    public static byte[] AES_XOR_Encrypt(byte[] cipher)
    {
    	//string b64_string = Convert.ToBase64String(cipher);

    	//byte[] b64_byte = Encoding.UTF8.GetBytes(b64_string);

        Console.WriteLine("\n[+] (AES -> XOR)'ed Output: ");

    	byte[] aes_byte = AES_Encrypt(cipher);

    	byte[] aes_xor_byte = XOR_Encrypt(aes_byte);

        Console.WriteLine("[+] Last One: (AES -> XOR)'ed Output\n");

    	return aes_xor_byte;
    }

    // Encryption: AES -> XOR -> B64
    // Convertion: input byte -aes-> aes_byte -xor-> aes_xor_byte -b64-> aes_xor_b64_string
    public static string AES_XOR_B64_Encrypt(byte[] cipher)
    {
        Console.WriteLine("[+] (b64 -> XOR -> AES)'ed Output:");
        
        byte[] aes_xor_byte = AES_XOR_Encrypt(cipher);

        string aes_xor_b64 = Convert.ToBase64String(aes_xor_byte);
        Console.WriteLine("\nB64 Encoded: ");
        Console.WriteLine(aes_xor_b64);

        return aes_xor_b64;
    }

    // For debugging purposes
    public static void PasteToConsole(byte[] encrypted)
    {
    	/*
    	Console.WriteLine("\n[+] Shellcode with \\x: ");
		Console.Write("\\x");
		Console.WriteLine(BitConverter.ToString(encrypted).Replace("-","\\x"));
		*/

		Console.WriteLine("\n[+] Shellcode with 0x: ");
		Console.Write("0x");
		Console.WriteLine(BitConverter.ToString(encrypted).Replace("-",", 0x"));
    }


    // For pasting encrypted shellcodes
    public static void PasteShellcode(byte[] encrypted)
    {
    	StringBuilder newshellcode = new StringBuilder();

        newshellcode.Append("byte[] shellcode = new byte[");
        newshellcode.Append(encrypted.Length);
        newshellcode.Append("] { ");

        for (int i = 0; i < encrypted.Length; i++)
        {
            newshellcode.Append("0x");
            newshellcode.AppendFormat("{0:x2}", encrypted[i]);
            if (i < encrypted.Length - 1)
            {
                newshellcode.Append(", ");
            }

        }
        newshellcode.Append(" };");
        Console.WriteLine(newshellcode.ToString());
        Console.WriteLine("\n");
    }

    public static void banner()
    {
        Console.WriteLine("\n[>] Usage: ");
        Console.WriteLine("1. encrypt.exe /file:file.bin /out:xor");
        Console.WriteLine("2. encrypt.exe /file:file.bin /out:aes");
        Console.WriteLine("3. encrypt.exe /file:file.bin /out:aes_xor");
        Console.WriteLine("4. encrypt.exe /file:file.bin /out:aes_xor_b64");
    }

	public static void Main(string[] args)
	{
		//var data type: tells the compiler to figure out the type of the variable at compilation time
		var arguments = new Dictionary<string, string>();

		string last_3_chars = "";							// To store input file extension
		byte[] xor_encrypted = new byte[] {}; 				// TO store XOR encrypted bytes
		byte[] aes_encrypted = new byte[] {}; 				// TO store AES encrypted bytes
		byte[] aes_xor_encrypted = new byte[] {}; 			// TO store AES->XOR encrypted bytes
        string aes_xor_b64_encrypted = "";                  // TO store AES->XOR->B64 encrypted bytes

		//Console.WriteLine("HERE: 48");

        foreach (var argument in args)
        {  	
            var id = argument.IndexOf(':');
            //Console.WriteLine($"id: {id}");	// 5
            if (id > 0)
            {
            	// key
            	string prefix = argument.Substring(0, id);
            	// value
				string postfix = argument.Substring(id+1);

            	// assigning value to key
            	// key <= value
                arguments[prefix] = postfix;

                Console.WriteLine($"[+] Value = {arguments[prefix]}");

                // Storing input file extension
                last_3_chars = arguments["/file"].Substring(arguments["/file"].Length-3);
            }
            else
            {
                arguments[argument] = string.Empty;
            }
            //Console.WriteLine("HERE: 71");
        }

        //Console.WriteLine("HERE: 73");

        if (arguments.Count == 0 || !arguments.ContainsKey("/file") || !arguments.ContainsKey("/out"))
        {
        	Console.WriteLine("\n[!] Please enter /file: and /out: as arguments");

            banner();
        }
        else if (string.IsNullOrEmpty(arguments["/file"]) || string.IsNullOrEmpty(arguments["/out"]))
        {
			Console.WriteLine("\n[!] Empty input file or out");

            banner();
        }
        // Checking last 3 characters of corresponding Value of a Key
        else if (last_3_chars != "txt" && last_3_chars != "bin")
		{
			Console.WriteLine("\n[!] Invalid file type. Only .txt or .bin are accepted");

            banner();
		}
		else
        {
        	//Console.WriteLine("HERE: 90");

            var filePath = arguments["/file"];

            if (!File.Exists(filePath)) //if file exists
            {
                Console.WriteLine("\n[+] Missing input file");
                Environment.Exit(0);
            }
            else
            {
                try
                {

                    if(IsBinary(filePath))
                    {
                        Console.WriteLine("[+] Input file has '.{0}' extension	=>	Raw payload detected!", last_3_chars);
                        byte[] rawshellcode = File.ReadAllBytes(filePath);

                        Console.WriteLine("\nRawshellcode: \n");
                        PasteToConsole(rawshellcode);

        				switch (arguments["/out"].ToLower())
        				{
							case "xor":
								// Convertion: byte -> byte
								xor_encrypted = XOR_Encrypt(rawshellcode);
                                
								break;
							case "aes":
								// Convertion: 3 number of byte streams (input, key, iv) -> byte
								aes_encrypted = AES_Encrypt(rawshellcode);
								break;								

							case "aes_xor":
								// Convertion: input byte -aes-> aes_byte -xor-> aes_xor_byte
								aes_xor_encrypted = AES_XOR_Encrypt(rawshellcode);
								break;

							case "aes_xor_b64":
								// Convertion: input byte -aes-> aes_byte -xor-> aes_xor_byte -b64-> aes_xor_b64_byte
								aes_xor_b64_encrypted = AES_XOR_B64_Encrypt(rawshellcode);
								break;

							default:
                            	// If wrong options inputed
                            	banner();
                            	break;
        				}
                    }
                    else
                    {
                        Console.WriteLine("[!] Couldn't detect file input content.");
                        Environment.Exit(0);
                    }
                }
                catch
                {
					Console.WriteLine("[!] Error encrypting");
                }
            }
		}
	}
}