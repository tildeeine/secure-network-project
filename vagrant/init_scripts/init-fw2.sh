#!/bin/bash

# Update
sudo apt update

# Infrastructure setup?
sudo ifconfig eth0 192.168.1.30/24 up
sudo ifconfig eth0 192.168.2.30/24 up

sudo systemctl restart NetworkManager