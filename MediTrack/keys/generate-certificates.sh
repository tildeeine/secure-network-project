#!/usr/bin/bash

generate_certificate(){
    if [[ -z "$1" ]]; then
       echo "Generate certificate takes an argument: name"
       exit 1
    fi
    openssl req -new -newkey rsa:4096 -nodes -keyout "$1.priv.pem" -out "$1-request.csr" -subj "/CN=$1"
    openssl rsa -in "$1.priv.pem" -pubout -out "$1.pub.pem"
}

if [[ "$1" == "clean" ]];then
    read -p "Do you which to remove all the certificate related files? [y/n]" option
    if [[ "$option" == "y" ]]; then
        rm meditrack-server* auth-server* database-server*
    fi
    exit 0
fi

# Generate certificate for meditrack-server and self-sign it
generate_certificate meditrack-server
openssl x509 -req -days 365 -in meditrack-server-request.csr -signkey meditrack-server.priv.pem -out meditrack-server.crt

# Generate certificate for database-server and sign-it with the meditrack authority
generate_certificate database-server
openssl x509 -req -days 365 -in database-server-request.csr -CA meditrack-server.crt -CAkey meditrack-server.priv.pem -out database-server.crt

# Generate certificate for auth server and self-sign it
generate_certificate auth-server
openssl x509 -req -days 365 -in auth-server-request.csr -signkey auth-server.priv.pem -out auth-server.crt

# Cleanup all the certificate request files
rm *csr
