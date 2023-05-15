
function ALuckyFind(game_state)
	return card:create("A Lucky Find", {
		card:pick("Leave it behind", {}, {}),

		card:pick("It's a coin!", {}, {
			add:res("Gold", 1)
		})
	})
end

function UnearthingRiches(game_state)
	return card:create("Unearthing Riches", {
		card:pick("Grab some gems", {}, {
			add:res("Gem", 3),
		}),

		card:pick("Delve for gold", {}, {
			add:res("Gold", 3)
		})
	})
end

function QuestingForTheHoard(game_state)
	return card:create("Questing for the Hoard", {
		card:pick("A minor discovery", {}, {
			add:res("Gold", 1),
			add:res("Gem", 1),
		}),

		card:pick("An arcane laboratory", {}, {
			add:damage(1),
			add:res("Gem", 2),
			add:res("Mercury", 1),
		}),
	})
end

return {
	"ALuckyFind",
	"UnearthingRiches",
	"QuestingForTheHoard"
}
