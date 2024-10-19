-- https://cursey.github.io/reframework-book/api/types/REMethodDefinition.html

--- @class REMethodDefinition
REMethodDefinition = {}

--- @return string
function REMethodDefinition:get_name() return '' end

--- Returns an RETypeDefinition*.
--- @return RETypeDefinition
function REMethodDefinition:get_return_type() return {} end

--- Returns a void*. Pointer to the actual function in memory.
--- @return userdata
function REMethodDefinition:get_function() return {}--[[@as any]] end

--- Returns an RETypeDefinition* corresponding to the class/type that declared this method.
--- @return RETypeDefinition
function REMethodDefinition:get_declaring_type() return {} end

--- Returns the number of parameters required to call the function.
--- @return integer
function REMethodDefinition:get_num_params() return 0 end

--- Returns a list of RETypeDefinition
--- @return RETypeDefinition[]
function REMethodDefinition:get_param_types() return {} end

--- Returns a list of strings for the parameter names
--- @return string[]
function REMethodDefinition:get_param_names() return {} end

--- Returns whether this method is static or not.
--- @return boolean
function REMethodDefinition:is_static() return false end

--- Equivalent to calling obj:call(args...)
--- Can also use self(obj, args...)
--- @param obj REManagedObject|nil nil if the method is static
--- @return any
function REMethodDefinition:call(obj, ...) return {} end
