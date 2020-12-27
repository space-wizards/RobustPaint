extends VBoxContainer

onready var main_display = find_node("TextureRect")
onready var coordinates_line: LineEdit = find_node("Coordinates")
onready var timecircuit: DuvetTimeCircuit = find_node("Timecircuit")
onready var pixel_history: ItemList = find_node("Pixel History")
onready var user_history: ItemList = find_node("UserHistoryList")
onready var user_history_label: Label = find_node("UserHistoryLabel")
onready var ibsca: CheckBox = find_node("IBSACb")
onready var tccb: CheckBox = find_node("TCCb")
var main_display_image_texture: ImageTexture

const VIEW_SIZE = 97
const VIEW_CEN = 48

const WORLD_SIZE = 1024
const WORLD_BASE = -512

const colours: Array = [
	Color.black, # empty
	Color.white,
	Color("#5acdf9"), # cyan
	Color("#9966fd"), # lavender
	Color("#f5a9b8"),  # pink
	Color.black
]

var camX = 0
var camY = 0

var selection_disable = false

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
	coordinates_line.text = str(camX) + "_" + str(camY) + "_" + DateManager.str_date(ceil(time))
	# responsible (1)
	var pos = db.format_pos(camX, camY)
	if db.pixels.has(pos):
		_refresh_list(pixel_history, db.pixels[pos], time, false)
	else:
		_refresh_list(pixel_history, [], time, false)

func _setup_user_history():
	var db: DuvetRoot = DuvetManager.database
	var time = timecircuit.time
	var ch_ref: DuvetChange = db.state_at_time(camX, camY, time)
	if ch_ref != null:
		user_history_label.text = ch_ref.who
		_refresh_list(user_history, db.users[ch_ref.who], time, true)
	else:
		user_history_label.text = "Nobody"
		_refresh_list(user_history, [], time, true)

func _refresh_list(list: ItemList, arr: Array, time: float, where: bool):
	selection_disable = true
	list.clear()
	var validIdx = -1
	for itmIdx in range(len(arr)):
		var itm: DuvetChange = arr[itmIdx]
		if where:
			list.add_item(DateManager.str_date(floor(itm.when)) + " @ " + itm.pos + " -> " + str(itm.value))
		else:
			list.add_item(DateManager.str_date(floor(itm.when)) + " : " + itm.who + " -> " + str(itm.value))
		list.set_item_metadata(itmIdx, itm)
		if time >= itm.when:
			validIdx = itmIdx
	if validIdx != -1:
		list.select(validIdx)
	selection_disable = false

func _listtravel(idx: int, list: ItemList):
	if selection_disable:
		return
	if idx == -1:
		return
	var md = list.get_item_metadata(idx)
	if md == null:
		return
	timecircuit.time = md.when
	camX = md.x
	camY = md.y
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

func _on_export():
	var db: DuvetRoot = DuvetManager.database
	var ch_ref: DuvetChange = db.state_at_time(camX, camY, timecircuit.time)
	var basename = "unknown.." + coordinates_line.text
	if ch_ref != null:
		basename = ch_ref.who + "." + coordinates_line.text
	var basefilename = basename.replace("@", ".").replace(":", ".").replace("-", ".").replace(":", ".")
	ibsca.pressed = false
	tccb.pressed = true
	_update_dsky()
	var imgV = "data:image/png;base64," + Marshalls.raw_to_base64(dsky_image.save_png_to_buffer())
	ibsca.pressed = true
	tccb.pressed = false
	_update_dsky()
	var imgO = "data:image/png;base64," + Marshalls.raw_to_base64(dsky_image.save_png_to_buffer())
	var fn = File.new()
	fn.open("./out/" + basefilename + ".html", File.WRITE)
	fn.store_line("<html><head>")
	fn.store_line(" <title>" + basename + "</title>")
	fn.store_line(" <style>")
	fn.store_line("  html { font-family: monospace; color: white; background: black; }")
	fn.store_line("  .evidence { width: " + str(VIEW_SIZE * 4) + "px; height: " + str(VIEW_SIZE * 4) + "px; image-rendering: crisp-edges; }")
	fn.store_line(" </style>")
	fn.store_line("</head><body>")
	fn.store_line(" <h1>DUVET INCIDENT REPORT</h1>")
	fn.store_line(" <h2>Images</h2>")
	fn.store_line(" <table border=\"1\">")
	fn.store_line("  <tr>")
	fn.store_line("   <td>Truecolour</td>")
	fn.store_line("   <td>Ownership</td>")
	fn.store_line("  </tr>")
	fn.store_line("  <tr>")
	fn.store_line("   <td><img class=\"evidence\" src=\"" + imgV + "\"/></td>")
	fn.store_line("   <td><img class=\"evidence\" src=\"" + imgO + "\"/></td>")
	fn.store_line("  </tr>")
	fn.store_line(" </table>")
	fn.store_line(" <h2>Data</h2>")
	fn.store_line(" <table border=\"1\">")
	fn.store_line("  <tr><td>Incident ID</td><td>" + coordinates_line.text + "</td></tr>")
	fn.store_line("  <tr><td>Pos</td><td>" + db.format_pos(camX, camY) + "</td></tr>")
	if ch_ref != null:
		fn.store_line("  <tr><td>Pos Author</td><td>" + ch_ref.who + "</td></tr>")
		fn.store_line("  <tr><td>Log Line</td><td>" + ch_ref.get_log_line() + "</td></tr>")
	else:
		fn.store_line("  <tr><td>Pos Author</td><td>?</td></tr>")
		fn.store_line("  <tr><td>Log Line</td><td>?</td></tr>")
	fn.store_line(" </table>")
	fn.store_line(" <h2>Notes</h2>")
	fn.store_line(" <p>")
	fn.store_line("No note supplied.")
	fn.store_line(" </p>")
	fn.store_line("</body></html>")
	fn.close()
	OS.shell_open("./out/" + basefilename + ".html")

func _on_Coordinates_text_entered(new_text):
	var parts = coordinates_line.text.split("_")
	if len(parts) != 3:
		return
	camX = int(parts[0])
	camY = int(parts[1])
	timecircuit.time = DateManager.parse_date(parts[2])
	timecircuit.emit_signal("time_changed")


func _on_world_pressed():
	var image: Image = Image.new()
	image.create(WORLD_SIZE, WORLD_SIZE, false, Image.FORMAT_RGB8)
	var db: DuvetRoot = DuvetManager.database
	var time = timecircuit.time
	# main
	image.lock()
	for x in range(WORLD_SIZE):
		for y in range(WORLD_SIZE):
			var tx = WORLD_BASE + x
			var ty = WORLD_BASE + WORLD_SIZE - (y + 1)
			var ch: DuvetChange = db.state_at_time(tx, ty, time)
			var state = Color.black
			if ch != null:
				state = colours[ch.value]
			image.set_pixel(x, y, state)
	image.unlock()
	image.save_png("./out/world.png")
	OS.shell_open("./out/world.png")
