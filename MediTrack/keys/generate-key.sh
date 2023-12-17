read -p "Name of the key holder: " name
openssl genrsa -out "$name.priv.pem" 2048
openssl rsa -in "$name.priv.pem" -pubout -out "$name.pub.pem"
