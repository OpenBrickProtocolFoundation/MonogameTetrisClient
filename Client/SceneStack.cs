using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonogameTetrisClient;

public class SceneStack : ISceneManager {
    private readonly List<IScene> _scenes = new();
    private readonly SortedSet<int> _scenesToRemove = new();
    private int _currentSceneIndex = 0;
    private readonly List<IScene> _scenesToAdd = new();

    public bool IsEmpty => _scenes.Count == 0;

    public void Update(GameTime gameTime, Assets assets) {
        for (var i = 0; i < _scenes.Count; ++i) {
            _currentSceneIndex = i;
            if (_scenes[i].Update(gameTime, this, assets) == UpdateResult.StopUpdating) {
                break;
            }
        }

        foreach (var sceneIndex in _scenesToRemove.Reverse()) {
            if (_scenes[sceneIndex] is IDisposable disposable) {
                disposable.Dispose();
            }
            _scenes.RemoveAt(sceneIndex);
        }

        _scenesToRemove.Clear();

        _scenes.AddRange(_scenesToAdd);

        foreach (var scene in _scenesToAdd) {
            scene.Initialize();
        }

        _scenesToAdd.Clear();
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Assets assets) {
        for (var i = _scenes.Count - 1; i >= 0; --i) {
            _scenes[i].Draw(gameTime, spriteBatch, assets);
        }
    }

    public void PopCurrentScene() {
        _scenesToRemove.Add(_currentSceneIndex);
    }

    public void PushScene(IScene scene) {
        _scenesToAdd.Add(scene);
    }
}
