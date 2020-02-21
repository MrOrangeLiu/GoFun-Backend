# DivingApplication-Backend
.NET Core 3.1 Backend for Diving Application


## Debugging

### IIS Express Remote Access

[[教學]](https://blog.yowko.com/iis-express-allow-external-access/)

##### 1. Using `ipconfig` in Commnad Prompt to get IPv4 Address (In my case, it's 192.168.1.135:65000)
##### 2. Using `netsh http add urlacl url=http://192.168.1.135:65000/ user=everyone` in the Command Prompt to add the URL to access control list

Other Helpful Commands:
```
netsh http show urlacl

netsh http delete urlacl url=http://192.168.1.135:65000/
```

##### 3.In the C:\Users\{UserName}\source\repos\{ApplicationName}\.vs\DivingApplication\config\applicationhost.config
Modify the <Sites>
  
  
```
// Adding this to id = 2

<binding protocol="http" bindingInformation="*:65000:192.168.1.135" />

```

##### 4. Searching "wf.msc" in the start menu to adding the 65000 port, or just close the Firewall

##### 5. Run Visual Studio as Admin

Then, should be able to connect to (http://192.168.1.135:65000/api/users/test)

https://blog.yowko.com/iis-express-allow-external-access/
