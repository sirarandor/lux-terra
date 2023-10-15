shader_type canvas_item;

void fragment() {
	COLOR = texture(TEXTURE, UV);
}

void vertex() {
	VERTEX += vec2(0, 0);
}
