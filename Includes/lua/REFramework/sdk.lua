sdk = {}

--- Returns the version of the type database. A good approximation of the version of the RE Engine the game is running on.
--- @return string
function sdk.get_tdb_version() return '' end

--- Returns game_namespace.name.
--- DMC5: name would get converted to app.name
--- RE3: name would get converted to offline.name
--- @return string
function sdk.game_namespace(name) return '' end

function sdk.get_thread_context() return {} end

--- Returns a void*. Can be used with sdk.call_native_func
--- Possible singletons can be found in the Native Singletons view in the Object Explorer.
function sdk.get_native_singleton(name) return {} end

--- Returns an REManagedObject*.
--- Possible singletons can be found in the Singletons view in the Object Explorer.
function sdk.get_managed_singleton(name) return {} end

--- @return RETypeDefinition
function sdk.find_type_definition(name) return {} end

--- Returns a System.Type.
--- Equivalent to calling sdk.find_type_definition(name):get_runtime_type().
--- Equivalent to typeof in C#.
--- @return System.Type
function sdk.typeof(name) return {} end

--- Returns an REManagedObject.
--- Equivalent to calling sdk.find_type_definition(typename):create_instance()
--- @return REManagedObject
--- @param typename string
--- @param simplify boolean|nil
function sdk.create_instance(typename, simplify) return {} end

--- Creates and returns a new System.String from str.
--- @param str string
--- @return System.String
function sdk.create_managed_string(str) return {} end

--- Creates and returns a new SystemArray of the given type, with length elements.
--- If type cannot resolve to a valid System.Type, a Lua error will be thrown.
--- @param type System.Type|string|RETypeDefinition
--- @param length integer
--- @return SystemArray
function sdk.create_managed_array(type, length) return {} end

--- Returns a fully constructed REManagedObject of type System.SByte given the value.
--- @return REManagedObject
function sdk.create_sbyte(value) return {} end

--- Returns a fully constructed REManagedObject of type System.Byte given the value.
--- @return REManagedObject
function sdk.create_byte(value) return {} end

--- Returns a fully constructed REManagedObject of type System.Int16 given the value.
--- @return REManagedObject
function sdk.create_int16(value) return {} end

--- Returns a fully constructed REManagedObject of type System.UInt16 given the value.
--- @return REManagedObject
function sdk.create_uint16(value) return {} end

--- Returns a fully constructed REManagedObject of type System.Int32 given the value.
--- @return REManagedObject
function sdk.create_int32(value) return {} end

--- Returns a fully constructed REManagedObject of type System.UInt32 given the value.
--- @return REManagedObject
function sdk.create_uint32(value) return {} end

--- Returns a fully constructed REManagedObject of type System.Int64 given the value.
--- @return REManagedObject
function sdk.create_int64(value) return {} end

--- Returns a fully constructed REManagedObject of type System.UInt64 given the value.
--- @return REManagedObject
function sdk.create_uint64(value) return {} end

--- Returns a fully constructed REManagedObject of type System.Single given the value.
--- @return REManagedObject
function sdk.create_single(value) return {} end

--- Returns a fully constructed REManagedObject of type System.Double given the value.
--- @return REManagedObject
function sdk.create_double(value) return {} end

--- Returns an REResource.
--- If the typename does not correctly correspond to the resource file or is not a resource type, nil will be returned.
--- @return REManagedObject
--- @param typename string
--- @param resource_path string
function sdk.create_resource(typename, resource_path) return {} end

--- Returns an REManagedObject which is a via.UserData. typename can be "via.UserData" unless you know the full typename.
--- @return REManagedObject
--- @param typename string
--- @param userdata_path string
function sdk.create_userdata(typename, userdata_path) return {} end

--- Returns a list of REManagedObject generated from data.
--- @param data any data is the raw RSZ data contained for example in a .scn file, starting at the RSZ magic in the header. data must in table format as an array of bytes.
function sdk.deserialize(data) return {} end

--- Return value is dependent on what the method returns.
--- Full function prototype can be passed as method_name if there are multiple functions with the same name but different parameters.
--- Should only be used with native types, not REManagedObject (though, it can be if wanted).
--- @param object REManagedObject
--- @param type_definition RETypeDefinition
--- @param method_name string
function sdk.call_native_func(object, type_definition, method_name, ...) return {} end

--- Return value is dependent on what the method returns.
--- Full function prototype can be passed as method_name if there are multiple functions with the same name but different parameters.
--- Alternative calling method: managed_object:call(method_name, args...)
function sdk.call_object_func(managed_object, method_name, ...) return {} end

--- @param object REManagedObject
--- @param type_definition RETypeDefinition
--- @param field_name string
--- @return any
function sdk.get_native_field(object, type_definition, field_name) return {} end

--- @param object REManagedObject
--- @param type_definition RETypeDefinition
--- @param field_name string
--- @param value any
function sdk.set_native_field(object, type_definition, field_name, value) return {} end

--- Returns a REManagedObject*. Returns the current camera being used by the engine.
--- @return REManagedObject
function sdk.get_primary_camera() return {} end

--- Creates a hook for method_definition, intercepting all incoming calls the game makes to it.
--- Using pre_function and post_function, the behavior of these functions can be modified.
--- NOTE: Some native methods may not be able to be hooked with this, e.g. if they are just a wrapper over the native function. Some additional work will need to be done from our end to make those work.
--- @param method_definition REMethodDefinition
--- @param pre_function nil|fun(thread_context: userdata, ...): sdk.PreHookResult|nil
--- @param post_function nil|fun(returnValue: userdata): any
--- @param ignore_jmp boolean|nil Skips trying to follow the first jmp in the function. Defaults to false.
function sdk.hook(method_definition, pre_function, post_function, ignore_jmp) end

--- Similar to sdk.hook but hooks on a per-object basis instead, instead of hooking the function globally for all objects.
--- Only works if the target method is a virtual method.
function sdk.hook_vtable(obj, method, pre, post) end

--- Returns true if value is a valid REManagedObject.
--- Use only if necessary. Does a bunch of checks and calls IsBadReadPtr a lot.
function sdk.is_managed_object(value) end

--- Attempts to convert value to an REManagedObject*.
--- A value that is not a valid REManagedObject* will return nil, equivalent to calling sdk.is_managed_object on it.
--- @param value any
--- @return REManagedObject|nil
function sdk.to_managed_object(value) return {} end

--- Attempts to convert value to a double.
--- @param value any
--- @return number
function sdk.to_double(value) return 0 end

--- Attempts to convert value to a float.
--- @param value any
--- @return number
function sdk.to_float(value) return 0 end

--- Attempts to convert value to a int64.
--- @param value any
--- @return integer
function sdk.to_int64(value) return 0 end

--- Attempts to convert value to a void*.
--- @param value any
--- @return userdata|nil
function sdk.to_ptr(value) end

--- Converts number to a void*.
--- @param number number
--- @return userdata|nil
function sdk.float_to_ptr(number) end

--- @enum sdk.PreHookResult
sdk.PreHookResult = {
    SKIP_ORIGINAL = 0,
    CALL_ORIGINAL = 1,
}
