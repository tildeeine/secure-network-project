#!/bin/bash

# Update
sudo apt update

# Infrastructure setup?
sudo ifconfig eth0 192.168.0.10/24 up

# Configure NAT
sudo iptables -t nat -F 
sudo iptables -t nat -A POSTROUTING  -o eth1 -j MASQUERADE

sudo systemctl restart NetworkManager

# Make iptables rules persistent
sudo apt install iptables-persistent #have to answer "yes" to prompt

# FOR IPv4
$ sudo sh -c 'iptables-save > /etc/iptables/rules.v4'
# FOR IPv6
$ sudo sh -c 'ip6tables-save > /etc/iptables/rules.v6'

# To automatically apply the saved iptables rules on boot
sudo systemctl enable netfilter-persistent.service

# Firewall setup
sudo /sbin/iptables -F
# Decide which firewall rules to use - allow only connections to CLI at 192.168.0.20, which port?
# Consider using UFW