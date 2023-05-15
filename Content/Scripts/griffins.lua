
function GriffinTraining(game_state)
	return card:create("Griffin Training", {
		card:pick("A Fledgling Ready", 
			{}, 
			{ add:units("Griffin", 1) }
		),

		card:pick("Further Training", 
			{ 
				give:res("Morale", 2),
				give:res("Luck", 2),
				give:units("Griffin", 1) 
			}, 
			{ add:units("RoyalGriffin", 1) }
		)
	})
end

return { 
	"GriffinTraining"
}