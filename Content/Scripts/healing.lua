
function TravelingRemedies(game_state)
	local options = {}

	if not game_state.HasDamage then
		table.insert(options, 
			card:pick("Morale grows", 
				{}, 
				{ add:res("Morale", 2) }
			)
		)
	else
		table.insert(options, 
			card:pick("Heal the hurt",
				{ give:damage(game_state.CountDamage / 2) }, 
				{}
			)
		)
	end

	return card:create("Traveling Remedies", options)
end


function JourneyingVigor(game_state)
	local options = {}

	if not game_state.HasDamage then
		table.insert(options, 
			card:pick("More time to explore",
				{}, 
				{ add:res("Gold", 1) }
			)
		)
	else
		damage = game_state.CountDamage / 2
		if damage < 2 then damage = 2 end
		if damage > 4 then damage = 4 end
		if damage > game_state.CountDamage then damage = game_state.CountDamage end

		table.insert(options, 
			card:pick("Patch the troops",
				{ give:damage(damage) }, 
				{ add:res("Morale", 1) }
			)
		)
	end

	return card:create("Journeying Vigor", options)
end

return { 
	"TravelingRemedies",
	"JourneyingVigor"
}
