CREATE DATABASE IF NOT EXISTS super_secret_labs;
USE super_secret_labs;
SET FOREIGN_KEY_CHECKS = 0;
TRUNCATE employee;
TRUNCATE facilities;
TRUNCATE supplier;
TRUNCATE products; -- Clear data to test inserts
TRUNCATE access;
TRUNCATE facility_inventory;
TRUNCATE access_log;
SET FOREIGN_KEY_CHECKS = 1;
CREATE TABLE IF NOT EXISTS employee(
employee_id int primary key,
first_name varchar(45),
last_name varchar(45),
hire_date datetime(6),
employment_status varchar(45),
phone_number int,
work_email varchar(45) UNIQUE,
personal_email varchar(45) UNIQUE,
job_title varchar(45),
fire_date datetime(6)
);
CREATE TABLE IF NOT EXISTS facilities(
facility_id int primary key,
facility_name varchar(45) UNIQUE,
facility_state varchar(45),
facility_city varchar(45),
facility_manager int,
facility_phone_number int,
foreign key (facility_manager) references employee(employee_id)
);
CREATE TABLE IF NOT EXISTS supplier(
supplier_id int primary key,
supplier_name varchar(45),
supplier_state varchar(45),
supplier_city varchar(45),
supplier_phone_number int,
supplier_email varchar(45),
supplier_start_date datetime(6),
supplier_end_date datetime(6),
supplier_status varchar(45)
);
CREATE TABLE IF NOT EXISTS products(
product_id int primary key,
product_name varchar(45),
expiration_date datetime(6)
);
CREATE TABLE IF NOT EXISTS access(
facility_id int,
employee_id int,
primary key (facility_id, employee_id),
foreign key (facility_id) references facilities(facility_id),
foreign key (employee_id) references employee(employee_id)
);
CREATE TABLE IF NOT EXISTS facility_inventory(
facility_id int,
product_id int,
supplier_id int,
primary key (facility_id, product_id, supplier_id),
foreign key (facility_id) references facilities(facility_id),
foreign key (product_id) references products(product_id),
foreign key (supplier_id) references supplier(supplier_id)
);
CREATE TABLE IF NOT EXISTS access_log(
entry_id int primary key,
employee_id int,
facility_id int,
access_status varchar(45),
access_time time,
access_date datetime(6),
foreign key (employee_id) references employee(employee_id),
foreign key (facility_id) references facilities(facility_id)
);
