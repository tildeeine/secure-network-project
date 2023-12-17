#!/bin/bash

# Update package repositories
sudo apt update

# Install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-5.0

# Configure NAT
iptables -t nat -F
iptables -t nat -A POSTROUTING -o eth1 -j MASQUERADE

# Make iptables rules persistent
sudo sh -c 'iptables-save > /etc/iptables/rules.v4'

# Ensure netfilter-persistent service is enabled
sudo systemctl enable netfilter-persistent

# Enable iptables service on boot
sudo systemctl start netfilter-persistent

# Restart networking service to apply changes
sudo systemctl restart networking

# Database setup #? Check if actually necessary to do this
# Install MySQL Server
sudo apt-get install -y mysql-server mysql-client

# Start MySQL Service
sudo systemctl start mysql

# ! Consider doing a secure MySQL Installation
# sudo mysql_secure_installation <<EOF
# y
# your_mysql_root_password
# your_mysql_root_password
# y
# y
# y
# y
# EOF

# Create Database and User
sudo mysql -u root -p"your_mysql_root_password" <<MYSQL_SCRIPT
CREATE DATABASE your_database_name;
CREATE USER 'your_user'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON your_database_name.* TO 'your_user'@'%';
FLUSH PRIVILEGES;
MYSQL_SCRIPT

# Enable MySQL Service on boot
sudo systemctl enable mysql
