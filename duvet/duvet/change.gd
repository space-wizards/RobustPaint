class_name DuvetChange
extends Reference

export var pos: String = "(0, 0)"
export var who: String = ""
export var when: float = 0.0
export var log_line: String = ""
export var value: int = 0

func _get_split_pos() -> PoolStringArray:
	var sp = pos.substr(1, len(pos) - 2)
	return sp.split(", ")

func get_x() -> int:
	return int(_get_split_pos()[0])

func get_y() -> int:
	return int(_get_split_pos()[1])
