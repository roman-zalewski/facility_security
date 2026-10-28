-- sample data with some nulls thrown in for error checking
INSERT INTO employee VALUES
(1, 'John', 'Smith', '2018-03-14 08:00:00', 'Active', 5551234567,
'jsmith@company.com', 'john.smith@gmail.com', 'Facility Manager', NULL),
(2, 'Maria', 'Garcia', '2019-06-01 08:00:00', 'Active', 5552345678,
'mgarcia@company.com', 'maria.g@gmail.com', 'Facility Manager', NULL),
(3, 'David', 'Lee', '2020-01-15 08:00:00', 'Active', 5553456789,
'dlee@company.com', 'david.lee@gmail.com', 'Warehouse Associate', NULL),
(4, 'Sarah', 'Johnson', '2020-09-10 08:00:00', 'Active', 5554567890,
'sjohnson@company.com', 'sarah.j@gmail.com', 'Warehouse Associate', NULL),
(5, 'Michael', 'Brown', '2017-11-20 08:00:00', 'Terminated', 5555678901,
'mbrown@company.com', 'mbrown@gmail.com', 'Supply Coordinator', '2023-05-30
08:00:00'),
(6, 'Emily', 'Davis', '2021-02-08 08:00:00', 'Active', 5556789012,
'edavis@company.com', 'emily.d@gmail.com', 'Security Officer', NULL),
(7, 'Chris', 'Wilson', '2022-07-19 08:00:00', 'Active', 5557890123,
'cwilson@company.com', 'chris.w@gmail.com', 'Security Officer', NULL),
(8, 'Amanda', 'Moore', '2016-04-25 08:00:00', 'Active', 5558901234,
'amoore@company.com', 'amanda.m@gmail.com', 'Regional Director', NULL);
INSERT INTO facilities VALUES
(1, 'North Distribution Center', 'AZ', 'Prescott', 1, 9285551000),
(2, 'South Warehouse', 'AZ', 'Phoenix', 2, 6025552000),
(3, 'East Regional Hub', 'NM', 'Albuquerque', 1, 5055553000),
(4, 'West Storage Facility', 'CA', 'Bakersfield', 2, 6615554000);
INSERT INTO supplier VALUES
(1, 'Acme Foods Inc', 'TX', 'Dallas', 2145551111, 'orders@acmefoods.com', '2019-01-
01 00:00:00', NULL, 'Active'),
(2, 'Southwest Produce Co', 'AZ', 'Yuma', 9285552222, 'sales@swproduce.com', '2020-
03-15 00:00:00', NULL, 'Active'),
(3, 'Pacific Packaging Ltd', 'CA', 'Fresno', 5595553333, 'info@pacificpack.com',
'2018-06-01 00:00:00', '2023-12-31 00:00:00', 'Inactive'),
(4, 'National Dairy Supply', 'WI', 'Madison', 6085554444, 'contact@natdairy.com',
'2021-09-10 00:00:00', NULL, 'Active');
INSERT INTO products VALUES
(1, 'Canned Tomatoes', '2027-06-01 00:00:00'),
(2, 'Frozen Chicken Breast', '2026-11-15 00:00:00'),
(3, 'Whole Milk', '2026-10-05 00:00:00'),
(4, 'Corrugated Boxes', NULL),
(5, 'Bottled Water 24pk', '2028-01-01 00:00:00'),
(6, 'Cheddar Cheese Block', '2026-12-20 00:00:00');
INSERT INTO access VALUES
(1, 1),
(1, 3),
(1, 6),
(2, 2),
(2, 4),
(2, 7),
(3, 1),
(3, 8),
(4, 2),
(4, 8);
INSERT INTO facility_inventory VALUES
(1, 1, 1),
(1, 4, 3),
(1, 5, 1),
(2, 2, 4),
(2, 3, 4),
(3, 1, 2),
(3, 6, 4),
(4, 2, 1),
(4, 5, 3);
INSERT INTO access_log VALUES
(1, 1, 1, 'Granted', '08:02:00', '2025-09-01 00:00:00'),
(2, 3, 1, 'Granted', '07:58:00', '2025-09-01 00:00:00'),
(3, 6, 1, 'Granted', '06:45:00', '2025-09-01 00:00:00'),
(4, 4, 2, 'Denied', '09:15:00', '2025-09-01 00:00:00'),
(5, 2, 2, 'Granted', '08:30:00', '2025-09-02 00:00:00'),
(6, 7, 2, 'Granted', '06:00:00', '2025-09-02 00:00:00'),
(7, 1, 3, 'Granted', '10:00:00', '2025-09-03 00:00:00'),
(8, 8, 3, 'Granted', '09:45:00', '2025-09-03 00:00:00'),
(9, 8, 4, 'Granted', '11:20:00', '2025-09-04 00:00:00'),
(10, 5, 1, 'Denied', '08:00:00', '2025-09-04 00:00:00');
