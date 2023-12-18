#!/bin/sh

# VMSETTINGS: Adapter1, internal network, sw-1, promiscuous mode allow VMs. 

sudo ifconfig eth0 192.168.0.20/24 up

# Set External Firewall as default gateway
sudo ip route add default via 192.168.0.10

# Restart to apply changes
sudo systemctl restart NetworkManager

# ! edit the corresponding /etc/network/interfaces file.