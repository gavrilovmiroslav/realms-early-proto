
function AdvancedLeadership(game_state)
	return card:create("Advanced Leadership", {
		card:pick("Boost the troops' morale", 
			{}, 
			{ add:res("Morale", 2) }),

		card:pick("Rally the flag", 
			{ give:any_units(1) }, 
			{ add:res("Morale", 3) })
	})
end

return { 
	"AdvancedLeadership"
}