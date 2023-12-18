#!/bin/bash

name="InitialCreate"

help(){
    echo    "This script helps you migrate your database." 
    echo -e "Commands: \n"
    echo    "./migrate.sh add name           To add a migration, name being your new migration name." 
    echo -e "                              It also generates a idempotent migration script, so you can migrate your production database.\n" 
    echo -e "./migrate.sh init name(opt)     If you want to generate a init.sql script so your database can be deployed with"
    echo    "                              this new migration. You can provide a optional name to your migration"
}

normal(){
    if [[ -z "$1" ]]; then
       help
       exit 1
    fi

    dotnet ef migrations add $1

    if [[ $? -ne 0 ]]; then
        echo "Please provide a different migration name."
        exit 1
    fi

    dotnet ef database update
}

if [[ "$1" == "init" ]]; then
    if [[ -n "$2" ]]; then
       name=$2 
    fi
    normal $name
    dotnet ef migrations script -o ./Migrations/init.sql
    exit 0
fi

if [[ "$1" == "add" ]]; then
    normal $2
    dotnet ef migrations script --idempotent -o ./Migrations/migration_$2.sql
    exit 0
fi

help
