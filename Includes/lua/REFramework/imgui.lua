-- https://cursey.github.io/reframework-book/api/imgui.html

imgui = {}

--- Creates a new window with the title of name.
--- open is a bool. Can be nil. If not nil, a close button will be shown in the top right of the window.
--- flags - ImGuiWindowFlags.
--- begin_window must have a corresponding end_window call.
--- This function may only be called in on_frame, not on_draw_ui.
--- Returns a bool. Returns false if the user wants to close the window.
---@param name string
---@param open boolean|nil
---@param flags ImGuiWindowFlags|number|nil
---@return boolean
function imgui.begin_window(name, open, flags) return false end

--- Ends the last begin_window call. Required.
function imgui.end_window() end

---@param size Vector2f
---@param border boolean
---@param flags ImGuiWindowFlags|number
function imgui.begin_child_window(size, border, flags) end

function imgui.end_child_window() end

function imgui.begin_group() end

function imgui.end_group() end

function imgui.begin_rect() end

--- These two methods draw a rectangle around the elements between begin_rect and end_rect
---@param additional_size number|nil
---@param rounding number|nil
function imgui.end_rect(additional_size, rounding) end

--- Will disable and darken elements in between it and end.
---@param disabled boolean|nil (default true)
function imgui.begin_disabled(disabled) end

--- Ends disable and darken elements.
function imgui.end_disabled() end

--- Draws a button with label text, and optional size.
--- Returns true when the user presses the button.
---@param label string
---@param size Vector2f|table|nil (size 2 array)
---@return boolean
function imgui.button(label, size) return false end

---@param label string
---@return boolean
function imgui.small_button(label) return false end

--- size is a Vector2f or a size 2 array.
---@param id string
---@param size Vector2f|table (size 2 array)
---@param flags number
---@return boolean
function imgui.invisible_button(id, size, flags) return false end

---@param id string
---@param dir number (ImguiDir)
---@return boolean
function imgui.arrow_button(id, dir) return false end

--- Draws text.
---@param text string
function imgui.text(text) end

--- Draws text with color.
--- color is an integer color in the form ARGB.
---@param text string
---@param color number (unsigned int)
function imgui.text_colored(text, color) end

--- Returns a tuple of changed, value
---@param label string
---@param value boolean
---@return boolean,boolean
function imgui.checkbox(label, value) return false, false end

--- Returns a tuple of changed, value
---@param label string
---@param value number
---@param speed number|nil
---@param min number|nil
---@param max number|nil
---@param display_format string|nil (ex "%.3f")
---@return boolean,number
function imgui.drag_float(label, value, speed, min, max, display_format) return false, 0 end

--- Returns a tuple of changed, value
---@param label string
---@param value Vector2f
---@param speed number|nil
---@param min number|nil
---@param max number|nil
---@param display_format string|nil (ex "%.3f")
---@return boolean,Vector2f
function imgui.drag_float2(label, value, speed, min, max, display_format) return false, {} end

--- Returns a tuple of changed, value
---@param label string
---@param value Vector3f
---@param speed number|nil
---@param min number|nil
---@param max number|nil
---@param display_format string|nil (ex "%.3f")
---@return boolean,Vector3f
function imgui.drag_float3(label, value, speed, min, max, display_format) return false, {} end

--- Returns a tuple of changed, value
---@param label string
---@param value Vector4f
---@param speed number|nil
---@param min number|nil
---@param max number|nil
---@param display_format string|nil (ex "%.3f")
---@return boolean,Vector4f
function imgui.drag_float4(label, value, speed, min, max, display_format) return false, {} end

--- Returns a tuple of changed, value
---@param label string
---@param value number
---@param speed number|nil
---@param min number|nil
---@param max number|nil
---@param display_format string|nil (ex "%d")
---@return boolean,number
function imgui.drag_int(label, value, speed, min, max, display_format) return false, 0 end

--- Returns a tuple of changed, value
---@param label string
---@param value number
---@param min number
---@param max number
---@param display_format string|nil (ex "%.3f")
---@return boolean,number
function imgui.slider_float(label, value, min, max, display_format) return false, 0 end

--- Returns a tuple of changed, value
---@param label string
---@param value integer
---@param min integer
---@param max integer
---@param display_format string|nil (ex "%d")
---@return boolean,integer
function imgui.slider_int(label, value, min, max, display_format) return false, 0 end

