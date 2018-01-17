# SP365
Component Library to Use Sharepoint 365 in Bizagi

Introduction
------------

This component library enables your processes to synchronize your Bizagi files with your Sharepoint 365, at the moment this provides these three options:

  - Publish Files
  - Delete Files
  - Create Folders

Installation
------------

Installing this component library is simple, follow these steps:

1. Download the 3 DLLs from the https://github.com/crossleyjuan/SP365/releases/download/Release_1_0/Release.1.0.zip and uncompress them in a local folder.
2. The file SP365.dll is the component library that you will need to register in the Component Library panel, as explained here: http://help.bizagi.com/bpmsuite/en/enterprise__net_example.htm
3. The dlls Microsoft.SharePoint.Client.dll and Microsoft.SharePoint.Client.Runtime.dll comes from Microsoft and they need to be copied to the bin folder of the web application, they dont require registration in the Studio but they will need to be uploaded manually in order for this to work.

Usage
-----

Using the dll is quite simple, here are the 3 supported methods:

Upload a file from bizagi data model to SP365:
```javascript
    var website = "http://mysharepoint/repository";
    var scontent = <customer.picture[1].data>;
    var file = <customer.picture[1].fileName>;
    var user = "my_sp_user";
    var password = "my_sp_pass";
    var parentFolder = "Documents";
    var folder = "innerfolder";
    BizagiCL.SP365.PublishFile(website, scontent, file, user, password, parentFolder, folder);
```

Delete a file:
```javascript
    var website = "http://mysharepoint/repository";
    var file = <customer.picture[1].fileName>;
    var user = "my_sp_user";
    var password = "my_sp_pass";
    var parentFolder = "Documents";
    var folder = "innerfolder";
    BizagiCL.SP365.DeleteFile(website, file, user, password, parentFolder, folder);
```

Create a folder:
```javascript
    var website = "http://mysharepoint/repository";
    var file = <customer.picture[1].fileName>;
    var user = "my_sp_user";
    var password = "my_sp_pass";
    var parentFolder = "Documents";
    var folder = "innerfolder";
    BizagiCL.SP365.CreateFolder(website, user, password, parentFolder, folder);
```


Support
-------

If you have any suggestions, changes or want to include new features, that's easy... create a fork of this project, perform your changes and send a pull request, we will incorporate your changes in this repo to keep growing the library.
