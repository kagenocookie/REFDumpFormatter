-- https://cursey.github.io/reframework-book/api/types/REField.html

--- @class REField
REField = {}

--- @return string
function REField:get_name() return '' end
--- @return RETypeDefinition
function REField:get_type() return {} end
--- @return integer
function REField:get_offset_from_base() return 0 end
--- @return integer
function REField:get_offset_from_fieldptr() return 0 end
--- @return RETypeDefinition
function REField:get_declaring_type() return {} end
--- @return integer
function REField:get_flags() return 0 end
--- @return boolean
function REField:is_static() return false end
--- @return boolean
function REField:is_literal() return false end
--- Returns the data contained in the field for obj.
--- @return nil|any obj
function REField:get_data(obj) end
