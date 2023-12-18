#!/bin/bash

#! Set up internet connection

#! Run auth server

# Database setup #? Check if actually necessary to do this
# Install MySQL Server
# sudo apt-get install -y mysql-server mysql-client

# # Start MySQL Service
# sudo systemctl start mysql

# # ! Consider doing a secure MySQL Installation
# # sudo mysql_secure_installation <<EOF
# # y
# # your_mysql_root_password
# # your_mysql_root_password
# # y
# # y
# # y
# # y
# # EOF

# # Create Database and User
# sudo mysql -u root -p"your_mysql_root_password" <<MYSQL_SCRIPT
# CREATE DATABASE your_database_name;
# CREATE USER 'your_user'@'%' IDENTIFIED BY 'your_password';
# GRANT ALL PRIVILEGES ON your_database_name.* TO 'your_user'@'%';
# FLUSH PRIVILEGES;
# MYSQL_SCRIPT

# # Enable MySQL Service on boot
# sudo systemctl enable mysql
