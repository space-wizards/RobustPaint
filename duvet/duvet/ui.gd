extends VBoxContainer

onready var main_display = find_node("TextureRect")
onready var coordinates_label = find_node("Coordinates")
onready var timecircuit: DuvetTimeCircuit = find_node("Timecircuit")
onready var item_list: ItemList = find_node("ItemList")
onready var ibsca: CheckBox = find_node("IBSACb")
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
	item_list.connect("item_selected", self, "_listtravel")
	_update_dsky()

func _update_dsky():
	var db: DuvetRoot = DuvetManager.database
	var time = timecircuit.time
	var colours: Array = [Color.black, Color.white, Color.cyan, Color.lavender, Color.pink]
	# pre
	var ch_ref: DuvetChange = db.state_at_time(camX, camY, time)
	# main
	dsky_image.lock()
	for x in range(VIEW_SIZE):
		for y in range(VIEW_SIZE):
			var tx = camX + (x - VIEW_CEN)
			var ty = camY - (y - VIEW_CEN)
			var ch: DuvetChange = db.state_at_time(tx, ty, time)
			var state = Color.red
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
	# responsible
	item_list.clear()
	var pos = db.format_pos(camX, camY)
	if db.pixels.has(pos):
		var arr: Array = db.pixels[pos]
		var validIdx = -1
		for itmIdx in range(len(arr)):
			var itm: DuvetChange = arr[itmIdx]
			item_list.add_item(DuvetManager.format_datetime(itm.when) + ": " + itm.who + " -> " + str(itm.value))
			item_list.set_item_metadata(itmIdx, itm.when)
			if time >= itm.when:
				validIdx = itmIdx
		if validIdx != -1:
			item_list.select(validIdx)

func _listtravel(idx: int):
	if idx == -1:
		return
	var md = item_list.get_item_metadata(idx)
	if md == null:
		return
	timecircuit.time = md + 0.01
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
