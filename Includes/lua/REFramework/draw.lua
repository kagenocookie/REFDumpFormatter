-- https://cursey.github.io/reframework-book/api/draw.html

draw = {}

--- Returns an optional Vector2f corresponding to the 2D screen position. Returns nil if world_pos is not visible.
--- @param world_pos via.vec3
--- @return Vector2f|nil
function draw.world_to_screen(world_pos) end

--- @param text string
--- @param pos_3d via.vec3
--- @param color integer
function draw.world_text(text, pos_3d, color) end

--- @param text string
--- @param x number
--- @param y number
--- @param color integer
function draw.text(text, x, y, color) end

--- @param x number
--- @param y number
--- @param w number
--- @param h number
--- @param color integer
function draw.filled_rect(x, y, w, h, color) end

--- @param x number
--- @param y number
--- @param w number
--- @param h number
--- @param color integer
function draw.outline_rect(x, y, w, h, color) end

--- @param x1 number
--- @param y1 number
--- @param x2 number
--- @param y2 number
--- @param color integer
function draw.line(x1, y1, x2, y2, color) end

--- @param x number
--- @param y number
--- @param radius number
--- @param color integer
--- @param num_segments integer
function draw.outline_circle(x, y, radius, color, num_segments) end

--- @param x number
--- @param y number
--- @param radius number
--- @param color integer
--- @param num_segments integer
function draw.filled_circle(x, y, radius, color, num_segments) end

--- @param x1 number
--- @param y1 number
--- @param x2 number
--- @param y2 number
--- @param x3 number
--- @param y3 number
--- @param x4 number
--- @param y4 number
--- @param color integer
function draw.outline_quad(x1, y1, x2, y2, x3, y3, x4, y4, color) end

--- @param x1 number
--- @param y1 number
--- @param x2 number
--- @param y2 number
--- @param x3 number
--- @param y3 number
--- @param x4 number
--- @param y4 number
--- @param color integer
function draw.filled_quad(x1, y1, x2, y2, x3, y3, x4, y4, color) end

--- Draws a 3D sphere with a 2D approximation in world space.
--- @param world_pos via.vec3
--- @param radius number
--- @param color integer
--- @param outline integer
function draw.sphere(world_pos, radius, color, outline) end

--- Draws a 3D capsule with a 2D approximation in world space.
--- @param world_start_pos via.vec3
--- @param world_end_pos via.vec3
--- @param radius number
--- @param color integer
--- @param outline integer
function draw.capsule(world_start_pos, world_end_pos, radius, color, outline) end

--- Returns a tuple of changed, mat. Mat is the modified matrix that was passed.
--- @param unique_id integer an int64 that must be unique for every gizmo. Usually an address of an object will work. The same ID will control multiple gizmos with the same ID.
--- @param matrix Matrix4x4f the Matrix4x4f the gizmo is modifying.
--- @param operation ImGuizmoOperation defaults to UNIVERSAL. Use imgui.ImGuizmoOperation enum.
--- @param mode ImGuizmoMode defaults to WORLD. WORLD or LOCAL. Use imgui.ImGuizmoMode enum.
function draw.gizmo(unique_id, matrix, operation, mode) end

--- @param matrix Matrix4x4f
function draw.cube(matrix) end

--- @param matrix Matrix4x4f
--- @param size number
function draw.grid(matrix, size) end

--- @enum ImGuizmoOperation
local ImGuizmoOperation = {
    TRANSLATE = "TRANSLATE",
    ROTATE = "ROTATE",
    SCALE = "SCALE",
    SCALEU = "SCALEU",
    UNIVERSAL = "UNIVERSAL",
}
--- @enum ImGuizmoMode
local ImGuizmoMode = {
    WORLD = "WORLD",
    LOCAL = "LOCAL",
}
