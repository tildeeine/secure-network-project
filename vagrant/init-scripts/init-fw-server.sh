#!/bin/bash

# VMSETTINGS: Adapter1, internal network, sw-1, promiscuous mode allow VMs. Adapter2, internal network, sw-2, promiscuous mode allow VMs. Adapter3, NAT. 

# Configure the network interfaces
sudo ip addr add 192.168.0.10/24 dev eth0 # App server
sudo ip addr add 192.168.1.20/24 dev eth1 # Database server
sudo ip link set dev eth0 up
sudo ip link set dev eth1 up

# Activate IP forwarding
sudo sysctl net.ipv4.ip_forward=1

# Write configurations to /etc/network/interfaces
echo "
auto eth0
iface eth0 inet static
    address 192.168.0.10
    netmask 255.255.255.0

auto eth1
iface eth1 inet static
    address 192.168.1.20
    netmask 255.255.255.0
" | sudo tee -a /etc/network/interfaces

# Enable IP forwarding permanently
sudo sed -i 's/^#net.ipv4.ip_forward=1/net.ipv4.ip_forward=1/' /etc/sysctl.conf
sudo sysctl -p

# Set up firewall rules

# Enable the firewall, flush NAT rules
# sudo ufw --force reset
# sudo ufw --force enable 
sudo iptables -t nat -F
sudo iptables -t nat -A POSTROUTING  -o eth2 -j MASQUERADE    # Creates a source NAT on interface eth2

# NAT rules
# Set default policies
sudo iptables -P INPUT DROP
sudo iptables -P FORWARD DROP
sudo iptables -P OUTPUT DROP

# Auth: 192.168.3.100
# Allow SSH connections from vagrant
sudo iptables -A FORWARD -p tcp -m tcp --dport 2222 -j ACCEPT # Requires fw to be set up first

# Forward incoming TLS/SSL connections on eth2 to app server (EXT -> AS)
sudo iptables -A FORWARD -d 192.168.3.10 -p tcp -m tcp --dport 443 -j ACCEPT
sudo iptables -t nat -A PREROUTING -d 192.168.3.10 -p tcp -m tcp --dport 443 -j DNAT --to-destination 192.168.0.20:5001
# Allow replies to established connection
sudo iptables -A FORWARD -i eth0 -o eth2 -m state --state ESTABLISHED -j ACCEPT
#? Do we need a nat rule for routing back to the client?

# Forward incoming TSL/SSL connections from app with destination db server (AS -> DB)
sudo iptables -A FORWARD -s 192.168.0.20 -d 192.168.1.30 -p tcp -m tcp --dport 443 -j ACCEPT
sudo iptables -t nat -A PREROUTING -s 192.168.0.20 -d 192.168.1.30 -p tcp -m tcp --dport 443 -j DNAT --to-destination 192.168.1.30:443
# Allow replies to established connection
sudo iptables -A FORWARD -i eth1 -o eth0 -m state --state ESTABLISHED -j ACCEPT 
#? NAT rule?

# Forward outgoing TLS/SSL connections from app server to auth server (AS -> AuthS) #! Only case where app server can initiate a connection
sudo iptables -A FORWARD -s 192.168.0.20 -p tcp -m tcp --dport 443 -j ACCEPT
sudo iptables -t nat -A PREROUTING -s 192.168.0.20 -d 192.168.3.100 -p tcp -m tcp --dport 5002 -j DNAT --to-destination 192.168.3.100:5002 #! check ports
# Allow replies to established connection
sudo iptables -A FORWARD -i eth2 -o eth0 -m state --state ESTABLISHED -j ACCEPT

# UFW firewall rules
# Set default rules
# sudo ufw default deny incoming
# sudo ufw default deny outgoing

# # Allow incoming TLS/SSL connections on eth2, addressed to app server
# sudo ufw allow in on eth2 from any to 192.168.3.10 port 443 proto tcp #allow incoming tcp
# sudo ufw allow in on eth0 from 192.168.0.20 to any port 5001 proto tcp #allow redirected tcp from NAT

# # Allow TLS/SSL connections from AS to the DB
# sudo ufw allow from 192.168.0.20 to 192.168.1.30 port 443 proto tcp
# # Allow TLS/SSL connections from DB to AS
# sudo ufw allow from 192.168.1.30 to 192.168.0.20 port 443 proto tcp

# # Allow TLS/SSL connections from AS to auth server
# sudo ufw allow from 192.168.0.20 to any port 5002 proto tcp
# # Allow TLS/SSL connections from auth server to AS
# sudo ufw allow from any to 192.168.0.20 port 5002 proto tcp

# # To allow Vagrant SSH connections
# sudo ufw allow 22

# # Drop all other incoming and forwarding traffic
# sudo ufw deny in on eth0
# sudo ufw deny in on eth1

# Save current iptables (NAT)
# FOR IPv4
sudo sh -c 'iptables-save > /etc/iptables/rules.v4'
# FOR IPv6
sudo sh -c 'ip6tables-save > /etc/iptables/rules.v6'

# Restart to apply changes
sudo systemctl restart systemd-networkd
