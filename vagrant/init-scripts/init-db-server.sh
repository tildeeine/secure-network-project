#!/bin/sh

# Write configurations to /etc/network/interfaces
echo "
iface eth0 inet static         
        address 192.168.0.20
        netmask 255.255.255.0
" | sudo tee -a /etc/network/interfaces

### Allow remote connections 
# Edit /etc/mysql/mysql.conf.d/mysqld.cnf file   
echo "[mysqld]
ssl_ca=/MediTrack/keys/meditrack-server.crt
ssl_cert=/MediTrack/keys/database-server.crt 
ssl_key=/MediTrack/keys/database-server.priv.pem
require_secure_transport=ON
bind-address = 192.168.1.30
" | sudo tee -a /etc/mysql/mysql.conf.d/mysqld.cnf

