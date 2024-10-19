--- @class RETypeDefinition
RETypeDefinition = {}

--- Returns the full name of the class.
--- Equivalent to concatenating self:get_namespace() and self:get_name().
function RETypeDefinition:get_full_name() return '' end

--- Returns the type name. Does not contain namespace.
function RETypeDefinition:get_name() return '' end

--- Returns the namespace this type is contained in.
function RETypeDefinition:get_namespace() return '' end

--- Returns an REMethodDefinition. To be used in things like sdk.hook.
--- The full function prototype can be supplied to get an overloaded function.
--- Example: foo:get_method("Bar(System.Int32, System.Single)")
--- @param name string
--- @return REMethodDefinition
function RETypeDefinition:get_method(name) return {} end

--- Returns a list of REMethodDefinition
--- Filters out methods that are potentially just stubs or null.
--- @return REMethodDefinition[]
function RETypeDefinition:get_methods() return {} end

--- Returns an REField.
--- @param name string
--- @return REField
function RETypeDefinition:get_field(name) return {} end

--- Returns a list of REField
--- @return REField[]
function RETypeDefinition:get_fields() return {} end

--- Returns the RETypeDefinition this type inherits from.
--- @return RETypeDefinition
function RETypeDefinition:get_parent_type() return {} end

--- Returns a System.Type. Useful for methods that require this. Equivalent to typeof in C#.
--- @return System.Type
function RETypeDefinition:get_runtime_type() return {} end

--- Returns the full size of the object. e.g. 0x14 for System.Int32.
--- @return integer
function RETypeDefinition:get_size() return 0 end

--- Returns the value type size. e.g. 4 for System.Int32.
--- @return integer
function RETypeDefinition:get_valuetype_size() return 0 end

--- @return RETypeDefinition[]
function RETypeDefinition:get_generic_argument_types() return {} end

--- @return RETypeDefinition|nil
function RETypeDefinition:get_generic_type_definition() end

--- Returns whether self or its parents are a typename or the RETypeDefinition passed.
--- @param type string|RETypeDefinition
--- @return boolean
function RETypeDefinition:is_a(type) return false end

--- Returns whether the type is a ValueType.
--- Does not necessarily need to inherit from System.ValueType for this to be true. An example would be via.vec3.
--- @return boolean
function RETypeDefinition:is_value_type() return false end

--- @return boolean
function RETypeDefinition:is_by_ref() return false end

--- @return boolean
function RETypeDefinition:is_pointer() return false end

--- @return boolean
function RETypeDefinition:is_primitive() return false end

--- @return boolean
function RETypeDefinition:is_generic_type() return false end

--- @return boolean
function RETypeDefinition:is_generic_type_definition() return false end

--- @return REManagedObject
function RETypeDefinition:create_instance() return {} end

