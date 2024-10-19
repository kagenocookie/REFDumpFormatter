-- https://cursey.github.io/reframework-book/api/types/SystemArray.html

--- @class SystemArray : REManagedObject
SystemArray = {}

--- Returns the array's elements as a lua table.
--- Keep in mind these objects will all be full REManagedObject types, not the ValueTypes they represent, if any, like System.Int32
--- @return REManagedObject[]
function SystemArray:get_elements() return {} end

--- Returns the object at index in the array.
--- @return REManagedObject|ValueType|any
function SystemArray:get_element(index) end

--- @return integer
function SystemArray:get_size() return 0 end
