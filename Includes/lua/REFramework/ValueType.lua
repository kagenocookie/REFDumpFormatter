-- https://cursey.github.io/reframework-book/api/types/ValueType.html

--- @class ValueType
local ValueType = {}

--- @return ValueType
function ValueType.new(type_definition) return {} end

--- @param name string
--- @return any
function ValueType:call(name, ...) end

--- @param name string
--- @return any
function ValueType:get_field(name) end

--- Note that this does not change anything in-game. ValueType is just a local copy.
--- You'll need to pass the ValueType somewhere that would make use of the changed data.
--- @param name string
--- @param value any
function ValueType:set_field(name, value) end

function ValueType:address() end

function ValueType:get_type_definition() end

--- @type any
ValueType.type = nil
--- @type any std::vector<uint8_t>
ValueType.data = {}

--- @param offset integer
function ValueType:read_byte(offset) end

--- @param offset integer
function ValueType:read_short(offset) end

--- @param offset integer
function ValueType:read_dword(offset) end

--- @param offset integer
function ValueType:read_qword(offset) end

--- @param offset integer
function ValueType:read_float(offset) end

--- @param offset integer
function ValueType:read_double(offset) end

--- @param offset integer
--- @param value integer
function ValueType:write_byte(offset, value) end

--- @param offset integer
--- @param value integer
function ValueType:write_short(offset, value) end

--- @param offset integer
--- @param value integer
function ValueType:write_dword(offset, value) end

--- @param offset integer
--- @param value integer
function ValueType:write_qword(offset, value) end

--- @param offset integer
--- @param value number
function ValueType:write_float(offset, value) end

--- @param offset integer
--- @param value number
function ValueType:write_double(offset, value) end