--- Returns a tuple of changed, value, selection_start, selection_end
---@param label string
---@param value string
---@param flags integer|nil
---@return boolean,string,number,number
function imgui.input_text(label, value, flags) return false, '', 0, 0 end

--- Returns a tuple of changed, value, selection_start, selection_end
---@param label string
---@param value string
---@param size Vector2f|integer|nil
---@param flags integer|nil
---@return boolean,string,number,number
function imgui.input_text_multiline(label, value, size, flags) return false, '', 0, 0 end

--- Returns a tuple of changed, value.
--- changed = true when selection changes.
--- value is the selection index within values (a table)
--- values can be a table with any type of keys, as long as the values are strings.
---@param label string
---@param selection number
---@param values table
---@return boolean,number
function imgui.combo(label, selection, values) return false, 0 end

--- Returns a tuple of changed, value. color is an integer color in the form ABGR which imgui and draw APIs expect.
---@param label string
---@param color number
---@param flags ImGuiColorEditFlags|number
---@return boolean,number
function imgui.color_picker(label, color, flags) return false, 0 end

--- Returns a tuple of changed, value. color is an integer color in the form ARGB.
---@param label string
---@param color number
---@param flags ImGuiColorEditFlags|number
---@return boolean,number
function imgui.color_picker_argb(label, color, flags) return false, 0 end

--- Returns a tuple of changed, value
---@param label string
---@param color Vector3f
---@param flags ImGuiColorEditFlags|number
---@return boolean,Vector3f
function imgui.color_picker3(label, color, flags) return false, {} end

--- Returns a tuple of changed, value
---@param label string
---@param color Vector4f
---@param flags ImGuiColorEditFlags|number
---@return boolean,Vector4f
function imgui.color_picker4(label, color, flags) return false, {} end

--- Returns a tuple of changed, value. color is an integer color in the form ABGR which imgui and draw APIs expect.
---@param label string
---@param color number
---@param flags ImGuiColorEditFlags|number
---@return boolean,number
function imgui.color_edit(label, color, flags) return false, 0 end

--- Returns a tuple of changed, value. color is an integer color in the form ARGB.
---@param label string
---@param color number
---@param flags ImGuiColorEditFlags|number
---@return boolean,number
function imgui.color_edit_argb(label, color, flags) return false, 0 end

--- Returns a tuple of changed, value
---@param label string
---@param color Vector3f
---@param flags ImGuiColorEditFlags|number
---@return boolean,Vector3f
function imgui.color_edit3(label, color, flags) return false, {} end

--- Returns a tuple of changed, value
--- flags for color_picker/edit APIs: ImGuiColorEditFlags
---@param label string
---@param color Vector4f
---@param flags ImGuiColorEditFlags|number|nil
---@return boolean,Vector4f
function imgui.color_edit4(label, color, flags) return false, {} end

--- Begins a tree node
---@param label string
---@return boolean
function imgui.tree_node(label) return false end

--- Begins a tree node
---@param id any
---@param label string
---@return boolean
function imgui.tree_node_ptr_id(id, label) return false end

--- Begins a tree node
---@param id string
---@param label string
---@return boolean
function imgui.tree_node_str_id(id, label) return false end

--- All of the above tree functions must have a corresponding tree_pop!
function imgui.tree_pop() end

--- Keeps the next item on the same line
function imgui.same_line() end

--- Adds space
function imgui.spacing() end

--- Adds a new line
function imgui.new_line() end

---@param flags integer|nil
---@return boolean
function imgui.is_item_hovered(flags) return false end

---@return boolean
function imgui.is_item_active() return false end

---@return boolean
function imgui.is_item_focused() return false end

---@param name string
---@return boolean
function imgui.collapsing_header(name) return false end

--- Loads a font file from the reframework/fonts subdirectory at the specified size with optional Unicode ranges (an array of start, end pairs ending with 0).
--- Returns a handle for use with imgui.push_font(). If filepath is nil, it will load the default font at the specified size.
---@param filepath string
---@param size number (int)
---@param ranges table
---@return any
function imgui.load_font(filepath, size, ranges) end

--- Sets the font to be used for the next set of ImGui widgets/draw commands until imgui.pop_font is called.
---@param font any
function imgui.push_font(font) end

