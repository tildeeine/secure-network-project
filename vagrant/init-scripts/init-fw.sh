#!/bin/bash

# VMSETTINGS: Adapter1, internal network, sw-1, promiscuous mode allow VMs. Adapter2, internal network, sw-2, promiscuous mode allow VMs. Adapter3, NAT. 

# Configure the network interfaces
sudo ip addr add 192.168.0.10/24 dev eth0 # App server
sudo ip addr add 192.168.1.20/24 dev eth1 # Database server
sudo ip link set dev eth0 up
sudo ip link set dev eth1 up


# Activate IP forwarding
sudo sysctl net.ipv4.ip_forward=1

# Set up NAT to let AS access the internet
sudo iptables -t nat -F            # Flushes all the rules from table NAT
sudo iptables -t nat -A POSTROUTING  -o eth2 -j MASQUERADE    # Creates a source NAT on interface eth2

# ! edit the corresponding /etc/network/interfaces file.

#! ou should also enable IP forwarding permanently on VM2. For that you need to edit /etc/sysctl.conf and uncomment the following line: net.ipv4.ip_forward=1

# Set up firewall rules
# Enable ufw to start on boot, add default rules, and deny all incoming traffic
sudo ufw --force reset
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw enable

# Allow loopback interface
sudo ufw allow in on lo
sudo ufw allow out on lo

# Allow HTTP connections from external machines (Auth and Client)
sudo ufw allow in on eth2 to any port 80

# Redirect all HTTP connections to the AS
sudo iptables -t nat -A PREROUTING -i eth0 -p tcp --dport 80 -j DNAT --to-destination 192.168.0.20:80

# Allow TLS/SSL connections from AS to the DB
sudo ufw allow in on eth0 from 192.168.0.20 to 192.168.1.30 port 443

# Allow TLS/SSL connections from DB to AS
sudo ufw allow in on eth1 from 192.168.1.30 to 192.168.0.20 port 443

# Drop all other incoming and forwarding traffic
sudo ufw deny in on eth0
sudo ufw deny in on eth1

# Restart to apply changes
sudo systemctl restart NetworkManager
