-- https://cursey.github.io/reframework-book/api/log.html

log = {}

--- @param text string
function log.info(text) end

--- @param text string
function log.warn(text) end

--- @param text string
function log.debug(text) end

--- Requires DebugView or a debugger to see this. Can also be viewed in the debug console spawned with "Spawn Debug Console" under ScriptRunner.
--- @param text string
function log.error(text) end
