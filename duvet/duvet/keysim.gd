extends Label

export var base_path: NodePath
onready var base = get_node(base_path)

var up: bool = false
var down: bool = false
var left: bool = false
var right: bool = false

var timer: float = 0.0

func _gui_input(event):
	if event is InputEventKey:
		var evk: InputEventKey = event
		if evk.scancode == KEY_UP or evk.scancode == KEY_W:
			var old = up
			up = evk.pressed
			if not old:
				generate_immediate()
			accept_event()
		if evk.scancode == KEY_DOWN or evk.scancode == KEY_S:
			var old = down
			down = evk.pressed
			if not old:
				generate_immediate()
			accept_event()
		if evk.scancode == KEY_LEFT or evk.scancode == KEY_A:
			var old = left
			left = evk.pressed
			if not old:
				generate_immediate()
			accept_event()
		if evk.scancode == KEY_RIGHT or evk.scancode == KEY_D:
			var old = right
			right = evk.pressed
			if not old:
				generate_immediate()
			accept_event()

func generate_immediate():
	if up:
		base.camY += 1
	if down:
		base.camY -= 1
	if left:
		base.camX -= 1
	if right:
		base.camX += 1
	if up or down or left or right:
		base._update_dsky()
	timer = 0.0

func _process(delta):
	timer += delta
	if timer >= 0.025:
		var nt = timer - 0.025
		generate_immediate()
		timer = nt