--- Unsets the previously pushed font.
function imgui.pop_font() end

--- Returns size of the default font for REFramework's UI.
---@return number
function imgui.get_default_font_size() return 0 end

---@param pos Vector2f
---@param condition ImGuiCond
---@param pivot Vector2f|table
function imgui.set_next_window_pos(pos, condition, pivot) end

--- condition is the ImGuiCond enum.
---@param size Vector2f
---@param condition ImGuiCond|number
function imgui.set_next_window_size(size, condition) end

--- id can be an int, const char*, or void*.
function imgui.push_id(id) end

function imgui.pop_id() end

---@return number
function imgui.get_id() return 0 end

--- Returns a Vector2f corresponding to the user's mouse position in window space.
---@return Vector2f
function imgui.get_mouse() return {} end

--- progress is a float between 0 and 1.
--- size is a Vector2f or a size 2 array.
--- overlay is a string on top of the progress bar.
--- local progress = 0.0
--- re.on_frame(function()
---     progress = progress + 0.001
---     if progress &gt; 1.0 then
---         progress = 0.0
---     end
--- end)
--- re.on_draw_ui(function()
---     imgui.progress_bar(progress, Vector2f.new(200, 20), string.format(&quot;Progress: %.1f%%&quot;, progress * 100))
--- end)
---@param progress number
---@param size Vector2f|table (size 2 array)
---@param overlay string
function imgui.progress_bar(progress, size, overlay) end

---@param pos Vector2f|table (size 2 array)
---@param size Vector2f|table (size 2 array)
function imgui.item_size(pos, size) end

--- Adds an item with the specified position and size to the current window.
---@param pos Vector2f|table (size 2 array)
---@param size Vector2f|table (size 2 array)
function imgui.item_add(pos, size) end

--- Clears the current window's draw list path.
function imgui.draw_list_path_clear() end

--- Adds a line to the current window's draw list path given the specified pos
--- pos is a Vector2f or a size 2 array.
---@param pos Vector2f|table (size 2 array)
function imgui.draw_list_path_line_to(pos) end

--- Strokes the current window's draw list path with the specified color, closed state, and thickness.
--- color is an integer color in the form ARGB.
--- closed is a bool.
--- thickness is a float.
---@param color number (ARGB int)
---@param closed boolean
---@param thickness number
function imgui.draw_list_path_stroke(color, closed, thickness) end

--- Returns the index of the specified imgui_key.
---@param imgui_key number
---@return integer
function imgui.get_key_index(imgui_key) return 0 end

--- Returns true if the specified key is currently being held down.
---@param key integer
---@return boolean
function imgui.is_key_down(key) return false end

--- Returns true if the specified key was pressed during the current frame.
---@param key integer
---@return boolean
function imgui.is_key_pressed(key) return false end

--- Returns true if the specified key was released during the current frame.
---@param key integer
---@return boolean
function imgui.is_key_released(key) return false end

--- Returns true if the specified mouse button is currently being held down.
---@param button integer
---@return boolean
function imgui.is_mouse_down(button) return false end

--- Returns true if the specified mouse button was clicked during the current frame.
---@param button integer
---@return boolean
function imgui.is_mouse_clicked(button) return false end

--- Returns true if the specified mouse button was released during the current frame.
---@param button integer
---@return boolean
function imgui.is_mouse_released(button) return false end

--- Returns true if the specified mouse button was double-clicked during the current frame.
---@param button integer
---@return boolean
function imgui.is_mouse_double_clicked(button) return false end

--- Indents the current line by indent_width pixels.
---@param indent_width number
function imgui.indent(indent_width) end

--- Unindents the current line by indent_width pixels.
---@param indent_width number
function imgui.unindent(indent_width) end

--- Starts a tooltip window that will be drawn at the current cursor position.
function imgui.begin_tooltip() end

--- Ends the current tooltip window.
function imgui.end_tooltip() end

--- Sets the text for the current tooltip window.
---@param text string
function imgui.set_tooltip(text) end

--- Opens a popup with the specified str_id and flags.
---@param str_id string
---@param flags ImGuiWindowFlags|number
function imgui.open_popup(str_id, flags) end

--- Begins a new popup with the specified str_id and flags.
---@param str_id string
---@param flags ImGuiWindowFlags|number
---@return boolean
function imgui.begin_popup(str_id, flags) return false end

