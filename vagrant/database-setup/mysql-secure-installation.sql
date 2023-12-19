-- mysql_secure_installation.sql
-- Automatically set the MySQL root password and remove anonymous users, disallow root login remotely, and remove the test database.

ALTER USER 'root'@'localhost' IDENTIFIED BY 'a31-MediTrack';
DELETE FROM mysql.user WHERE User='';
DELETE FROM mysql.user WHERE User='root' AND Host NOT IN ('localhost', '127.0.0.1', '::1');
DROP DATABASE IF EXISTS test;
DELETE FROM mysql.db WHERE Db='test' OR Db='test\\_%';
FLUSH PRIVILEGES;
