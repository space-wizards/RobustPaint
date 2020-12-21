class_name DuvetRoot
extends Reference

# dictionary mapping pixel position strings to sorted arrays of changes
var pixels: Dictionary

func _init():
	pixels = {}

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
		var tmp: Array = pixels[c.pos]
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
	var date = _parse_date(c.substr(0, dateSplit))
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
	dc.value = int(contentC2[1])
	add_change(dc)

# if this returns -1 it failed
func _parse_date(c: String) -> float:
	var ts = c.split("T")
	if len(ts) != 2:
		return -1.0
	var ts2 = ts[1].split("+")
	if len(ts2) != 2:
		return -1.0
	var ts3 = ts[0].split("-")
	if len(ts3) != 3:
		return -1.0
	var ts4 = ts2[0].split(":")
	if len(ts4) != 3:
		return -1.0
	# YES I KNOW THIS IS BAD
	if ts2[1] != "00:00":
		OS.alert("Encountered date stamp with different timezone, KILL THE PROCESS, DO NOT TRUST OUTPUT")
		return -1.0
	# YES I KNOW THIS IS WORSE
	var year = int(ts3[0])
	var month = int(ts3[1])
	var day = int(ts3[2])
	var hour = int(ts4[0])
	var minute = int(ts4[1])
	var seconds = float(ts4[2])
	# for january 1970, this must == 0
	var totalAmountOfMonths = ((year - 1970) * 12) + (month - 1)
	# calculate month base time
	var monthDaysNoLeap = [
		31, 28, 31, 30,
		31, 30, 31, 31,
		30, 31, 30, 31
	]
	var currentYear = 1970
	var monthBaseDays = 0
	for i in range(totalAmountOfMonths):
		var currentMonth = i % 12
		monthBaseDays += monthDaysNoLeap[currentMonth]
		if (currentMonth == 1) and _is_leap_year(currentYear):
			monthBaseDays += 1
		if currentMonth == 11:
			currentYear += 1
	# finish this
	var totalDays = monthBaseDays + (day - 1)
	var totalHours = (totalDays * 24) + hour
	var totalMinutes = (totalHours * 60) + minute
	return (totalMinutes * 60) + seconds

func _is_leap_year(currentYear: int) -> bool:
	if (currentYear % 4) != 0:
		return false
	# 400 accept takes precedence over 100 reject
	if (currentYear % 400) == 0:
		return true
	if (currentYear % 100) != 0:
		return false
	# neither
	return true
