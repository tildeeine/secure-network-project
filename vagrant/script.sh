#!/usr/bin/bash

# TERM=xterm vagrant ssh app_server -c "screen -S server -dm cd /MediTrack/MediTrackBackend/; dotnet run"
echo "Please press <C-a>d to detach."
echo "Setting up app_server"
TERM=xterm vagrant ssh app_server -c "screen -mS server dotnet run --project /MediTrack/MediTrackBackend/"

echo "Please press <C-a>d to detach."
echo "Setting up auth_server"
TERM=xterm vagrant ssh auth_server -c "screen -mS server dotnet run --project /MediTrack/AuthServer/"

echo "Please press <C-a>d to detach."
echo "Setting up client_machine"
TERM=xterm vagrant ssh client_machine -c "screen -mS client dotnet run --project /MediTrack/Client/ start doctor 000000000 /MediTrack/keys/Bob.priv.pem /MediTrack/keys/Bob.pub.pem"
