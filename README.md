# Facility Security SQL Project

This project was created for a database systems class to demonstrate a full SQL pipeline.

It is a facility access system with a dashboard for secure facilities. Employees request entry to or exit from a facility, the system grants or denies the request, and every attempt is logged. A manager dashboard shows who is currently inside each facility and lets a manager change who has access.

## What it does

- **Access reader:** simulates a keycard scanner. An employee requests entry or exit, the system decides, and the attempt is recorded in the access log.
- **Facility dashboard:** lists the facilities with a headcount for each. Select a facility to see who is inside right now.
- **Access management:** add or remove employees' access to a facility in bulk.
- **Employee names:** a manager can update an employee's first and last name.

## How it fits together

```
Unity front end  -->  Flask API (port 49170)  -->  MySQL (database: super_secret_labs)
```

- The **Unity** front end sends SQL to the API as JSON.
- The **Flask** API runs the SQL against MySQL and returns the rows as JSON.
- **MySQL** stores employees, facilities, access permissions and the access log.

## Repository layout

| Path | What is in it |
|---|---|
| `My project/` | Unity project. Scenes: `AccessReader` and `FacilityDashboard`. Scripts are in `Assets/Scripts`, and `PythonBackendConnector.cs` sends the requests to Flask. |
| `flask/app.py` | The Flask API. |
| `migrations/` | SQL files that build the database, in order (see Setup). |
| `queries.sql` | The queries the application uses, with notes on their parameters. |
| `documents/` | Business rules, ER diagram and application plan. |

## Database

Tables: `employee`, `facilities`, `access`, `access_log`, plus `supplier`, `products` and `facility_inventory`.

- `access` holds one row per employee and facility they may enter.
- `access_log` holds every attempt: employee, facility, `Granted` or `Denied`, the time and date, and `in_or_out` (`In` or `Out`).
- `access_log.entry_id` is auto-incremented, so new log rows get ids automatically.

### Access rules

- **Entry (In):** granted if the employee has an `access` row for that facility and their status is Active.
- **Exit (Out):** granted only if the person's last granted event at that facility was an In.
- **Denied:** logged anyway, with `in_or_out` left empty, so every attempt leaves a record.

### MySQL users

`migrations/004_create_roles.sql` creates two users:

- `GUI_interface` can insert into `access_log`.
- `manager` can select, insert, update and delete across the database. The Flask API connects as `manager`.

## Setup

### 1. Database

1. Run MySQL 8.x (we used 8.4 in Docker) and create a database named `super_secret_labs`.
2. Run the migrations in this order:
   1. `migrations/001_create_tables.sql` creates the tables.
   2. `migrations/002_insert_test_data.sql` loads the sample data. It starts by clearing every table, so only run it on a database you are happy to reset.
   3. `migrations/004_create_roles.sql` creates the users.
   4. `migrations/003_initial_test_functionality.sql` is an optional set of check queries.

**Upgrading an existing database.** `001` skips tables that already exist. If your `access_log` table was created before it had the `in_or_out` column or auto-increment ids, run:

```sql
ALTER TABLE access_log ADD COLUMN in_or_out VARCHAR(45);
ALTER TABLE access_log MODIFY entry_id INT NOT NULL AUTO_INCREMENT;
```

### 2. Flask API

Install the dependencies (`flask` and `mysql-connector-python`) and start the server from the `flask/` folder:

```
python app.py
```

It listens on `127.0.0.1:49170`. The database connection settings (host, database name and login) are at the top of `app.py`. Restart it after changing `app.py`.

### 3. Unity

Open the `My project` folder with Unity 6000.6.8f1. The Flask API must be running before you press play.

## API

All routes take a POST with a JSON body and return JSON. `GET /` returns `hello`, which is handy for checking the server is up.

| Route | Body | What it does |
|---|---|---|
| `/data/keycard` | `{"sql": "INSERT ..."}` | Runs an INSERT, for the scanner. Anything else returns `{"answer":"no"}`. |
| `/data/manager` | `{"sql": "..."}` | Runs one SQL statement and returns the rows, or `{"affected": n}` for changes. |
| `/data/manager/batch` | `{"statements": [{"sql": "...", "params": [...]}, ...]}` | Runs a list of statements in one transaction. All of them commit, or none do. |

Notes:

- Send one statement per request to `/data/keycard` and `/data/manager`. Use the batch route when several changes belong together.
- In the batch route, write placeholders as `%s` and put the values in `params`.
- A SQL error returns HTTP 400 with the MySQL message. On the batch route the response also includes `"rolled_back": true`.
- MySQL `TIME` values, such as `access_time`, are returned as `"HH:MM:SS"` text.

Example batch request, adding two employees to facility 3:

```json
{"statements": [
  {"sql": "INSERT INTO access (facility_id, employee_id) VALUES (%s, %s)", "params": [3, 4]},
  {"sql": "INSERT INTO access (facility_id, employee_id) VALUES (%s, %s)", "params": [3, 6]}
]}
```

## Sample data

`002_insert_test_data.sql` loads 8 employees, 4 facilities, 4 suppliers, 6 products, 10 access permissions, 9 inventory rows and 18 access log entries. The log has 15 granted attempts, 3 denied attempts, and full In and Out histories. Employees 6, 2 and 8 are still inside facilities 1, 2 and 4.

## Responsibilities

### Roman
 - README
 - Presentation
 - Migrations
 - Access

### Jonah

 - Functional SQL Queries

### Collin

 - Keycard front end
 - Manager dashboard
