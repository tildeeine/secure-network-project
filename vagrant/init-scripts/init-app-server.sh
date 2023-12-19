#!/bin/sh

# VMSETTINGS: Adapter1, internal network, sw-1, promiscuous mode allow VMs. 

# Configure the network interfaces
sudo ip addr add 192.168.0.20/24 dev eth0
sudo ip link set dev eth0 up

#? Set External Firewall as default gateway
# sudo ip route add default via 192.168.0.10

# Restart to apply changes
sudo systemctl restart systemd-networkd

# ! edit the corresponding /etc/network/interfaces file.