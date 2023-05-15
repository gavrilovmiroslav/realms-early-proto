
function Campfire(game_state)
	return card:create("Campfire", {
		card:pick("Have fun with the troops", 
			{
				give:any_res(4),				
			}, 
			{ add:res("Luck", 2) }
		),

		card:pick("Sit around the fire", 			
			{
				give:res("Wood", 1),
				give:damage(1)
			}, 
			{ add:res("Luck", 1) }
		),

		card:pick("Enjoy a long night", {},
		{
			give:res("Luck", 1),
			give:res("Morale", 1)
		})
	})
end

function ExploringAnAncientSite(game_state)
	return card:create("Exploring an Ancient Site", {
		card:pick("Be safe", {}, 
			{ add:any_artifacts(1) }
		),

		card:pick("Be wild",
			{ give:any_units(1) }, 
			{ add:any_artifacts(2) }
		),
	})
end

function AnAbandonedLibrary(game_state)
	local topics = { "Griffins", "Treasure", "Healing", "Leadership" }

	if game_state:CountResource("Luck") > 0 then
		return card:create("An Abandoned Library", {
			card:pick("Search for riches", 
				{ give:res("Luck", 1) }, 
				{ add:any_artifacts(1) }
			),

			card:pick("Search for wisdom",
				{ give:res("Luck", 1) }, 
				{ add:res("Morale", 3) }
			),

			card:pick("Search aimlessly", 
				{ give:res("Luck", 1) },
				{ add:pack(topics[math.random(#topics)], all) }
			)
		})
	else
		return card:create("An Abandoned Library", {
			card:pick("There is nothing for us here", {}, {})
		})
	end
end

function TheLoudnessOfTheScholarsTent(game_state)
	local topics = { "Griffins", "Treasure", "Healing", "Leadership" }
	local index = math.random(#topics)
	local first = topics[index]
	table.remove(topics, index)
	index = math.random(#topics)
	local second = topics[index]

	return card:create("The Loudness of the Scholar's Tent", {
		card:pick("Learn about " .. first, 
			{}, 
			{ add:pack(first) }
		),

		card:pick("Learn about " .. second,
			{},
			{ add:pack(second) }
		)
	})
end

return {
	"Campfire",
	"ExploringAnAncientSite",
	"AnAbandonedLibrary",
	--"TheLoudnessOfTheScholarsTent"
}