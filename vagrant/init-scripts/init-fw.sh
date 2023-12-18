#!/bin/sh

# VMSETTINGS: Adapter1, internal network, sw-1, promiscuous mode allow VMs. Adapter2, internal network, sw-2, promiscuous mode allow VMs. Adapter3, NAT. 

# Configure the network interfaces
sudo ifconfig eth0 192.168.0.10/24 up # App server
sudo ifconfig eth1 192.168.1.19/24 up # Database server

# Activate IP forwarding
sudo sysctl net.ipv4.ip_forward=1

# Set up NAT to let AS access the internet
sudo iptables -t nat -F            # Flushes all the rules from table NAT
sudo iptables -t nat -A POSTROUTING  -o eth2 -j MASQUERADE    # Creates a source NAT on interface eth2

# Restart to apply changes
sudo systemctl restart NetworkManager

# ! edit the corresponding /etc/network/interfaces file.

#! ou should also enable IP forwarding permanently on VM2. For that you need to edit /etc/sysctl.conf and uncomment the following line: net.ipv4.ip_forward=1

# Set up firewall rules
# Flush existing rules and set the default policies
sudo iptables -F
sudo iptables -P INPUT DROP
sudo iptables -P FORWARD DROP
sudo iptables -P OUTPUT ACCEPT

# Allow loopback interface
sudo iptables -A INPUT -i lo -j ACCEPT
sudo iptables -A OUTPUT -o lo -j ACCEPT

# Allow SSH and HTTP connections from external machines (Auth and Client)
sudo iptables -A INPUT -i eth2 -p tcp --dport 22 -j ACCEPT
sudo iptables -A INPUT -i eth2 -p tcp --dport 80 -j ACCEPT

# Redirect all HTTP connections to the App server
sudo iptables -t nat -A PREROUTING -i eth0 -p tcp --dport 80 -j DNAT --to-destination 192.168.0.20:80

# Allow SSH connections from App server to the database
sudo iptables -A FORWARD -i eth0 -o eth1 -s 192.168.0.20 -d 192.168.1.30 -p tcp --dport 22 -j ACCEPT

# Allow SSH connections from database to app server
sudo iptables -A FORWARD -i eth0 -o eth1 -s 192.168.1.30 -d 192.168.0.20 -p tcp --dport 22 -j ACCEPT

# Drop all other incoming and forwarding traffic
sudo iptables -A INPUT -j DROP
sudo iptables -A FORWARD -j DROP

# Make iptables persistent
sudo apt install iptables-persistent -y

# Save current iptables rules
# FOR IPv4
sudo sh -c 'iptables-save > /etc/iptables/rules.v4'
# FOR IPv6
sudo sh -c 'ip6tables-save > /etc/iptables/rules.v6'

# Enable netfilter-persisten.service to automatically apply iptables rules on boot
sudo systemctl enable netfilter-persistent.service
