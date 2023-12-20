# A31 MediTrack Project Report

## 1. Introduction

Today, more and more services are undergoing a digital transformation, to make use of the benefits in communication and distribution that come with it. Healthcare systems are also a part of this transition, for example through the use of Electronic Health Records (EHR's). These records are used for digitizing patient data management, which makes it easier for healthcare providers to manage the data, and for the broader healthcare services to share patient data between themselves. 
This means MediTrack can greatly ehance health services and facilitate collaboration between healthcare entities. However, healthcare data is very sensitive, and needs to be handled in a way that guarantees the security of the data. 

The first part of the project, secure documents, is about designing and implementing a way to ensure confidentiality and authenticity for the EHR's. This is done through the design and implementation of a custom cryptographic library, [CryptoLib](./src/MediTrack/CryptoLib/CryptoLib.cs). CryptoLib provides essential cryptographic operations such as document protection and sender verification. Through this use of encryption and digital signatures, CryptoLib ensures the confidentiality and authenticity of patient data within the EHR's. 

The second part of the project, infrastructure, is about setting up and configuring servers to support the MediTrack system. This is done through setting up a virtual environment with 5 machines and 3 networks, configuring the interfaces and programs running on each machine, and setting up the firewall and secure TLS/SSL communications for the system. 

The third and final part of the project is the security challenge, where we were introduced to some security challenges to handle for the MediTrack system. These challenges required us to expand our existing solution, as we will describe later in this report. 



(_Include a structural diagram, in UML or other standard notation._)

## 2. Project Development

### 2.1. Secure Document Format

#### 2.1.1. Design
Our cryptographic library, [CryptoLib](./src/MediTrack/CryptoLib/CryptoLib.cs), was designed to provide the security requirements of the MediTrack Electronic Health Records (EHR) system. The main objectives for this section were to ensure the _authenticity_ and _confidentiality_ of patient data and metadata in the documents. 

We made two key design choices in this phase:
1. **Hybrid Cryptography** - We use a mix of asymmetric and symmetric cryptography to provide confidentiality for the documents through encryption. 

   **Symmetric Cryptography**: We chose symmetric cryptography through AES with CBC-mode to encrypt the content of the documents itself, as symmetric cryptography is more efficient than asymmetric cryptography, and therefore better suited for larger amounts of data. However, we need a safe way to make sure the sender and recipient share this symmetric key. For this key exchange, we use asymmetric cryptography. 
   
    **Asymmetric Cryptography**: A unique symmetric key is generated for each document we call `protect()` on. This key, as well as the CBC-mode Initialization Vector, is then encrypted using the receiver's public key. This allows a safe exchange of keys for each document that is protected. 

2. **Digital signatures** - To provide _authenticity_ for the documents, each document is signed when it is encrypted. The user who calls `protect()` provides an authentication key, which is their private key, to sign the document. Then, the recipient provides the sender's public key when they call `unprotect()` as the authentication key for `unprotect()`. This allows users to easily check the authenticity of the document by verifying the sender. 



(_Include a complete example of your data format, with the designed protections._)
- Not sure what this means

#### 2.1.2. Implementation

We chose to use C# as our programming language for the implementation of secure documents. This decision was made based on what languages the team were most familiar with, that had support for cryptographic operations. C# does this through the `System.Security.Cryptography` namespace. C# provides a good balance of readability and efficiency, making it good for creating an understandable but efficient cryptographic library. 

The previously mentioned `System.Security.Cryptography` namespace allows us to use built-in cryptographic modules to provide secure documents. Specifically, we use the RSA and AES with CBC modules to provide hybrid encryption, as detailed above. 

<!We also use the `System.Diagnostics` namespace for debugging in `unprotect()`, if the header is wrong. This might not be relevant to mention if this is only debugging for our own sake during development>

In accordance with the project task requirements, our functions were designed as command-line interface (CLI) commands for ease of use. This makes it easy for us to test our commands. 

(_Include challenges faced and how they were overcome._)

<!Possibly include switch from asymmetric to hybrid cryptography approach >

### 2.2. Infrastructure

#### 2.2.1. Network and Machine Setup

We chose to use the following setup for our network: 
![Infrastructure](/img/Infrastructure.png)

We network setup contains two external machines, the client and the authentication server, and a firewall server acting as a bridge between the external and internal networks. The internal networks consist of one DMZ network, where the application server is, and one internal network where we find the database. The application server is the component of our system that ties together the other components to create a functioning MediTrack system.

The setup of our external machines is pretty simple. Each one is running a dotnet program to communicate with the other devices in the system. 
(something more about functionality?)

The firewall server runs iptables for firewall configurations and Network Address Translation (NAT). We decided to go with iptables for all the firewall configurations, because it is easy to set up and could be used for both the firewall rules and the NAT rules. We first started using ufw for the rules, because of the simplicity of the program, but had some difficulties combining this with the NAT and therefore decided to move over to all iptables for the firewall server. 

The application server runs the .NET program we developed and contains the main functionality of the secure documents section. We chose to use this technology due to the teams familiarity with #C and the .NET-framework, which provides a robust and scalable environment for the main program. Since this was the framework chosen for the main secure documents functionality, we continued using this for the communicating programs on the client and authentication server as well. 

For the database server, we are using MySQL. Again, this was selected mainly due to the teams familiarity with the system, as well as the widespread use it already has leading to a big community and therefore it is easy to find information on using the system. It is also easy to set up, as well as compatible with our .NET application. 

- ADD VAGRANT REASONING HERE, MAYBE?

#### 2.2.2. Server Communication Security

Communication security in this system is based mainly on three things
- Encrypting the EHR's, as per the _secure documents_ section
- Use of TLS/SSL for communication between devices
- The firewall server

These three things together work to ensure confidentiality for our system. On top of this, the digital signatures detailed in the section on [secure document design](#211-design) also provide authenticity for the communications. 

The use of both TLS/SSL and encryption of the EHR's work to provide security in layers. Not only is the sensitive content of the health records encrypted, but the communication channel they are transferred over is also encrypted. This adds an extra layer of security. 

The firewall adds security for the application server and database server through the use of NAT and firewall rules. We set the rules to drop as default, and only allow specific traffic that matches flows we expect to see from the legitimate communications. 
 
- ADD SOMETHING ABOUT FRESHNESS
- ADD SOMETHING ABOUT KEY EXCHANGE
- SHOULD WE INCLUDE THE SPECIFIC FIREWALL RULES?

CHALLENGES?

We faced some issues with setting up the firewall rules, as we found it difficult to keep track of which ports and addresses were used when communication arrived at the different interfaces for the firewall. We were also relatively unfamiliar with the use of iptables, so learning the syntax and use cases for the technology was also a big part of the challenge here. 

One such challenge was the result of our use of Vagrant for the system, as we don't build machines that are intended to be used with the GUI. The normal use of Vagrant systems is to SSH into the machines as needed. Therefore, when we first set up the firewall rules, we were unable to connect to the firewall server since the firewall rules didn't allow SSH connections. Because of this, we had to add an iptable rule to allow SSH connections directed at the firewall server, to be able to access it using Vagrant. This access was important for testing the system, as well as for getting the network traces of running the security tests.

- any other ones to mention here? Was the ceritificate implementation fine?

KEY DISTRIBUTION
- ANSWER THIS (_Explain what keys exist at the start and how are they distributed?_)

### 2.3. Security Challenge

#### 2.3.1. Challenge Overview

(_Describe the new requirements introduced in the security challenge and how they impacted your original design._)

#### 2.3.2. Attacker Model

(_Define who is fully trusted, partially trusted, or untrusted._)

(_Define how powerful the attacker is, with capabilities and limitations, i.e., what can he do and what he cannot do_)

#### 2.3.3. Solution Design and Implementation

(_Explain how your team redesigned and extended the solution to meet the security challenge, including key distribution and other security measures._)

(_Identify communication entities and the messages they exchange with a UML sequence or collaboration diagram._)  

## 3. Conclusion

(_State the main achievements of your work._)

(_Describe which requirements were satisfied, partially satisfied, or not satisfied; with a brief justification for each one._)

(_Identify possible enhancements in the future._)

(_Offer a concluding statement, emphasizing the value of the project experience._)

## 4. Bibliography

(_Present bibliographic references, with clickable links. Always include at least the authors, title, "where published", and year._)

----
END OF REPORT
