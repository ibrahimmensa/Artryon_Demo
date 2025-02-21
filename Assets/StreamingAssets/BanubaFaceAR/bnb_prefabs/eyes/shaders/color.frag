#include <bnb/glsl.frag>
#include <bnb/color_spaces.glsl>

BNB_IN(0)
vec2 var_uv;
BNB_IN(1)
vec2 var_l_eye_mask_uv;
BNB_IN(2)
vec2 var_r_eye_mask_uv;

BNB_DECLARE_SAMPLER_2D(0, 1, tex_camera);
BNB_DECLARE_SAMPLER_2D(2, 3, tex_l_eye_mask);
BNB_DECLARE_SAMPLER_2D(4, 5, tex_r_eye_mask);

BNB_DECLARE_SAMPLER_2D(6, 7, tex_l_eye_pupil_mask);
BNB_DECLARE_SAMPLER_2D(8, 9, tex_r_eye_pupil_mask);

BNB_DECLARE_SAMPLER_2D(10, 11, tex_l_eye_corneosclera_mask);
BNB_DECLARE_SAMPLER_2D(12, 13, tex_r_eye_corneosclera_mask);

vec4 color(vec4 base, vec4 target)
{
    vec4 base_yuva = bnb_rgba_to_yuva(base);
    vec4 target_yuva = bnb_rgba_to_yuva(target);
    float y_norm = 1. / 0.7;
    vec4 res_yuva = vec4(
        base_yuva.x * (target_yuva.x * y_norm),
        target_yuva.y,
        target_yuva.z,
        target_yuva.w
    );
    vec3 res_rgb = bnb_yuva_to_rgba(res_yuva).rgb;

    vec3 colored = mix(base.rgb, res_rgb, target.a);

    return vec4(colored, base.a);
}

void main()
{
    float mask = BNB_TEXTURE_2D(BNB_SAMPLER_2D(tex_l_eye_mask), var_l_eye_mask_uv).x + BNB_TEXTURE_2D(BNB_SAMPLER_2D(tex_r_eye_mask), var_r_eye_mask_uv).x;

    float mask_pupil = BNB_TEXTURE_2D(BNB_SAMPLER_2D(tex_l_eye_pupil_mask), var_l_eye_mask_uv).x + BNB_TEXTURE_2D(BNB_SAMPLER_2D(tex_r_eye_pupil_mask), var_r_eye_mask_uv).x;

    float mask_corneosclera = BNB_TEXTURE_2D(BNB_SAMPLER_2D(tex_l_eye_corneosclera_mask), var_l_eye_mask_uv).x + BNB_TEXTURE_2D(BNB_SAMPLER_2D(tex_r_eye_corneosclera_mask), var_r_eye_mask_uv).x;

    vec4 camera = BNB_TEXTURE_2D(BNB_SAMPLER_2D(tex_camera), var_uv);

    vec4 colored_eyes = color(camera, eyes_color) * mask;

    vec4 colored_pupil = color(camera, pupil_color) * mask_pupil;

    vec4 colored_corneosclera = color(camera, corneosclera_color) * mask_corneosclera;

    bnb_FragColor = colored_eyes + colored_pupil + colored_corneosclera;
}
