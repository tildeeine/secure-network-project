#!/bin/bash

# Update package repositories
sudo apt-get update

# Install .NET SDK
sudo apt-get install -y dotnet-sdk-5.0

# Install iptables
sudo apt-get install -y iptables 
sudo apt-get install -y iptables-persistent

# Configure NAT
iptables -t nat -F
iptables -t nat -A POSTROUTING -o eth0 -j MASQUERADE

# Make iptables rules persistent
sudo sh -c 'iptables-save > /etc/iptables/rules.v4'

# Ensure netfilter-persistent service is enabled
sudo systemctl enable netfilter-persistent

# Enable iptables service on boot
sudo systemctl start netfilter-persistent

# Restart networking service to apply changes
sudo systemctl restart networking

#? Should we set up a connection to app server?