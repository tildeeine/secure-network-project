#!/bin/sh

# Write configurations to /etc/network/interfaces
echo "
iface eth0 inet static         
        address 192.168.0.20
        netmask 255.255.255.0
" | sudo tee -a /etc/network/interfaces