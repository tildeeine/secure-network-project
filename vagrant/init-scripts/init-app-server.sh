#!/bin/sh

# VMSETTINGS: Adapter1, internal network, sw-1, promiscuous mode allow VMs. 

# Configure the network interfaces
sudo ip addr add 192.168.0.20/24 dev eth0
sudo ip link set dev eth0 up

# Restart to apply changes
sudo systemctl restart systemd-networkd

# Write configurations to /etc/network/interfaces
echo "
iface eth0 inet static         
        address 192.168.0.20
        netmask 255.255.255.0
" | sudo tee -a /etc/network/interfaces