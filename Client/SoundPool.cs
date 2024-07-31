using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace MonogameTetrisClient;

public sealed class SoundPool {
    private readonly List<SoundEffect> _sounds = new();
    private int? _lastSoundIndex = null;

    public SoundPool(List<string> soundNames, ContentManager content) {
        foreach (var soundName in soundNames) {
            _sounds.Add(content.Load<SoundEffect>(soundName));
        }
    }

    public void Play() {
        if (_sounds.Count == 0) {
            Console.Error.WriteLine("No sounds to play.");
            return;
        }

        if (_sounds.Count == 1) {
            Play(0);
            return;
        }

        if (_lastSoundIndex is null) {
            _lastSoundIndex = Random.Shared.Next(_sounds.Count);
            Play(_lastSoundIndex.Value);
            return;
        }

        while (true) {
            var index = Random.Shared.Next(_sounds.Count);
            if (index != _lastSoundIndex) {
                _lastSoundIndex = index;
                Play(index);
                break;
            }
        }
    }

    private void Play(int index) {
        _sounds[index].Play();
    }
}
