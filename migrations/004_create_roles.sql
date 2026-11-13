CREATE USER 'GUI_interface'@'%' IDENTIFIED BY 'password';
GRANT INSERT ON super_secret_labs.access_log TO 'GUI_interface'@'%';
CREATE USER 'manager'@'%' IDENTIFIED BY 'P@ssword';
GRANT SELECT, INSERT, UPDATE, DELETE ON super_secret_labs.* TO 'manager'@'%';
