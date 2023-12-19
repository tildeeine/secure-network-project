# A31 MediTrack Project Read Me

## Team

| Number | Name              | User                             | E-mail                              |
| -------|-------------------|----------------------------------| ------------------------------------|
| 108444  | Tilde Eriksen Eine     | <https://github.com/tildeeine>   | <mailto:tilde.eine@tecnico.ulisboa.pt>   |
| 96392  | Guilherme Luís Francisco Soares      | <https://github.com/guilhas07>     | <mailto:guilherme.luis.s@tecnico.ulisboa.pt>     |
| 89377  | David Daniel Oliveira Cruz  | <https://github.com/ddavidcruzr> | <mailto:daviddcruz@tecnico.ulisboa.pt> |

<img src="./img/tilde_profileimg.jpg" width="150">

<img src="./img/guilherme_profile.png" height="150px"> ![Charlie](img/charlie.png)

*(add face photos with 150px height; faces should have similar size and framing)*

## Contents

This repository contains documentation and source code for the *Network and Computer Security (SIRS)* project.

The [REPORT](REPORT.md) document provides a detailed overview of the key technical decisions and various components of the implemented project.
It offers insights into the rationale behind these choices, the project's architecture, and the impact of these decisions on the overall functionality and performance of the system.

This document presents installation and demonstration instructions.

*(adapt all of the following to your project, changing to the specific Linux distributions, programming languages, libraries, etc)*

## Installation

To see the project in action, it is necessary to setup a virtual environment, with 2 internal networks and 5 machines.  

The following diagram shows the networks and machines:

![Infrastructure](img/inrfa-op2-single-fw.png)

### Prerequisites

All the virtual machines are based on: Linux 64-bit, Kali 2023.3  

[Download](https://...link_to_download_installation_media) and [install](https://...link_to_installation_instructions) a virtual machine of Kali Linux 2023.3.  
Clone the base machine to create the other machines.


### Machine configurations

For each machine, there is an initialization script with the machine name, with prefix `init-` and suffix `.sh`, that installs all the necessary packages and makes all required configurations in the a clean machine.

Since we use Vagrant, we should not need to manually run anything. To see detailed instuctions for project setup, please see [SETUP](SETUP.md). 

Here, we will explain the content of each machine, and give commands to verify and and test for each machine. You can access the machines either through SSH, as described in [SETUP](SETUP.md), or open them normally through the vbox GUI. 


#### Application Server

This machine runs the main functionality of the MediTrack system, by running the backend and exposing a CLI API to clients. It runs the MediTrackBackend code, using the CryptoLib. It should be open to communication from clients runnng the Client program. 

To verify:

```sh
$ setup command
```

To test:

```sh
$ test command
```

*(replace with actual commands)*

The expected results are ...

*(explain what is supposed to happen if all goes well)*

If you receive the following message ... then ...

*(explain how to fix some known problem)*

#### Client machine

*(similar content structure as Machine 1)*

#### Firewall Server

*(similar content structure as Machine 1)*

#### Database Server

*(similar content structure as Machine 1)*

#### Authentication Server

*(similar content structure as Machine 1)*


## Demonstration

Now that all the networks and machines are up and running, ...

*(give a tour of the best features of the application; add screenshots when relevant)*

```sh
$ demo command
```

*(replace with actual commands)*

*(IMPORTANT: show evidence of the security mechanisms in action; show message payloads, print relevant messages, perform simulated attacks to show the defenses in action, etc.)*

This concludes the demonstration.

## Additional Information

### Links to Used Tools and Libraries

- [Java 11.0.16.1](https://openjdk.java.net/)
- [Maven 3.9.5](https://maven.apache.org/)
- ...

### Versioning

We use [SemVer](http://semver.org/) for versioning.  

### License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) for details.

----
END OF README
