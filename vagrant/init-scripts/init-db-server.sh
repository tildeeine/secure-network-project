#!/bin/bash

# Update package repositories
sudo apt update

# Configure network interface
sudo ip addr add 192.168.1.40/24 dev eth0

# Database setup
# Install MySQL Server
sudo apt install -y mysql-server

# Start MySQL Service
sudo systemctl start mysql

# ! Consider doing secure installation of db

# Create Database and User
mysql -u root -p <<MYSQL_SCRIPT
CREATE DATABASE your_database_name;
CREATE USER 'your_user'@'%' IDENTIFIED BY 'your_password';
GRANT ALL PRIVILEGES ON your_database_name.* TO 'your_user'@'%';
FLUSH PRIVILEGES;
MYSQL_SCRIPT

# Enable MySQL Service on boot
sudo systemctl enable mysql

