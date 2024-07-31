using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameTetrisClient;

public enum UpdateResult {
    KeepUpdating,
    StopUpdating,
}

public abstract class Scene {
    protected Assets Assets;

    protected Scene(Assets assets) {
        Assets = assets;
    }

    public abstract void Initialize();
    public abstract UpdateResult Update(GameTime gameTime, ISceneManager sceneManager);
    public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
}
