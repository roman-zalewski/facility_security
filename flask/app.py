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
    if perm_level == 'scanner' and not sql.strip().upper().startswith("INSERT"):
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

@app.route('/')
def home():
    return "hello"

@app.route('/data/keycard', methods = ["POST"])
def receive_json_1():
    data = request.get_json(force = True)
    sql = data.get("sql")
    result = compute_sql(sql, 'scanner')
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

if __name__ == "__main__":
    app.run(host = "127.0.0.1", port = 49170, debug = False)
