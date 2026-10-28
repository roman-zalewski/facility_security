--tests required by assignment
SHOW TABLES;
SELECT * FROM employee;
SELECT * FROM facilities;
SELECT e.first_name, e.last_name, f.facility_name, f.facility_manager
FROM employee e
RIGHT JOIN facilities f ON e.employee_id = f.facility_manager ;
SELECT * FROM employee
WHERE employment_status = 'active';
