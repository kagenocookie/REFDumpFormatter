thread = {}

--- Returns the ID of the current thread.
--- @return any
function thread.get_id() return 0 end

--- Returns the hash of the ID of the current thread.
--- @return integer
function thread.get_hash() return 0 end

--- Returns the ephemeral hook storage meant to be used within sdk.hook.
--- This is preferred over storing variables you need in a global variable in the pre hook when you need the data in the post hook.
--- The hook storage is popped/destroyed at the end of the post hook. Safe to be used within a recursive context.
--- This API is preferred because there are no longer any guarantees that the entire hook will be locked during pre/post hooks, due to deadlocking issues seen.
--- @return table
function thread.get_hook_storage() return {} end
