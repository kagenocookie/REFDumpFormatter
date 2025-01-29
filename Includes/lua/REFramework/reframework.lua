reframework = {}

--- Returns true if the REFramework menu is open.
--- @return boolean
function reframework.is_drawing_ui() return false end

--- Returns the name of the game this REFramework was compiled for.
--- e.g. "dmc5" or "re2"
function reframework.get_game_name() end

--- key is a Windows virtual key code.
--- @return boolean
function reframework:is_key_down(key) return false end

--- Returns the total number of commits on the current branch of the REFramework build.
--- @return integer
function reframework.get_commit_count() return 0 end

--- Returns the branch name of the REFramework build.
--- ex: "master"
--- @return string
function reframework.get_branch() return '' end


--- Returns the commit hash of the REFramework build.
--- @return string
function reframework.get_commit_hash() return '' end

--- Returns the last tag of the REFramework build on its current branch.
--- ex: "v1.5.4"
--- @return string
function reframework.get_tag() return '' end

--- @return string
function reframework.get_tag_long() return '' end

--- Returns the number of commits past the last tag.
--- @return integer
function reframework.get_commits_past_tag() return 0 end

--- Returns the date that REFramework was built (mm/dd/yyyy).
--- @return string
function reframework.get_build_date() return '' end

--- Returns the time that REFramework was built.
--- @return string
function reframework.get_build_time() return '' end
