using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameTetrisClient;

public sealed class Assets {
    public Texture2D MinoTexture { get; }
    public Texture2D TetrionTexture { get; }
    public Texture2D WhiteTexture { get; }
    public SpriteFont Font { get; }
    public SoundPool ClickSound { get; }
    public SoundPool SwiffSound { get; }
    public SoundEffect Clear1Sound { get; }
    public SoundEffect Clear2Sound { get; }
    public SoundEffect Clear3Sound { get; }
    public SoundEffect Clear4Sound { get; }

    public Assets(GraphicsDevice graphicsDevice, ContentManager content) {
        MinoTexture = content.Load<Texture2D>("mino02");
        TetrionTexture = content.Load<Texture2D>("tetrion_garbage");
        WhiteTexture = new Texture2D(graphicsDevice, 1, 1);
        WhiteTexture.SetData(new[] { Color.White });
        Font = content.Load<SpriteFont>("font");
        ClickSound = new SoundPool(new List<string>
            {
                "sfx/click01",
                "sfx/click02",
                "sfx/click03",
                "sfx/click04",
                "sfx/click05",
                "sfx/click06",
                "sfx/click07",
                "sfx/click08",
                "sfx/click09",
            },
            content
        );
        SwiffSound = new SoundPool(new List<string>
            {
                "sfx/swiff01",
                "sfx/swiff02",
                "sfx/swiff03",
                "sfx/swiff04",
                "sfx/swiff05",
                "sfx/swiff06",
                "sfx/swiff07",
                "sfx/swiff08",
            },
            content
        );
        Clear1Sound = content.Load<SoundEffect>("sfx/clear1");
        Clear2Sound = content.Load<SoundEffect>("sfx/clear2");
        Clear3Sound = content.Load<SoundEffect>("sfx/clear3");
        Clear4Sound = content.Load<SoundEffect>("sfx/clear4");
    }
}
