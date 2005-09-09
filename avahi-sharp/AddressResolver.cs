using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;
using Mono.Unix;

namespace Avahi
{

    internal delegate void AddressResolverCallback (IntPtr resolver, int iface, Protocol proto,
                                                    ResolverEvent revent, Protocol aproto, IntPtr address,
                                                    IntPtr hostname, IntPtr userdata);

    public delegate void HostAddressHandler (object o, string host, IPAddress address);
    
    public class AddressResolver : IDisposable
    {
        private IntPtr handle;
        private Client client;
        private int iface;
        private Protocol proto;
        private IPAddress address;

        private IPAddress currentAddress;
        private string currentHost;

        private ArrayList foundListeners = new ArrayList ();
        private ArrayList timeoutListeners = new ArrayList ();
        
        [DllImport ("avahi-client")]
        private static extern IntPtr avahi_address_resolver_new (IntPtr client, int iface, Protocol proto,
                                                                 IntPtr address, AddressResolverCallback cb,
                                                                 IntPtr userdata);

        [DllImport ("avahi-client")]
        private static extern void avahi_address_resolver_free (IntPtr handle);

        public event HostAddressHandler Found
        {
            add {
                foundListeners.Add (value);
                Start ();
            }
            remove {
                foundListeners.Remove (value);
                Stop (false);
            }
        }
        
        public event EventHandler Timeout
        {
            add {
                timeoutListeners.Add (value);
                Start ();
            }
            remove {
                timeoutListeners.Remove (value);
                Stop (false);
            }
        }

        public IPAddress Address
        {
            get { return currentAddress; }
        }

        public string HostName
        {
            get { return currentHost; }
        }

        public AddressResolver (Client client, IPAddress address) : this (client, -1, Protocol.Unspecified, address)
        {
        }

        public AddressResolver (Client client, int iface, Protocol proto, IPAddress address)
        {
            this.client = client;
            this.iface = iface;
            this.proto = proto;
            this.address = address;
        }

        ~AddressResolver ()
        {
            Dispose ();
        }

        public void Dispose ()
        {
            Stop (true);
        }

        private void Start ()
        {
            if (handle != IntPtr.Zero || (foundListeners.Count == 0 && timeoutListeners.Count == 0))
                return;

            IntPtr addrPtr = Utility.StringToPtr (address.ToString ());
            handle = avahi_address_resolver_new (client.Handle, iface, proto, addrPtr,
                                                 OnAddressResolverCallback, IntPtr.Zero);
            Utility.Free (addrPtr);
        }

        private void Stop (bool force)
        {
            if (handle != IntPtr.Zero && (force || (foundListeners.Count == 0 && timeoutListeners.Count == 0))) {
                avahi_address_resolver_free (handle);
                handle = IntPtr.Zero;
            }
        }

        private void OnAddressResolverCallback (IntPtr resolver, int iface, Protocol proto,
                                                ResolverEvent revent, Protocol aproto, IntPtr address,
                                                IntPtr hostname, IntPtr userdata)
        {
            if (revent == ResolverEvent.Found) {
                currentAddress = Utility.PtrToAddress (address);
                currentHost = Utility.PtrToString (hostname);

                foreach (HostAddressHandler handler in foundListeners)
                    handler (this, currentHost, currentAddress);
            } else {
                currentAddress = null;
                currentHost = null;
                
                foreach (EventHandler handler in timeoutListeners)
                    handler (this, new EventArgs ());
            }
        }
    }
}