--- Begins a new popup with the specified str_id and flags, anchored to the last item.
---@param str_id string
---@param flags ImGuiWindowFlags|number
---@return boolean
function imgui.begin_popup_context_item(str_id, flags) return false end

--- Ends the current popup window.
function imgui.end_popup() end

--- Closes the current popup window.
function imgui.close_current_popup() end

--- Returns true if the popup with the specified str_id is open.
---@param str_id string
---@return boolean
function imgui.is_popup_open(str_id) return false end

--- Calculates and returns the size of the specified text as a Vector2f.
---@param text string
---@return Vector2f
function imgui.calc_text_size(text) return {} end

--- Returns the size of the current window as a Vector2f.
---@return Vector2f
function imgui.get_window_size() return {} end

--- Returns the position of the current window as a Vector2f.
---@return Vector2f
function imgui.get_window_pos() return {} end

--- Sets the open state of the next collapsing header or tree node to is_open based on the specified condition.
---@param is_open boolean
---@param condition ImGuiCond|number
function imgui.set_next_item_open(is_open, condition) end

--- Begins a new list box with the specified label and size.
---@param label string
---@param size Vector2f
---@return boolean
function imgui.begin_list_box(label, size) return false end

--- Ends the current list box.
function imgui.end_list_box() end

--- Begins a new menu bar.
function imgui.begin_menu_bar() end

--- Ends the current menu bar.
---@return boolean
function imgui.end_menu_bar() return false end

--- Begins the main menu bar.
---@return boolean
function imgui.begin_main_menu_bar() return false end

--- Ends the main menu bar.
function imgui.end_main_menu_bar() end

--- Begins a new menu with the specified label. The menu will be disabled if enabled is false.
---comment
---@param label string
---@param enabled boolean
---@return boolean
function imgui.begin_menu(label, enabled) return false end

--- Ends the current menu.
function imgui.end_menu() end

--- Adds a menu item with the specified label, shortcut, selected state, and enabled state.
---@param label string
---@param shortcut string
---@param selected boolean
---@param enabled boolean
---@return boolean
function imgui.menu_item(label, shortcut, selected, enabled) return false end

--- Returns the size of the display as a Vector2f.
---@return Vector2f
function imgui.get_display_size() return {} end

--- Pushes the width of the next item to item_width pixels.
---@param item_width number
function imgui.push_item_width(item_width) end

--- Pops the last item width off the stack.
function imgui.pop_item_width() end

--- Sets the width of the next item to item_width pixels.
---@param item_width number
function imgui.set_next_item_width(item_width) end

--- Calculates and returns the current item width.
---@return number
function imgui.calc_item_width() return 0 end

--- Pushes a new style color onto the style stack.
---@param style_color integer
---@param color number|Vector4f
function imgui.push_style_color(style_color, color) end

--- Pops count style colors off the style stack.
---@param count number
function imgui.pop_style_color(count) end

--- Pushes a new style variable onto the style stack.
---@param idx number
---@param value any
function imgui.push_style_var(idx, value) end

--- Pops count style variables off the style stack.
---@param count number
function imgui.pop_style_var(count) end

--- Returns the current cursor position as a Vector2f.
---@return Vector2f
function imgui.get_cursor_pos() return {} end

--- Sets the current cursor position to pos.
---@param pos Vector2f
function imgui.set_cursor_pos(pos) end

--- Returns the initial cursor position as a Vector2f.
---@return Vector2f
function imgui.get_cursor_start_pos() return {} end

--- Returns the current cursor position in screen coordinates as a Vector2f.
---@return Vector2f
function imgui.get_cursor_screen_pos() return {} end

--- Sets the current cursor position in screen coordinates to pos.
---@param pos Vector2f
function imgui.set_cursor_screen_pos(pos) end

--- Sets the default focus to the next widget.
--- Scroll APIs
function imgui.set_item_default_focus() end

--- Returns the horizontal scroll position.
---@return number
function imgui.get_scroll_x() return 0 end

--- Returns the vertical scroll position.
---@return number
function imgui.get_scroll_y() return 0 end

--- Sets the horizontal scroll position to scroll_x.
---@param scroll_x number
function imgui.set_scroll_x(scroll_x) end

--- Sets the vertical scroll position to scroll_y.
---@param scroll_y number
function imgui.set_scroll_y(scroll_y) end

