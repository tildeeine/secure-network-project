-- create-user.sql
-- Create a MySQL user

CREATE USER 'mysql'@'localhost' IDENTIFIED BY '1234';
CREATE DATABASE IF NOT EXISTS meditrack;
GRANT ALL PRIVILEGES ON *.* TO 'mysql'@'localhost' WITH GRANT OPTION;
FLUSH PRIVILEGES;