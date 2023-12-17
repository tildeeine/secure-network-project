#!/bin/sh

# Update package repositories
sudo apt update

# Install necessary software
sudo apt install iptables iptables-persistent

# Configure network interfaces
sudo ip addr add 192.168.0.10/24 dev eth0 #! Set to DMZ
sudo ip addr add 192.168.1.10/24 dev eth1 #! Set to external network

# Configure NAT
iptables -t nat -F
iptables -t nat -A POSTROUTING -o eth1 -j MASQUERADE

# Make iptables rules persistent
sudo sh -c 'iptables-save > /etc/iptables/rules.v4'

# Ensure netfilter-persistent service is enabled
sudo systemctl enable netfilter-persistent

# Enable iptables service on boot
sudo systemctl start netfilter-persistent

# Restart networking service to apply changes
sudo systemctl restart networking

# Firewall setup
# !Add specific firewall rules
# Example Rules:
iptables -A FORWARD -i eth0 -o eth1 -p tcp --dport 80 -j ACCEPT  # Allow traffic from Internet to DMZ
iptables -A FORWARD -i eth0 -o eth1 -p tcp --dport 443 -j ACCEPT # Allow HTTPS traffic from Internet to DMZ
iptables -A FORWARD -i eth1 -o eth0 -p tcp --dport 5432 -j ACCEPT # Allow PostgreSQL traffic from DMZ to Internet
