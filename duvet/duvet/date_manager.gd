extends Node

var _relative_month_cache_a: int = -1
var _relative_month_cache_b: int = -1

const _month_days_no_leap = [
	31, 28, 31, 30,
	31, 30, 31, 31,
	30, 31, 30, 31
]

# does not include timezone suffix
func str_date(date: float) -> String:
	var seconds = fmod(date, 60.0)
	var minute_base = int(date - seconds) / 60
	var minute = minute_base % 60
	var hour_base = minute_base / 60
	var hour = hour_base % 24

	# remaining days to process
	# the loop pulls from here into year/month,
	# then day is just this + 1
	var remainingDays = hour_base / 24
	var year = 1970
	var month = 1
	while true:
		var monthDays = _month_days_no_leap[month - 1]
		if month == 2 and _is_leap_year(year):
			monthDays += 1
		if remainingDays >= monthDays:
			remainingDays -= monthDays
			month += 1
			if month == 13:
				month = 1
				year += 1
		else:
			break
	# finish this off
	var day = remainingDays + 1
	# seconds/etc.
	var seconds_str_split = str(seconds).split(".")
	var seconds_str = ""
	if len(seconds_str_split) == 1:
		seconds_str = seconds_str_split[0].pad_zeros(2)
	elif len(seconds_str_split) == 2:
		seconds_str = seconds_str_split[0].pad_zeros(2) + "." + seconds_str_split[1]
	return str(year) + "-" + (str(month).pad_zeros(2)) + "-" + (str(day).pad_zeros(2)) + "T" + (str(hour).pad_zeros(2)) + ":" + (str(minute).pad_zeros(2)) + ":" + seconds_str

# if this returns -1 it failed
func parse_date(c: String) -> float:
	var ts = c.split("T")
	if len(ts) != 2:
		return -1.0
	var ts2 = ts[1].split("+")
	if len(ts2) > 2:
		return -1.0
	var ts3 = ts[0].split("-")
	if len(ts3) != 3:
		return -1.0
	var ts4 = ts2[0].split(":")
	if len(ts4) != 3:
		return -1.0
	# YES I KNOW THIS IS BAD
	if len(ts2) == 2:
		if ts2[1] != "00:00":
			OS.alert("Encountered date stamp with different timezone. If encountered during import, DO NOT TRUST OUTPUT")
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
	var monthBaseDays = _get_days_for_relative_month(totalAmountOfMonths)
	# finish this
	var totalDays = monthBaseDays + (day - 1)
	var totalHours = (totalDays * 24) + hour
	var totalMinutes = (totalHours * 60) + minute
	return (totalMinutes * 60) + seconds

func _get_days_for_relative_month(relativeMonth: int) -> int:
	if _relative_month_cache_a == relativeMonth:
		return _relative_month_cache_b
	# calculate month base time
	var currentYear = 1970
	var monthBaseDays = 0
	for i in range(relativeMonth):
		var currentMonth = i % 12
		monthBaseDays += _month_days_no_leap[currentMonth]
		if (currentMonth == 1) and _is_leap_year(currentYear):
			monthBaseDays += 1
		if currentMonth == 11:
			currentYear += 1
	_relative_month_cache_a = relativeMonth
	_relative_month_cache_b = monthBaseDays
	return monthBaseDays

func _is_leap_year(currentYear: int) -> bool:
	if (currentYear % 4) != 0:
		return false
	# 400 accept takes precedence over 100 reject
	if (currentYear % 400) == 0:
		return true
	if (currentYear % 100) == 0:
		return false
	# neither
	return true
