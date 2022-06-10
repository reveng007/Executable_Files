1.
```
$ msfvenom --platform windows -a x64 -p windows/x64/messagebox Text="Hello from shellcode" -f exe > mssgbox_x64.exe
```
[link](https://github.com/reveng007/Executable_Files/blob/main/binaries/mssgbox_x64.exe)

2.
```
$ msfvenom --platform windows -a x64 -p windows/x64/messagebox Text="Hello from shellcode" -f csharp > mssgbox_csharp_x64.txt
```
[link](https://github.com/reveng007/Executable_Files/blob/main/binaries/mssgbox_csharp_x64.txt)

