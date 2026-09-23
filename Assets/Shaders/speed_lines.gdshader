shader_type canvas_item;


// Use a seamless noise texture, otherwise a seam shows where the angle wraps around.
uniform sampler2D noise: repeat_enable, filter_linear;
uniform vec4 line_color: source_color;
uniform float line_count: hint_range(1.0, 8.0, 1.0) = 1.0;
uniform float line_density: hint_range(0.0, 1.0) = 0.5;
uniform float line_faloff: hint_range(0.0, 1.0) = 0.25;
uniform float mask_size: hint_range(0.0, 1.5) = 0.1;
uniform float mask_edge: hint_range(0.0, 1.5) = 0.5;
uniform float line_taper: hint_range(0.0, 1.0) = 1.0;
uniform float animation_speed: hint_range(1.0, 30.0) = 12.0;
uniform float intensity: hint_range(0.0, 1.0) = 1.0;




float inv_lerp(float from, float to, float value){
  return (value - from) / (to - from);
}




float hash(float x) {
	return fract(sin(x * 12.9898) * 43758.5453);
}




void fragment(){
	// Aspect-correct offset from the center, in units of screen height
	float aspect = SCREEN_PIXEL_SIZE.y / SCREEN_PIXEL_SIZE.x;
	vec2 dir = (UV - vec2(0.5)) * vec2(aspect, 1.0);
	float dist = length(dir);
	float angle = atan(dir.y, dir.x) / TAU + 0.5;

	// Each animation step samples a fresh random spot of the noise so the pattern never loops
	float step_id = floor(TIME * animation_speed);
	vec2 offset = vec2(hash(step_id), hash(step_id + 17.0));
	vec2 polar_uv = vec2(angle * line_count, dist * 0.02) + offset;
	float lines = texture(noise, polar_uv).r;

	float density = line_density * intensity;
	float mask = clamp(inv_lerp(mask_size, mask_edge, dist), 0.0, 1.0);
	float threshold = 1.0 - density * mix(1.0, mask, line_taper);

	float result = smoothstep(threshold, threshold + line_faloff, lines);
	result *= smoothstep(mask_size, mask_size + 0.02, dist);

	COLOR.rgb = line_color.rgb;
	COLOR.a = min(line_color.a, result) * intensity;
}
