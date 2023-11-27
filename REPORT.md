# A31 MediTrack Project Report

## 1. Introduction

Today, more and more services are undergoing a digital transformation, to make use of the benefits in communication and distribution that come with it. Healthcare systems are also a part of this transition, for example through the use of Electronic Health Records (EHR's). These records are used for digitizing patient data management, which makes it easier for healthcare providers to manage the data, and for the broader healthcare services to share patient data between themselves. 
This means MediTrack can greatly ehance health services and facilitate collaboration between healthcare entities. However, healthcare data is very sensitive, and needs to be handled in a way that guarantees the security of the data. 

The first part of the project, secure documents, is about designing and implementing a way to ensure confidentiality and authenticity for the EHR's. This is done throgh the design and implementation of a custom cryptographic library, [CryptoLib](./src/MediTrack/CryptoLib/CryptoLib.cs). CryptoLib provides essential cryptographic operations such as document protection and sender verification. Through this use of encryption and digital signatures, CryptoLib ensures the confidentiality and authenticity of patient data within the EHR's. 

<!ADD THE REST AS WE GO ALONG>
(_Provide a brief overview of your project, including the business scenario and the main components: secure documents, infrastructure, and security challenge._)

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


<!CONSIDER MENTIONING FRESHNESS ONCE ADDED. >


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

(_Provide a brief description of the built infrastructure._)

(_Justify the choice of technologies for each server._)

#### 2.2.2. Server Communication Security

(_Discuss how server communications were secured, including the secure channel solutions implemented and any challenges encountered._)

(_Explain what keys exist at the start and how are they distributed?_)

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
