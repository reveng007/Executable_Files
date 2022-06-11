1.
```
$ msfvenom --platform windows -a x64 -p windows/x64/messagebox Text="Hello from shellcode" -f exe > mssgbox_x64.exe
```
[mssgbox_x64.exe](https://github.com/reveng007/Executable_Files/blob/main/dotNETbinaries/mssgbox_x64.exe)

2.
```
$ msfvenom --platform windows -a x64 -p windows/x64/messagebox Text="Hello from shellcode" -f csharp > mssgbox_csharp_x64.txt
```
[mssgbox_csharp_x64.txt](https://github.com/reveng007/Executable_Files/blob/main/dotNETbinaries/mssgbox_csharp_x64.txt)

