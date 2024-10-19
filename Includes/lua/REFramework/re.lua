re = {}

--- Creates a MessageBox with text. Note that this will pause game execution until the user presses OK.
function re.msg(text) end

--- Calls function when scripts are being reset. Useful for cleaning up stuff. Calls on_config_save().
--- @param func function
function re.on_script_reset(func) end

--- Called when REFramework wants to save its config.
--- @param func function
function re.on_config_save(func) end

--- Called every frame when the "Script Generated UI" in the menu is open.
--- imgui functions can be called here, and will be placed in their own dropdown in the REFramework menu.
--- @param func function
function re.on_draw_ui(func) end

--- Called every frame. draw functions can be used here. Don't use imgui functions unless using begin_window etc...
--- @param func function
function re.on_frame(func) end

--- @param name string
--- @param func function
function re.on_pre_application_entry(name, func) end

--- Triggers function when the application/module entry associated with name is being executed.
--- This is very powerful, and can be used to run code at many important points in the engine's logic loop.
--- @param name string
--- @param func function
function re.on_application_entry(name, func) end

--- When false is returned, the GUI element is not drawn.
--- @param func fun (element: REManagedObject, context: any): boolean|nil
function re.on_pre_gui_draw_element(func) end

--- Function prototype: function on_*_draw_element(element, context)
--- Triggers function when a GUI element is being drawn.
--- Requires that a bool is returned in on_pre_gui_draw_element. When false is returned, the GUI element is not drawn.
--- @param func fun (element: REManagedObject, context: any)
function re.on_gui_draw_element(func) end
