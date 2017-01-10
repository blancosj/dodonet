# dodonet 
(moved from codeplex)

## Description

It is a framework for creating distributed applications based on HTTP protocol.

It allows to get a web server with all features (dynamic pages, AJAX/JSON) in any application. 
Also It is possible to create high-scalability and high-performance systems.

## Architecture

DodoNet is a network of nodes. The HTTP messages go from node a other, 
even they can skip between nodes if communication direct does not exist.

In figure of below It shows two separated networks in which Node 1 send message to
Node 4 through Node 3.

![image](https://raw.githubusercontent.com/blancosj/dodonet/master/Resources/scheme_jumps.jpg)

When a node receive a message, it is assigned to a module that process it.
After this process the module to create a reply to send back to node that sent it.

Note that modules are used like plug-in. They are called applications. A node can have multiples applications.
A application have got methods that other node can be execute remotely.

In other hand, if you would want to create a web server, it would be necessary to add a special application to node.
This application have got a methods that allow process a request HTTP and generate a reply.

![image](https://raw.githubusercontent.com/blancosj/dodonet/master/Resources/process_http.jpg)


Each node is identified with unique name. This name can be used to send messages nevertheless you can use node IP
if you know it.

## Features
- All connections are TCP
- NAT-Traversal
- Compression of HTTP content with GZIP
- Support AJAX and serialization JSON
- Support upload HTML Form with any type of input, even files by using type of content.
- Include module JavaScript to call remote methods
- Kernel thread-pool is SmartThreadPool http://www.codeplex.com/smartthreadpool
- Interpreter of dynamic pages similar to ASPX inspired on modules of Mono.NET http://www.mono-project.com
- Dynamic language used is JScript.NET. I used source code from Rotor proyect SSCL to debug.
- Module for distributing message to groups to which belong multiples nodes.
- Module for management connections or session between nodes.

## LightApp

LightApp is simple application that allow create a executable like Windows Application and Windows Services. 
It a only executable that you can execute like application with simple double-click on file in Windows Explorer or, 
execute as Window Service because let you install easily. When LightApp is started it read a script with 
language JScript.NET in which for instance, you can create a main node and add it modules, or it is up to you. 