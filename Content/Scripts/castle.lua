
function SeekingOutAChampion(game_state)
	return card:create("Seeking out a Champion", {
		card:pick("Edric the Knight", {}, {
			add:hero("Castle_Edric_Knight"),
			add:pack("Griffins", all),
			add:pack("Leadership", all)
		}),

		card:pick("Caitlin the Cleric", {}, {
			add:hero("Castle_Caitlin_Cleric"),
			add:pack("Treasure", all),
			add:pack("Healing", all),
		})
	})
end

function VisitingTheGuardhouses(game_state)
	return card:create("Visiting the Guardhouses", {
		card:pick("Get 3 Pikemen", {}, {
			add:units("Pikeman", 3),
		}),

		card:pick("Get 1 Halberdier", {}, {
			add:units("Halberdier", 1)
		})
	})
end

function BrotherhoodOfTheSword(game_state)
	return card:create("Brotherhood of the Sword", {
		card:pick("Visit the Cult", {}, {
			add:res("Morale", 3),
		}),

		card:pick("Conscribe", 
			{ give:any_units(1) }, 
			{ add:res("Morale", 5) })
	})
end

function TarryInTheGriffinTower(game_state)
	return card:create("Tarry in the Griffin Tower", {
		card:pick("Get 3 Griffins", {}, {
			add:units("Griffin", 3),
		}),

		card:pick("Get 1 Royal Griffin", {}, {
			add:units("RoyalGriffin", 1),
		})
	})
end

function StopByTheArcherTower(game_state)
	return card:create("Stop by the Archer Tower", {
		card:pick("Get 3 Archers", {}, {
			add:units("Archer", 3),
		}),

		card:pick("Get 1 Marksman", {}, {
			add:units("Marksman", 1),
		})
	})
end

function AcquireProvisions(game_state)
	return card:create("Acquire Provisions", {
		card:pick("Forage", {}, {
			add:res("Gold", 3),
			add:res("Wood", 3),
			add:res("Ore", 3)
		}),

		card:pick("Explore", {}, {
			add:res("Gem", 1),			
			add:res("Mercury", 1)
		})
	})
end

return { 
	"SeekingOutAChampion", 
	"VisitingTheGuardhouses", 
	"BrotherhoodOfTheSword", 
	"TarryInTheGriffinTower", 
	"StopByTheArcherTower",
	"AcquireProvisions"
}