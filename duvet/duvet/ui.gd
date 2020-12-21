extends VBoxContainer

onready var main_display = find_node("TextureRect")
onready var coordinates_label = find_node("Coordinates")
onready var timecircuit: DuvetTimeCircuit = find_node("Timecircuit")
onready var pixel_history: ItemList = find_node("Pixel History")
onready var user_history: ItemList = find_node("UserHistoryList")
onready var user_history_label: Label = find_node("UserHistoryLabel")
onready var ibsca: CheckBox = find_node("IBSACb")
onready var tccb: CheckBox = find_node("TCCb")
var main_display_image_texture: ImageTexture

const VIEW_SIZE = 97
const VIEW_CEN = 48

var camX = 0
var camY = 0

var dsky_image: Image

func _ready():
	dsky_image = Image.new()
	dsky_image.create(VIEW_SIZE, VIEW_SIZE, false, Image.FORMAT_RGB8)
	main_display_image_texture = ImageTexture.new()
	main_display_image_texture.create_from_image(dsky_image)
	main_display.texture = main_display_image_texture
	timecircuit.connect("time_changed", self, "_update_dsky")
	ibsca.connect("pressed", self, "_update_dsky")
	tccb.connect("pressed", self, "_update_dsky")
	pixel_history.connect("item_selected", self, "_listtravel", [pixel_history])
	user_history.connect("item_selected", self, "_listtravel", [user_history])
	pixel_history.connect("item_activated", self, "_listtravel", [pixel_history])
	user_history.connect("item_activated", self, "_listtravel", [user_history])
	_update_dsky()

func _update_dsky():
	var db: DuvetRoot = DuvetManager.database
	var time = timecircuit.time
	var colours: Array = [Color.black, Color.white, Color.cyan, Color.lavender, Color.pink]
	# pre
	var ch_ref: DuvetChange = db.state_at_time(camX, camY, time)
	# main
	dsky_image.lock()
	var default_state = Color.red
	if tccb.pressed:
		default_state = Color.black
	for x in range(VIEW_SIZE):
		for y in range(VIEW_SIZE):
			var tx = camX + (x - VIEW_CEN)
			var ty = camY - (y - VIEW_CEN)
			var ch: DuvetChange = db.state_at_time(tx, ty, time)
			var state = default_state
			if ch != null:
				state = colours[ch.value]
				if (ibsca.pressed) and (ch_ref != null):
					if ch_ref.who == ch.who:
						state = Color.yellow
			dsky_image.set_pixel(x, y, state)
	dsky_image.unlock()
	main_display_image_texture.create_from_image(dsky_image, 0)
	# coords
	coordinates_label.text = DuvetManager.database.format_pos(camX, camY)
	# responsible (1)
	var pos = db.format_pos(camX, camY)
	if db.pixels.has(pos):
		_refresh_list(pixel_history, db.pixels[pos], time)
	else:
		_refresh_list(pixel_history, [], time)

func _setup_user_history():
	var db: DuvetRoot = DuvetManager.database
	var time = timecircuit.time
	var ch_ref: DuvetChange = db.state_at_time(camX, camY, time)
	if ch_ref != null:
		user_history_label.text = ch_ref.who
		_refresh_list(user_history, db.users[ch_ref.who], time)
	else:
		user_history_label.text = "Nobody"
		_refresh_list(user_history, [], time)

func _refresh_list(list: ItemList, arr: Array, time: float):
	list.clear()
	var validIdx = -1
	for itmIdx in range(len(arr)):
		var itm: DuvetChange = arr[itmIdx]
		list.add_item(DuvetManager.format_datetime(itm.when) + ": " + itm.who + " -> " + str(itm.value))
		list.set_item_metadata(itmIdx, itm)
		if time >= itm.when:
			validIdx = itmIdx
	if validIdx != -1:
		list.select(validIdx)

func _listtravel(idx: int, list: ItemList):
	if idx == -1:
		return
	var md = list.get_item_metadata(idx)
	if md == null:
		return
	timecircuit.time = md.when + 0.01
	camX = md.get_x()
	camY = md.get_y()
	timecircuit.emit_signal("time_changed")

func _fine_down():
	camY -= 1
	_update_dsky()

func _fine_right():
	camX += 1
	_update_dsky()

func _fine_left():
	camX -= 1
	_update_dsky()

func _fine_up():
	camY += 1
	_update_dsky()
