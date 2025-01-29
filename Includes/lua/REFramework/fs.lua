fs = {}

--- Returns a table of file paths that match the filter. filter should be a regex string for the files you wish to match.
--- @param filter string
--- @param modifier nil|'$natives'|'$autorun'|string
--- @return string[]
function fs.glob(filter, modifier) return {} end

--- Returns a table of file paths that match the filter. filter should be a regex string for the files you wish to match.
--- @param filename string
--- @return string
function fs.read(filename) return '' end

--- Returns a table of file paths that match the filter. filter should be a regex string for the files you wish to match.
--- @param filename string
--- @param data string
function fs.write(filename, data) end
