namespace EnhancedMultiPlayer;

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
public class AllyStatusUI : UIState
{
    public static readonly int ITEM_WIDTH = 160;
    public static readonly int ITEM_HEIGHT = 40;
    public static readonly int ITEM_GAP = 20;

    public static bool Visible = true;
    private UIElement _area;

    private Player[] _teamMembers = [];
    private List<int> _teamMemberIds = [];
    private AllyStatusDisplay[] _teamStatuses = [];

    public override void OnInitialize()
    {
        _area = new();
        _area.Left.Set(20, 0f); // Place the resource bar to the left of the hearts.
        _area.Top.Set(300, 0f); // Placing it just a bit below the top of the screen.
        _area.Width.Set(ITEM_WIDTH, 0f);
        _area.Height.Set(ITEM_HEIGHT, 0f);
        Append(_area);
    }

    public override void Update(GameTime gameTime)
    {
        Player player = Main.LocalPlayer;

        if (player.team == 0)
        {
            _area.RemoveAllChildren();
        }
        else
        {
            List<int> newTeamMemberIds = [];
            for (int i = 0; i < Main.player.Length; i++)
            {
                if (Main.player[i].team == player.team && Main.player[i].active)
                {
                    newTeamMemberIds.Add(i);
                }
            }


            bool teamChanged = Enumerable.SequenceEqual(newTeamMemberIds, _teamMemberIds);
            if (!teamChanged)
            {
                _teamMemberIds = newTeamMemberIds;
                int teamCount = _teamMemberIds.Count;
                _teamMembers = new Player[teamCount];
                _teamStatuses = new AllyStatusDisplay[teamCount];

                _area.RemoveAllChildren();
                _area.Height.Set(teamCount * ITEM_HEIGHT + (teamCount - 1) * ITEM_GAP, 0f);

                for (int i = 0; i < teamCount; i++)
                {
                    _teamMembers[i] = Main.player[_teamMemberIds[i]];
                    _teamStatuses[i] = new AllyStatusDisplay(_teamMembers[i]);
                    _teamStatuses[i].Width.Set(ITEM_WIDTH, 0f);
                    _teamStatuses[i].Height.Set(ITEM_HEIGHT, 0f);
                    _teamStatuses[i].Left.Set(0, 0f);
                    _teamStatuses[i].Top.Set(i * (ITEM_HEIGHT + ITEM_GAP), 0f);
                    _area.Append(_teamStatuses[i]);
                }
            }
            else
            {
                for (int i = 0; i < _teamMembers.Length; i++)
                {
                    _teamStatuses[i].SetPlayer(_teamMembers[i]);
                }
            }
        }

        base.Update(gameTime);
    }
}

public class AllyStatusDisplay : UIElement
{
    private Player _player;
    private readonly UIText _nameText;
    private readonly UIText _healthText;

    public AllyStatusDisplay(Player player)
    {
        _player = player;

        float initialX = 50;
        float initialY = 20;

        _nameText = new("Player", 0.6f);
        _nameText.Top.Set(initialY - 10, 0f);
        _nameText.Left.Set(initialX, 0f);
        _nameText.HAlign = 0f;

        _healthText = new("100/100", 0.6f);
        _healthText.Top.Set(initialY + 10, 0f);
        _healthText.Left.Set(initialX, 0f);
        _healthText.HAlign = 0f;

        Append(_nameText);
        Append(_healthText);
    }

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        Rectangle headBounds = new(0, 0, 40, 56);
        Vector2 drawpos = new(Parent.GetDimensions().X + Left.Pixels - 3, Parent.GetDimensions().Y + Top.Pixels - 3);

        bool invalid = _player.whoAmI != Main.myPlayer && (_player.dead || !_player.active || _player.team != Main.LocalPlayer.team && _player.team != 0);

        // draw head
        spriteBatch.Draw(TextureAssets.Players[0, 0].Value, drawpos, headBounds, invalid ? Color.Gray : _player.skinColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // draw eyes
        spriteBatch.Draw(TextureAssets.Players[0, 2].Value, drawpos, headBounds, invalid ? Color.Gray : _player.eyeColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // draw sclera
        spriteBatch.Draw(TextureAssets.Players[0, 1].Value, drawpos, headBounds, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
        // draw hair
        spriteBatch.Draw(TextureAssets.PlayerHair[_player.hair].Value, drawpos, headBounds, invalid ? Color.Gray : _player.hairColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);

        base.DrawSelf(spriteBatch);
    }

    public override void Update(GameTime gameTime)
    {
        _nameText.SetText($"{_player.name}:");
        _healthText.SetText($"{_player.statLife}/{_player.statLifeMax2}");
        base.Update(gameTime);
    }
}