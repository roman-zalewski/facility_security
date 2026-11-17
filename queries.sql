USE super_secret_labs;

-- 1. Getting all facilities
SELECT f.facility_id, f.facility_name, f.facility_city, f.facility_state,
       f.facility_phone_number,
       CONCAT(e.first_name, ' ', e.last_name) AS manager
FROM facilities f
LEFT JOIN employee e ON e.employee_id = f.facility_manager
ORDER BY f.facility_id;

-- 2. Number of people inside a specific facility (Parameters: @facilityId)
SELECT COUNT(*) AS people_inside
FROM (
    SELECT l.employee_id, l.in_or_out,
           ROW_NUMBER() OVER (PARTITION BY l.employee_id
                              ORDER BY l.access_date DESC, l.access_time DESC, l.entry_id DESC) AS rn
    FROM access_log l
    WHERE l.facility_id = @facility_id
      AND l.access_status = 'Granted'
      AND l.in_or_out IS NOT NULL
) latest
WHERE rn = 1 AND in_or_out = 'In';

-- 3. Check a person's acess eligibility and add to access record (Parameters: @employeeId, @facilityId, @direction ('In' or 'Out'))
-- 3a. Decide and log
INSERT INTO access_log
    (employee_id, facility_id, access_status, access_time, access_date, in_or_out)
SELECT @employeeId, @facilityId,
       IF(d.granted, 'Granted', 'Denied'),
       CURTIME(), CURDATE(),
       IF(d.granted, @direction, NULL)
FROM (
    SELECT IF(@direction = 'In',
        EXISTS (
            SELECT 1
            FROM access a
            JOIN employee e ON e.employee_id = a.employee_id
            WHERE a.employee_id = @employeeId
              AND a.facility_id = @facilityId
              AND e.employment_status = 'Active'
        ),
        EXISTS (
            SELECT 1
            FROM (
                SELECT in_or_out
                FROM access_log
                WHERE employee_id = @employeeId
                  AND facility_id = @facilityId
                  AND access_status = 'Granted'
                  AND in_or_out IS NOT NULL
                ORDER BY access_date DESC, access_time DESC, entry_id DESC
                LIMIT 1
            ) last_event
            WHERE last_event.in_or_out = 'In'
        )
    ) AS granted
) d;

-- 3b. Read back the result
SELECT entry_id, access_status
FROM access_log
WHERE entry_id = LAST_INSERT_ID();

-- 4. Every person currently inside a facility
-- 4a. parameter: @facilityId
SELECT e.employee_id, e.first_name, e.last_name, e.job_title, latest.access_time AS entered_at
FROM (
    SELECT l.employee_id, l.in_or_out, l.access_time,
           ROW_NUMBER() OVER (PARTITION BY l.employee_id
                              ORDER BY l.access_date DESC, l.access_time DESC, l.entry_id DESC) AS rn
    FROM access_log l
    WHERE l.facility_id = @facilityId
      AND l.access_status = 'Granted'
      AND l.in_or_out IS NOT NULL
) latest
JOIN employee e ON e.employee_id = latest.employee_id
WHERE latest.rn = 1 AND latest.in_or_out = 'In'
ORDER BY e.last_name, e.first_name;

-- 4b. Everyone eligible to enter a facility (Parameters: @facilityId)
SELECT e.employee_id, e.first_name, e.last_name, e.job_title, e.employment_status
FROM access a
JOIN employee e ON e.employee_id = a.employee_id
WHERE a.facility_id = @facility_id
ORDER BY e.last_name, e.first_name;

-- 5. add and remove access (Parameters: @facilityId and @employeeId)
-- 5a. add 
INSERT INTO access (facility_id, employee_id)
VALUES (@facilityId, @employeeId);

-- 5b. remove 
DELETE FROM access
WHERE facility_id = @facilityId AND employee_id = @employeeId;

-- 6. Update a name (Parameters: @employeeId, @firstName, and @lastName)
UPDATE employee
SET first_name = @firstName, last_name = @lastName
WHERE employee_id = @employeeId;
