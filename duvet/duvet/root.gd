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
	var tmp: Array = pixels[pos]
	var refChange = DuvetChange.new()
	refChange.when = time
	var idx = tmp.bsearch_custom(refChange, self, "compare_time", true) - 1
	if idx < 0:
		return null
	if idx >= tmp.size():
		idx = tmp.size() - 1
	return tmp[idx]

func compare_time(x, y):
	return x.when < y.when

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
