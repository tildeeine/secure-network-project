#!/bin/bash

# Update
sudo apt update

# Infrastructure setup?
sudo ifconfig eth0 192.168.2.40/24 up

sudo systemctl restart NetworkManager


# Database setup
sudo apt install mysql-server
sudo systemctl start mysql

# Secure MySQL Installation
mysql_secure_installation <<EOF

y
your_mysql_root_password
your_mysql_root_password
y
y
y
y
EOF

# Create Database and User
mysql -u root -p"your_mysql_root_password" <<MYSQL_SCRIPT
CREATE DATABASE your_database_name;
CREATE USER 'your_user'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON your_database_name.* TO 'your_user'@'%';
FLUSH PRIVILEGES;
MYSQL_SCRIPT

# Update firewall rules? Or only handled by firewall servers?

# Enable ssh if we decide to have comms app server -> db over ssh