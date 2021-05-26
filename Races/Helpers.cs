using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GTA;
using GTA.Math;

namespace Races
{
    public static class Helpers
    {
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
            int _countdown,
            List<Vehicle> _participants,
            List<Vector3> _checkpoints,
            Blip _nextBlip, Blip _secondBlip,
            bool _isInRace,
            Race _currentRace,
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