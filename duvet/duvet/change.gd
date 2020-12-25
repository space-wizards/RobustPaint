class_name DuvetChange
extends Reference

export var x: int = 0
export var y: int = 0
export var pos: String = "(0, 0)"
export var who: String = ""
export var when: float = 0.0
export var log_line_src: String = ""
export var log_line_idx: int = 1
export var value: int = 0

func recalculate_pos() -> void:
	pos = "(" + str(x) + ", " + str(y) + ")"

func read(f: File, fn: String) -> void:
	x = f.get_16()
	if x >= 0x8000:
		x -= 0x10000
	y = f.get_16()
	if y >= 0x8000:
		y -= 0x10000
	recalculate_pos()
	who = f.get_pascal_string()
	when = f.get_double()
	log_line_src = fn
	log_line_idx = f.get_32()
	value = f.get_16()

func write(f: File) -> void:
	f.store_16(x)
	f.store_16(y)
	f.store_pascal_string(who)
	f.store_double(when)
	f.store_32(log_line_idx)
	f.store_16(value)

func get_log_line() -> String:
	var tx: File = File.new()
	tx.open(log_line_src, File.READ)
	var idx: int = 1
	while not tx.eof_reached():
		var line = tx.get_line()
		if idx == log_line_idx:
			return line
		idx += 1
	return "<unavailable>"
