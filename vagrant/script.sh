#!/usr/bin/bash

# vagrant ssh app_server -c "screen -S server -dm /MediTrackApp/MediTrackBackend"
# echo "Sent screen to app_server"
vagrant ssh auth_server -c "screen -S server -dm /AuthServerApp/AuthServer"
echo "Sent screen to auth_server"
vagrant ssh client_machine -c "screen -S client -dm /ClientApp/Client"
echo "Sent screen to client_machine"
