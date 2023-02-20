from flask import Flask, jsonify, request
app = Flask(__name__)

database = []

@app.route("/save-benchmark", methods=["POST"])
def save_benchmark():
    args_json = request.json
    required_args = ["cpu", "energy", "ram"]
    benchmark_object = {}

    for arg in required_args:
        if arg not in args_json:
            return jsonify({"code": "422", "message": "Not enough parameters"})

        benchmark_object[arg] = args_json[arg]

    database.append(benchmark_object)
    return jsonify({"code": "200", "message": "Benchmark uploaded sucessfully"})

@app.route("/get-benchmark", methods=["GET"])
def get_data():
    args = request.args
    
    if "id" in args.keys():
        id = int(args["id"])

        if id >= len(database):
            return jsonify({"code": "404", "message": "Resource with specified index was not found"})

        return jsonify(database[id])
    
    return jsonify(database)

if __name__ == "__main__":
    app.run()