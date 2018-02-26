using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OalSoft.NET.OpenALSharp;

namespace OalSoft.NET
{
    /// <summary>
    /// Managed wrapper for an OpenAL playback device.
    /// </summary>
    public sealed class AlDevice : IDisposable
    {
        public static readonly string DefaultDeviceName;
        private static AlDevice _defaultDevice;

        static AlDevice()
        {
            var ptr = ALC10.alcGetString(IntPtr.Zero, ALC10.ALC_DEFAULT_DEVICE_SPECIFIER);
            if (ptr != IntPtr.Zero)
                DefaultDeviceName = Marshal.PtrToStringAnsi(ptr);
        }

        /// <summary>
        /// Close the default device if it is opened.
        /// </summary>
        public static void CloseDefault()
        {
            if (_defaultDevice != null && !_defaultDevice._disposed)
                _defaultDevice.Dispose();
        }

        /// <summary>
        /// Opens the default playback device if not open and returns it.
        /// </summary>
        public static AlDevice GetDefault()
        {
            if (_defaultDevice == null || _defaultDevice._disposed)
                _defaultDevice = new AlDevice(null);
            return _defaultDevice;
        }

        /// <summary>
        /// Create a managed wrapper for the device with the given name.
        /// If <code>null</code>, the empty string or the default device name is given,
        /// the default device will be opened (if not yet open) and returned;
        /// </summary>
        /// <param name="name">Name of the device.</param>
        public static AlDevice Create(string name)
        {
            if (string.IsNullOrEmpty(name) || name == DefaultDeviceName)
                return GetDefault();
            return new AlDevice(name);
        }

        /// <summary>
        /// The name of this device.
        /// </summary>
        public string Name { get; }
        private readonly IntPtr _handle;

        /// <summary>
        /// The main context of this device. This context is created when the device is created.
        /// </summary>
        public AlContext MainContext => _contexts[0];

        private readonly List<AlContext> _contexts;
        private readonly List<uint> _buffers;

        private bool _disposed;

        /// <summary>
        /// Create a managed wrapper for the specified audio playback device.
        /// Available devices can be queried with <see cref="GetDevices"/>.
        /// </summary>
        /// <param name="deviceName">Name of the device.</param>
        /// <exception cref="Exception">If opening the device or creating the context fails.</exception>
        private AlDevice(string deviceName)
        {
            Name = deviceName ?? DefaultDeviceName;
            _handle = ALC10.alcOpenDevice(deviceName);
            if (_handle == IntPtr.Zero)
                throw new Exception("Failed to open device.");

            _contexts = new List<AlContext>();
            _buffers = new List<uint>();

            var ctxHandle = ALC10.alcCreateContext(_handle, new int[0]);
            if (ctxHandle == IntPtr.Zero)
            {
                ALC10.alcCloseDevice(_handle);
                throw new Exception("Failed to create context.");
            }
            var ctx = new AlContext(this, ctxHandle);
            _contexts.Add(ctx);
        }

        /// <summary>
        /// Create a context for this device.
        /// </summary>
        public AlContext CreateContext()
        {
            CheckDisposed();
            var ctxHandle = ALC10.alcCreateContext(_handle, new int[0]);
            if (ctxHandle == IntPtr.Zero)
            {
                AlHelper.AlcCheckError(_handle, "Failed to create context.");
                throw new Exception("Failed to create context.");
            }
            var ctx = new AlContext(this, ctxHandle);
            _contexts.Add(ctx);
            return ctx;
        }

        /// <summary>
        /// Create a number of OpenAL buffers for this device.
        /// </summary>
        /// <param name="buffers">Array to fill with buffer names.</param>
        /// <param name="n">Number of buffers to generate.</param>
        public void CreateBuffers(uint[] buffers, int n)
        {
            CheckDisposed();
            ALC10.alcMakeContextCurrent(MainContext.Handle);
            AL10.alGenBuffers(n, buffers);
            AlHelper.AlAlwaysCheckError("Failed to generate buffers.");
            for (var i = 0; i < n; i++)
                _buffers.Add(buffers[i]);
        }

