namespace MonogameTetrisClient;

public interface ISceneManager {
    void PopCurrentScene();
    void PushScene(IScene scene);
}
