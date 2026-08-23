using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BunnyOfWar
{
    public static class robbyPort
    {
        private static int crashReported;

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBoxW(nint windowHandle, string text, string caption, uint type);

        public static void InstallCrashManager()
        {
            AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
            {
                ReportCrash("Unhandled AppDomain exception", eventArgs.ExceptionObject as Exception);
            };
            TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
            {
                ReportCrash("Unobserved task exception", eventArgs.Exception);
                eventArgs.SetObserved();
            };
        }

        public static void LogException(string context, Exception exception)
        {
            string message = $"[{DateTime.Now:O}] {context}{Environment.NewLine}{exception}{Environment.NewLine}{Environment.NewLine}";
            Console.Error.WriteLine(message);
            File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "robbyPort-crash.log"), message);
        }

        public static void ReportCrash(string context, Exception exception)
        {
            if (Interlocked.Exchange(ref crashReported, 1) != 0)
            {
                return;
            }

            exception ??= new Exception("The process terminated without a managed exception object.");
            LogException(context, exception);
            string reportPath = Path.Combine(AppContext.BaseDirectory, "robbyPort-crash.log");
            string details = $"{context}{Environment.NewLine}{Environment.NewLine}{exception.GetType().Name}: {exception.Message}{Environment.NewLine}{Environment.NewLine}A full crash report was written to:{Environment.NewLine}{reportPath}";
            MessageBoxW(0, details, "Flappy Monkey crashed", 0x10);
        }
    }
}

namespace Microsoft.Xna.Framework.GamerServices
{
    public class Gamer
    {
        private static readonly GamerCollection<SignedInGamer> signedInGamers = new(new List<SignedInGamer>
        {
            new SignedInGamer { Gamertag = "Player", PlayerIndex = PlayerIndex.One }
        });

        public static GamerCollection<SignedInGamer> SignedInGamers => signedInGamers;
        public string Gamertag { get; set; } = "Player";
    }

    public sealed class GamerPrivileges
    {
        public bool AllowOnlineSessions => false;
        public bool AllowPurchaseContent => false;
    }

    public sealed class SignedInGamer : Gamer
    {
        public bool IsSignedInToLive => false;
        public PlayerIndex PlayerIndex { get; internal set; }
        public GamerPrivileges Privileges { get; } = new();
    }

    public sealed class GamerServicesComponent : GameComponent
    {
        public GamerServicesComponent(Game game) : base(game) { }
    }

    public enum MessageBoxIcon { None, Alert, Error, Warning }

    public static class Guide
    {
        public static bool IsVisible => false;
        public static bool IsTrialMode => false;
        public static bool SimulateTrialMode => false;

        public static void ShowSignIn(int paneCount, bool onlineOnly) { }
        public static void ShowMarketplace(PlayerIndex playerIndex) { }
        public static void ShowGameInvite(PlayerIndex playerIndex, IEnumerable<Gamer> gamers) { }

        public static IAsyncResult BeginShowKeyboardInput(PlayerIndex playerIndex, string title, string description, string defaultText, AsyncCallback callback, object state)
        {
            Task<string> task = Task.FromResult(defaultText ?? string.Empty);
            callback?.Invoke(task);
            return task;
        }

        public static string EndShowKeyboardInput(IAsyncResult result) => ((Task<string>)result).GetAwaiter().GetResult();

        public static IAsyncResult BeginShowMessageBox(PlayerIndex playerIndex, string title, string text, IEnumerable<string> buttons, int focusButton, MessageBoxIcon icon, AsyncCallback callback, object state)
        {
            Task<int?> task = Task.FromResult<int?>(0);
            callback?.Invoke(task);
            return task;
        }
    }

    public sealed class InviteAcceptedEventArgs : EventArgs { }

    public sealed class GamerPrivilegeException : Exception
    {
        public GamerPrivilegeException() { }
        public GamerPrivilegeException(string message) : base(message) { }
    }

    public class GamerCollection<T> : ReadOnlyCollection<T>
    {
        public GamerCollection(IList<T> list) : base(list) { }

