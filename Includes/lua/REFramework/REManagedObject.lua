-- https://cursey.github.io/reframework-book/api/types/REManagedObject.html

--- @class REManagedObject : System.Object
REManagedObject = {}

--- Return value is dependent on the method's return type. Wrapper over sdk.call_object_func.
--- Full function prototype can be passed as method_name if there are multiple functions with the same name but different parameters.
--- e.g. self:call("foo(System.String, System.Single, System.UInt32, System.Object)", a, b, c, d)
--- Valid method names can be found in the Object Explorer. Find the type you're looking for, and valid methods will be found under TDB Methods.
--- @return any
function REManagedObject:call(method_name, ...) end

--- @return RETypeDefinition
function REManagedObject:get_type_definition() return {} end

--- Return type is dependent on the field type.
function REManagedObject:get_field(name) end

--- @param name string
--- @param value any
function REManagedObject:set_field(name, value) end

function REManagedObject:get_address() end

function REManagedObject:get_reference_count() end

--- Experimental API to deserialize data into self.
--- @param data any RSZ data in table format
--- @param objects any
function REManagedObject:deserialize_native(data, objects) end

--- Increments the object's internal reference count.
--- @return REManagedObject
function REManagedObject:add_ref() return {} end

--- Increments the object's internal reference count without REFramework managing it. Any objects created with REFramework and also using this method will not be deleted after the Lua state is destroyed.
--- @return REManagedObject
function REManagedObject:add_ref_permanent() return {} end

--- Decrements the object's internal reference count. Destroys the object if it reaches 0. Can only be used on objects managed by Lua.
function REManagedObject:release() end

--- Decrements the object's internal reference count. Destroys the object if it reaches 0. Can be used on any REManagedObject. Can crash the game or cause undefined behavior.
function REManagedObject:force_release() end

--- @param offset integer
function REManagedObject:read_byte(offset) end

--- @param offset integer
function REManagedObject:read_short(offset) end

--- @param offset integer
function REManagedObject:read_dword(offset) end

--- @param offset integer
function REManagedObject:read_qword(offset) end

--- @param offset integer
function REManagedObject:read_float(offset) end

--- @param offset integer
function REManagedObject:read_double(offset) end

--- @param offset integer
--- @param value any
function REManagedObject:write_byte(offset, value) end

--- @param offset integer
--- @param value any
function REManagedObject:write_short(offset, value) end

--- @param offset integer
--- @param value any
function REManagedObject:write_dword(offset, value) end

--- @param offset integer
--- @param value any
function REManagedObject:write_qword(offset, value) end

--- @param offset integer
--- @param value any
function REManagedObject:write_float(offset, value) end

--- @param offset integer
--- @param value any
function REManagedObject:write_double(offset, value) end
