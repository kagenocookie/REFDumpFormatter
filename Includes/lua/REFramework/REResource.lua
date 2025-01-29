-- https://cursey.github.io/reframework-book/api/types/REResource.html

--- @class REResource
REResource = {}

function REResource:get_address() end

--- Increments the object's internal reference count.
--- @return REResource
function REResource:add_ref() return {} end

--- Decrements the object's internal reference count. Destroys the object if it reaches 0. Can only be used on objects managed by Lua.
function REResource:release() end

--- Returns a via.ResourceHolder variant which holds self. Automatically adds a reference to self.
--- @param holderClassname string
--- @return REManagedObject
function REResource:create_holder(holderClassname) return {} end
