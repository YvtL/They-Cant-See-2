1. For ALL portal effects, including depth and passthrough, you MUST have both Depth Texture and Opaque Texture enabled in URP.
2. For custom post-processing lighting and grid projection, see the renderer feature setup for Mirza Beig/Portal VFX/Settings/URP-PortalVFX-Renderer.
-- You must add Full Screen Pass Renderer Features with the Portal Glow materials to your URP renderer settings in order to use the custom post-processing effects.

If you're on Unity 6, you will need to manually set up the URP renderer features with Compatibility Mode enabled.