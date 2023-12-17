# VM and Infrastructure Setup

This document provides a step-by-step guide on how to set up the virtual machines (VMs) and infrastructure for this project using Vagrant.


## Prerequisites
Before you begin, ensure you have the following software installed on your machine:

- [Vagrant](https://www.vagrantup.com/downloads) - Download and install Vagrant.
- [VirtualBox](https://www.virtualbox.org/wiki/Downloads) - Download and install VirtualBox.

### Mac Alternatives
If you're using a Mac, you can use the following alternatives:

- [Docker](https://www.docker.com/get-started) - Download and install Docker.
- [Homebrew](https://brew.sh/) - If you prefer, you can use Homebrew to install Vagrant: `brew install vagrant`.

## VM Setup
Follow these steps to set up the virtual machines:

1. **Clone and open the repository**
   ```bash
   git clone https://github.com/tecnico-sec/a31-tilde-david-guilherme.git
   cd a31-tilde-david-guilherme
   ```

2. **Navigate the to Vagrant folder**  
    ```
    cd vagrant
    ```
3. **Find the right Vagrantfile**

    If you are using Windows, and previously installed virtualbox, you should use the file called `Vagrantfile.vbox`. If you are using MAC, and previously installed Docker, you should use the file called `Vagrantfile.docker`. 

    Rename the file you are using to just `Vagrantfile`, without the filetype ending. You do not need to do anything with the other file. 

4. **Start the Vagrant environment**
   ```bash
   vagrant up
   ```
   Make sure you are in the `Vagrant` directory when you run this command. 

   Running this command may take a while, so just wait while Vagrant downloads the base boxes and provisions the VMs. 
5. **Access the VMs**
   
    After Vagrant finishes provisioning the VMs, you can access individual VMs using SSH:

    ```
    vagrant ssh <name of VM>
    ```
    Replace `name of VM` with the specific VM you want to access. The VMs set up for this project are:
    | Machine               | Name of VM                   |
    | --------------------- | ---------------------------- |
    | Client machine        | a31-MediTrack-client-machine |
    | Application server    | a31-MediTrack-app-server     |
    | Database server       | a31-MediTrack-db-server      |
    | Authentication server | a31-MediTrack-auth-server    |
    | Firewall server       | a31-MediTrack-fw-server      |

6. **Stop VMs**
   
   When you're done, you can pause the VMs using halt:
    
    ```
    vagrant halt
    ```

7. **Remove VMs**
   
    To remove the VMs and clean up, use:
    
    ```
    vagrant destroy -f
    ```
    **Note:** This will permanently delete the VMs and their data.
    