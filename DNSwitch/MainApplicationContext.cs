﻿using DNSwitch.Properties;
using System.Net.NetworkInformation;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;

namespace DNSwitch
{
    internal class MainApplicationContext : ApplicationContext
    {
        readonly NotifyIcon notifyIcon;

        public MainApplicationContext()
        {
            notifyIcon = new()
            {
                Text = "Dextop",
                Visible = true,
                Icon = Resources.LightIcon,
            };

            notifyIcon.DoubleClick += (_, _) => Application.Exit();
            Application.ApplicationExit += ApplicationExit;
            var res = GetAdapterList();
            UpdateMenu(res);
            NetworkChange.NetworkAvailabilityChanged += NetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged += NetworkAddressChanged;
        }

        void ApplicationExit(object? _, EventArgs __)
        {
            notifyIcon.Visible = false;
            NetworkChange.NetworkAvailabilityChanged -= NetworkAvailabilityChanged;
            NetworkChange.NetworkAddressChanged -= NetworkAddressChanged;
        }

        private void NetworkAddressChanged(object? sender, EventArgs e)
        {
            Debug.WriteLine("--NetworkAddressChanged");
            var res = GetAdapterList();
            UpdateMenu(res);
        }

        private void NetworkAvailabilityChanged(object? sender, NetworkAvailabilityEventArgs e)
        {
            Debug.WriteLine("--NetworkAvailabilityChanged");
            var res = GetAdapterList();
            UpdateMenu(res);
        }

        NetworkInterface[] GetAdapterList()
        {
            List<NetworkInterface> adapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(
                x => x.OperationalStatus == OperationalStatus.Up
                && x.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                && x.NetworkInterfaceType != NetworkInterfaceType.Loopback
                ).ToList();

            if (adapters.Count == 1)
                return [.. adapters];

            var internetAdapter = adapters.FirstOrDefault(x => x.GetIPProperties().GatewayAddresses.Select(x => x.Address).Any(x => x.AddressFamily == AddressFamily.InterNetwork && !x.Equals(IPAddress.Any)));

            if (internetAdapter != null)
            {
                adapters.Remove(internetAdapter);
                adapters.Insert(0, internetAdapter);
            }

            return [.. adapters];
        }

        void UpdateMenu(NetworkInterface[] networkInterfaces)
        {
            Debug.WriteLine("-UpdateMenu");
            networkInterfaces.ToList().ForEach(x => Debug.WriteLine(x.Name));
        }
    }
}
