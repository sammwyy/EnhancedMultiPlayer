namespace EnhancedMultiPlayer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

public class EnhancedMultiPlayer : Mod
{
	// Client side.
	[Autoload(Side = ModSide.Client)]
	public sealed class ClientSystem : ModSystem
	{
		private AllyStatusUI _allyStatusUI;
		private UserInterface _allyStatusInterface;

		public override void Load()
		{
			_allyStatusUI = new();
			_allyStatusUI.Activate();

			_allyStatusInterface = new();
			_allyStatusInterface.SetState(_allyStatusUI);
		}

		public override void UpdateUI(GameTime gameTime)
		{
			if (AllyStatusUI.Visible)
			{
				_allyStatusInterface?.Update(gameTime);
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1)
			{
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"EnhancedMultiPlayer: Ally Status",
					delegate
					{
						_allyStatusInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}


}