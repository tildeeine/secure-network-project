# A31 MediTrack Project Read Me

## Team

| Number | Name              | User                             | E-mail                              |
| -------|-------------------|----------------------------------| ------------------------------------|
| 108444  | Tilde Eriksen Eine     | <https://github.com/tildeeine>   | <mailto:tilde.eine@tecnico.ulisboa.pt>   |
| 96392  | Guilherme Luís Francisco Soares      | <https://github.com/guilhas07>     | <mailto:guilherme.luis.s@tecnico.ulisboa.pt>     |

<img src="./img/tilde_profileimg.jpg" height="150px">   <img src="./img/guilherme_profile.png" height="150px">

## Contents

This repository contains documentation and source code for the *Network and Computer Security (SIRS)* project.

The [REPORT](REPORT.md) document provides a detailed overview of the key technical decisions and various components of the implemented project.
It offers insights into the rationale behind these choices, the project's architecture, and the impact of these decisions on the overall functionality and performance of the system.

This document presents installation and demonstration instructions.

## Installation

To see the project in action, it is necessary to set up a virtual environment, with 5 machines, one DMZ and one internal network, and a third network that simulates an external network, or running over the internet.   

The following diagram shows the networks and machines:

![Infrastructure](img/Infrastructure.png)

All the virtual machines are based on: Ubuntu 64-bit.

We chose to use Vagrant to automate the setup of our machines and infrastructure as much as possible. The following instructions will therefore be focused on how to set up vagrant. 

### Prerequisites

