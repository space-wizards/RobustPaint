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
		if nxt.ends_with(".txt"):
			if not dir.file_exists(nxt + ".bin"):
				var fileR = File.new()
				fileR.open(res, File.READ)
				var fileW = File.new()
				fileW.open(res + ".bin", File.WRITE)
				var idx = 1
				while not fileR.eof_reached():
					var ln = fileR.get_line()
					var lnc = _parse_change(ln, idx)
					idx += 1
					if lnc != null:
						fileW.store_8(0)
						lnc.write(fileW)
			# it is done
			var fileB = File.new()
			fileB.open(res + ".bin", File.READ)
			while not fileB.eof_reached():
				var type = fileB.get_8()
				if type == 0:
					var lnc = DuvetChange.new()
					lnc.read(fileB, res)
					database.add_change(lnc)
				else:
					push_warning("Incorrect type ID " + str(type))
					break

func _parse_change(c: String, idx: int) -> DuvetChange:
	# first line of sanity
	var headline1: String = "[WARN] c.s.go.co.brush: "
	var headline2: String = "[WARN] c.s.worlddumptolog: "
	if c.count("[WARN] c.s.") != 1:
		return null
	# All lines are prefixed with a date.
	# If there is no date, it's not valid.
	# "2020-12-18T12:00:39.2850864+00:00 [WARN] c.s.go.co.brush: localhost@JoeGenero at (-8, 3) = 0"
	var dateSplit = c.find(" ")
	if dateSplit == -1:
		return null
	var dateOriginal = c.substr(0, dateSplit)
	var date = DateManager.parse_date(dateOriginal)
	if date == -1.0:
		return null
	var content: String = c.substr(dateSplit + 1)
	if content.begins_with(headline1):
		content = content.substr(len(headline1))
	elif content.begins_with(headline2):
		# ew ew ew ew
		content = "*WorldDumpToLog at " + content.substr(len(headline2))
		date /= 1000
	else:
		return null
	# "localhost@JoeGenero at (-8, 3) = 0"
	var contentC1 = content.split(" at ") # [0] = "localhost@JoeGenero", [1] = "(-8, 3) = 0"
	if len(contentC1) != 2:
		return null
	var contentC2 = contentC1[1].split(" = ") # [0] = "(-8, 3)", [1] = "0"
	if len(contentC2) != 2:
		return null
	var posStr: String = contentC2[0]
	posStr = posStr.substr(1, len(posStr) - 2)
	var posStrSplit = posStr.split(", ")
	if len(posStrSplit) != 2:
		return null
	var dc: DuvetChange = DuvetChange.new()
	dc.x = int(posStrSplit[0])
	dc.y = int(posStrSplit[1])
	dc.recalculate_pos()
	dc.who = contentC1[0]
	dc.when = date
	dc.log_line_idx = idx
	dc.value = int(contentC2[1])
	return dc
