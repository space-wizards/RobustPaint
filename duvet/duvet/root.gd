class_name DuvetRoot
extends Reference

# dictionary mapping pixel position strings to sorted arrays of changes
var pixels: Dictionary
var users: Dictionary

func _init():
	pixels = {}
	users = {}

# -1 is unknown
func state_at_time(x: int, y: int, time: float) -> DuvetChange:
	var pos = format_pos(x, y)
	if not pixels.has(pos):
		return null
	var tmp = pixels[pos]
	var lastChange: DuvetChange = null
	# this could be made into a binary search
	for ch in tmp:
		if ch.when <= time:
			lastChange = ch
	return lastChange

func format_pos(x: int, y: int) -> String:
	return "(" + str(x) + ", " + str(y) + ")"

func add_change(c: DuvetChange) -> void:
	if not pixels.has(c.pos):
		pixels[c.pos] = [c]
	else:
		_add_change_order(pixels[c.pos], c)
	if not users.has(c.who):
		users[c.who] = [c]
	else:
		_add_change_order(users[c.who], c)

func _add_change_order(tmp: Array, c: DuvetChange) -> void:
	var chkIdx = len(tmp) - 1
	while chkIdx >= 0:
		if tmp[chkIdx].when < c.when:
			# Here + 1
			tmp.insert(chkIdx + 1, c)
			return
		chkIdx -= 1
	tmp.insert(0, c)

func try_add_change(c: String) -> void:
	# first line of sanity
	var headline: String = "[WARN] c.s.go.co.brush: "
	if c.count(headline) != 1:
		return
	# All lines are prefixed with a date.
	# If there is no date, it's not valid.
	# "2020-12-18T12:00:39.2850864+00:00 [WARN] c.s.go.co.brush: localhost@JoeGenero at (-8, 3) = 0"
	var dateSplit = c.find(" ")
	if dateSplit == -1:
		return
	var dateOriginal = c.substr(0, dateSplit)
	var date = DateManager.parse_date(dateOriginal)
	if date == -1.0:
		return
	var content: String = c.substr(dateSplit + 1)
	if not content.begins_with(headline):
		return
	content = content.substr(len(headline))
	# "localhost@JoeGenero at (-8, 3) = 0"
	var contentC1 = content.split(" at ") # [0] = "localhost@JoeGenero", [1] = "(-8, 3) = 0"
	if len(contentC1) != 2:
		return
	var contentC2 = contentC1[1].split(" = ") # [0] = "(-8, 3)", [1] = "0"
	if len(contentC2) != 2:
		return
	var dc: DuvetChange = DuvetChange.new()
	dc.pos = contentC2[0]
	dc.who = contentC1[0]
	dc.when = date
	dc.when_original = dateOriginal
	dc.value = int(contentC2[1])
	add_change(dc)
