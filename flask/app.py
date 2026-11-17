import threading
from flask import Flask, request, jsonify
import mysql.connector
from mysql.connector import Error

app = Flask(__name__)

try:
    conn = mysql.connector.connect(
        host = "localhost",
        user = "manager",
        password = "P@ssword",
        database = "super_secret_labs"
    )
    assert conn.is_connected()
except Error as e:
    print(f"Database error: {e}")


def compute_sql(sql, perm_level):
    conn.ping(reconnect = True)
    if perm_level == 'keycard' and not sql.strip().upper().startswith("INSERT"):
        return {"answer":"no"}
    cursor = conn.cursor(dictionary = True)      
    try:
        cursor.execute(sql)
    except Error as e:
        return {"error": str(e)}
    if cursor.with_rows:
        rows = cursor.fetchall()
        conn.commit()
        return rows
    conn.commit()
    return {"affected": cursor.rowcount}

db_lock = threading.Lock()   

def compute_batch(statements):
    with db_lock:
        conn.ping(reconnect = True)
        cursor = conn.cursor(dictionary = True)
        results = []
        i = 0
        try:
            for i, stmt in enumerate(statements):
                cursor.execute(stmt["sql"], stmt.get("params"))
                if cursor.with_rows:
                    results.append(cursor.fetchall())
                else:
                    results.append({"affected": cursor.rowcount})
            conn.commit()
            return results
        except (Error, KeyError, TypeError, AttributeError) as e:
            conn.rollback()
            return {"error": f"statement {i}: {e}", "rolled_back": True}
        finally:
            cursor.close()

@app.route('/')
def home():
    return "hello"

@app.route('/data/keycard', methods = ["POST"])
def receive_json_1():
    data = request.get_json(force = True)
    sql = data.get("sql")
    result = compute_sql(sql, 'keycard')
    if isinstance(result, dict) and "error" in result:
        return jsonify(result), 400
    return jsonify(result)

@app.route('/data/manager', methods = ["POST"])
def receive_json_2():
    data = request.get_json(force = True)
    sql = data.get("sql")
    result = compute_sql(sql, 'manager')
    if isinstance(result, dict) and "error" in result:
        return jsonify(result), 400
    return jsonify(result)

@app.route('/data/manager/batch', methods = ["POST"])
def receive_batch():
    data = request.get_json(force = True)
    statements = data.get("statements")
    if not isinstance(statements, list) or not statements:
        return jsonify({"error": "statements must be a non-empty list"}), 400
    result = compute_batch(statements)
    if isinstance(result, dict) and "error" in result:
        return jsonify(result), 400
    return jsonify(result)

if __name__ == "__main__":
    app.run(host = "127.0.0.1", port = 49170, debug = False)
