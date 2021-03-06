# DivingApplication-Backend
.NET Core 3.1 Backend for Diving Application

## Implementation (MosCoW)

|Done|Task|Priority|RP|Completed Date|Note|
|:---:|:---|:---:|:---:|:---:|:---:|
|<ul><li>- [x] </li></ul>|**User**: Register User|Must | Richard | 20 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Sending Verification Email |Must | Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Verify Email Address By Code |Must | Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Adding User|Must | Richard | 20 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Partially Update User|Must | Richard | 20 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get User by ID|Must | Richard | 20 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all owing Posts|Must | Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all liked Posts|Must | Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all saved Posts|Must | Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Following & Followers|Must | Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all Following|Must | Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Get all Follwers|Must | Richard |24 Feb 2020||
|<ul><li>- [x] </li></ul>|**User**: Has a Coach Role|Must | Richard |26 Feb 2020||
|<ul><li>- [ ] </li></ul>|**User**: Subsribe User|Could | Could ||Can be implemented in frontend |
|<ul><li>- [x] </li></ul>|**Authorization**: JWT Authorization|Must | Richard | 21 Feb 2020||
|<ul><li>- [x] </li></ul>|**Authorization** Role: Normal User|Must | Richard | 21 Feb 2020||
|<ul><li>- [x] </li></ul>|**Authorization** Role: Admin |Must | Richard | 21 Feb 2020||
|<ul><li>- [x] </li></ul>|**Authorization** Policy: NormalAndAdmin |Must | Richard | 21 Feb 2020||
|<ul><li>- [x] </li></ul>|**Authorization**: User Reaching Test|Must | Richard | 22 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Create Post|Must | Richard | 22 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post|Must | Richard | 22 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post Pagination|Must | Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Previous & Next Page Uri|Must | Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post Sorting|Must | Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post Searching by Title|Must | Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Post DataShape|Must | Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Partially Update Post|Must | Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Delete Post|Must | Richard | 23 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Like a Post|Must | Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Save a Post|Must | Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**Post**: Get Posts From Following Users |Must | Richard |25 Feb 2020 ||
|<ul><li>- [ ] </li></ul>|**Post**: Post should have a Location (longitude, latitude) |Must | Richard | ||
|<ul><li>- [x] </li></ul>|**Comment**: Create Comment |Must | Richard |24 Feb 2020 ||
|<ul><li>- [x] </li></ul>|**Comment**: Partially Update Comment |Must | Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**Comment**: Find Comment |Must | Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**Comment**: Find Comments for Post |Must | Richard | 24 Feb 2020||
|<ul><li>- [x] </li></ul>|**Comment**: Delete Comment |Must | Richard | 24 Feb 2020||
|<ul><li>- [ ] </li></ul>|**Comment**: Like Comment |Could | Richard | ||
|<ul><li>- [x] </li></ul>|**CoachInfo**: Create CoachInfo |Must | Richard |1 Mar 2020| For storing the info of certain coach user|
|<ul><li>- [x] </li></ul>|**CoachInfo**: Get CoachInfo by ID|Must | Richard | 1 Mar 2020||
|<ul><li>- [x] </li></ul>|**CoachInfo**: Get CoachInfo by Location|Must | Richard | ||
|<ul><li>- [x] </li></ul>|**CoachInfo**: Get CoachInfo by FollowersCount|Must | Richard |1 Mar 2020 ||
|<ul><li>- [x] </li></ul>|**CoachInfo**: Partially Update Coach Info |Must | Richard | 1 Mar 2020||
|<ul><li>- [x] </li></ul>|**CoachInfo**: Delete Coach Info |Must | Richard | 1 Mar 2020||
|<ul><li>- [x] </li></ul>|**ServiceInfo**: Create ServiceInfo |Must | Richard |  1 Mar 2020| Storing the service you provide|
|<ul><li>- [x] </li></ul>|**ServiceInfo**: Partially Update ServiceInfo |Must | Richard |  1 Mar 2020||
|<ul><li>- [x] </li></ul>|**ServiceInfo**: Get ServiceInfo by Location |Must | Richard | ||
|<ul><li>- [x] </li></ul>|**ServiceInfo**: Get ServiceInfo by Id |Must | Richard | 1 Mar 2020 ||
|<ul><li>- [x] </li></ul>|**ServiceInfo**: Get ServiceInfo by FollowersCount |Must | Richard | 1 Mar 2020 ||











## Underwater Photo (Style Transfrom)
[[GitHub]](https://github.com/cameronfabbri/Underwater-Color-Correction)


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


### There is already an object named 'XXXX' in the database.

Solution: Delete the Database using "context.Database.EnsureDeleted();" then remove the old migraiton and migrate again in Package Manager


### (When using MainKit to Sending Email) mx.google.com gave this error:
[40.92.254.58 14] Messages missing a valid address in From: header, or having no From: header, are not accepted. cq16si1609741pjb.130 - gsmtp

Solution: 

Adding this in the Sending:

```c#
sendingMessage.From.Add(new MailboxAddress("GoFun", "diving_app_2020@outlook.com"));
```
