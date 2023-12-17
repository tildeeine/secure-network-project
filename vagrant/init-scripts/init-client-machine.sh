#!/bin/bash

# Update package repositories
sudo apt update

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