# DivingApplication-Backend
.NET Core 3.1 Backend for Diving Application

## Implementation (MosCoW)

|Done|Task|Priority|RP|Completed Date|Note|
|:---:|:---|:---:|:---:|:---:|:---:|
|<ul><li>- [x] </li></ul>|**User**: Register User|Must Have| Richard | 20 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Sending Verification Email |Must Have| Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Verify Email Address By Code |Must Have| Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Adding User|Must Have| Richard | 20 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Partially Update User|Must Have| Richard | 20 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get User by ID|Must Have| Richard | 20 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all owing Posts|Must Have| Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all liked Posts|Must Have| Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all saved Posts|Must Have| Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Following & Followers|Must Have| Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all Following|Must Have| Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all Follwers|Must Have| Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**Authorization**: JWT Authorization|Must Have| Richard | 21 Feb 2020||
|<ul><li>- [x] </li></ul>|**Authorization** Role: Normal User|Must Have| Richard | 21 Feb 2020||
|<ul><li>- [x] </li></ul>|**Authorization** Role: Admin |Must Have| Richard | 21 Feb 2020||
|<ul><li>- [x] </li></ul>|**Authorization** Policy: NormalAndAdmin |Must Have| Richard | 21 Feb 2020||
|<ul><li>- [x] </li></ul>|**Authorization**: User Reaching Test|Must Have| Richard | 22 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Create Post|Must Have| Richard | 22 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post|Must Have| Richard | 22 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post Pagination|Must Have| Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Previous & Next Page Uri|Must Have| Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post Sorting|Must Have| Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post Searching by Title|Must Have| Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post DataShape|Must Have| Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Partially Update Post|Must Have| Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Delete Post|Must Have| Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Like a Post|Must Have| Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Save a Post|Must Have| Richard | 24 Feb 2020||







## Debugging

### IIS Express Remote Access

[[Tutorial]](https://blog.yowko.com/iis-express-allow-external-access/)

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
