extends Control

func _ready():
	DuvetManager.initialize()
	get_tree().change_scene("res://index.tscn")