        public T this[PlayerIndex playerIndex] => this[(int)playerIndex];

        public new GamerCollectionEnumerator<T> GetEnumerator() => new(Items.GetEnumerator());
    }

    public sealed class GamerCollectionEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;
        public GamerCollectionEnumerator(IEnumerator<T> enumerator) => this.enumerator = enumerator;
        public T Current => enumerator.Current;
        object IEnumerator.Current => Current;
        public bool MoveNext() => enumerator.MoveNext();
        public void Reset() => enumerator.Reset();
        public void Dispose() => enumerator.Dispose();
    }
}

namespace Microsoft.Xna.Framework.Storage
{
    public sealed class StorageDevice
    {
        public bool IsConnected => true;

        public static IAsyncResult BeginShowSelector(AsyncCallback callback, object state)
        {
            Task<StorageDevice> task = Task.FromResult(new StorageDevice());
            callback?.Invoke(task);
            return task;
        }

        public static StorageDevice EndShowSelector(IAsyncResult result) => ((Task<StorageDevice>)result).GetAwaiter().GetResult();

        public IAsyncResult BeginOpenContainer(string displayName, AsyncCallback callback, object state)
        {
            Task<StorageContainer> task = Task.FromResult(new StorageContainer(displayName));
            callback?.Invoke(task);
            return task;
        }

        public StorageContainer EndOpenContainer(IAsyncResult result) => ((Task<StorageContainer>)result).GetAwaiter().GetResult();
    }

    public sealed class StorageContainer : IDisposable
    {
        private readonly string path;
        public StorageContainer(string displayName)
        {
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FlappyMonkey", displayName);
            Directory.CreateDirectory(path);
        }

        public bool IsDisposed { get; private set; }
        public bool FileExists(string fileName) => File.Exists(Path.Combine(path, fileName));
        public Stream OpenFile(string fileName, FileMode mode) => File.Open(Path.Combine(path, fileName), mode, FileAccess.ReadWrite);
        public Stream CreateFile(string fileName) => File.Create(Path.Combine(path, fileName));
        public void Dispose() => IsDisposed = true;
    }
}

namespace Microsoft.Xna.Framework.Net
{
    using Microsoft.Xna.Framework.GamerServices;

    public enum NetworkSessionType { SystemLink = 1, PlayerMatch = 2 }
    public enum NetworkSessionState { Lobby = 0, Playing = 1 }
    public enum NetworkSessionEndReason { Unknown }
    public enum NetworkSessionJoinError { Unknown }
    public enum SendDataOptions { None, Reliable, InOrder, Chat }

    public sealed class NetworkException : Exception { public NetworkException() { } public NetworkException(string message) : base(message) { } }
    public sealed class NetworkSessionJoinException : Exception { public NetworkSessionJoinError JoinError => NetworkSessionJoinError.Unknown; }
    public sealed class NetworkSessionProperties { }

    public sealed class PacketWriter : BinaryWriter
    {
        public PacketWriter() : base(new MemoryStream()) { }

        public void Write(Vector2 value)
        {
            Write(value.X);
            Write(value.Y);
        }
    }

    public sealed class PacketReader : BinaryReader
    {
        public PacketReader() : base(new MemoryStream()) { }
        public int Length => (int)BaseStream.Length;
        public int Position { get => (int)BaseStream.Position; set => BaseStream.Position = value; }
        public Vector2 ReadVector2() => new(ReadSingle(), ReadSingle());
    }

    public class NetworkGamer : Gamer
    {
        public byte Id { get; internal set; }
        public bool IsHost => false;
        public bool IsLocal => false;
    }

    public sealed class LocalNetworkGamer : NetworkGamer
    {
        public bool IsDataAvailable => false;
        public void SendData(PacketWriter writer, SendDataOptions options) { writer.BaseStream.SetLength(0); }
        public void SendPartyInvites() { }
        public void ReceiveData(PacketReader reader, ref NetworkGamer sender) { sender = null; }
    }

    public sealed class AvailableNetworkSession
    {
        public int CurrentGamerCount => 0;
        public string HostGamertag => "Offline";
    }

