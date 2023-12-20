#!/bin/bash

# VMSETTINGS: Adapter1, internal network, sw-1, promiscuous mode allow VMs. Adapter2, internal network, sw-2, promiscuous mode allow VMs. Adapter3, NAT. 

# Configure the network interfaces
sudo ip addr add 192.168.0.10/24 dev enp0s9 # App server
sudo ip addr add 192.168.1.10/24 dev enp0s10 # Database server
sudo ip link set dev enp0s9 up
sudo ip link set dev enp0s10 up

# Activate IP forwarding
sudo sysctl net.ipv4.ip_forward=1

# Write configurations to /etc/network/interfaces
echo "
auto enp0s9
iface enp0s9 inet static
    address 192.168.0.10
    netmask 255.255.255.0

auto enp0s10
iface enp0s10 inet static
    address 192.168.1.10
    netmask 255.255.255.0
" | sudo tee -a /etc/network/interfaces

# Enable IP forwarding permanently
sudo sed -i 's/^#net.ipv4.ip_forward=1/net.ipv4.ip_forward=1/' /etc/sysctl.conf
sudo sysctl -p

# Set up firewall rules

# Enable the firewall, flush NAT rules # TODO: Add flush for firewall
sudo iptables -t nat -F
sudo iptables -t nat -A POSTROUTING  -o enp0s8 -j MASQUERADE    # Creates a source NAT on interface enp0s8

# NAT rules
# Set default policies
sudo iptables -P INPUT DROP
sudo iptables -P FORWARD DROP
sudo iptables -P OUTPUT DROP

# Auth: 192.168.3.100
# Allow SSH connections from vagrant to the firewall server
sudo iptables -A INPUT -p tcp --dport 22 -m state --state NEW,ESTABLISHED -j ACCEPT 
sudo iptables -A OUTPUT -p tcp --sport 22 -m state --state ESTABLISHED -j ACCEPT
# sudo iptables -A FORWARD -p tcp -m tcp --dport 22 --state ESTABLISHED -j ACCEPT # Requires fw to be set up first

# Forward incoming TLS/SSL connections on enp0s8 to app server (EXT -> AS)
sudo iptables -A FORWARD -d 192.168.3.10 -p tcp -m tcp --dport 443 -j ACCEPT
sudo iptables -t nat -A PREROUTING -d 192.168.3.10 -p tcp -m tcp --dport 443 -j DNAT --to-destination 192.168.0.20:5001
# Allow replies to established connection
sudo iptables -A FORWARD -i enp0s9 -o enp0s8 -m state --state ESTABLISHED -j ACCEPT
#? Do we need a nat rule for routing back to the client?

# Forward incoming TSL/SSL connections from app with destination db server (AS -> DB)
sudo iptables -A FORWARD -s 192.168.0.20 -d 192.168.1.30 -p tcp -m tcp --dport 443 -j ACCEPT
sudo iptables -t nat -A PREROUTING -s 192.168.0.20 -d 192.168.1.30 -p tcp -m tcp --dport 443 -j DNAT --to-destination 192.168.1.30:443
# Allow replies to established connection
sudo iptables -A FORWARD -i enp0s10 -o enp0s9 -m state --state ESTABLISHED -j ACCEPT 
#? Might have to set up SNAT for the return traffic (DB->)
#sudo iptables -t nat -A POSTROUTING -s 192.168.1.30 -d 192.168.0.20 -p tcp --sport 443 -m state --state ESTABLISHED -j SNAT --to-source 192.168.0.20:443

# Forward outgoing TLS/SSL connections from app server to auth server (AS -> AuthS) #! Only case where app server can initiate a connection
sudo iptables -A FORWARD -s 192.168.0.20 -p tcp -m tcp --dport 443 -j ACCEPT
sudo iptables -t nat -A PREROUTING -s 192.168.0.20 -d 192.168.3.100 -p tcp -m tcp --dport 5002 -j DNAT --to-destination 192.168.3.100:5002 #! check ports

# Allow replies to established connection
sudo iptables -A FORWARD -i enp0s8 -o enp0s9 -m state --state ESTABLISHED -j ACCEPT


# Save current iptables (NAT)
# FOR IPv4
sudo sh -c 'iptables-save > /etc/iptables/rules.v4'
# FOR IPv6
sudo sh -c 'ip6tables-save > /etc/iptables/rules.v6'

# To apply the saved iptables rules on boot
sudo systemctl enable netfilter-persistent.service

# Restart to apply changes
sudo systemctl restart systemd-networkd

