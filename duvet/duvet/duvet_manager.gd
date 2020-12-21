extends Node

var database: DuvetRoot

func initialize():
	database = DuvetRoot.new()

	var dir = Directory.new()
	dir.open("logs")
	dir.list_dir_begin(true, true)
	while true:
		var nxt = dir.get_next()
		if nxt == "":
			break
		var res = "logs/" + nxt
		var file = File.new()
		file.open(res, File.READ)
		while not file.eof_reached():
			var ln = file.get_line()
			database.try_add_change(ln)
