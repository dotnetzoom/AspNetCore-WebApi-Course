"C:\Program Files\Git\usr\bin\openssl.exe" genrsa -out private.key 2048
"C:\Program Files\Git\usr\bin\openssl.exe" req -new -x509 -key private.key -out publickey.cer -days 1398
"C:\Program Files\Git\usr\bin\openssl.exe" pkcs12 -export -out idp.pfx -inkey private.key -in publickey.cer
pause