Before you begin, ensure you have the following software installed on your machine:
- [Vagrant](https://www.vagrantup.com/downloads) - Download and install Vagrant.
- [VirtualBox](https://www.virtualbox.org/wiki/Downloads) - Download and install VirtualBox.

### Machine configurations

For each machine, there is an initialization script with the machine name, with prefix `init-` and suffix `.sh`, that you can find in the [init-scripts folder](./vagrant/init-scripts/). These scripts, combined with the shell scripts in the Vagrantfile, install all the necessary packages and make all the required configurations in a clean machine.

Here, we will first give instructions for setting up Vagrant, before we explain the content of each machine, and give commands to verify and test the setup for each  machine. To access the machines after they have been set up, you have to use Vagrant SSH.  

#### Vagrant setup
1. **Clone and open the repository**
    On your host machine, run these commands to clone and open the repository:
   ```bash
   git clone https://github.com/tecnico-sec/a31-tilde-david-guilherme.git
   cd a31-tilde-david-guilherme
   ```
2. **Navigate the to Vagrant folder**  
    ```
    cd vagrant
    ```
3. **Start the Vagrant environment**
   ```bash
   vagrant up
   ```
   Make sure you are in the `Vagrant` directory when you run this command. 

   Running this command may take a while, so just wait while Vagrant downloads the base boxes and provisions the VMs. 
4. **Access the VMs**
   
    After Vagrant finishes provisioning the VMs, you can access individual VMs using vagrant SSH:

    ```
    vagrant ssh <name of VM>
    ```
    Replace `name of VM` with the specific VM you want to access. The VMs set up for this project are:
    | Machine               | Name of VM     |
    | --------------------- | -------------- |
    | Client machine        | client_machine |
    | Application server    | app_server     |
    | Database server       | db_server      |
    | Authentication server | auth_server    |
    | Firewall server       | fw_server      |

You should now be able to access the VMs. 

When you are done and want to stop running the VMs, you can use halt:
```sh
vagrant halt
```
To remove the VMs permanently and clean up, use:
```sh
vagrant destroy -f
```
**Note:** This will permanently delete the VMs and their data.

#### Application Server

This machine runs the main functionality of the MediTrack system, by running the backend and exposing a CLI API to clients. It runs the MediTrackBackend code, using the CryptoLib. It should be able to communicate with clients and the database over TLS/SSL. 

**Verify Setup:**

**Vagrant**
```sh
vagrant ssh app_server
```
This command will log you into the application server VM. After executing this command, you should be inside the VM, and the command prompt should reflect that, as you should see the user as `vagrant@a31-MediTrack-app-server:~$`.

**Dotnet**
```sh
dotnet --version
```
This command checks if the .NET runtime is installed. You should see the version of .NET installed without any errors.

**To test:**

Check that you can run the AppServer program:
```sh
/AppServerApp/AppServer
```

Check that the ip address for the interface has been set up correctly:
```sh
ip addr show enp0s8
```
This command displays the network configurations for the VM. The provisioning script should set up the enp0s8 interface with the IP address `192.168.0.20`. 

#### Firewall Server

The firewall server is an important part of securing the application and database servers in our system, and ensuring correct communication between different components in the MediTrack system. It manages network traffic, controls access, and ensures that data exchanges follow predefined rules. Below are instructions for verifying and testing the setup of the firewall server. 

**To verify:**
**Vagrant**

```sh
vagrant ssh fw_server
```
This command logs you into the firewall server VM. After executing this command, you should be inside the VM, and the command prompt should reflect that, as you should see the user as `vagrant@a31-MediTrack-fw-server:~$`.

**Iptables persistent**
```sh
iptables-persistent --version
```
This command checks if iptables-persistent is installed. This is important for the firewall and NAT rules that we set up with iptables-persistent to be persistent between boots of the VM. You should see the version of iptables-persistent installed during the Vagrant provisioning. Since this installation requires answers to user prompts, there might be issues during the installation, even though we use debconf in the Vagrant provisioning. If you don't have iptables-persistent on your macine, please run the following command to install it:

```sh
sudo apt install iptables-persistent
```
Answer yes to the prompts. This should automatically save the current iptables setup, so the rules that were declared in the [firewall initialization script](/vagrant/init-scripts/init-fw-server.sh). 

**Test setup:**
Still using SSH to access the firewall machine, run the following commands to test the setup. 

```sh
sudo iptables -L -n
```
This command shows you the current iptables setup. (INSERT WHAT TABLE SHOULD LOOK LIKE HERE)

```sh
ip addr show enp0s8
ip addr show enp0s9
ip addr show enp0s10
```
These commands display the network configurations for the different interfaces we set up for the VM. The provisioning script should set up the enp0s9 interface with the IP address `192.168.0.10`, enp0s10 interface as `192.168.1.10`, and the enp0s8 interface with the address `192.168.2.10`.

#### Database Server

The database server is responsible for hosting the MySQL database used by the MediTrack system. It is essential for storing and managing data, and should only communicate with the application server, through the firewall. 

**Verify setup:**
**Vagrant**
```sh
vagrant ssh db_server
```
This command logs you into the database server VM. After executing this command, you should be inside the VM, and the command prompt should reflect that, as you should see the user as `vagrant@a31-MediTrack-db-server:~$`.

**MySQL**

```
mysql --version
```
This command checks if MySQL is installed. You should see the version of MySQL installed without any errors.

**Test setup:**

First, check that the dummy database content for demonstration purposes has been set up as intended. 

```sh
sudo mysql -mysql -p'1234' --database='meditrack'
```
This should log you in to MySQL. To check the database, run the following commands:

```sql
use meditrack;
show tables;
```
You should now see the three tables Patients, Consultation and __EFMigrationsHistory. Verify that all of these contain data by running:

```sql
SELECT * FROM Patients;
SELECT * FROM Consultation;
SELECT * FROM __EFMigrationsHistory;
```
When you have verified that all of these contain data, you can exit MySql by writing

```sql
exit;
```
If you have any issues with the database or missing data, you can try importing the data again by running the following command:

```sh
mysql -umysql -p'1234' --database='meditrack' < /MediTrack/MediTrackBackend/Migrations/init.sql
```

Next, we verify the IP setup by running:
```sh
ip addr show enp0s8
```
This command displays the network configurations for the VM. The provisioning script should set up the enp0s8 interface with the IP address `192.168.1.30`.

#### Authentication Server
The authentication server is an important part of ensuring the security of the system. It is used to verify the signatures and encryption used in the other components of the system. 

**Vagrant**
```sh
vagrant ssh auth_server
```
This command logs you into the database server VM. After executing this command, you should be inside the VM, and the command prompt should reflect that, as you should see the user as `vagrant@a31-MediTrack-auth-server:~$`.

**Dotnet**

```sh
dotnet --version
```
This command checks if the .NET runtime is installed. You should see the version of .NET installed without any errors.

**Testing**

Check that you can run the AppServer program:
```sh
/AppServerApp/AppServer
```

Verify that the network configurations for the IP were set up correctly:
```sh
ip addr show enp0s8
```

#### Client machine

This machine server as a client in the MediTrack system. The client can interact with the application server to securely access patient data. The machine runs the client-side code, which runs a CLI interface for the MediTrack program. 

**Verify setup:**

**Vagrant**

```sh
vagrant ssh client_machine
```
You should be able to SSH into the machine without issues. After executing the command, you should be inside the VM, and the command prompt should have the user `vagrant@a31-MediTrack-client-machine`

**Dotnet**

```sh
dotnet --version
```
This command checks if the .NET runtime is installed. You should see the version of .NET installed without any errors. 

**Test setup:**
The client can be considered the last entity to connect to the network, as it is connected every time a patient or a physician wants to perform a task. You should therefore verify that all other machines in the system are up and running as intended before testing this machine. 

INSERT TEST COMMANDS

```sh
ip addr show enp0s8
```
This command displays the network configurations for the VM. The provisioning script should set up the enp0s8 interface with the IP address `192.168.1.30`.

## Demonstration

Now that all the networks and machines are up and running, we can have a look at the actual functionality of the system. 


*(give a tour of the best features of the application; add screenshots when relevant)*
- Show the different functions of the client-side system
- Show what it looks like when we encrypt or decrypt a record
- Perform simulated attacks to show the defenses in action
    - Use nmap and try port scanning from client on firewall server
    - Show "packet interception" to show that both the communication itself (TLS) and the packets (secure documents) are encrypted as a security mechanism

```sh
$ demo command
```

*(replace with actual commands)*

*(IMPORTANT: show evidence of the security mechanisms in action; show message payloads, print relevant messages, perform simulated attacks to show the defenses in action, etc.)*

This concludes the demonstration.

## Additional Information

### Links to Used Tools and Libraries

- [C#](https://learn.microsoft.com/en-us/dotnet/csharp/)
- [.NET](https://learn.microsoft.com/en-us/dotnet/)
- [Vagrant](https://www.vagrantup.com/)
- [VirtualBox](https://www.virtualbox.org/)
- [MySQL](https://www.mysql.com/)

### License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) for details.

----
END OF README



