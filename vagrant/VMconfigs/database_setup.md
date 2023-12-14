# Notes to self on implementation
What is done in virtualbox gui?
- Interfaces: VM settings/Network/adapter1
    - attach to internal network, name sw-x
    - Promiscuous mode: Allow VMS
- On internet-connected node: Create network adapter that is nat-ed with physical address.



# TODO
- Make it so vagrant doesn't open and run the VMs after starting
- Handle the "Available bridged network interfaces" question
- Running vagrant up takes a while as it needs to download kali linux
- Set up CLI for VM
- Give better names to vagrant machines
- Finish instructions for running vagrant


- Look at linked vm with vagrant - possible? Only downloads kali for first

# Qs
- Do we need to set up any gateway/IP forwarding? For app server through firewall 1?
- Do we need to "make the changes permanent" so they are persistent for the VMs through reboots? source /etc/network/interfaces.d/*
- Should we redirect any connections, or just drop things trying to connect to something other than application server?
- Is it up to us to use SSH or TLS between app server and database server


Firewall
- App server accepts HTTP (CLI) and SSH (DB) connections from internal and external. Does not start connections. 
- DB accepts SSH requests from app server
- FW1 redirects HTTP connections to app server, rejects all other
- FW2 accepts SSH to-from app and db, rejects all other
- 