class_name DuvetTimeCircuit
extends HBoxContainer

var time: float = 0.0

signal time_changed()

func _init():
	time = OS.get_unix_time()
	connect("time_changed", self, "_update_text")

func _ready():
	_update_text()

func _update_text():
	# print("time circuit = " + str(time))
	$Label.text = DateManager.str_date(time)

func mod(v):
	time += v
	emit_signal("time_changed")

const DAY = 86400
const HOUR = 3600

func _m1d():
	mod(-DAY)

func _m1h():
	mod(-HOUR)

func _m1m():
	mod(-60)

func _m1s():
	mod(-1)


func _p1s():
	mod(1)

func _p1m():
	mod(60)

func _p1h():
	mod(HOUR)

func _p1d():
	mod(DAY)

