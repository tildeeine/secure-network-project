-- create_database.sql
-- Create a MySQL user and database

CREATE DATABASE meditrackdb;
CREATE USER 'meditrackuser'@'localhost' IDENTIFIED BY 'a31-MediTrack';
GRANT ALL PRIVILEGES ON meditrackdb.* TO 'meditrackuser'@'localhost';
FLUSH PRIVILEGES;