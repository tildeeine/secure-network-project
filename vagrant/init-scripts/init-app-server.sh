#!/bin/bash

# Update package repositories
sudo apt update

# Configure network interfaces
sudo ip addr add 192.168.0.20/24 dev eth0

# Install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-5.0

# Restart networking service
sudo systemctl restart networking