using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using GTA;
using GTA.Math;
using GTA.Native;
using NativeUI;

namespace Races
{
    public static class Helpers
    {
        public static void CountdownRace
            (
            ref DateTime _lasttime,
            ref uint _seconds,
            ref bool _isInRace,
            ref int _countdown,
            ref Sprite _fadeoutSprite,
            List<Vehicle> _participants,
            ref uint _missionStart
            )
        {
            if (DateTime.Now.Second != _lasttime.Second)
            {
                _seconds++;
                _lasttime = DateTime.Now;
                if (_isInRace && _countdown > 0)
                {
                    var screen = UIMenu.GetScreenResolutionMantainRatio();
                    var w = Convert.ToInt32(screen.Width / 2);
                    _countdown--;
                    if (_countdown > 3) return;
                    _fadeoutSprite = new Sprite(
                        "mpinventory",
                        "in_world_circle",
                        new Point(w - 125, 200),
                        new Size(250, 250), 0f,
                        _countdown == 0 ? Color.FromArgb(49, 235, 126) : Color.FromArgb(241, 247, 57)
                        );
                    Function.Call(Hash.REQUEST_SCRIPT_AUDIO_BANK, "HUD_MINI_GAME_SOUNDSET", true);
                    Function.Call(Hash.PLAY_SOUND_FRONTEND, 0, "CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET");
                    if (_countdown == 0)
                    {
                        _participants.ForEach(car => car.FreezePosition = false);
                        _missionStart = _seconds;
                    }
                }
                else if (_isInRace && _countdown == 0)
                {
                    _countdown = -1;
                }
            }
        }

        public static int LoadRaces(List<Race> _races)
        {
            int counter = 0;
            if (!Directory.Exists("scripts\\Races")) return 0;
            foreach (string path in Directory.GetFiles("scripts\\Races", "*.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Race));
                StreamReader file = new StreamReader(path);
                var raceout = (Race)serializer.Deserialize(file);
                file.Close();
                _races.Add(raceout);
                counter++;
            }
            return counter;
        }

        public static void EndRace(
            List<Rival> _finishedParticipants,
            List<Rival> _currentRivals,
            ref int _countdown,
            List<Vehicle> _participants,
            ref List<Vector3> _checkpoints,
            ref Blip _nextBlip,
            ref Blip _secondBlip,
            ref bool _isInRace,
            ref Race _currentRace,
            List<Entity> _cleanupBag,
            List<Tuple<Rival, int>> _rivalCheckpointStatus
            )
        {
            _isInRace = false;
            _currentRace = null;

            _secondBlip?.Remove();
            _nextBlip?.Remove();
            _checkpoints.Clear();
            foreach (Entity entity in _cleanupBag)
            {
                //commented that don't remove player vehicle
                //entity?.Delete();
            }
            _cleanupBag.Clear();
            _participants.Clear();
            _countdown = -1;
            foreach (Rival rival in _currentRivals)
            {
                rival.Clean();
            }
            _currentRivals.Clear();
            _rivalCheckpointStatus.Clear();
            _finishedParticipants.Clear();
        }

        public static Vector3 RotationToDirection(Vector3 rotation)
        {
            var z = DegToRad(rotation.Z);
            var x = DegToRad(rotation.X);
            var num = Math.Abs(Math.Cos(x));
            return new Vector3
            {
                X = (float)(-Math.Sin(z) * num),
                Y = (float)(Math.Cos(z) * num),
                Z = (float)Math.Sin(x)
            };
        }

        public static Vector3 DirectionToRotation(Vector3 direction)
        {
            direction.Normalize();

            var x = Math.Atan2(direction.Z, direction.Y);
            var y = 0;
            var z = -Math.Atan2(direction.X, direction.Y);

            return new Vector3
            {
                X = (float)RadToDeg(x),
                Y = (float)RadToDeg(y),
                Z = (float)RadToDeg(z)
            };
        }

        public static double DegToRad(double deg)
        {
            return deg * Math.PI / 180.0;
        }

        public static double RadToDeg(double deg)
        {
            return deg * 180.0 / Math.PI;
        }

        public static double BoundRotationDeg(double angleDeg)
        {
            var twoPi = (int)(angleDeg / 360);
            var res = angleDeg - twoPi * 360;
            if (res < 0) res += 360;
            return res;
        }

        public static Model RequestModel(int hash, int limit = 1000)
        {
            var tmpModel = new Model(hash);
            int counter = 0;
            while (!tmpModel.IsLoaded && counter < limit)
            {
                tmpModel.Request();
                Script.Yield();
                counter++;
            }
            return tmpModel;
        }
    }
}