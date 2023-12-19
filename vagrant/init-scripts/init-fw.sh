#!/bin/bash

# VMSETTINGS: Adapter1, internal network, sw-1, promiscuous mode allow VMs. Adapter2, internal network, sw-2, promiscuous mode allow VMs. Adapter3, NAT. 

# Configure the network interfaces
sudo ip addr add 192.168.0.10/24 dev eth0 # App server
sudo ip addr add 192.168.1.20/24 dev eth1 # Database server
sudo ip link set dev eth0 up
sudo ip link set dev eth1 up

# Activate IP forwarding
sudo sysctl net.ipv4.ip_forward=1

# ! edit the corresponding /etc/network/interfaces file.

#! ou should also enable IP forwarding permanently on VM2. For that you need to edit /etc/sysctl.conf and uncomment the following line: net.ipv4.ip_forward=1

# Set up firewall rules

# Enable the firewall 
sudo ufw --force reset
sudo ufw --force enable 

# Set default rules
sudo ufw default deny incoming
sudo ufw default deny outgoing

# Allow incoming TLS/SSL connections on eth2, address to app server #? Does it allow app server to responds to these connections?
sudo ufw allow in on eth2 from any to any port 443 proto tcp
sudo ufw route allow in on eth2 out on eth0 from any to any port 443 proto tcp 

#! This might allow the app server to initiate connections to external networks
#sudo ufw allow out on eth0 from 192.168.0.20 to any port 443 proto tcp

# Allow TLS/SSL connections from AS to the DB
sudo ufw allow from 192.168.0.20 to 192.168.1.30 port 443 proto tcp

# Allow TLS/SSL connections from DB to AS
sudo ufw allow from 192.168.1.30 to 192.168.0.20 port 443 proto tcp

# To allow Vagrant SSH connections
sudo ufw allow 22

# Drop all other incoming and forwarding traffic
sudo ufw deny in on eth0
sudo ufw deny in on eth1

# Restart to apply changes
sudo systemctl restart systemd-networkd