        /// <summary>
        /// Create an OpenAL buffer for this device.
        /// </summary>
        public uint CreateBuffer()
        {
            CheckDisposed();
            ALC10.alcMakeContextCurrent(MainContext.Handle);
            AL10.alGenBuffers(1, out var name);
            if (name == 0)
            {
                AlHelper.AlCheckError("alGenBuffer call failed.");
                throw new Exception("Failed to create buffer.");
            }

            _buffers.Add(name);
            return name;
        }

        /// <summary>
        /// Delete a collection of buffers.
        /// </summary>
        /// <param name="buffers">Array to delete buffers from, starting at index 0.</param>
        /// <param name="n">Number of buffers to delete.</param>
        /// <remarks>
        /// Buffers that are not owned by this device are ignored.
        /// </remarks>
        /// <exception cref="IndexOutOfRangeException">
        /// If <paramref name="n"/> is larger than or equal to <code>buffers.Length</code>.
        /// </exception>
        public void DeleteBuffers(uint[] buffers, int n)
        {
            CheckDisposed();
            ALC10.alcMakeContextCurrent(MainContext.Handle);

            for (var i = 0; i < n; i++)
            {
                var buffer = buffers[i];
                if (_buffers.Remove(buffer))
                    AL10.alDeleteBuffers(1, ref buffer);
            }
            AlHelper.AlCheckError("alDeleteBuffers call failed.");
        }

        /// <summary>
        /// Delete a buffer.
        /// </summary>
        /// <param name="name">Name of the buffer.</param>
        /// <exception cref="InvalidOperationException">If the buffer is not owned by this device.</exception>
        public void DeleteBuffer(uint name)
        {
            CheckDisposed();

            if (!_buffers.Remove(name))
                throw new InvalidOperationException("Device does not own given buffer.");

            ALC10.alcMakeContextCurrent(MainContext.Handle);
            AL10.alDeleteBuffers(1, ref name);
            AlHelper.AlCheckError("alDeleteBuffers call failed.");
        }

        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName, "Can't use closed device.");
        }

        internal void DestroyContext(AlContext ctx)
        {
            if (!_contexts.Remove(ctx))
                throw new InvalidOperationException("Device does not own given context.");
            ALC10.alcMakeContextCurrent(IntPtr.Zero);
            ctx.Destroy();
        }

        private void ReleaseUnmanagedResources()
        {
            if (_disposed)
                return;

            ALC10.alcMakeContextCurrent(MainContext.Handle);
            for (var i = 0; i < _buffers.Count; i++)
            {
                var buf = _buffers[i];
                AL10.alDeleteBuffers(_buffers.Count, ref buf);
            }
            _buffers.Clear();

            ALC10.alcMakeContextCurrent(IntPtr.Zero);
            foreach (var ctx in _contexts)
                ctx.Destroy();

            ALC10.alcCloseDevice(_handle);

            _disposed = true;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~AlDevice()
        {
            ReleaseUnmanagedResources();
        }

        /// <summary>
        /// Get the available devices.
        /// </summary>
        public static IEnumerable<string> GetDevices()
        {
            var deviceList = ALC11.alcIsExtensionPresent(IntPtr.Zero, "ALC_ENUMERATE_ALL_EXT") ?
                ALC10.alcGetString(IntPtr.Zero, ALC11.ALC_ALL_DEVICES_SPECIFIER) :
                ALC10.alcGetString(IntPtr.Zero, ALC10.ALC_DEVICE_SPECIFIER);
            var curString = Marshal.PtrToStringAnsi(deviceList);
            while (!string.IsNullOrEmpty(curString))
            {
                yield return curString;
                deviceList += curString.Length + 1;
                curString = Marshal.PtrToStringAnsi(deviceList);
            }
        }
    }
}