--- Returns the maximum horizontal scroll position.
---@return number
function imgui.get_scroll_max_x() return 0 end

--- Returns the maximum vertical scroll position.
---@return number
function imgui.get_scroll_max_y() return 0 end

--- Centers the horizontal scroll position.
---@param center_x_ratio number
function imgui.set_scroll_here_x(center_x_ratio) end

--- Centers the vertical scroll position.
---@param center_y_ratio number
function imgui.set_scroll_here_y(center_y_ratio) end

--- Sets the horizontal scroll position from the specified local_x and center_x_ratio.
---@param local_x number
---@param center_x_ratio number
function imgui.set_scroll_from_pos_x(local_x, center_x_ratio) end

--- Sets the vertical scroll position from the specified local_y and center_y_ratio.
--- Table API
---@param local_y number
---@param center_y_ratio number
function imgui.set_scroll_from_pos_y(local_y, center_y_ratio) end

--- Begins a new table with the specified str_id, column count, flags, outer_size, and inner_width.
--- str_id is a string.
--- column is an integer.
--- flags is an optional ImGuiTableFlags enum.
--- outer_size is a Vector2f or a size 2 array.
--- inner_width is an optional float.
---@param str_id string
---@param column number
---@param flags ImGuiTableFlags
---@param outer_size Vector2f|table (size 2 array)
---@param inner_width number
---@return boolean
function imgui.begin_table(str_id, column, flags, outer_size, inner_width) return false end

--- Ends the current table.
function imgui.end_table() end

--- Begins a new row in the current table with the specified row_flags and min_row_height.
--- row_flags is an optional ImGuiTableRowFlags enum.
--- min_row_height is an optional float.
---@param row_flags ImGuiTableRowFlags
---@param min_row_height number
function imgui.table_next_row(row_flags, min_row_height) end

--- Advances to the next column in the current table.
---@return boolean
function imgui.table_next_column() return false end

--- Sets the current column index to column_index.
---@param column_index number
---@return boolean
function imgui.table_set_column_index(column_index) return false end

--- Sets up a column in the current table with the specified label, flags, init_width_or_weight, and user_id.
---comment
---@param label string
---@param flags integer
---@param init_width_or_weight number
---@param user_id number (ImGuiID)
function imgui.table_setup_column(label, flags, init_width_or_weight, user_id) end

--- Sets up a scrolling region in the current table with cols columns and rows rows frozen.
---@param cols number
---@param rows number
function imgui.table_setup_scroll_freeze(cols, rows) end

--- Submits a header row in the current table.
function imgui.table_headers_row() end

--- Submits a header cell with the specified label in the current table.
---@param label string
function imgui.table_header(label) end

--- Returns the sort specifications for the current table.
---@return any
function imgui.table_get_sort_specs() end

--- Returns the number of columns in the current table.
---@return number
function imgui.table_get_column_count() return 0 end

--- Returns the current column index.
---@return number
function imgui.table_get_column_index() return 0 end

--- Returns the current row index.
---@return number
function imgui.table_get_row_index() return 0 end

--- Returns the name of the specified column in the current table.
---@param column number
---@return string
function imgui.table_get_column_name(column) return '' end

--- Returns the flags of the specified column in the current table.
---@param column number
---@return integer
function imgui.table_get_column_flags(column) return 0 end

--- Sets the background color of the specified target in the current table with the given color and column index.
---@param target integer
---@param color number
---@param column number
function imgui.table_set_bg_color(target, color, column) end

--- @enum ImGuiCond
local ImGuiCond = {
    ImGuiCond_None          = 0,        --- No condition (always set the variable), same as _Always
    ImGuiCond_Always        = 1 << 0,   --- No condition (always set the variable)
    ImGuiCond_Once          = 1 << 1,   --- Set the variable once per runtime session (only the first call will succeed)
    ImGuiCond_FirstUseEver  = 1 << 2,   --- Set the variable if the object/window has no persistently saved data (no entry in .ini file)
    ImGuiCond_Appearing     = 1 << 3    --- Set the variable if the object/window is appearing after being hidden/inactive (or the first time
}
--- @alias ImGuiColorEditFlags integer
--- @alias ImGuiWindowFlags integer
--- @alias ImGuiTableFlags integer
--- @alias ImGuiTableRowFlags integer