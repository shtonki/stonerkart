using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using stonerkart.Properties;

namespace stonerkart.src.util
{
    public enum AudioType
    {
        Song,
        EntersField,
        EntersGraveyard,
        CreatureToDisplace,
        CreatureToHand,
        Misc,
    }

    public enum musicName
    {
        pew,
        c304_2,
    }

    public class Boombox
    {
        static Queue<musicName> musicQueue = new Queue<musicName>(); 
        static Queue<string> soundQueue = new Queue<string>();

        private static SoundPlayer player = new SoundPlayer();

        private void audioPlayed(object sender, EventArgs e)
        {
            dequeueSound();
        }

        public void playAudio(string name, EventHandler doneCallBack = null)
        {
            Task.Factory.StartNew(() => play(name, doneCallBack));
        }

        public void playAudio(CardTemplate ct, AudioType at, EventHandler doneCallBack = null)
        {
            Task.Factory.StartNew(() => play(CardTemplate.Fresh_sFox, AudioType.EntersField, doneCallBack));
        }

        private void play(string name, EventHandler doneCallback = null)
        {
            player.Stream = Resources.ResourceManager.GetStream(name);
            player.PlaySync();
            doneCallback(this, new EventArgs());
        }

        private void play(CardTemplate ct, AudioType at, EventHandler doneCallback = null)
        {
            player.Stream = Resources.ResourceManager.GetStream("audio" + ct.ToString().Replace("_s", "") + at);
            player.PlaySync();
            doneCallback(this, new EventArgs());
        }

        public void queueMusic(musicName name)
        {
            musicQueue.Enqueue(name);
        }

        public void queueSound(CardTemplate ct, AudioType at)
        {
            soundQueue.Enqueue("audio" + ct.ToString().Replace("_s", "") + at);    
        }

        public void dequeueSound()
        {
            if (soundQueue.Count > 0)
            {
                playAudio(soundQueue.Dequeue(), audioPlayed);
            }
        }
    }
}
