using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameTetrisClient;

public enum UpdateResult {
    KeepUpdating,
    StopUpdating,
}

public interface IScene {
    void Initialize();
    UpdateResult Update(GameTime gameTime, ISceneManager sceneManager, Assets assets);
    void Draw(GameTime gameTime, SpriteBatch spriteBatch, Assets assets);
}
