#!/bin/sh

# Configure the network interfaces
sudo ip addr add 192.168.1.30/24 dev enp0s8
sudo ip link set dev enp0s8 up

# Restart to apply changes
sudo systemctl restart systemd-networkd

# Write configurations to /etc/network/interfaces
echo "
iface enp0s8 inet static         
        address 192.168.1.30
        netmask 255.255.255.0
" | sudo tee -a /etc/network/interfaces
        # gateway 192.168.1.10

### Allow remote connections 
# Edit /etc/mysql/mysql.conf.d/mysqld.cnf file   
echo "
ssl_ca=/MediTrack/keys/meditrack-server.crt
ssl_cert=/MediTrack/keys/database-server.crt 
ssl_key=/MediTrack/keys/database-server.priv.pem
require_secure_transport=ON
bind-address = 192.168.1.30
" | sudo tee -a /etc/mysql/mysql.conf.d/mysqld.cnf

# Restart MySQL to apply changes
service mysql restart
