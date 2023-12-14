#!/bin/bash

# Update
sudo apt update

# Infrastructure setup?
sudo ifconfig eth0 192.168.0.20/24 up
sudo ifconfig eth1 192.168.1.20/24 up

sudo systemctl restart NetworkManager

# Set up for running CLI, from cloning repo to meditrack-cli