    public sealed class AvailableNetworkSessionCollection : GamerCollection<AvailableNetworkSession>, IDisposable
    {
        public AvailableNetworkSessionCollection() : base(new List<AvailableNetworkSession>()) { }
        public void Dispose() { }
    }

    public sealed class GameStartedEventArgs : EventArgs { }
    public sealed class GameEndedEventArgs : EventArgs { }
    public sealed class GamerLeftEventArgs : EventArgs { public NetworkGamer Gamer { get; init; } }
    public sealed class NetworkSessionEndedEventArgs : EventArgs { public NetworkSessionEndReason EndReason => NetworkSessionEndReason.Unknown; }

    public sealed class NetworkSession : IDisposable
    {
        private readonly GamerCollection<LocalNetworkGamer> localGamers = new(new List<LocalNetworkGamer>());
        private readonly GamerCollection<NetworkGamer> remoteGamers = new(new List<NetworkGamer>());
        private readonly GamerCollection<NetworkGamer> allGamers = new(new List<NetworkGamer>());

        public static event EventHandler<InviteAcceptedEventArgs> InviteAccepted;
        public event EventHandler<GameStartedEventArgs> GameStarted;
        public event EventHandler<GameEndedEventArgs> GameEnded;
        public event EventHandler<GamerLeftEventArgs> GamerLeft;
        public event EventHandler<NetworkSessionEndedEventArgs> SessionEnded;

        public bool IsDisposed { get; private set; }
        public bool IsHost => true;
        public bool AllowHostMigration { get; set; }
        public bool AllowJoinInProgress { get; set; }
        public int MaxGamers => 6;
        public int PrivateGamerSlots { get; set; }
        public NetworkSessionState SessionState { get; private set; } = NetworkSessionState.Lobby;
        public GamerCollection<LocalNetworkGamer> LocalGamers => localGamers;
        public GamerCollection<NetworkGamer> RemoteGamers => remoteGamers;
        public GamerCollection<NetworkGamer> AllGamers => allGamers;

        public static NetworkSession Create(NetworkSessionType type, int maxLocalGamers, int maxGamers) => new();
        public static NetworkSession Create(NetworkSessionType type, int maxLocalGamers, int maxGamers, int privateSlots, NetworkSessionProperties properties) => new();
        public static IAsyncResult BeginFind(NetworkSessionType type, int maxLocalGamers, NetworkSessionProperties properties, AsyncCallback callback, object state) => Complete(new AvailableNetworkSessionCollection(), callback);
        public static AvailableNetworkSessionCollection EndFind(IAsyncResult result) => ((Task<AvailableNetworkSessionCollection>)result).GetAwaiter().GetResult();
        public static IAsyncResult BeginJoin(AvailableNetworkSession session, AsyncCallback callback, object state) => Complete(new NetworkSession(), callback);
        public static NetworkSession EndJoin(IAsyncResult result) => ((Task<NetworkSession>)result).GetAwaiter().GetResult();
        public static NetworkSession EndCreate(IAsyncResult result) => ((Task<NetworkSession>)result).GetAwaiter().GetResult();
        public static IAsyncResult BeginJoinInvited(int maxLocalGamers, AsyncCallback callback, object state) => Complete(new NetworkSession(), callback);
        public static NetworkSession EndJoinInvited(IAsyncResult result) => ((Task<NetworkSession>)result).GetAwaiter().GetResult();
        public static NetworkSession JoinInvited(int maxLocalGamers) => new();

        public void Update() { }
        public void StartGame() { SessionState = NetworkSessionState.Playing; GameStarted?.Invoke(this, new GameStartedEventArgs()); }
        public void EndGame() { SessionState = NetworkSessionState.Lobby; GameEnded?.Invoke(this, new GameEndedEventArgs()); }
        public void Dispose() => IsDisposed = true;

        private static Task<T> Complete<T>(T value, AsyncCallback callback)
        {
            Task<T> task = Task.FromResult(value);
            callback?.Invoke(task);
            return task;
        }
    }
}